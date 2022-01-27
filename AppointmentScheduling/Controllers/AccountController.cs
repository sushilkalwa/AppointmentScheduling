using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers
{ 
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _db;
		UserManager<ApplicationUser> _userManager;
		SignInManager<ApplicationUser> _signInManager;
		RoleManager<IdentityRole> _roleManager;
		public AccountController(ApplicationDbContext Db, UserManager<ApplicationUser> UserManager, SignInManager<ApplicationUser> SignInManager,
			RoleManager<IdentityRole> RoleManager)
		{
			_db = Db;
			_userManager = UserManager;
			_signInManager = SignInManager;
			_roleManager = RoleManager;
		}
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel lvm)
		{
			if(ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(lvm.Email, lvm.Password, lvm.RemeberMe, false);
				if(result.Succeeded)
				{
					var user = await _userManager.FindByNameAsync(lvm.Email);
					HttpContext.Session.SetString("ssuserName", user.Name);
					//var userName = HttpContext.Session.GetString("ssuserName");
					return RedirectToAction("Index", "Appointment");
				}
				else
				{
					ModelState.AddModelError("", "Invalid login attempt.");
				}
			}
			return View(lvm);
		}
		public async Task<IActionResult> Register()
		{
			//if(!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
			//{
			//	await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
			//	await _roleManager.CreateAsync(new IdentityRole(Helper.Patient));
			//	await _roleManager.CreateAsync(new IdentityRole(Helper.Doctor));	
			//}
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel rvm)
		{
			if(ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					UserName = rvm.Email,
					Email = rvm.Email,
					Name = rvm.Name
				};

				var result = await _userManager.CreateAsync(user,rvm.Password);
				if(result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, rvm.RoleName);
					if (!User.IsInRole(Helper.Admin))
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
					}
					else
					{
						TempData["newAdminSignUp"] = user.Name;
					}
					return RedirectToAction("Index", "Appointment");
				}
				foreach(var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(rvm);
		}

		[HttpPost]
		public async Task<IActionResult> LogOff()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}
	}
}
