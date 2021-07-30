namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public static class QueryStrings
    {
        public const string RootUser = "{name:user.name, id:user.id, friends: collect(friends)} as rootUser";
        
        public const string Friends = "user, {name:friend.name, id:friend.id, friends: collect(friends0)} as friends";

        public const string Friends0 =
            "user, friend, {name:friend0.name, id:friend0.id, friends: collect(friends1)} as friends0";


        public const string Friends1 =
            "user,friend, friend0, {name:friend1.name, id:friend1.id, friends: collect(friends2)} as friends1";

        public const string Friends2 =
            "user,friend, friend0, friend1, {name:friend2.name, id:friend2.id, friends: collect(friends3)} as friends2";

        public const string Friends3 =
            "user, friend, friend0, friend1, friend2, {name:friend3.name, id:friend3.id, friends: collect(friends4)} as friends3";

        public const string Friends4 =
            "user, friend, friend0, friend1, friend2, friend3 {name:friend4.name, id:friend4.id} as friends4";
        
    }
}