using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Splitwiser.EntityFramework;
using Splitwiser.EntityFramework.Migrations.Splitwiser;
using Splitwiser.Models;
using Splitwiser.Models.PaymentMember;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Splitwiser.Services
{
    public class PaymentMemberEntityService : IPaymentMemberEntityService
    {
        private readonly SplitwiserDbContext _context;

		public PaymentMemberEntityService(SplitwiserDbContext context)
        {
            _context = context ?? throw new NullReferenceException();
        }

        public PaymentMemberEntity Add(PaymentMemberEntity paymentMember)
        {
            _context.PaymentMembers.Add(paymentMember);
            _context.SaveChanges();
            return paymentMember;
        }

        public PaymentMemberEntity Update(PaymentMemberEntity paymentMember)
        {
            _context.PaymentMembers.Update(paymentMember);
            _context.SaveChanges();
            return paymentMember;
        }

        public List<PaymentMemberEntity> GetAllPaymentMemberOfGroup(Guid groupId)
        {
            var paymentHistory = (from paymentMembers in _context.PaymentMembers
                                  where paymentMembers.GroupId == groupId
                                  select new PaymentMemberEntity
                                  {
                                      Id = paymentMembers.Id,
                                      GroupId = paymentMembers.GroupId,
                                      UserId = paymentMembers.UserId,
                                      AddDate = paymentMembers.AddDate,
                                      PaymentId = paymentMembers.PaymentId,
                                      wasPaid = paymentMembers.wasPaid

                                  }).ToList();

            return paymentHistory;
        }

        public List<PaymentMemberEntity> GetAllPaymentMemberOfGroupWithNames(Guid groupId)
        {
            var paymentHistory = (from paymentMembers in _context.PaymentMembers
                                  where paymentMembers.GroupId == groupId
                                  select new PaymentMemberEntity
                                  {
                                      Id = paymentMembers.Id,
                                      GroupId = paymentMembers.GroupId,
                                      UserId = paymentMembers.UserId,
                                      AddDate = paymentMembers.AddDate,
                                      PaymentId = paymentMembers.PaymentId,
                                      wasPaid = paymentMembers.wasPaid

                                  }).ToList();

            return paymentHistory;
        }

        public List<PaymentMemberEntity> GetAllPaymentMemberOfPayment(Guid paymentId)
        {
            var paymentHistory = (from paymentMembers in _context.PaymentMembers
                                  where paymentMembers.PaymentId == paymentId
                                  select new PaymentMemberEntity
                                  {
                                      Id = paymentMembers.Id,
                                      GroupId = paymentMembers.GroupId,
                                      UserId = paymentMembers.UserId,
                                      AddDate = paymentMembers.AddDate,
                                      PaymentId = paymentMembers.PaymentId,
                                      wasPaid = paymentMembers.wasPaid

                                  }).ToList();

            return paymentHistory;
        }

        public List<PaymentMemberEntity> GetUserPaymentInGroup(Guid groupId, Guid userId)
        {
            var paymentHistoryOfUser = _context.PaymentMembers
                 .Where(pm => pm.UserId == userId)
                 .Where(pm => pm.GroupId == groupId)
                 .ToList();

            return paymentHistoryOfUser;
        }

        public void SettleUserInPayment(Guid userId, Guid groupId)
        {
            var userPaymentsInGroup = GetUserPaymentInGroup(groupId, userId);

            foreach(var payment in userPaymentsInGroup)
            {
                payment.wasPaid = true;
                Update(payment);
            }
        }

    }
}
