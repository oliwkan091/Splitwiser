using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Splitwiser.Models
{
    public class UserModel : IdentityUser
    {
        public UserModel() { }

        public UserModel(string email, string userName)
        {
            Email = email;
            UserName = userName;
        }
    }
}
