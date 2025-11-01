using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using S_DES_project4.Models;
using System.Diagnostics;

namespace S_DES_project4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            // Check if the user is currently signed in (has a valid cookie)
            if (_signInManager.IsSignedIn(User))
            {
                // If authenticated, redirect them directly to the Chat Dashboard
                // Assumes your chat controller is named "Chat" and the action is "Index"
                return RedirectToAction("Index", "Chats");
            }

            // If not authenticated, proceed to display the standard Home/Index view
            return View();
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