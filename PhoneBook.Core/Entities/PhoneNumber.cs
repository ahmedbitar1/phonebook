namespace PhoneBookApp.Core.Entities
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}
