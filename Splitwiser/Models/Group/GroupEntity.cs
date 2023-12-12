using System;
using System.ComponentModel.DataAnnotations;

namespace Splitwiser.Models.Group
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Nazwa grupy jest wymagana")]
        public string GroupName { get; set; }
    }
}
