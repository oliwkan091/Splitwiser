using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.UserGroup;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splitwiser.Controllers
{
    [Controller]
    [Route("group")]
    [Authorize]
    public class GroupController : Controller
    {
		private readonly IGroupService _groupService;
        private readonly IGroupPaymentHistoryService _groupPaymentHistoryService;
        private readonly UserManager<UserModel> _userManager;
        private readonly IUserGroupService _userGroupManager;

        public GroupController(
            IGroupService groupService,
            IGroupPaymentHistoryService groupPaymentHistoryService,
            UserManager<UserModel> userManager,
            IUserGroupService userGroupManager
            )
		{
			_groupService = groupService ?? throw new NullReferenceException(nameof(groupService));
            _groupPaymentHistoryService = groupPaymentHistoryService ?? throw new NullReferenceException(nameof(groupPaymentHistoryService));
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
            _userGroupManager = userGroupManager ?? throw new NullReferenceException(nameof(userGroupManager));
        }

		[HttpGet]
        public IActionResult Index()
        {
			var groups = _groupService.GetAll();
			return View(groups);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

		[HttpPost("create")]
		public async Task<IActionResult> Create(GroupEntity group)
		{
			if (!ModelState.IsValid)
			{
				return View(group);
			}

            var createdGroup = _groupService.Add(group);
            var userGroup = new UserGroupEntity();
            userGroup.GroupId = createdGroup.Id;
            var currentUser = await _userManager.GetUserAsync(User);
            userGroup.UserId = new Guid(currentUser.Id);

            _userGroupManager.Add(userGroup);
            return RedirectToAction("Index");
		}

        [HttpGet("chosenGroup/{id}")]
        public IActionResult ChosenGroup(Guid id)
        {
            var model = new Tuple<List<GroupPaymentHistoryViewModel>, Guid> (_groupPaymentHistoryService.GetGroupDetails(), id);
            return View(model);
        }

		[HttpGet("chosenGroup/{id}/addPayment")]
		public IActionResult AddPayment(Guid id)
		{
            return View(id);
		}

		[HttpPost("chosenGroup/{Gid}/addPayment")]
											//UWAGA jeżeli Guid id będzie się nazywać tak samo
											//co Guid w innym przesyłanym modelu to zostanie podpięte pod jedno 
		public async Task<IActionResult> AddPayment(Guid gid, GroupPaymentHistoryEntity payment)
		{
            var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null) throw new NullReferenceException(nameof(currentUser));
			if (currentUser.Id == null) throw new NullReferenceException(nameof(currentUser.Id));
  
            payment.UserId = new Guid(currentUser.Id);
            payment.GroupId = gid;
			var paymentHistory = _groupPaymentHistoryService.Add(payment);
			string apiUrl = $"chosenGroup/{gid}";

			return RedirectToAction($"chosenGroup", "Group", new { id = gid });

        }
	}
}
