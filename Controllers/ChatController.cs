using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    
    [Authorize(Roles = "Administrator")]
    public class ChatController : Controller
    {
        private readonly OpenAIService _openAIService;

        public ChatController(OpenAIService chatService)
        {
            _openAIService = chatService;
        }

        public IActionResult Index()
        {
            return View(new ChatViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatViewModel model)
        {
            //string Response = await _openAIService.GetResponseWithoutSaving(model.UserInput);
            string Response = "WIP";
            model.BotResponse = Response;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveResponse(ChatViewModel model) 
        {
            string userInput = model.BotResponse;

            if (!string.IsNullOrEmpty(userInput))
            {
                _openAIService.SaveResponseToDatabase(userInput);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Admin()
        {
            return View();
        }
    }
}
