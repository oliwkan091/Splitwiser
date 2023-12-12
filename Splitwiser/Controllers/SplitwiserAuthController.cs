using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Splitwiser.Models;
using Splitwiser.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Splitwiser.Controllers
{
    [Controller]
    [Route("splitwiserAuth")]
    public class SplitwiserAuthController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public SplitwiserAuthController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
            _signInManager = signInManager ?? throw new NullReferenceException(nameof(signInManager));
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

			var loginStatus = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!loginStatus.Succeeded)
            {
                ViewBag.Error = "Dane logowania są niepoprawne";
                return View(login);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("register")]
		public IActionResult Register()
		{
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
		}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel register)
		{
			if (!ModelState.IsValid)
			{
				return View(register);
			}

			if(!register.Password.Equals(register.PasswordAgain))
			{
				ViewBag.Error = "Hasła są różne";
				return View(register);
			}

			//Musi być taki model
			var newUser = new UserModel
			{
				Email = register.Email,
				UserName = register.Username
			};

			var success = await _userManager.CreateAsync(newUser, register.Password);

			if(!success.Succeeded)
			{
				ViewBag.Error = "Błąd podczas tworzenia użytkownika";
				return View(register);
			}

			return RedirectToAction("Login", "SplitwiserAuth");
		}

        [HttpGet("Logout")]
        public async Task<IActionResult> LogOut()
        {
			//var user = await _userManager.GetUserAsync(User);
			await _signInManager.SignOutAsync();
            //HttpContext.Session.Clear();
            return RedirectToAction("Login", "SplitwiserAuth");
        }

        //[HttpGet]
        //      public IActionResult Index()
        //      {
        //	var groups = _groupService.GetAll();
        //	return View(groups);
        //      }

        //      [HttpGet("create")]
        //      public IActionResult Create()
        //      {
        //          return View();
        //      }

        //[HttpPost("create")]
        //public IActionResult Add(GroupModel book)
        //{
        //	if (!ModelState.IsValid)
        //	{
        //		return View(book);
        //	}

        //          var books = _groupService.Add(book);

        //          return RedirectToAction("Index");
        //}
    }
}
