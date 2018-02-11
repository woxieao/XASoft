using System;

namespace XASoft.CommonModel.ExceptionModel
{
    public class AuthException : Exception
    {
        public AuthException() : base() { }
        public AuthException(string msg) : base(msg) { }
    }
}
