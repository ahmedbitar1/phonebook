using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneBookApp.Core.Entities
{
    [Table("user_Login")]
    public class UserLogin
    {
        [Key]
        public int serial { get; set; }
        public string User_Name { get; set; }
        public string Password_Encr { get; set; }
    }
}
