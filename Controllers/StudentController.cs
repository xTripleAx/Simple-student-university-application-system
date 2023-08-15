using Microsoft.AspNetCore.Mvc;

namespace TheTask.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
