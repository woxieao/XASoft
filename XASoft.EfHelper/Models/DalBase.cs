using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using XASoft.CommonModel.PagingModel;
using XASoft.EfHelper.Models.Msg;

namespace XASoft.EfHelper.Models
{
    internal class SwapVisitor : ExpressionVisitor
    {
        private readonly Expression _from, _to;

        public SwapVisitor(Expression from, Expression to)
        {
            this._from = from;
            this._to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }

    public abstract class DalBase<TSource, TEntity> : IDisposable
        where TSource : DbBase
        where TEntity : DbContext

    {
        protected readonly IQueryable<TSource> Source;
        protected readonly TEntity Db;

        protected DalBase(TEntity db)
        {
            Db = db;
            Source = db.Set<TSource>();
        }

        private Expression<Func<TSource, bool>> CombineLambda(Expression<Func<TSource, bool>> predicate0,
            Expression<Func<TSource, bool>> predicate1)
        {
            return Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(
                new SwapVisitor(predicate0.Parameters[0], predicate1.Parameters[0]).Visit(predicate0.Body),
                predicate1.Body), predicate1.Parameters);
        }

        private Expression<Func<TSource, bool>> DelDataFilter(Expression<Func<TSource, bool>> predicate)
        {
            return CombineLambda(predicate, x => !x.DelFlag);
        }

        public TSource First(Expression<Func<TSource, bool>> predicate)
        {
            var entity = Source.First(DelDataFilter(predicate));
            if (entity == null)
            {
                throw new DbMsgException("object not found");
            }
            else
            {
                return entity;
            }
        }

        public TSource GetById(int id)
        {
            var entity = Source.FirstOrDefault(i => i.Id == id && !i.DelFlag);
            if (entity == null)
            {
                throw new DbMsgException($"Id [{id}] not found");
            }
            else
            {
                return entity;
            }
        }

        public TSource FirstOrDefault(Expression<Func<TSource, bool>> predicate)
        {
            return Source.FirstOrDefault(DelDataFilter(predicate));
        }

        public IQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate)
        {
            return Source.Where(DelDataFilter(predicate));
        }

        public TSource SingleOrDefault(Expression<Func<TSource, bool>> predicate)
        {
            return Source.SingleOrDefault(DelDataFilter(predicate));
        }

        public TSource Single(Expression<Func<TSource, bool>> predicate)
        {
            return Source.Single(DelDataFilter(predicate));
        }

        public PagingData<TSource> GetList(Expression<Func<TSource, bool>> predicate, int pageIndex, int pageSize)
        {
            var paging = new Paging(pageIndex, pageSize);
            var p = DelDataFilter(predicate);
            var totalCount = Source.Count(p);
            return new PagingData<TSource>
            {
                List = Source.Where(p).OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = totalCount,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                PageCount = (int)Math.Ceiling(totalCount / (double)paging.PageSize)
            };
        }

        public PagingData<TSource> GetList(int pageIndex, int pageSize)
        {
            var paging = new Paging(pageIndex, pageSize);
            var totalCount = Source.Count(i => !i.DelFlag);
            return new PagingData<TSource>
            {
                List = Source.Where(i => !i.DelFlag).OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = totalCount,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                PageCount = (int)Math.Ceiling(totalCount / (double)paging.PageSize)
            };
        }

        public void Remove(TSource entity)
        {
            if (entity != null)
                entity.DelFlag = true;
        }

        public void Remove(int id)
        {
            var entity = GetById(id);
            entity.DelFlag = true;
        }

        public void RemoveRange(IEnumerable<TSource> entityList)
        {
            if (entityList != null)
            {
                foreach (var entity in entityList)
                {
                    entity.DelFlag = true;
                }
            }
        }

        public void UpdateById(int id, TSource data2Update, params string[] fieldNotUpdate)
        {
            var entity = GetById(id);
            var properties = typeof(TSource).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(i => !fieldNotUpdate.Contains(i.Name)).ToList();
            foreach (var prop in properties)
            {
                prop.SetValue(entity, prop.GetValue(data2Update, null), null);
            }
        }
        /// <summary>
        /// update entity 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data2Update">eg:{Fideld0="foo",Fideld1="bar"}</param>
        public void UpdateById(int id, object data2Update)
        {
            var entity = GetById(id);
            var properties = data2Update.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var entityProperties = typeof(TSource).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .ToList();
            foreach (var prop in properties)
            {
                var entityProp = entityProperties.FirstOrDefault(i => i.Name == prop.Name);
                if (entityProp != null)
                {
                    entityProp.SetValue(entity, prop.GetValue(data2Update, null), null);
                }
            }
        }

        public void SaveChanges()
        {
            Db.SaveChanges();
        }
        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
