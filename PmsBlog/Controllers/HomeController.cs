using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsBlog.Data;
using PmsBlog.Models;
using PmsBlog.Models.Article;
using PmsBlog.Models.Home;
using System.Diagnostics;

namespace PmsBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<PmsBlogUser> _singInManager;
        private readonly PmsBlogContext _context;

        public HomeController(ILogger<HomeController> logger, SignInManager<PmsBlogUser>singInManager, PmsBlogContext context)
        {
            _logger = logger;
            _singInManager = singInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (_singInManager.IsSignedIn(User))
                return View("AuthorizedIndex");

            var topReads = _context.Articles
                .Include(x => x.ArticleTopics)
                .ThenInclude(x => x.Topic)
                .Include(x => x.Author)
                .OrderByDescending(x => x.ReadingCount).Take(5).ToList();

            var articleViewModels = topReads.Select(x => new ArticleViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                AvgReadingMins = x.AvgReadingMins,
                ReadingCount = x.ReadingCount,
                CreatedDate = x.CreatedDate,
                Author = x.Author.FullName,
                AuthorUrl = x.Author.Url,
                Topics = x.ArticleTopics.Select(x => x.Topic.Name).ToList()
            });

            var vm = new HomeViewModel
            {
                TopReads = articleViewModels.ToList()
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
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
