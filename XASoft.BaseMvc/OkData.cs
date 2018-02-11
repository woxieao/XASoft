namespace XASoft.BaseMvc
{
    public  sealed class OkData
    {
        public enum StatusCode
        {
            LoggedOut = -1,
            Error = 0,
            Success = 1
        }
        public StatusCode Status { get; set; }

        public object Data { get; set; }
        public string Msg { get; set; }
    }
}
