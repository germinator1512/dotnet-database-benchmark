using System;
using Bogus.DataSets;

namespace BenchmarkApp.Server.Database.Core
{
    public interface IBaseUser
    {
        public string Identifier { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public Name.Gender Gender { get; set; }
    }
}