using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.UserGroup;
using System;
using System.Collections.Generic;

namespace Splitwiser.Services.Interfaces
{
    public interface IUserGroupService
    {
        UserGroupEntity Add(UserGroupEntity user);
        UserGroupEntity Delete(UserGroupEntity user);
        List<UserViewModel> GetAllUsersFromGroup(Guid groupId);
        UserGroupEntity GetUserGroupByUserIdAndGroupId(Guid userId, Guid groupId);
        bool IsUserInGroupByIds(Guid userId, Guid groupId);
    }
}
