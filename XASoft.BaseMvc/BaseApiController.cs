using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using XASoft.CommonModel.ExceptionModel;

namespace XASoft.BaseMvc
{
    public abstract class BaseApiController : Controller
    {
        private static bool _hideUnknownException = true;
        private static Action<RequestLog> _log2DbAct = i => { };
     
        public static void HideUnknownException(bool isHide)
        {
            _hideUnknownException = isHide;
        }
        public static void SetLog2DbAct(Action<RequestLog> log2DbAct)
        {
            _log2DbAct = log2DbAct;
        }

        protected static void LogRequest2Db(ControllerContext context, string responseData, bool flag, Action<RequestLog> log2Db)
        {
            try
            {
                var request = context.RequestContext.HttpContext.Request;
                var rawUrl = request.RawUrl;
                var header = request.Headers.ToString();
                var forms = request.Form.ToString();
                log2Db(new RequestLog
                {
                    RawUrl = rawUrl,
                    FormData = forms,
                    Header = header,
                    Flag = flag,
                    ResponseData = responseData
                });
            }
            catch
            {
                //do nothing
            }
        }
        public class Ok : ActionResult
        {
            private readonly JsonSerializerSettings _settings;
            private readonly OkResult _result = new OkResult
            {
                Status = OkResult.StatusCode.Success,
                Msg = String.Empty
            };

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Clear();
                response.TrySkipIisCustomErrors = true;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                var resultData = JsonConvert.SerializeObject(_result, _settings);
                response.Write(resultData);
                response.End();
                LogRequest2Db(context, resultData, true, _log2DbAct);
            }

            public Ok()
            {
                _settings = new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd"
                };
            }
            public Ok(object data) : this()
            {
                _result.Data = data;
            }
            public Ok(OkResult customerData) : this()
            {
                _result = customerData;
            }
            public Ok(object data, JsonSerializerSettings jsonSerializerSettings) : this(data)
            {
                _settings = jsonSerializerSettings;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception.GetBaseException();
            var result = new OkResult
            {
                Status = OkResult.StatusCode.Error,
                Data = null,
            };
            try
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.StatusCode = 200;
                throw ex;
            }
            catch (DbEntityValidationException e)
            {
                result.Msg = String.Join("\n", e.EntityValidationErrors.Select(i => String.Join("\n", i.ValidationErrors.Select(x => x.ErrorMessage))));
            }
            catch (MsgException)
            {
                result.Msg = ex.Message;
            }
            catch (Exception)
            {
                result.Msg = _hideUnknownException ? "服务器异常" : ex.Message;
            }
            finally
            {
                var resultData = JsonConvert.SerializeObject(result);
                Response.Write(resultData);
                filterContext.ExceptionHandled = true;
                Response.End();
                LogRequest2Db(filterContext, resultData, false, _log2DbAct);
            }
        }
    }
}
