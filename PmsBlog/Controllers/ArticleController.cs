using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsBlog.Data;
using PmsBlog.Models.Article;
using System.Security.Claims;

namespace PmsBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly PmsBlogContext _context;

        public ArticleController(PmsBlogContext context)
        {
            _context = context;
        }


        [Route("[controller]/{id}")]
        public IActionResult Index([FromRoute] string id)
        {
            var article = _context.Articles
                .AsNoTracking()
                .Include(x=>x.ArticleTopics)
                .ThenInclude(x => x.Topic)
                .Where(x => x.Id == id)
                .FirstOrDefault();

            // article yoksa Home sayasına geri dön.
            if (article == null) return RedirectToAction("Index", "Home"); 
            
            var authorName = _context.UserClaims.FirstOrDefault(x => x.UserId == article.AuthorId && x.ClaimType == "FullName")?.ClaimValue;
            

            var vm = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                AvgReadingMins = article.AvgReadingMins,
                ReadingCount = article.ReadingCount,
                CreatedDate = article.CreatedDate,
                Author = authorName??"unknown",
                Topics = article.ArticleTopics.Select(x => x.Topic.Name).ToList()
            };


            return View(vm);
        }


        [Route("[controller]/Form/New")]
        [Authorize]
        public IActionResult Form()
        {
            var vm = new ArticleFormViewModel();

            var topics = _context.Topics.AsNoTracking().ToList();
            vm.Topics = topics;

            return View(vm);
        }

        [Route("[controller]/Form/{id}")]
        [Authorize]
        public IActionResult Form([FromRoute] string id)
        {
            var vm = new ArticleFormViewModel
            {
                Id = id,
            };

            var topics = _context.Topics.AsNoTracking().ToList();
            vm.Topics = topics;

            if(vm.Id != null)
            {
                var article = _context.Articles
                    .AsNoTracking()
                    .Include(x => x.ArticleTopics)
                    .Where(x => x.Id == vm.Id)
                    .FirstOrDefault();

                if (article == null) return RedirectToAction("Index", "Home");

                vm.Title = article.Title;
                vm.Content = article.Content;
                vm.AvgReadingMins = article.AvgReadingMins;
                vm.SelectedTopicIds = article.ArticleTopics.Select(x => x.TopicId).ToList();
            }

            return View(vm);
        }

        [Route("[controller]/Save")]
        [HttpPost]
        [Authorize]
        public IActionResult Save([FromForm] ArticleFormViewModel vm)
        {
            var topics = _context.Topics.AsNoTracking().ToList();
            vm.Topics = topics;

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    Id = vm.Id ?? Guid.NewGuid().ToString(),
                    Title = vm.Title,
                    Content = vm.Content,
                    AvgReadingMins = vm.AvgReadingMins,
                    AuthorId = userId
                };

                if (vm.Id == null)
                {
                    _context.Articles.Add(article);

                }
                else
                {
                    _context.Articles.Update(article);
                    var articleTopics = _context.ArticleTopics.Where(x => x.ArticleId == article.Id).ToList();
                    _context.ArticleTopics.RemoveRange(articleTopics);
                   
                }

                var newArticleTopics = vm.SelectedTopicIds.Select(x => new ArticleTopic
                {
                    TopicId = x,
                    ArticleId = article.Id
                });
                _context.ArticleTopics.AddRange(newArticleTopics);

                _context.SaveChanges();

                vm.Id = article.Id;

                return RedirectToAction("Index", new {Id=vm.Id});
            }

            return RedirectToAction("Index", new { Id = vm.Id });
        }
    }
}
