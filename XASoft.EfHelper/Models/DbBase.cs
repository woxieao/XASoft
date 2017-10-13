using System;

namespace XASoft.EfHelper.Models
{
    public abstract class DbBase<TId> 
    {
        protected DbBase()
        {
            DelFlag = false;
            CreateDate = DateTime.Now;
        }

        public TId Id { get; internal set; }
        public bool DelFlag { get; internal set; }
        public DateTime CreateDate { get; internal set; }
    }
}
