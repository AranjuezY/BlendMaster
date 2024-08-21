using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Tools;

namespace WebApplication2.Controllers
{
    public class CartController : Controller
    {
        private readonly TestDbContext _testDbContext;

        public CartController(TestDbContext testDbContext)
        {
            _testDbContext = testDbContext;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(CartItem item)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            var itemToUpdate = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = item.Quantity;
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult AddQuantity(CartItem item)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            var itemToUpdate = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = item.Quantity + 1;
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult SubtractQuantity(CartItem item)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            var itemToUpdate = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (itemToUpdate != null && item.Quantity > 1)
            {
                itemToUpdate.Quantity = item.Quantity - 1;
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult RemoveItem(CartItem item)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            var itemToRemove = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Checkout()
        {
            Guid orderId = Guid.NewGuid();
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            decimal total = 0;
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");

            if (cart == null) 
            { 
                return new EmptyResult();
            }

            foreach (var cartItem in cart)
            {
                OrderDetail detail = new OrderDetail();

                detail.OrderDetailId = Guid.NewGuid();
                detail.OrderId = orderId;
                detail.ProductId = cartItem.ProductId;
                detail.UnitPrice = cartItem.Price;
                detail.Quantity = cartItem.Quantity;

                _testDbContext.OrderDetail.Add(detail);

                total += detail.UnitPrice * detail.Quantity;
            }

            var tableId = HttpContext.Session.GetObject<int>("TableId");

            CustomerOrder order = new CustomerOrder
            {
                CreatedDate = DateTime.Today,
                OrderId = orderId,
                Total = total,
                OrderStatus = OrderStatusType.Preparing,
                TableId = tableId,
                MemberId = memberId
            };

            _testDbContext.CustomerOrder.Add(order);
            
            var member = _testDbContext.Member.FirstOrDefault(m => m.MemberId == memberId);
            if (member != null)
            {
                member.TotalCredit += total;
                _testDbContext.Member.Update(member);
            }

            _testDbContext.SaveChanges();

            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Cart");

            ViewBag.TableId = tableId;

            return View();
        }
    }
}
