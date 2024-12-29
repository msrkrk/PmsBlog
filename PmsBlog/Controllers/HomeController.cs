using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PmsBlog.Data;
using PmsBlog.Models;
using System.Diagnostics;

namespace PmsBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<PmsBlogUser> _singInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<PmsBlogUser>singInManager)
        {
            _logger = logger;
            _singInManager = singInManager;
        }

        public IActionResult Index()
        {
            if (_singInManager.IsSignedIn(User))
                return View("AuthorizedIndex");

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
