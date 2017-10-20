using System;

namespace XASoft.CommonModel.ExceptionModel
{
    public class MsgException : Exception
    {
        public MsgException() : base() { }
        public MsgException(string msg) : base(msg) { }
    }
}
