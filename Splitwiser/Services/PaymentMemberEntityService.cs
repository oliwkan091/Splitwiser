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

        public List<PaymentMemberEntity> GetAllPaymentMemberOfGroups(Guid groupId)
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



            //var ids = paymentHistory.Select(user => user.UserId.ToString()).Distinct().ToList();

            //var usersTouple = _userManager.Users
            //.Where(user => ids.Contains(user.Id))
            //.Select(user => new Tuple<string, Guid>(user.UserName, new Guid(user.Id)))
            //.ToList();

            //paymentHistory.ForEach(
            //    payment => payment.UserName = usersTouple.FirstOrDefault(el => el.Item2 == payment.UserId).Item1
            //    );

            return paymentHistory;

        }

    }
}
