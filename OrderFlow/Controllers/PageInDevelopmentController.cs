using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Controllers
{
    public class PageInDevelopmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
