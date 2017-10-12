using System;
using System.Linq;
using System.Linq.Expressions;
using XASoft.EfHelper.Models;

namespace XASoft.EfHelper
{
    public static class EfExtension
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
        private static Expression<Func<TSource, bool>> CombineLambda<TSource>(this Expression<Func<TSource, bool>> predicate0, Expression<Func<TSource, bool>> predicate1)
        {
            return Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(
          new SwapVisitor(predicate0.Parameters[0], predicate1.Parameters[0]).Visit(predicate0.Body),
          predicate1.Body), predicate1.Parameters);
        }
        private static Expression<Func<TSource, bool>> DelDataFilter<TSource>(this Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return predicate.CombineLambda(x => !x.DelFlag);
        }
        public static TSource FirstNotDel<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return source.First(predicate.DelDataFilter());
        }
        public static TSource FirstOrDefaultNotDel<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return source.FirstOrDefault(predicate.DelDataFilter());

        }
        public static IQueryable<TSource> WhereNotDel<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return source.Where(predicate.DelDataFilter());
        }
        public static TSource SingleOrDefaultNotDel<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return source.SingleOrDefault(predicate.DelDataFilter());
        }
        public static TSource SingleNotDel<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : DbBase
        {
            return source.Single(predicate.DelDataFilter());
        }
        public static PagingData<TSource> GetList<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, int pageIndex, int pageSize) where TSource : DbBase
        {
            var paging = new Paging(pageIndex, pageSize);
            var p = predicate.DelDataFilter();
            return new PagingData<TSource>
            {
                List = source.Where(p).OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = source.Count(p)
            };
        }
        public static PagingData<TSource> GetList<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize) where TSource : DbBase
        {
            var paging = new Paging(pageIndex, pageSize);
            return new PagingData<TSource>
            {
                List = source.OrderByDescending(i => i.Id).Skip(paging.Skip).Take(paging.PageSize),
                TotalCount = source.Count()
            };
        }
        public static void Delete<TSource>(TSource entity) where TSource : DbBase
        {
            entity.DelFlag = true;
        }
    }
}

