using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly TestDbContext _context;

        public AdminController(TestDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Recipes()
        {
            List<Recipe> recipes = _context.Recipe.OrderBy(r => r.Status).ToList();
            return View(recipes);
        }

        [HttpGet]
        public IActionResult RecipeDetail(Guid id)
        {
            Recipe recipe = _context.Recipe.FirstOrDefault(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Category.Select(c => c.CategoryName).ToList();

            return View(recipe);
        }

        [HttpPost]
        public IActionResult RecipeDetail(Recipe model)
        {
            Recipe recipe = _context.Recipe.Find(model.RecipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            try
            {
                recipe.Status = model.Status;
                _context.Recipe.Update(recipe);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }

            return RedirectToAction("RecipeDetail", new { recipe.RecipeId });
        }

        [HttpPost]
        public IActionResult AddRecipeToMenu(Guid id, string categoryName, string price)
        {
            Recipe recipe = _context.Recipe.FirstOrDefault(r => r.RecipeId == id);
            Category category = _context.Category.FirstOrDefault(c => c.CategoryName == categoryName);

            if (recipe == null || category == null)
            {
                return NotFound();
            }

            Product product = new Product()
            {
                ProductName = recipe.Name,
                ProductDescription = recipe.Description,
                Price = decimal.Parse(price),
                CategoryId = category.CategoryId,
                ProductId = Guid.NewGuid()
            };

            _context.Product.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Recipes");
        }

        public IActionResult OngoingOrders()
        {
            List<OngoingOrdersViewModel> models = new List<OngoingOrdersViewModel>();

            List<CustomerOrder> orders = _context.CustomerOrder
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreatedDate)
                .ToList();

            foreach (var order in orders)
            {
                var orderItems = order.OrderDetails
                    .Join(_context.Product,
                        od => od.ProductId,
                        p => p.ProductId,
                        (od, p) => new { p.ProductName, od.Quantity })?
                    .ToDictionary(
                        x => x.ProductName,
                        x => x.Quantity);

                models.Add(new OngoingOrdersViewModel
                {
                    OrderId = order.OrderId,
                    CreatedDate = order.CreatedDate,
                    OrderStatus = order.OrderStatus,
                    TableId = order.TableId,
                    OrderItems = orderItems
                });
            }

            return View(models);
        }

        public IActionResult CompleteOrder(Guid id)
        {
            CustomerOrder order = _context.CustomerOrder.Find(id);

            if (order == null)
            {
                return View("Error");
            }

            order.OrderStatus = OrderStatusType.Completed;

            _context.CustomerOrder.Update(order);
            _context.SaveChanges();

            CreateBill(id);

            return RedirectToAction("OngoingOrders");
        }

        public IActionResult CancelOrder(Guid id)
        {
            CustomerOrder order = _context.CustomerOrder.Find(id);

            if (order == null)
            {
                return View("Error");
            }

            order.OrderStatus = OrderStatusType.Cancelled;

            _context.CustomerOrder.Update(order);
            _context.SaveChanges();

            return RedirectToAction("OngoingOrders");
        }

        private void CreateBill(Guid id)
        {
            CustomerOrder order = _context.CustomerOrder.Find(id);
            Bill bill = new Bill()
            {
                BillId = Guid.NewGuid(),
                OrderId = order.OrderId,
                TableId = order.TableId,
                BillDate = DateTime.Now,
                TotalAmount = order.Total,
                Tax = order.Total * 0.13m,
            };

            _context.Bill.Add(bill);
            _context.SaveChanges();
        }

        [HttpGet]
        public IActionResult BillDetail(Guid id)
        {
            Bill bill = _context.Bill.FirstOrDefault(b => b.OrderId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        [HttpPost]
        public IActionResult BillDetail(Bill bill)
        {
            Bill billToUpdate = _context.Bill.FirstOrDefault(b => b.BillId == bill.BillId);
            if (billToUpdate == null)
            {
                return NotFound();
            }

            billToUpdate.AmountPaid = bill.AmountPaid;
            billToUpdate.PaymentMethod = bill.PaymentMethod;
            billToUpdate.Status = bill.Status;

            _context.Bill.Update(billToUpdate);
            _context.SaveChanges();

            return View(billToUpdate);
        }

        public IActionResult Dashboard()
        {
            var orders = _context.CustomerOrder.Include(co => co.OrderDetails).ThenInclude(od => od.Product).ToList();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver() // Optional: use camel case for property names
            };

            var modelJson = JsonConvert.SerializeObject(orders, settings);
            ViewBag.OrdersData = modelJson;

            return View();
        }

        public IActionResult Members()
        {
            var members = _context.Member.ToList();

            return View(members);
        }
    }
}
