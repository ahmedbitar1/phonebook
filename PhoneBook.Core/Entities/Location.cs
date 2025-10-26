using System;

namespace PhoneBookApp.Core.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<Person> People { get; set; } = new List<Person>();
    }
}
