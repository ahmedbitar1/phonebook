using PhoneBookApp.Application.Interfaces;
using PhoneBookApp.Core.Entities;
using PhoneBookApp.Core.Interfaces;

namespace PhoneBookApp.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _personRepository.GetAllAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _personRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Person person)
        {
            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Person person)
        {
            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person != null)
            {
                _personRepository.Delete(person);
                await _personRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Person>> SearchAsync(string searchTerm)
        {
            return await _personRepository.SearchAsync(searchTerm);
        }
    }
}
