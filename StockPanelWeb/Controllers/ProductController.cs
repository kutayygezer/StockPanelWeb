using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPanelWeb.Data;
using StockPanelWeb.Models;
using System.Drawing.Printing;

namespace StockPanelWeb.Controllers
{
	[Authorize]
	public class ProductController : Controller
	{
		private readonly StockPanelDBContext _dbcontext;
		public ProductController(StockPanelDBContext dbcontext)
		{
			_dbcontext = dbcontext;
		}
		public IActionResult Index(string searchQuery, int page = 1, int pageSize = 10)
		{
			var GetProductsFromDatabase = _dbcontext.Products.Include(p => p.Category);
			IEnumerable<Product> objProductList = GetProductsFromDatabase;

			if (!string.IsNullOrEmpty(searchQuery))
			{
				objProductList = objProductList.Where(p => p.ProductName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
			}
			int totalLogs = objProductList.Count();

			int totalPages = (int)Math.Ceiling((double)totalLogs / pageSize);

			page = Math.Max(1, Math.Min(page, totalPages));

			int skipCount = (page - 1) * pageSize;

			var products = objProductList
				.OrderByDescending(product => product.ProductName)
				.Skip(skipCount)
				.Take(pageSize)
				.ToList();

			ViewBag.Products = products;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			return View();
		}
		//GET
		[Authorize]
		public IActionResult Add()
		{
			ViewBag.Categories = _dbcontext.Categories.ToList();
			return View();
		}

		//POST
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(Product obj)
		{
			if (ModelState.IsValid)
			{
				bool productExists = _dbcontext.Products.Any(p => p.ProductName == obj.ProductName);

				if (productExists)
				{
					ModelState.AddModelError("ProductName", "This product already exists.");
					ViewBag.Categories = _dbcontext.Categories.ToList();
					return View(obj);
				}
				_dbcontext.Products.Add(obj);
				var message = User.Identity.Name + " added product : " + obj.ProductName;
				var logEntry = new Log
				{
					LogUser = User.Identity.Name,
					LogMessage = message,
					LogTime = DateTime.Now
				};
				_dbcontext.Logs.Add(logEntry);
				TempData["success"] = "Product successfully added";
				_dbcontext.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Categories = _dbcontext.Categories.ToList();
			return View(obj);
		}

		//GET
		[Authorize]
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var productFromDb = _dbcontext.Products.Find(id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			ViewBag.Categories = _dbcontext.Categories.ToList();
			return View(productFromDb);
		}

		//POST
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Product obj)
		{
			if (ModelState.IsValid)
			{
				_dbcontext.Products.Update(obj);
				var message = User.Identity.Name + " updated product : " + obj.ProductName;
				var logEntry = new Log
				{
					LogUser = User.Identity.Name,
					LogMessage = message,
					LogTime = DateTime.Now
				};
				_dbcontext.Logs.Add(logEntry);
				_dbcontext.SaveChanges();
				TempData["success"] = "Product successfully updated";
				return RedirectToAction("Index");
			}
			var value = ModelState.Values.SelectMany(v => v.Errors);
			// If the ModelState is not valid, return the Edit view with the model data
			ViewBag.Categories = _dbcontext.Categories.ToList();
			return View("Edit", obj);
		}


		//GET
		[Authorize]
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var productFromDb = _dbcontext.Products.Find(id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			ViewBag.Categories = _dbcontext.Categories.ToList();
			return View(productFromDb);
		}

		//POST
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var productFromDb = _dbcontext.Products.Find(id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			_dbcontext.Products.Remove(productFromDb);
			var message = User.Identity.Name + " deleted product : " + productFromDb.ProductName;
			var logEntry = new Log
			{
				LogUser = User.Identity.Name,
				LogMessage = message,
				LogTime = DateTime.Now
			};
			_dbcontext.Logs.Add(logEntry);
			_dbcontext.SaveChanges();
			ViewBag.Categories = _dbcontext.Categories.ToList();
			TempData["success"] = "Product successfully deleted";
			return RedirectToAction("Index");
		}
		[Authorize]
		public IActionResult Sell(int id)
		{
			var productFromDb = _dbcontext.Products.Find(id);
			if (productFromDb == null)
			{
				return NotFound();
			}

			// Reduce the product amount by one if it's greater than zero
			if (productFromDb.ProductAmount > 0)
			{
				productFromDb.ProductAmount--;
				var message = User.Identity.Name + " sold product : " + productFromDb.ProductName;
				var logEntry = new Log
				{
					LogUser = User.Identity.Name,
					LogMessage = message,
					LogTime = DateTime.Now
				};
				_dbcontext.Logs.Add(logEntry);
				_dbcontext.SaveChanges();
			}
			TempData["success"] = "Product successfully sold";
			return RedirectToAction("Index");
		}
	}
}
