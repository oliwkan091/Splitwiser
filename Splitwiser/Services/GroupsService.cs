using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Splitwiser.EntityFramework;
using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.UserEntity;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Splitwiser.Services
{
    public class GroupService : IGroupService
    {
        private readonly SplitwiserDbContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupService(
            SplitwiserDbContext context,
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _context = context ?? throw new NullReferenceException();
            _userManager = userManager ?? throw new NullReferenceException();
            _httpContextAccessor = httpContextAccessor ?? throw new NullReferenceException(nameof(httpContextAccessor));
        }

        public List<GroupEntity> GetAll()
        {
            var groups = _context.Groups.ToList();
            return groups;
        }

        public async Task<List<GroupEntity>> GetGroupsOfUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var groupsToReturn = (
                from groups in _context.Groups
                join userGroups in _context.UserGroups
                on groups.Id equals userGroups.GroupId
                where userGroups.UserId == new Guid(user.Id)
                orderby userGroups.AddDate
                select groups

            ).ToList();


            //var groups = _context.Groups.ToList();
            return groupsToReturn;
        }

        public GroupEntity Add(GroupEntity group)
        {
            group.AddDate = DateTime.Now;
            _context.Groups.Add(group);
            _context.SaveChanges();
            return group;
        }

        public List<UserViewModel> GetAllUsersFromGroup(Guid groupId)
        {
            var groupMembers = (
                                              from groups in _context.Groups
                                              join userGroups in _context.UserGroups
                                              on groups.Id equals userGroups.GroupId
                                              where groups.Id == groupId
                                              select new UserViewModel
                                              {
                                                  UserId = userGroups.UserId
                                              }).ToList();

            var ids = groupMembers.Select(user => user.UserId.ToString()).Distinct().ToList();

            var usersTouple = _userManager.Users
                  .Where(user => ids.Contains(user.Id))
                  .Select(user => new Tuple<string, Guid>(user.UserName, new Guid(user.Id)))
                  .ToList();

            groupMembers.ForEach(
                payment => payment.UserName = usersTouple.FirstOrDefault(el => el.Item2 == payment.UserId).Item1
                );

            return groupMembers;
        }
    }
}
