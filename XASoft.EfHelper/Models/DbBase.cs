using System;

namespace XASoft.EfHelper.Models
{
    public abstract class DbBase
    {
        protected DbBase()
        {
            DelFlag = false;
            CreateDate = DateTime.Now;
        }
        public int Id { get; set; }
        public bool DelFlag { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
