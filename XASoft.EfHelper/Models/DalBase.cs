﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XASoft.EfHelper.Models
{

    #region ChildDalBase

    public abstract class IdIntDalBase<TSource, TEntity> : DalBase<TSource, TEntity, int>
        where TSource : DbBase<int>
        where TEntity : DbContext
    {
        protected IdIntDalBase(TEntity db) : base(db)
        {
        }
    }

    public abstract class IdStringDalBase<TSource, TEntity> : DalBase<TSource, TEntity, string>
        where TSource : DbBase<string>
        where TEntity : DbContext
    {
        protected IdStringDalBase(TEntity db) : base(db)
        {
        }
    }

    public abstract class IdGuidDalBase<TSource, TEntity> : DalBase<TSource, TEntity, Guid>
        where TSource : DbBase<Guid>
        where TEntity : DbContext
    {
        protected IdGuidDalBase(TEntity db) : base(db)
        {
        }
    }

    public abstract class IdLongDalBase<TSource, TEntity> : DalBase<TSource, TEntity, long>
        where TSource : DbBase<long>
        where TEntity : DbContext
    {
        protected IdLongDalBase(TEntity db) : base(db)
        {
        }
    }

    #endregion

    public abstract class DalBase<TSource, TEntity, TId> : IDisposable
        where TSource : DbBase<TId>
        where TEntity : DbContext
    {
        protected readonly IQueryable<TSource> Source;
        protected readonly TEntity Db;

        protected DalBase(TEntity db)
        {
            Db = db;
            Source = db.Set<TSource>();
        }


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
            return Source.First(DelDataFilter(predicate));
        }

        public TSource GetById(TId id)
        {
            return Source.First(i => i.Id.Equals(id) && !i.DelFlag);
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
            return new PagingData<TSource>
            {
                List = Source.Where(p).OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = Source.Count(p)
            };
        }

        public PagingData<TSource> GetList(int pageIndex, int pageSize)
        {
            var paging = new Paging(pageIndex, pageSize);
            return new PagingData<TSource>
            {
                List =
                    Source.Where(i => !i.DelFlag).OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = Source.Count(i => !i.DelFlag)
            };
        }

        public void Remove(TSource entity)
        {
            if (entity != null)
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

        public void Update(TId id, TSource data2Update)
        {
            var entity = GetById(id);
            var properties = typeof(TSource).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                prop.SetValue(entity, prop.GetValue(data2Update, null), null);
            }
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
