using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Splitwiser.Models.GroupPaymentHistory
{
    public class GroupPaymentHistoryEntity
    {
        public GroupPaymentHistoryEntity()
        {
            TransactionName = "";
            Amount = 0;
        }

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Nazwa grupy jest wymagana")]
        public string TransactionName { get; set; }
        [Required(ErrorMessage = "Podaj kwotę")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kwota musi być dodatnia")]
        public double Amount { get; set; }
        public DateTime AddDate{ get; set; }
    }
}
