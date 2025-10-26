using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Application.Interfaces;
using PhoneBookApp.Core.Entities;
using PhoneBookApp.Infrastructure.Data;

namespace PhoneBookApp.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IPersonService _personService;
        private readonly AppDbContext _context;

        public PeopleController(IPersonService personService, AppDbContext context)
        {
            _personService = personService;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                context.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index(string search, int? locationId, int pageNumber = 1, int pageSize = 10)
        {
            // load locations for dropdown
            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name");
            ViewBag.Search = search;
            ViewBag.LocationId = locationId;
            ViewBag.PageNumber = pageNumber;

            var query = _context.People
                .Include(p => p.Location)
                .Include(p => p.PhoneNumbers)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(p =>
                    p.FullName.Contains(search) ||
                    p.Department.Contains(search) ||
                    p.Email.Contains(search) ||
                    p.PhoneNumbers.Any(ph => ph.Number.Contains(search)) ||
                    p.Location.Name.Contains(search)
                );
            }

            if (locationId.HasValue)
            {
                query = query.Where(p => p.LocationId == locationId.Value);
            }

            int totalRecords = await query.CountAsync();
            double totalPages = Math.Ceiling((double)totalRecords / pageSize);

            var pagedData = await query
                .OrderBy(p => p.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            ViewBag.IsAdmin = isAdmin;
            return View(pagedData);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchQuery, int? locationId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.People
                .Include(p => p.Location)
                .Include(p => p.PhoneNumbers)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                query = query.Where(p =>
                    p.FullName.Contains(searchQuery) ||
                    p.Department.Contains(searchQuery) ||
                    p.Email.Contains(searchQuery) ||
                    p.Location.Name.Contains(searchQuery) ||
                    p.PhoneNumbers.Any(ph => ph.Number.Contains(searchQuery))
                );
            }

            // ✅ هنا بنضيف الفلترة بالموقع
            if (locationId.HasValue)
            {
                query = query.Where(p => p.LocationId == locationId.Value);
            }

            int totalRecords = await query.CountAsync();
            double totalPages = Math.Ceiling((double)totalRecords / pageSize);

            var people = await query
                .OrderBy(p => p.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return PartialView("_PeopleTable", people);
        }

        // ✅ Create GET
        public async Task<IActionResult> Create()
        {
            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person, List<string> phoneNumbers)
        {
            if (await _context.People.AnyAsync(p => p.Email == person.Email))
            {
                ModelState.AddModelError("Email", "This email already exists.");
            }

            var duplicatePhones = new List<string>();

            foreach (var number in phoneNumbers.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                if (await _context.PhoneNumbers.AnyAsync(p => p.Number == number))
                {
                    duplicatePhones.Add(number);
                    ModelState.AddModelError("", $"Phone number {number} already exists.");
                }
            }

            // ✅ مهم جداً: لو في Error، لازم نرجع نفس الـ phoneNumbers علشان تفضل ظاهرة بعد Reload
            if (!ModelState.IsValid)
            {
                ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name", person.LocationId);
                ViewBag.PhoneNumbers = phoneNumbers; // دي اللي بترجع الأرقام
                return View(person);
            }

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            foreach (var number in phoneNumbers.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                _context.PhoneNumbers.Add(new PhoneNumber { Number = number, PersonId = person.Id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Edit GET
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _context.People
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return NotFound();

            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", person.LocationId);
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person, List<string> phoneNumbers)
        {
            if (id != person.Id)
                return NotFound();

            if (await _context.People.AnyAsync(p => p.Email == person.Email && p.Id != person.Id))
            {
                ModelState.AddModelError("Email", "This email already exists.");
            }

            var duplicatePhones = new List<string>();

            foreach (var number in phoneNumbers.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                if (await _context.PhoneNumbers.AnyAsync(p => p.Number == number && p.PersonId != person.Id))
                {
                    duplicatePhones.Add(number);
                    ModelState.AddModelError("", $"Phone number {number} already exists.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name", person.LocationId);
                ViewBag.PhoneNumbers = phoneNumbers;
                return View(person);
            }

            try
            {
                _context.Update(person);
                await _context.SaveChangesAsync();

                // احذف الأرقام القديمة وارجع ضيف الجديدة
                var existingPhones = _context.PhoneNumbers.Where(p => p.PersonId == person.Id);
                _context.PhoneNumbers.RemoveRange(existingPhones);

                foreach (var number in phoneNumbers.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    _context.PhoneNumbers.Add(new PhoneNumber { Number = number, PersonId = person.Id });
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.People.Any(e => e.Id == person.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var person = await _context.People
                .Include(p => p.Location)
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return NotFound();
            return View(person);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.People
                .Include(p => p.Location)
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _personService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
