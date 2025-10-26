using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneBook.Application.Interfaces;
using PhoneBook.Application.Services;
using PhoneBook.Core.Interfaces;
using PhoneBook.Infrastructure.Data;
using PhoneBook.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneBook.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Database Connection
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer("Server=10.10.200.23\\bek_sql2;Database=PhoneBook_DB;User Id=sa;Password=20@dminPa$$13;TrustServerCertificate=True;"));

                    // Repositories
                    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                    services.AddScoped<IPersonRepository, PersonRepository>();

                    // Services
                    services.AddScoped<IPersonService, PersonService>();
                })
                .Build();

            var personService = host.Services.GetRequiredService<IPersonService>();

            Console.WriteLine("=== PhoneBook CRUD Demo ===\n");

            // 1️⃣ Create
            var newPerson = new PhoneBook.Core.Entities.Person
            {
                Department = "IT Department",
                FullName = "Ahmed Essam",
                Email = "ahmed@company.com",
                LocationId = 1,
                PhoneNumbers = new List<PhoneBook.Core.Entities.PhoneNumber>
                {
                    new PhoneBook.Core.Entities.PhoneNumber { Number = "01000000000" },
                    new PhoneBook.Core.Entities.PhoneNumber { Number = "01111111111" }
                }
            };

            await personService.AddAsync(newPerson);
            Console.WriteLine("✅ Added person successfully!\n");

            // 2️⃣ Read / Search
            Console.Write("Enter search term: ");
            var term = Console.ReadLine();
            var results = await personService.SearchAsync(term!);

            Console.WriteLine("\n=== Search Results ===");
            foreach (var person in results)
            {
                Console.WriteLine($"{person.Id}. {person.FullName} | {person.Department} | {person.Email}");
                foreach (var phone in person.PhoneNumbers)
                    Console.WriteLine($"  - {phone.Number}");
            }

            // 3️⃣ Update
            Console.Write("\nEnter Person ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int updateId))
            {
                var personToUpdate = await personService.GetByIdAsync(updateId);
                if (personToUpdate != null)
                {
                    Console.Write("Enter new department name: ");
                    var newDept = Console.ReadLine();

                    personToUpdate.Department = newDept ?? personToUpdate.Department;
                    await personService.UpdateAsync(personToUpdate);

                    Console.WriteLine("✅ Person updated successfully!\n");
                }
                else
                {
                    Console.WriteLine("⚠️ Person not found.\n");
                }
            }

            // 4️⃣ Delete
            Console.Write("Enter Person ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int deleteId))
            {
                var personToDelete = await personService.GetByIdAsync(deleteId);
                if (personToDelete != null)
                {
                    await personService.DeleteAsync(personToDelete.Id);
                    Console.WriteLine("✅ Person deleted successfully!\n");
                }
                else
                {
                    Console.WriteLine("⚠️ Person not found.\n");
                }
            }

            // 5️⃣ Display All
            Console.WriteLine("\n=== All People in PhoneBook ===");
            var allPeople = await personService.GetAllAsync();
            foreach (var person in allPeople)
            {
                Console.WriteLine($"{person.Id}. {person.FullName} | {person.Department} | {person.Email}");
                foreach (var phone in person.PhoneNumbers)
                    Console.WriteLine($"  - {phone.Number}");
            }

            Console.WriteLine("\nDone ✅");
        }
    }
}
