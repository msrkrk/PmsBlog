using PmsBlog.Models.Article;

namespace PmsBlog.Models
{
    public class AuthorViewModel
    {
        public string Url { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public List<ArticleViewModel> Articles { get; set; }
        

    }
}
