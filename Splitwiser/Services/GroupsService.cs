using Splitwiser.EntityFramework;
using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitwiser.Services
{
    public class GroupService : IGroupService
    {
        private readonly SplitwiserDbContext _context;

        public GroupService(SplitwiserDbContext context)
        {
            _context = context ?? throw new NullReferenceException();
        }

        public List<GroupEntity> GetAll()
        {
            var books = _context.Groups.ToList();
            return books;
        }

        public GroupEntity Add(GroupEntity book)
        {
            _context.Groups.Add(book);
            _context.SaveChanges();
            return book;
        }
    }
}
