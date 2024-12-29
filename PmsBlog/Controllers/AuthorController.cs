using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using PmsBlog.Data;
using PmsBlog.Migrations;
using PmsBlog.Models;
using PmsBlog.Models.Article;
using System.Security.Claims;

namespace PmsBlog.Controllers
{
    public class AuthorController : Controller
    {
        private readonly PmsBlogContext _context;

        public AuthorController(PmsBlogContext context)
        {
            _context = context;
        }

        [Route("[controller]/{url}")]
        public IActionResult Index([FromRoute] string url)
        {
            var user = _context.Users.Where(x => x.Url == url).FirstOrDefault();

            if (user== null)
                return Redirect("/Home/Index");

            var articles = _context.Articles.AsNoTracking().Include(x => x.ArticleTopics).ThenInclude(x => x.Topic).Where(x => x.AuthorId == user.Id).OrderByDescending(x => x.CreatedDate).ToList();

            var articleViewModels = articles.Select(x => new ArticleViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                AvgReadingMins = x.AvgReadingMins,
                ReadingCount = x.ReadingCount,
                CreatedDate = x.CreatedDate,
                Author = user.FullName,
                Topics = x.ArticleTopics.Select(x => x.Topic.Name).ToList()
            });

            var vm = new AuthorViewModel
            {
                Url = url,
                FullName = user.FullName,
                Description = user.Description,
                Articles = articleViewModels.ToList()
            };



            return View(vm);

        }


    }
}
