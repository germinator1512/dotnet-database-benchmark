using System;
using System.Linq;

namespace QueryString
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GenerateQueryString(2));
        }

        private static string GenerateQueryString(int level)
        {
            var queryString = @"(user:User{name:$name})-[:KNOWS]->(friend1:User) ";

            foreach (var i in Enumerable.Range(1, level))
            {
                queryString += ($"-[:KNOWS]->(friend{i + 1}:User) ");
            }

            foreach (var depth in Enumerable.Range(0, level + 1).Reverse())
            {
                var withQuery = "with user,";

                foreach (var userIndex in Enumerable.Range(1, depth))
                {
                    withQuery += ($"friend{userIndex}, ");
                }

                withQuery += $"{{name: friend{depth + 1}.name, id: friend{depth + 1}.id ";

                if (depth != level)
                {
                    withQuery += $", friends: collect(friends{depth + 2})";
                }

                withQuery += $"}} as friends{depth + 1} ";

                queryString += withQuery;
            }

            return queryString + "with {name:user.name, id:user.id, friends: collect(friends1)} as rootUser";
        }
    }
}