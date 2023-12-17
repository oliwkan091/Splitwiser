using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Splitwiser.Models
{
    public class UserViewModel
    {

        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Nazwa nie może zostać pusta")]
        public string UserName { get; set; }
    }
}
