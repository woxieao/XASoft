namespace XASoft.EfHelper.Models
{
    public class DbMsg
    {
        protected DbMsg()
        {

        }

        public DbMsg(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        private int _pageIndex;
        private int _pageSize;

        public int PageIndex
        {
            get { return _pageIndex <= 0 ? 1 : _pageIndex; }
            set { _pageIndex = value; }
        }

        public int PageSize
        {
            get
            {
                _pageSize = _pageSize <= 0 ? 10 : _pageSize;
                _pageSize = _pageSize >= 100 ? 100 : _pageSize;
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        public int Skip => (PageIndex - 1) * PageSize;
    }
}
