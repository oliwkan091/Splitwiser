using System.ComponentModel.DataAnnotations;
using System;

namespace Splitwiser.Models.PaymentInGroup
{
    public class PaymentInGroupViewModel
    {

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserWhoReturnsId { get; set; }
        public Guid UserToBePaidId { get; set; }
        public double AmountToPay { get; set; }
        public string UserToBePaidName { get; set; }
        public string UserWhoReturnsName { get; set; }
    }
}
