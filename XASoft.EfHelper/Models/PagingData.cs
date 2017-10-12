using System.Collections.Generic;

namespace XASoft.EfHelper.Models
{
    public  class PagingData<T>
    {
        public PagingData()
        {
            List = new List<T>();
        }
        public IEnumerable<T> List { get; set; }
        public int TotalCount { get; set; }
    }
}
