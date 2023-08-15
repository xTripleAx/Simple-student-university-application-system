using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheTask.Models;

namespace TheTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() //The Home Page of the website
        {
            return View();
        }

        public IActionResult AccessDenied() //The page for denied access if a role tried to access a different role's page
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