using Splitwiser.EntityFramework;
using Splitwiser.Models;
using Splitwiser.Models.Group;
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

        public UserGroupService(SplitwiserDbContext context)
        {
            _context = context ?? throw new NullReferenceException();
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
            return user;
        }
    }
}
