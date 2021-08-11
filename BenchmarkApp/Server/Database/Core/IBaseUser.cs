using System;
using Bogus.DataSets;

namespace BenchmarkApp.Server.Database.Core
{
    public interface IBaseUser
    {
        public string Identifier { get; }
        
        public string FirstName { get; }

        public string LastName { get; }

        public int Age { get; }

        public string Email { get; }

        public string UserName { get; }

        public Name.Gender Gender { get; }
    }
}