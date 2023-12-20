using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Splitwiser.Models
{
    public class GroupPaymentHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string TransactionName { get; set; }
        public double Amount { get; set; }
        public DateTime AddDate { get; set; }
    }
}
