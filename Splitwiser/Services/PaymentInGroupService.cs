using AutoMapper;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwiser.EntityFramework;
using Splitwiser.EntityFramework.Migrations.Splitwiser;
using Splitwiser.Models;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.PaymentInGroup;
using Splitwiser.Models.UserEntity;
using Splitwiser.Models.UserGroup;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Splitwiser.Services
{
    public class PaymentInGroupService : IPaymentInGroupService
    {
        private readonly SplitwiserDbContext _context;
        private readonly IUserGroupService _userGroupService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;


        public PaymentInGroupService(
            SplitwiserDbContext context,
            IUserGroupService userGroupService,
            UserManager<UserEntity> userManager,
            IMapper mapper
            )
        {
            _context = context ?? throw new NullReferenceException(nameof(context));
            _userGroupService = userGroupService ?? throw new NullReferenceException(nameof(userGroupService));
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
            _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        }

        public List<PaymentInGroupEntity> GetAllPayersOfUserToBePaidInGroup(Guid userToBePaidId, Guid groupId)
        {
            var paymentHistory = _context.PaymentInGroups
                             .Where(pm => pm.GroupId == groupId)
                             .Where(pm => pm.UserToBePaidId == userToBePaidId)
                             .ToList();

            return paymentHistory;
        }

        public List<PaymentInGroupEntity> GetAllPaymentsOfUserWhoReturnsToAllItsDebtorsInGroup(Guid userWhoReturns, Guid groupId)
        {
            var paymentHistory = _context.PaymentInGroups
                             .Where(pm => pm.GroupId == groupId)
                             .Where(pm => pm.UserWhoReturnsId == userWhoReturns)
                             .ToList();

            return paymentHistory;
        }

        public bool AddPaymentSplit(GroupPaymentHistoryEntity payment, List<UserViewModelCheckbox> userListCheckbox)
        {
            //_context.PaymentInGroups.AddRange(payment);
            if (userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0)
            {
                return false;
            }
            double dividedAmount = payment.Amount / (userListCheckbox.Where(pm => pm.IsChecked == true).Count());

            var usersToUpdateUserToBePaidId = GetAllPayersOfUserToBePaidInGroup(payment.UserId, payment.GroupId);
            //var payer = userListCheckbox.Where(u => u.UserId == payment.UserId);
            //if (payer != null)
            //{
            //    userListCheckbox.Remove(payer.First());
            //}

            foreach (var user in userListCheckbox)
            {
                var userToUpdate = usersToUpdateUserToBePaidId.Where(u => u.UserWhoReturnsId == user.UserId);
                if (userToUpdate.Count() > 0)
                {
                    userToUpdate.First().AmountToPay += dividedAmount;
                }
            }

            var usersToUpdateUserWhoReturns = GetAllPaymentsOfUserWhoReturnsToAllItsDebtorsInGroup(payment.UserId, payment.GroupId);

            foreach (var user in userListCheckbox)
            {
                var userToUpdate = usersToUpdateUserWhoReturns.Where(u => u.UserToBePaidId == user.UserId);
                if (userToUpdate.Count() > 0)
                {
                    userToUpdate.First().AmountToPay += -dividedAmount;
                }
            }
            _context.SaveChanges();

            return true;
        }

        public void AddNewMemberPayments(UserGroupEntity addedUser)
        {
            var usersInGroup = _userGroupService.GetAllUsersFromGroup(addedUser.GroupId);
            
            foreach(var user in usersInGroup) 
            {
                if (user.UserId != addedUser.UserId)
                {
                    _context.PaymentInGroups.Add(new PaymentInGroupEntity(addedUser.GroupId, user.UserId, addedUser.UserId, 0));
                    _context.PaymentInGroups.Add(new PaymentInGroupEntity(addedUser.GroupId, addedUser.UserId, user.UserId, 0));
                }
            }
            _context.SaveChanges();
        }

        public async Task<List<PaymentInGroupViewModel>> GetHowMuchCurrentUserOwnToOtherGroupUsers(Guid userId, Guid groupId)
        {
            var payments = _context.PaymentInGroups
                 .Where(pm => pm.GroupId == groupId)
                 .Where(pm => pm.UserToBePaidId == userId)
                 .ToList();


            var paymentsViewModel = _mapper.Map<List<PaymentInGroupEntity>, List<PaymentInGroupViewModel>>(payments);
            foreach (var payment in paymentsViewModel)
            {
                payment.UserToBePaidName = (await _userManager.FindByIdAsync(payment.UserToBePaidId.ToString()))?.UserName;
            }

            return paymentsViewModel;
        }

        public void SettleUser(PaymentInGroupViewModel paidUser)
        {
            var paymentForUserWhoIsBeingPaidList = _context.PaymentInGroups
                 .Where(pm => pm.GroupId == paidUser.GroupId)
                 .Where(pm => pm.UserToBePaidId == paidUser.UserToBePaidId)
                 .Where(pm => pm.UserWhoReturnsId == paidUser.UserWhoReturnsId)
                 .ToList();

            var paymentForUserWhoPaysList = _context.PaymentInGroups
                 .Where(pm => pm.GroupId == paidUser.GroupId)
                 .Where(pm => pm.UserToBePaidId == paidUser.UserWhoReturnsId)
                 .Where(pm => pm.UserWhoReturnsId == paidUser.UserToBePaidId)
                 .ToList();

            if (paymentForUserWhoIsBeingPaidList.Count == 0 || paymentForUserWhoPaysList.Count == 0)
                return;

            var paymentForUserWhoIsBeingPaid = paymentForUserWhoIsBeingPaidList.First();
            paymentForUserWhoIsBeingPaid.AmountToPay = 0;
            var paymentForUserWhoPays = paymentForUserWhoPaysList.First();
            paymentForUserWhoPays.AmountToPay = 0;

            _context.PaymentInGroups.Update(paymentForUserWhoIsBeingPaid);
            _context.PaymentInGroups.Update(paymentForUserWhoPays);
            _context.SaveChanges();
        }
    }
}
