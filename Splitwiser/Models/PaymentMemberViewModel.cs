using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Splitwiser.Models.PaymentMember
{
    public class PaymentMemberViewModel
    {
        public PaymentMemberViewModel() { }

        public PaymentMemberViewModel(Guid paymentId, Guid groupId, Guid userId)
        {
            PaymentId = paymentId;
            GroupId = groupId;
            UserId = userId;
        }

        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AddDate { get; set; }
        public bool wasPaid { get; set; }
        public string memberName { get; set; }

    }
}
