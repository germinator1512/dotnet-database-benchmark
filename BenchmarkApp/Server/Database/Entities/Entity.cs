using System.ComponentModel.DataAnnotations;

namespace BenchmarkApp.Server.Database.Entities
{
    public class Entity
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
    }
}