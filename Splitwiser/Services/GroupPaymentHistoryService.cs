using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Splitwiser.EntityFramework;
using Splitwiser.Models;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.UserEntity;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Splitwiser.Services
{
    public class GroupPaymentHistoryService : IGroupPaymentHistoryService
    {
        private readonly SplitwiserDbContext _context;
		private readonly UserManager<UserEntity> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GroupPaymentHistoryService(SplitwiserDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<UserEntity> userManager)
        {
            _context = context ?? throw new NullReferenceException();
			_httpContextAccessor = httpContextAccessor ?? throw new NullReferenceException();
			_userManager = userManager ?? throw new NullReferenceException();
		}

        public async Task<GroupPaymentHistoryEntity> Add(GroupPaymentHistoryEntity payment)
        {
			var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

			_context.Payments.Add(payment);
            _context.SaveChanges();
            return payment;
        }

		public List<GroupPaymentHistoryEntity> GetAll()
        {
            var paymentHistory = _context.Payments.ToList();
            return paymentHistory;
        }

		public List<GroupPaymentHistoryViewModel> GetGroupDetails(Guid groupId)
		{
			var paymentHistory = (from payments in _context.Payments
								  join groups in _context.Groups
								  on payments.GroupId equals groups.Id
								  orderby payments.AddDate ascending
								  where groups.Id == groupId
                                  select new GroupPaymentHistoryViewModel
								  {
									Id = payments.Id,
									GroupId = groups.Id,
									GroupName = groups.GroupName,
									UserId = payments.UserId,
									TransactionName = payments.TransactionName,
									Amount = payments.Amount,
									AddDate = payments.AddDate
                                  }).ToList();

			var ids = paymentHistory.Select(user => user.UserId.ToString()).Distinct().ToList();

            var usersTouple = _userManager.Users
            .Where(user => ids.Contains(user.Id))
            .Select(user => new Tuple<string, Guid>(user.UserName, new Guid(user.Id)))
            .ToList();

			paymentHistory.ForEach(
				payment => payment.UserName = usersTouple.FirstOrDefault(el => el.Item2 == payment.UserId).Item1
				);

			return paymentHistory;

        }
	}
}
