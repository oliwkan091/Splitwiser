using Splitwiser.Models;
using Splitwiser.Models.GroupPaymentHistory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splitwiser.Services.Interfaces
{
    public interface IGroupPaymentHistoryService
    {
        Task<GroupPaymentHistoryEntity> Add(GroupPaymentHistoryEntity book);
		List<GroupPaymentHistoryEntity> GetAll();
        List<GroupPaymentHistoryViewModel> GetGroupDetails(Guid groupId);
        GroupPaymentHistoryEntity GetPaymentById(Guid paymentId);
        void Update(GroupPaymentHistoryEntity payment);
    }
}
