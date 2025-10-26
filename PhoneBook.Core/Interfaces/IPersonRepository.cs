using PhoneBookApp.Core.Entities;

namespace PhoneBookApp.Core.Interfaces
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Task<IEnumerable<Person>> SearchAsync(string searchTerm);
    }
}
