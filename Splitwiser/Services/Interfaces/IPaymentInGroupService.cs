using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.PaymentInGroup;
using Splitwiser.Models.UserGroup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splitwiser.Services.Interfaces
{
    public interface IPaymentInGroupService
    {
        void AddNewMemberPayments(UserGroupEntity payment);
        List<PaymentInGroupEntity> GetAllPaymentsOfUserWhoReturnsToAllItsDebtorsInGroup(Guid userWhoReturns, Guid groupId);
        bool AddPaymentSplit(GroupPaymentHistoryEntity payment, List<UserViewModelCheckbox> userListCheckbox);
        Task<List<PaymentInGroupViewModel>> GetHowMuchCurrentUserOwnToOtherGroupUsers(Guid userId, Guid groupId);
        List<PaymentInGroupEntity> GetAllPayersOfUserToBePaidInGroup(Guid userToBePaidId, Guid groupId);
        void SettleUser(PaymentInGroupViewModel paidUser);
        void DeleteMemberPayments(Guid deletedUserId, Guid groupId);
    }
}
