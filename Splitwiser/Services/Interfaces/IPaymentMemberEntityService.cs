using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.PaymentMember;
using Splitwiser.Models.UserGroup;
using System;
using System.Collections.Generic;

namespace Splitwiser.Services.Interfaces
{
    public interface IPaymentMemberEntityService
    {
        PaymentMemberEntity Add(PaymentMemberEntity paymentMember);
        List<PaymentMemberEntity> GetAllPaymentMemberOfGroup(Guid groupId);
        List<PaymentMemberEntity> GetAllPaymentMemberOfPayment(Guid paymentId);
        List<PaymentMemberEntity> GetUserPaymentInGroup(Guid groupId, Guid userId);
        void SettleUserInPayment(Guid userId, Guid groupId);
        PaymentMemberEntity Update(PaymentMemberEntity paymentMember);
    }
}
