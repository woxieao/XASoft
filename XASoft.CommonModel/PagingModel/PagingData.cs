using System.Collections.Generic;

namespace XASoft.CommonModel.PagingModel
{
    public class PagingData<T>
    {
        public PagingData()
        {
            List = new List<T>();
        }
        public IEnumerable<T> List { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
