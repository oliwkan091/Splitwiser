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
        List<PaymentMemberEntity> GetAllPaymentMemberOfGroups(Guid groupId);
    }
}
