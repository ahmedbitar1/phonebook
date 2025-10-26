using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Core.Entities;
using PhoneBookApp.Core.Interfaces;
using PhoneBookApp.Infrastructure.Data;

namespace PhoneBookApp.Infrastructure.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.People
                    .Include(p => p.Location)
                    .Include(p => p.PhoneNumbers)
                    .ToListAsync();

            searchTerm = searchTerm.Trim();

            return await _context.People
                .Include(p => p.Location).ThenInclude(l => l.Category)
                .Include(p => p.PhoneNumbers)
                .Where(p =>
                    p.FullName.Contains(searchTerm) ||
                    p.Department.Contains(searchTerm) ||
                    (p.Email ?? "").Contains(searchTerm) ||
                    p.Location.Name.Contains(searchTerm) ||
                    p.PhoneNumbers.Any(ph => ph.Number.Contains(searchTerm))
                )
                .ToListAsync();
        }
    }
}
