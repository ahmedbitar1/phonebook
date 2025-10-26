using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Helpers;
using PhoneBookApp.Models;
using PhoneBookApp.Infrastructure.Data;

namespace PhoneBook.Controllers
{
    public class LoginController : Controller
    {
        private readonly MainDbContext _context;
        private readonly AppDbContext _appDbContext;
        private const string EncryptionKey = "12345";

        public LoginController(MainDbContext context , AppDbContext appDbContext)
        {
            _context = context;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string encryptedPassword = EncryptionHelper.EncryptTripleDES(model.Password, EncryptionKey);

            var user = await _context.UserLogin
                .FirstOrDefaultAsync(u => u.User_Name.ToLower() == model.Username.ToLower()
                                          && u.Password_Encr == encryptedPassword);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.User_Name);

                // ✅ بدل الليست — نقرأ من جدول Admin
                bool isAdmin = await _appDbContext.Admins
                    .AnyAsync(a => a.User_Name.ToLower() == user.User_Name.ToLower());

                HttpContext.Session.SetString("IsAdmin", isAdmin ? "true" : "false");

                return RedirectToAction("Index", "People");
            }

            ViewBag.Error = "Invalid username or password";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
