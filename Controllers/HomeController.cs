using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Tools;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TestDbContext _testDbContext;

        public HomeController(ILogger<HomeController> logger, TestDbContext testDbContext)
        {
            _logger = logger;
            _testDbContext = testDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Table()
        {
            var model = _testDbContext.Table.ToList();
            var existingTableId = HttpContext.Session.GetObject<int>("TableId");

            if (existingTableId != 0)
            {
                ViewData["TableId"] = existingTableId;
            }

            return View(model);
        }

        public IActionResult SelectTable(int id) 
        {
            if (HttpContext.Session.GetObject<int>("TableId") == 0)
            {
                HttpContext.Session.SetObject("TableId", id);
            }

            return RedirectToAction("Index", "Menu");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
