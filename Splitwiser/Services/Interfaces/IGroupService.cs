using Splitwiser.Models;
using Splitwiser.Models.Group;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splitwiser.Services.Interfaces
{
    public interface IGroupService
	{
		GroupEntity Add(GroupEntity book);
		List<GroupEntity> GetAll();
        List<UserViewModel> GetAllUsersFromGroup(Guid groupId);
        Task<List<GroupEntity>> GetGroupsOfUser();
    }
}
