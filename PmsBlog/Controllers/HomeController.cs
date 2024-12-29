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
        private readonly UserManager<PmsBlogUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<PmsBlogUser> singInManager, PmsBlogContext context, UserManager<PmsBlogUser> userManager)
        {
            _logger = logger;
            _singInManager = singInManager;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (_singInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);

                var userTopics = _context.UserTopics.AsNoTracking().Where(x => x.PmsBlogUserId == userId).Select(x => x.Topic).ToList();



                var authorizedVm = new AuthorizedHomeViewModel
                {
                    SubscribedTopics = userTopics
                };

                if (userTopics.Count > 0)
                {
                    var userTopicIds = userTopics.Select(x => x.Id).ToHashSet();

                    authorizedVm.Articles = _context.ArticleTopics
                    .Where(x => userTopicIds.Contains(x.TopicId))
                    .Include(x => x.Article)
                    .Include(x => x.Article.Author)
                    .Include(x => x.Article.ArticleTopics)
                    .ThenInclude(x => x.Topic)
                    .Select(x => new ArticleViewModel()
                    {
                        Id = x.ArticleId,
                        Title = x.Article.Title,
                        Content = x.Article.Content,
                        Author = x.Article.Author.FullName,
                        AuthorUrl = x.Article.Author.Url,
                        CreatedDate = x.Article.CreatedDate,
                        Topics = x.Article.ArticleTopics.Select(x => x.Topic.Name).ToList(),
                        AvgReadingMins = x.Article.AvgReadingMins,
                        ReadingCount = x.Article.ReadingCount,

                    }).ToList();
                }
                else
                {
                      authorizedVm.Articles = _context.Articles.OrderByDescending(x => x.ReadingCount).Take(5)
                      .Include(x => x.Author)
                      .Include(x => x.ArticleTopics)
                      .ThenInclude(x => x.Topic)
                      .Select(x => new ArticleViewModel()
                      {
                          Id = x.Id,
                          Title = x.Title,
                          Content = x.Content,
                          Author = x.Author.FullName,
                          AuthorUrl = x.Author.Url,
                          CreatedDate = x.CreatedDate,
                          Topics = x.ArticleTopics.Select(x => x.Topic.Name).ToList(),
                          AvgReadingMins = x.AvgReadingMins,
                          ReadingCount = x.ReadingCount,

                      }).ToList();
                }




                return View("AuthorizedIndex", authorizedVm);
            }


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
