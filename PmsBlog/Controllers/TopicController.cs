using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsBlog.Data;
using PmsBlog.Models.Topic;

namespace PmsBlog.Controllers
{
    public class TopicController : Controller
    {
        private readonly PmsBlogContext _context;
        private readonly SignInManager<PmsBlogUser> _signInManager;
        private readonly UserManager<PmsBlogUser> _userManager;

        public TopicController(PmsBlogContext context, SignInManager<PmsBlogUser> signInManager, UserManager<PmsBlogUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var vm = _context.Topics.AsNoTracking().Select(x => new TopicViewModel
            {
                Id = x.Id,
                Name = x.Name,
                IsUserSuscribing = false,
            }).ToList();

            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);

                var userTopics = _context.UserTopics.AsNoTracking().Where(x => x.PmsBlogUserId == userId).Select(x => x.TopicId).ToList();

                foreach (var item in vm)
                {
                    if (userTopics.Contains(item.Id))
                    {
                        item.IsUserSuscribing = true;
                    }
                }
            }

            return View(vm);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Subscribe([FromForm] string topicId)
        {
            var userId = _userManager.GetUserId(User);

            var newTopic = new UserTopic
            {
                PmsBlogUserId = userId,
                TopicId = topicId
            };

            _context.UserTopics.Add(newTopic);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult UnSubscribe([FromForm] string topicId)
        {
            var userId = _userManager.GetUserId(User);

            var userTopic = _context.UserTopics.Where(x => x.PmsBlogUserId == userId && x.TopicId == topicId).FirstOrDefault();

            _context.UserTopics.Remove(userTopic);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
