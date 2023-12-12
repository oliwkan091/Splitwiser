using System.ComponentModel.DataAnnotations;
using System;

namespace Splitwiser.Models.UserGroup
{
    public class UserGroupEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
