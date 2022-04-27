# C# Database Benchmark App with Blazor Frontend

dotnet Program with Blazor Frontend which runs various benchmark tests against a postgres, mongoDb and neo4j Database to compare Database performance when dealing with same queries

## Screenshots from UI

### Dashboard Overview
![alt text](https://user-images.githubusercontent.com/65281921/165586885-5e34fe00-209d-4263-9f7e-f719166fa464.png)


### Result Overview
![alt text](https://user-images.githubusercontent.com/65281921/165586876-81c4a482-3040-4607-a105-bddc2f075477.png)


Folder Structure:

- Client contains Blazor Code
- Deployment contains docker-images for postgres, mongo and neo4j Database
- Server contains actual Benchmark Code wrapped in dotnet api
- Shared contains various models used by front- and backend
- QueryString contains Script for generating neo4J nested query

Benchmark includes 5 different test

```C#
    public interface IDataRepository
    {
        // read functions
        
        /// <summary>
        /// loads 10 users with 10 ^ level nested users from database // neighbors second test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadNestedEntitiesAsync(int level);

        /// <summary>
        /// loads 10 ^ level users from database // single-read test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadEntitiesAsync(int level);
        
        /// <summary>
        /// loads aggregated value of 10 ^ level users from database  // aggregation test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadAggregateAsync(int level);
        
        /// <summary>
        /// writes 10 ^ level users to database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of written entities</returns>
        Task<int> WriteEntitiesAsync(int level);


        /// <summary>
        /// writes 10 ^ (level + 1) users to database which are all connected to a single user
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of written entities</returns>
        Task<int> WriteNestedEntitiesAsync(int level);    
    }
```

Interface is implemented for all three databases, for example write Test:

### NEO4J
```C#
 await _client.Cypher
                .Unwind(fakeUsers, "friend")
                .Merge("(user:WriteUser {" +
                       "identifier: friend.identifier, " +
                       "id: friend.id," +
                       "firstName:friend.firstName," +
                       "lastName:friend.lastName," +
                       "age: friend.age," +
                       "email:friend.email," +
                       "userName:friend.userName," +
                       "gender:friend.gender}" +
                       ")")
                .ExecuteWithoutResultsAsync();
```

### MongoDB
```c#
 var friends = Enumerable
                .Range(1, howMany)
                .Select(z => _faker.GenerateFakeUser<MongoUserEntity>(Config.UserName(level, z)))
                .ToList();

            await _ctx.WriteUsers.InsertManyAsync(friends);
```

### Postgres
```c#
        var names = Enumerable.Range(1, howMany)
                .Select(i => Config.UserName(level, i))
                .ToList();
            
            var fakeUsers = _faker.GenerateFakeUsers<SqlWriteUserEntity>(names);

            await _ctx.WriteUsers.AddRangeAsync(fakeUsers);
            await _ctx.SaveChangesAsync();
```