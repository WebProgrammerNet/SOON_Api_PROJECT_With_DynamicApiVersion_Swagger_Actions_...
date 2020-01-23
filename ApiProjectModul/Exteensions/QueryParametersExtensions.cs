using ApiProjectModul.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.Exteensions
{
    public static class QueryParametersExtensions
    {
        public static bool HasPrevious(this QueryParameters queryParameters)
        {
            return (queryParameters.Page > 1);
        }

        public static bool HasNext(this QueryParameters queryParameters, int totalCount)
        {
            return (queryParameters.Page < (int)GetTotalPages(queryParameters, totalCount));
        }

        public static double GetTotalPages(this QueryParameters queryParameters, int totalCount)
        {
            return Math.Ceiling(totalCount / (double)queryParameters.PageCount);
        }

        public static bool HasQuery(this QueryParameters queryParameters)
        {
            //For comment for you
            // bool var1 = !String.IsNullOrEmpty(queryParameters.Query); //false - query var
            // bool var2 = String.IsNullOrEmpty(queryParameters.Query); //true - query yoxdur

            return !String.IsNullOrEmpty(queryParameters.Query);
        }

        public static bool IsDescending(this QueryParameters queryParameters)
        {
            if (!String.IsNullOrEmpty(queryParameters.OrderBy))
            {
                // var kkk = queryParameters.OrderBy.Split(' ').Last().ToLowerInvariant().StartsWith("desc");
                return queryParameters.OrderBy.Split(' ').Last().ToLowerInvariant().StartsWith("desc");
            }
            return false;
        }
    }
}
