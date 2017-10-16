using System;

namespace XASoft.ExceptionHelper
{
    public class MsgException : Exception
    {
        public MsgException() : base() { }
        public MsgException(string msg) : base(msg) { }
    }
}
