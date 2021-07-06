using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

namespace BenchmarkApp.Server.Database.SQL
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> If<T, P>(
            this IIncludableQueryable<T, P> source,
            bool condition,
            Func<IIncludableQueryable<T, P>, IQueryable<T>> transform
        )
            where T : class
        {
            return condition ? transform(source) : source;
        }
    }
}