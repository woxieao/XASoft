using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using XASoft.CommonModel.PagingModel;

namespace XASoft.Extensions
{
    public static class MergerExtensions
    {
        private static void Copy2Dynamic<T>(T sourceData, IDictionary<string, object> resultRef) where T : class
        {
            if (sourceData == null)
            {
                return;
            }
            foreach (var pi in JObject.FromObject(sourceData).Properties())
            {
                resultRef[pi.Name] = pi.Value;
            }
        }

        private static void SetVal<T0>(Func<T0, object> combineValFunc, T0 sourceData, IDictionary<string, object> resultRef) where T0 : class
        {
            if (sourceData == null)
            {
                return;
            }
            var result = combineValFunc(sourceData);
            Copy2Dynamic(result, resultRef);
        }

        /// <summary>
        /// create a new dynamic object not reference sourceData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData">sourceData</param>
        /// <returns>new dynamic object not reference sourceData</returns>
        public static dynamic Merger<T>(this T sourceData) where T : class
        {
            IDictionary<string, object> result = new ExpandoObject();
            Copy2Dynamic(sourceData, result);
            return result;
        }

        /// <summary>
        /// create a new dynamic object not reference sourceData
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sourceData">sourceData</param>
        /// <param name="combineVal">combineValue</param>
        /// <returns>new dynamic object not reference sourceData</returns>
        public static dynamic Merger<T0, T1>(this T0 sourceData, T1 combineVal)
            where T0 : class
            where T1 : class
        {
            IDictionary<string, object> result = new ExpandoObject();
            Copy2Dynamic(sourceData, result);
            Copy2Dynamic(combineVal, result);
            return result;
        }

        public static dynamic Merger<T0, T1>(this T0 sourceData, Func<T0, T1> combineValFunc)
            where T0 : class
            where T1 : class
        {
            IDictionary<string, object> result = new ExpandoObject();
            Copy2Dynamic(sourceData, result);
            SetVal(combineValFunc, sourceData, result);
            return result;
        }

        public static dynamic Merger<T0, T1, T2>(this T0 sourceData, T1 combineVal0, T2 combineVal1)
            where T0 : class
            where T1 : class
            where T2 : class
        {
            IDictionary<string, object> result = new ExpandoObject();
            Copy2Dynamic(sourceData, result);
            Copy2Dynamic(combineVal0, result);
            Copy2Dynamic(combineVal1, result);
            return result;
        }


        public static IEnumerable<dynamic> ListMerger<T0, T1>(this IEnumerable<T0> sourceDataList, Func<T0, T1> func)
            where T0 : class
            where T1 : class
        {
            var resultList = new List<IDictionary<string, object>>();
            foreach (var sourceData in sourceDataList)
            {
                resultList.Add((IDictionary<string, object>)sourceData.Merger(func));
            }
            return resultList;
        }

        public static PagingData<dynamic> PagingDataMerger<T0, T1>(this PagingData<T0> sourceDataList, Func<T0, T1> func)
            where T0 : class
            where T1 : class
        {
            var resultList = sourceDataList.List.ListMerger(func);
            return new PagingData<dynamic>
            {
                PageCount = sourceDataList.PageCount,
                PageSize = sourceDataList.PageSize,
                PageIndex = sourceDataList.PageIndex,
                TotalCount = sourceDataList.TotalCount,
                List = resultList
            };
        }
    }
}
