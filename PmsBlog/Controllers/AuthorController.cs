using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using PmsBlog.Data;
using PmsBlog.Migrations;
using PmsBlog.Models;
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

            var vm = new AuthorViewModel
            {
                Url = url,
                FullName = fullName,
                Description = description,
            };

            return View(vm);

        }


    }
}
