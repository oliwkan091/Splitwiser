using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Splitwiser.Models;
using Splitwiser.Models.Group;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.PaymentInGroup;
using Splitwiser.Models.PaymentMember;
using Splitwiser.Models.UserEntity;
using Splitwiser.Models.UserGroup;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
        private readonly UserManager<UserEntity> _userManager;
        private readonly IUserGroupService _userGroupManager;
        private readonly IMapper _mapper;
        private readonly IPaymentMemberEntityService _paymentMemberEntityService;
        private readonly IPaymentInGroupService _paymentInGroupService;

        public GroupController(
            IGroupService groupService,
            IGroupPaymentHistoryService groupPaymentHistoryService,
            UserManager<UserEntity> userManager,
            IUserGroupService userGroupManager,
            IMapper mapper,
            IPaymentMemberEntityService paymentMemberEntityService,
            IPaymentInGroupService paymentInGroupService
            )
        {
            _groupService = groupService ?? throw new NullReferenceException(nameof(groupService));
            _groupPaymentHistoryService = groupPaymentHistoryService ?? throw new NullReferenceException(nameof(groupPaymentHistoryService));
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
            _userGroupManager = userGroupManager ?? throw new NullReferenceException(nameof(userGroupManager));
            _mapper = mapper;
            _paymentMemberEntityService = paymentMemberEntityService ?? throw new NullReferenceException(nameof(paymentMemberEntityService));
            _paymentInGroupService = paymentInGroupService ?? throw new NullReferenceException(nameof(paymentInGroupService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var groups = await _groupService.GetGroupsOfUser();
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
            _paymentInGroupService.AddNewMemberPayments(userGroup);
            return RedirectToAction("Index");
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> ChosenGroup(Guid groupId)
        {
            var groupPaymentHistory = _groupPaymentHistoryService.GetGroupDetails(groupId);
            //var groupPaymentHistoryMapped = _mapper.Map<List<GroupPaymentHistoryViewModel>, List<GroupPaymentHistoryEntity>>(groupPaymentHistory);
            //var paymentMemberEntity = _paymentMemberEntityService.GetAllPaymentMemberOfGroups(groupId);
            //var currentUser = await _userManager.GetUserAsync(User);

            var paymentMembersOfGroup = _paymentMemberEntityService.GetAllPaymentMemberOfGroup(groupId);
            var paymentMembersOfGroupWithNames = _mapper.Map<List<PaymentMemberEntity>, List<PaymentMemberViewModel>>(paymentMembersOfGroup);
            foreach (var element in paymentMembersOfGroupWithNames)
            { 
                var user = await _userManager.FindByIdAsync(element.UserId.ToString());
                element.memberName = user.UserName;
            }
			var settlements = await _paymentInGroupService.GetHowMuchCurrentUserOwnToOtherGroupUsers(new Guid(_userManager.GetUserAsync(User).Result.Id), groupId);
            var model = new Tuple<List<GroupPaymentHistoryViewModel>, Guid, List<PaymentInGroupViewModel>, List<PaymentMemberViewModel>>(groupPaymentHistory, groupId, settlements, paymentMembersOfGroupWithNames);
            return View(model);
        }

        [HttpGet("addPayment/{id}")]
        public IActionResult addPayment(Guid id)
        {
            var usersFromGroup = _userGroupManager.GetAllUsersFromGroup(id);
            var usersFromGroupCheckbox = _mapper.Map<List<UserViewModel>, List<UserViewModelCheckbox>>(usersFromGroup);
            bool wasAddingSuccessful = true;
            var models = new Tuple<Guid, List<UserViewModelCheckbox>, bool>(id, usersFromGroupCheckbox, wasAddingSuccessful);
            return View(models);
        }

        [HttpPost("addPayment/{Gid}")]
        //UWAGA jeżeli Guid id będzie się nazywać tak samo
        //co Guid w innym przesyłanym modelu to zostanie podpięte pod jedno 
        public async Task<IActionResult> AddPayment(Guid gid, GroupPaymentHistoryEntity payment, List<UserViewModelCheckbox> userListCheckbox)
        {
            if (userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0 || payment.TransactionName == null || payment.TransactionName.Length < 1 || !(payment.Amount > 0))
            {
                var usersFromGroup = _userGroupManager.GetAllUsersFromGroup(gid);
                var usersFromGroupCheckbox = _mapper.Map<List<UserViewModel>, List<UserViewModelCheckbox>>(usersFromGroup);
                bool wasAddingSuccessful = true;
                if (userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0)
                {
                    wasAddingSuccessful = false;
                }
                var models = new Tuple<Guid, List<UserViewModelCheckbox>, bool>(gid, usersFromGroupCheckbox, wasAddingSuccessful);
                return View(models);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) throw new NullReferenceException(nameof(currentUser));
            if (currentUser.Id == null) throw new NullReferenceException(nameof(currentUser.Id));

            payment.UserId = new Guid(currentUser.Id);
            payment.GroupId = gid;
            var paymentHistory = _groupPaymentHistoryService.Add(payment);

            foreach (var user in userListCheckbox)
            {
                var paymentMember = new PaymentMemberEntity(payment.Id, gid, user.UserId);
                if (user.IsChecked)
                {
                    _paymentMemberEntityService.Update(paymentMember);
                }
            }

            _paymentInGroupService.AddPaymentSplit(payment, userListCheckbox);

            return RedirectToAction($"{gid}", "group");
        }

        [HttpGet("editPayment/{groupId}")]
        public IActionResult editPayment(Guid groupId, Guid paymentId)
        {
            var usersFromGroup = _userGroupManager.GetAllUsersFromGroup(groupId);
            var usersFromGroupCheckbox = _mapper.Map<List<UserViewModel>, List<UserViewModelCheckbox>>(usersFromGroup);

            var paymentMembers = _paymentMemberEntityService.GetAllPaymentMemberOfPayment(paymentId);

            var payment = _groupPaymentHistoryService.GetPaymentById(paymentId);

            //var usersFromGroupCheckboxWithChecks = usersFromGroupCheckbox.Select(member =>
            //{
            //    member.IsChecked = paymentMembers.Where(payment => payment.UserId == member.UserId).
            //    member.IsChecked = paymentMembers.wasP
            //    return member;
            //}
            //).ToList(); ;

            foreach (var userFromGroup in usersFromGroupCheckbox)
            {
                var matchingPaymentMember = paymentMembers.FirstOrDefault(pm => pm.UserId == userFromGroup.UserId);
                if (matchingPaymentMember != null)
                {
                    userFromGroup.wasPaid = matchingPaymentMember.wasPaid;
                    userFromGroup.IsChecked = true;
                }
            }

            bool wasAddingSuccessful = true;
            var models = new Tuple<Guid, List<UserViewModelCheckbox>, bool, GroupPaymentHistoryEntity>(groupId, usersFromGroupCheckbox, wasAddingSuccessful, payment);
            return View(models);
        }

        [HttpPost("editPayment/{groupId}")]
        //UWAGA jeżeli Guid id będzie się nazywać tak samo
        //co Guid w innym przesyłanym modelu to zostanie podpięte pod jedno 
        public async Task<IActionResult> EditPayment(Guid groupId, GroupPaymentHistoryEntity payment, List<UserViewModelCheckbox> userListCheckbox)
        {
            if (userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0 || payment.TransactionName == null || payment.TransactionName.Length < 1 || !(payment.Amount > 0))
            {
                //var usersFromGroup = _userGroupManager.GetAllUsersFromGroup(groupId);
                //var usersFromGroupCheckbox = _mapper.Map<List<UserViewModel>, List<UserViewModelCheckbox>>(usersFromGroup);
                bool wasAddingSuccessful = true;
                if (userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0)
                {
                    wasAddingSuccessful = false;
                }
                var models = new Tuple<Guid, List<UserViewModelCheckbox>, bool>(groupId, userListCheckbox, wasAddingSuccessful);
                return View(models);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) throw new NullReferenceException(nameof(currentUser));
            if (currentUser.Id == null) throw new NullReferenceException(nameof(currentUser.Id));

            payment.UserId = new Guid(currentUser.Id);
            payment.GroupId = groupId;
            _groupPaymentHistoryService.Update(payment);

            foreach (var user in userListCheckbox)
            {
                var paymentMember = new PaymentMemberEntity(payment.Id, groupId, user.UserId);
                if (user.IsChecked)
                {
                    _paymentMemberEntityService.Update(paymentMember);
                }
            }

            _paymentInGroupService.AddPaymentSplit(payment, userListCheckbox);

            //foreach (var user in userListCheckbox)
            //{
            //    var paymentMember = new PaymentMemberEntity(payment.Id, gid, user.UserId);
            //    if (user.IsChecked)
            //    {
            //        _paymentMemberEntityService.Update(paymentMember);
            //    }
            //}

            //_paymentInGroupService.AddPaymentSplit(payment, userListCheckbox);

            return RedirectToAction($"{groupId}", "group");
        }

        [HttpGet("addMember/{id}")]
        public IActionResult AddMember(Guid id)
        {
            var members = _groupService.GetAllUsersFromGroup(id);
            var models = new Tuple<Guid, UserEntity, List<UserViewModel>>(id, null, members);
            return View(models);
        }

        [HttpPost("addMember/{id}")]
        //Nazwa obiektu musi byc taka sama jak tego wysyłanego
        public async Task<IActionResult> AddMember(Guid id, UserViewModel searchUser)
        {
            var user = await _userManager.FindByNameAsync(searchUser.UserName);
            if (user != null && !_userGroupManager.IsUserInGroupByIds(new Guid(user.Id), id))
            {
                UserGroupEntity userGroup = new UserGroupEntity();
                userGroup.UserId = new Guid(user.Id);
                userGroup.GroupId = id;
                _userGroupManager.Add(userGroup);
                _paymentInGroupService.AddNewMemberPayments(userGroup);
            }
            var members = _groupService.GetAllUsersFromGroup(id);
            var models = new Tuple<Guid, UserEntity, List<UserViewModel>>(id, user, members);
            return View(models);
        }

        [HttpGet("deleteMember/{id}")]
        //Nazwa obiektu musi byc taka sama jak tego wysyłanego
        public async Task<IActionResult> DeleteMember(Guid id, Guid groupId)
        {
            var userGroup = _userGroupManager.GetUserGroupByUserIdAndGroupId(id, groupId);
            _userGroupManager.Delete(userGroup);
            _paymentInGroupService.DeleteMemberPayments(id, groupId);
            var members = _groupService.GetAllUsersFromGroup(id);
            var models = new Tuple<Guid, UserEntity, List<UserViewModel>>(id, null, members);
            return RedirectToAction($"addMember", "Group", new { id = groupId });
        }

        [HttpPost("paid/{groupId}")]
        public IActionResult Paid(Guid groupId, PaymentInGroupViewModel chosenSettlement)
        {
            _paymentInGroupService.SettleUser(chosenSettlement);

            _paymentMemberEntityService.SettleUserInPayment(chosenSettlement.UserWhoReturnsId, groupId);

            return RedirectToAction($"{chosenSettlement.GroupId}", "group");
        }
    }
}
