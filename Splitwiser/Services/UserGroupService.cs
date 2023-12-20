using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwiser.EntityFramework;
using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.UserEntity;
using Splitwiser.Models.UserGroup;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitwiser.Services
{
    public class UserGroupService: IUserGroupService
    {
        private readonly SplitwiserDbContext _context;
        private readonly UserManager<UserEntity> _userManager;
        //private readonly IPaymentInGroupService _paymentInGroupService;

        public UserGroupService(
            SplitwiserDbContext context,
            UserManager<UserEntity> userManager//,
            //IPaymentInGroupService paymentInGroupService
            )
        {
            _context = context ?? throw new NullReferenceException(nameof(context));
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
            //_paymentInGroupService = paymentInGroupService ?? throw new NullReferenceException(nameof(paymentInGroupService));
        }

        //public List<GroupEntity> GetAll()
        //{
        //    var books = _context.Groups.ToList();
        //    return books;
        //}

        public UserGroupEntity Add(UserGroupEntity user)
        {
            _context.UserGroups.Add(user);
            _context.SaveChanges();
            //_paymentInGroupService.AddUserPayments(user);
            return user;
        }

        public bool IsUserInGroupByIds(Guid userId, Guid groupId)
        {
            var paymentHistory = (from userGroups in _context.UserGroups
                                  where userGroups.UserId == userId
                                  where userGroups.GroupId == groupId
                                  select new UserGroupEntity
                                  {
                                      Id = userGroups.Id
                                  });

            var ids = paymentHistory.Select(user => user.Id.ToString()).Distinct().ToList();
            if (ids.Count > 0)
                return true;

            return false;
        }

        public UserGroupEntity Delete(UserGroupEntity user)
        {
            _context.UserGroups.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public UserGroupEntity GetUserGroupByUserIdAndGroupId(Guid userId, Guid groupId)
        {

            var userGroupList = (from userGroups in _context.UserGroups
                                  where userGroups.UserId == userId
                                  where userGroups.GroupId == groupId
                                  select new UserGroupEntity
                                  {
                                      Id = userGroups.Id,
                                      GroupId = userGroups.GroupId,
                                      UserId = userGroups.UserId
                                  });


            var userGroup = userGroupList.Select(user => user).ToList();
            if (userGroup != null)
            {
                UserGroupEntity userG = userGroup.First();
                return userG;
            }

            return null;
        }

        public List<UserViewModel> GetAllUsersFromGroup(Guid groupId)
        {

            var userGroupList = (from userGroups in _context.UserGroups
                                 where userGroups.GroupId == groupId
                                 select new UserViewModel
                                 {
                                     UserId = userGroups.UserId,
                                 }).ToList();

            //return userGroupList;


 

            var ids = userGroupList.Select(user => user.UserId.ToString()).Distinct().ToList();

            var usersTouple = _userManager.Users
            .Where(user => ids.Contains(user.Id))
            .Select(user => new Tuple<string, Guid>(user.UserName, new Guid(user.Id)))
            .ToList();

            userGroupList.ForEach(
                payment => payment.UserName = usersTouple.FirstOrDefault(el => el.Item2 == payment.UserId).Item1
                );

            return userGroupList;


            //var userGroup = userGroupList.Select(user => user).ToList();
            //if (userGroup != null)
            //{
            //    UserGroupEntity userG = userGroup.First();
            //    return userG;
            //}
            ////_context.UserGroups.Add(user);
            ////_context.SaveChanges();
            //return null;
        }
    }
}
