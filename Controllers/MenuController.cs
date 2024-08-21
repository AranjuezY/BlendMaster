using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Tools;

namespace WebApplication2.Controllers
{
    public class MenuController : Controller
    {
        private readonly TestDbContext _context;

        public MenuController(TestDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(Guid categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(userId==null);
            List<Category> categories = _context.Category.ToList();
            var products = _context.Product.Where(p => p.CategoryId == categoryId).ToList();

            if (!products.Any())
            {
                products = _context.Product.Where(p => p.CategoryId == _context.Category.FirstOrDefault().CategoryId).ToList();
            }

            MenuViewModel viewModel = new MenuViewModel() { Categories = categories, Products = products };

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            if (cart != null)
            {
                ViewBag.CartItem = from item in cart select item.ProductId;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(Guid productId)
        {
            var Product = _context.Product.Find(productId);
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = Product.ProductName,
                    Price = Product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Edit()
        {
            var menu = _context.Product.Include(p => p.Category).ToList();
            return View(menu);
        }
    }
}
