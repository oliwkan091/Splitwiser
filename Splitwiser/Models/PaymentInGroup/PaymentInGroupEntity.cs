using System.ComponentModel.DataAnnotations;
using System;

namespace Splitwiser.Models.PaymentInGroup
{
    public class PaymentInGroupEntity
    {
        public PaymentInGroupEntity()
        {
        }

        public PaymentInGroupEntity(Guid groupId, Guid userWhoReturnsId, Guid userToBePaidId, double amountToPay)
        {
            GroupId = groupId;
            UserWhoReturnsId = userWhoReturnsId;
            UserToBePaidId = userToBePaidId;
            AmountToPay = amountToPay;
        }

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserWhoReturnsId { get; set; }
        public Guid UserToBePaidId { get; set; }
        public double AmountToPay { get; set; }
    }
}
