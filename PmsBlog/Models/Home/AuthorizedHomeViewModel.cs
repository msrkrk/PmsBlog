using PmsBlog.Models.Article;

namespace PmsBlog.Models.Home
{
    public class AuthorizedHomeViewModel
    {
        public List<Data.Topic> SubscribedTopics { get; set; }
        public List<ArticleViewModel> Articles { get; set; }


    }
}
