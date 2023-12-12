using Splitwiser.Models.Group;
using System.Collections.Generic;

namespace Splitwiser.Services.Interfaces
{
    public interface IGroupService
	{
		GroupEntity Add(GroupEntity book);
		List<GroupEntity> GetAll();
	}
}
