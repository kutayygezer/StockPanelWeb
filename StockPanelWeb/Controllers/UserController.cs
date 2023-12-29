using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPanelWeb.Data;
using StockPanelWeb.Models;
using System.Drawing.Printing;

namespace StockPanelWeb.Controllers
{
	[Authorize]
	public class UserController : Controller
	{
		private readonly StockPanelDBContext _dbcontext;
		public UserController(StockPanelDBContext dbcontext)
		{
			_dbcontext = dbcontext;
		}
		public IActionResult Index(string searchQuery, int page = 1, int pageSize = 10)
		{
			IEnumerable<User> objUserList = _dbcontext.Users;
			if (!string.IsNullOrEmpty(searchQuery))
			{
				objUserList = objUserList.Where(p => p.UserName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
			}

			int totalLogs = objUserList.Count();

			int totalPages = (int)Math.Ceiling((double)totalLogs / pageSize);

			page = Math.Max(1, Math.Min(page, totalPages));

			int skipCount = (page - 1) * pageSize;

			var users = objUserList
				.OrderByDescending(user => user.UserName)
				.Skip(skipCount)
				.Take(pageSize)
				.ToList();

			// Pass the logs and pagination information to the view
			ViewBag.Logs = users;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			return View(objUserList);
		}

		//GET
		public IActionResult Add()
		{
			ViewBag.Users = _dbcontext.Users.ToList();
			return View();
		}

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(User obj)
		{
			if (ModelState.IsValid)
			{
				bool userExists = _dbcontext.Users.Any(p => p.UserName == obj.UserName);

				if (userExists)
				{
					ModelState.AddModelError("UserName", "This user already exists.");
					ViewBag.Users = _dbcontext.Users.ToList();
					return View(obj);
				}
				string selectedUserType = Request.Form["UserType"];

				obj.UserType = selectedUserType;

				var message = User.Identity.Name + " added user : " + obj.UserName + ", type : " + obj.UserType;
				var logEntry = new Log
				{
					LogUser = User.Identity.Name,
					LogMessage = message,
					LogTime = DateTime.Now
				};
				_dbcontext.Logs.Add(logEntry);
				_dbcontext.Users.Add(obj);
				_dbcontext.SaveChanges();
				TempData["success"] = "User successfully added";
				return RedirectToAction("Index");
			}

			ViewBag.Users = _dbcontext.Users.ToList();
			return View(obj);
		}


		//GET
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var userFromDb = _dbcontext.Users.Find(id);
			if (userFromDb == null)
			{
				return NotFound();
			}
			ViewBag.Users = _dbcontext.Users.ToList();
			return View(userFromDb);
		}

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(User obj)
		{
			if (ModelState.IsValid)
			{
                // User type for the Select tool
                string selectedUserType = Request.Form["UserType"];

                // User type assignment
                obj.UserType = selectedUserType;

				var message = User.Identity.Name + " Updated user : " + obj.UserName + ", type : " + obj.UserType;
				var logEntry = new Log
				{
					LogUser = User.Identity.Name,
					LogMessage = message,
					LogTime = DateTime.Now
				};
				_dbcontext.Logs.Add(logEntry);

				_dbcontext.Users.Update(obj);
				_dbcontext.SaveChanges();
				TempData["success"] = "User successfully updated";
				return RedirectToAction("Index");
			}

			ViewBag.Users = _dbcontext.Users.ToList();
			return View(obj);
		}

		//GET
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var userFromDb = _dbcontext.Users.Find(id);
			if (userFromDb == null)
			{
				return NotFound();
			}
			ViewBag.Users = _dbcontext.Users.ToList();
			return View(userFromDb);
		}

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var userFromDb = _dbcontext.Users.Find(id);
			if (userFromDb == null)
			{
				return NotFound();
			}
			var message = User.Identity.Name + " deleted user : " + userFromDb.UserName + ", type : " + userFromDb.UserType;
			var logEntry = new Log
			{
				LogUser = User.Identity.Name,
				LogMessage = message,
				LogTime = DateTime.Now
			};
			_dbcontext.Logs.Add(logEntry);
			_dbcontext.Users.Remove(userFromDb);
			_dbcontext.SaveChanges();
			ViewBag.Users = _dbcontext.Users.ToList();
			TempData["success"] = "User successfully deleted";
			return RedirectToAction("Index");
		}
	}
}
