using System.ComponentModel.DataAnnotations;

namespace PhoneBookApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال اسم المستخدم")]
        public string Username { get; set; }

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
