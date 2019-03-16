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
        /// <summary>
        /// if you have other type of Id, you can write code like below:
        /// public new string Id { get; internal set; }
        /// and in this case,you need write your own method [GetById]/[UpdateById]
        /// </summary>
        public int Id { get; set; }
        public bool DelFlag { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
