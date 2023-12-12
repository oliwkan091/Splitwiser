using System.ComponentModel.DataAnnotations;

namespace Splitwiser.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mail jest wymagany")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Ponowne hasło jest wymagane")]
        public string PasswordAgain { get; set; }
    }
}
