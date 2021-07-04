using System;
using System.Linq;

namespace BenchmarkApp.Server.Database.SQL
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> If<T>(
            this IQueryable<T> source,
            bool condition,
            Func<IQueryable<T>, IQueryable<T>> transform
        )
        { 
            return condition? transform(source) : source;
        }
        
    }
}