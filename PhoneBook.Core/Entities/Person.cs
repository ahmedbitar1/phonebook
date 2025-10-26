namespace PhoneBookApp.Core.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string Department { get; set; } = null!;  
        public string FullName { get; set; } = null!;    
        public string? Email { get; set; }               
        public int LocationId { get; set; }
        public Location? Location { get; set; }
        public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    }
}
