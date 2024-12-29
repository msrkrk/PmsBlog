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
            var userId = _context.UserClaims.Where(x => x.ClaimType == "Url" && x.ClaimValue == url).Select(x => x.UserId).FirstOrDefault();

            if (userId == null)
                return Redirect("/Home/Index");


            var fullName = _context.UserClaims.FirstOrDefault(x => x.ClaimType == "FullName")?.ClaimValue;
            var description = _context.UserClaims.FirstOrDefault(x => x.ClaimType == "Description")?.ClaimValue;
            var articles = _context.Articles.AsNoTracking().Include(x => x.ArticleTopics).ThenInclude(x => x.Topic).Where(x => x.AuthorId == userId).OrderByDescending(x => x.CreatedDate).ToList();

            var articleViewModels = articles.Select(x => new ArticleViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                AvgReadingMins = x.AvgReadingMins,
                ReadingCount = x.ReadingCount,
                CreatedDate = x.CreatedDate,
                Author = fullName ?? "unknown",
                Topics = x.ArticleTopics.Select(x => x.Topic.Name).ToList()
            });

            var vm = new AuthorViewModel
            {
                Url = url,
                FullName = fullName,
                Description = description,
                Articles = articleViewModels.ToList()
            };



            return View(vm);

        }


    }
}
