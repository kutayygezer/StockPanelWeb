using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPanelWeb.Data;
using StockPanelWeb.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StockPanelWeb.Controllers
{
	public class AccessController : Controller
	{
		private readonly StockPanelDBContext _dbcontext;

		public AccessController(StockPanelDBContext dbcontext)
		{
			_dbcontext = dbcontext;
		}

		public IActionResult Login()
		{
			return View(new LoginViewModel());
		}

		public User GetUserByCredentials(string username, string password)
		{
			return _dbcontext.Users.SingleOrDefault(u => u.UserName == username && u.UserPassword == password);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			if (ModelState.IsValid)
			{
				User foundUser = GetUserByCredentials(loginViewModel.UserName, loginViewModel.UserPassword);

				if (foundUser != null)
				{
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, foundUser.UserName),
					};

					if (!string.IsNullOrEmpty(foundUser.UserType))
					{
						claims.Add(new Claim(ClaimTypes.Role, foundUser.UserType));
					}

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

					AuthenticationProperties authProperties = null;

					if (loginViewModel.KeepLoggedIn)
					{
						authProperties = new AuthenticationProperties
						{
							IsPersistent = true,
							ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
						};
					}
					else
					{
						authProperties = new AuthenticationProperties
						{
							IsPersistent = false
						};
					}

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

					return RedirectToAction("Index", "Home");
				}

				ViewData["ValidateMessage"] = "Invalid username or password.";
			}
			return View(loginViewModel);
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}
	}
}
