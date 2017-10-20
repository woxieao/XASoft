using XASoft.CommonModel.ExceptionModel;

namespace XASoft.EfHelper.Models.Msg
{
    public class DbMsgException : MsgException
    {
        public DbMsgException() : base() { }
        public DbMsgException(string msg) : base(msg) { }
    }
}