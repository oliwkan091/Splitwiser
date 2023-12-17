using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Splitwiser.Models.UserEntity
{
    public class UserEntity : IdentityUser
    {
        public UserEntity() { }

        public UserEntity(string email, string userName)
        {
            Email = email;
            UserName = userName;
        }
    }
}
