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
        public int Id { get; internal set; }
        public bool DelFlag { get; internal set; }
        public DateTime CreateDate { get; internal set; }
    }
}
