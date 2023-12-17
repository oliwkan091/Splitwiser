using AutoMapper;
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
using Splitwiser.Services;
using Splitwiser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            var groupPaymentHistoryMapped = _mapper.Map<List<GroupPaymentHistoryViewModel>, List<GroupPaymentHistoryEntity>>(groupPaymentHistory);
            var paymentMemberEntity = _paymentMemberEntityService.GetAllPaymentMemberOfGroups(groupId);
            var currentUser = await _userManager.GetUserAsync(User);
            var settlements = await _paymentInGroupService.GetHowMuchCurrentUserOwnToOtherGroupUsers(new Guid(_userManager.GetUserAsync(User).Result.Id), groupId);
            //var settlements = _paymentInGroupService.GetAllPaymentsByUserIdAndGroupId(new Guid(currentUser.Id), groupId);
            //List <Settlement2> outt = CalculateSettlements2(paymentMemberEntity, groupPaymentHistoryMapped);
            var model = new Tuple<List<GroupPaymentHistoryViewModel>, Guid, List<PaymentInGroupViewModel>>(groupPaymentHistory, groupId, settlements);
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
            if(userListCheckbox.Where(pm => pm.IsChecked == true).Count() == 0)
            {
                var usersFromGroup = _userGroupManager.GetAllUsersFromGroup(gid);
                var usersFromGroupCheckbox = _mapper.Map<List<UserViewModel>, List<UserViewModelCheckbox>>(usersFromGroup);
                bool wasAddingSuccessful = false;
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
                _paymentMemberEntityService.Add(paymentMember);
            }

            _paymentInGroupService.AddPaymentSplit(payment, userListCheckbox);

            return RedirectToAction($"{gid}", "group");
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
        public async Task<IActionResult> DeleteMember(Guid id, Guid gid)
        {
            //return Ok();
            var userGroup = _userGroupManager.GetUserGroupByUserIdAndGroupId(id, gid);
            _userGroupManager.Delete(userGroup);

            var members = _groupService.GetAllUsersFromGroup(id);
            var models = new Tuple<Guid, UserEntity, List<UserViewModel>>(id, null, members);
            return RedirectToAction($"addMember", "Group", new { id = gid });
        }

        [HttpPost("paid/{id}")]
        public IActionResult DeleteMember(Guid id, PaymentInGroupViewModel chosenSettlement)
        {
            _paymentInGroupService.SettleUser(chosenSettlement);

            return RedirectToAction($"{chosenSettlement.GroupId}", "group");
            //Redirect($"/group/{chosenSettlement.GroupId}");
        }

        //    public List<Settlement2> CalculateSettlements2(List<PaymentMemberEntity> paymentMembers, List<GroupPaymentHistoryEntity> paymentHistories)
        //    {
        //        var totalAmountByUser = new Dictionary<Guid, double>();
        //        var settlements = new List<Settlement2>();

        //        // Calculate total amount paid by each user
        //        foreach (var payment in paymentMembers)
        //        {
        //            if (!totalAmountByUser.ContainsKey(payment.UserId))
        //                totalAmountByUser[payment.UserId] = 0;

        //            totalAmountByUser[payment.UserId] += paymentHistories.Select(x => x.UserId == payment.UserId).Where(x => x.UserId == payment.UserId)  .         [payment.Amount];
        //        }

        //        // Calculate the share each member should contribute
        //        var totalAmount = totalAmountByUser.Sum(kvp => kvp.Value);
        //        var numberOfMembers = paymentMembers.Select(pm => pm.UserId).Distinct().Count();
        //        var averageAmount = totalAmount / numberOfMembers;

        //        // Calculate how much each member needs to settle
        //        var balanceByUser = totalAmountByUser.ToDictionary(kvp => kvp.Key, kvp => kvp.Value - averageAmount);

        //        while (balanceByUser.Any(b => b.Value > 0))
        //        {
        //            var payer = balanceByUser.FirstOrDefault(b => b.Value < 0);
        //            var payee = balanceByUser.First(b => b.Value > 0);

        //            var amount = Math.Min(Math.Abs(payer.Value), payee.Value);
        //            settlements.Add(new Settlement2 { PayerId = payer.Key, PayeeId = payee.Key, Amount = amount });

        //            balanceByUser[payer.Key] += amount;
        //            balanceByUser[payee.Key] -= amount;
        //        }

        //        return settlements;
        //    }



        //    public static List<Settlement1> CalculateSettlements1(
        //        List<GroupPaymentHistoryEntity> payers,
        //        List<PaymentMemberEntity> paymentsMembers
        //        )
        //    {
        //        // Create dictionaries to track balances for each user
        //        Dictionary<Guid, double> balances = new Dictionary<Guid, double>();

        //        // Initialize balances for payers
        //        foreach (var payer  in payers)
        //        {
        //            if (!balances.ContainsKey(payer.UserId))
        //            {
        //                balances[payer.UserId] = 0;
        //            }

        //            balances[payer.UserId] += payer.Amount;
        //        }

        //        // Update balances based on paymentsMembers
        //        foreach (var paymentMember in paymentsMembers)
        //        {
        //            if (!balances.ContainsKey(paymentMember.UserId))
        //            {
        //                balances[paymentMember.UserId] = 0;
        //            }

        //            // Decrease the balance of the user who paid
        //            balances[paymentMember.UserId] -= payers.Sum(p => p.Amount) / payers.Count;

        //            // Increase the balance of the payer
        //            foreach (var payer in payers)
        //            {
        //                balances[payer.UserId] += payer.Amount / payers.Count;
        //            }
        //        }

        //        // Generate settlements
        //        List<Settlement1> settlements = new List<Settlement1>();

        //        foreach (var payerId in balances.Keys)
        //        {
        //            foreach (var receiverId in balances.Keys)
        //            {
        //                double amount = balances[payerId] - Math.Abs(balances[receiverId]);

        //                if (amount != 0)
        //                {
        //                    settlements.Add(new Settlement1
        //                    {
        //                        UserId = payerId,
        //                        Amount = amount,
        //                        ToFromUserId = receiverId
        //                    });
        //                }
        //            }
        //        }

        //        // Remove zero settlements
        //        settlements.RemoveAll(s => s.Amount == 0);

        //        return settlements;
        //    }
        //}

        //public class Settlement1
        //{
        //    public Guid UserId { get; set; }
        //    public double Amount { get; set; }
        //    public Guid ToFromUserId { get; set; }
        //}

        //public class Settlement2
        //{
        //    public Guid PayerId { get; set; }
        //    public Guid PayeeId { get; set; }
        //    public double Amount { get; set; }
        //}
    }
}
