using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;

namespace BenchmarkApp.Server.Database.Core
{
    public class FakeDataGeneratorService
    {
        public FakeDataGeneratorService() => Randomizer.Seed = new Random(8675309);

        public T GenerateFakeUser<T>(string identifier) where T : class, IBaseUser
            => new Faker<T>()
                .RuleFor(u => u.Identifier, (_, _) => identifier)
                .RuleFor(u => u.Gender, f => f.PickRandom<Name.Gender>())
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(u.Gender))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender))
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.Age, (f, u) => f.Random.Number(18, 100))
                .Generate();


        public IEnumerable<T> GenerateFakeUsers<T>(IList<string> identifiers) where T : class, IBaseUser =>
            Enumerable.Range(0, identifiers.Count).Select((index) => GenerateFakeUser<T>(identifiers[index]));
    }
}