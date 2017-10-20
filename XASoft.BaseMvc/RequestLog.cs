using System;

namespace XASoft.BaseMvc
{
    public class RequestLog
    {
        public RequestLog()
        {
            DelFlag = false;
            CreateDate = DateTime.Now;
        }
        public string RawUrl { get; set; }
        public string FormData { get; set; }
        public string Header { get; set; }
        public bool Flag { get; set; }
        public string ResponseData { get; set; }
        public int Id { get; internal set; }
        public bool DelFlag { get; internal set; }
        public DateTime CreateDate { get; internal set; }
    }
}