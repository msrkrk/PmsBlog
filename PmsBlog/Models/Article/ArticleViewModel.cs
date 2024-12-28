using PmsBlog.Data;

namespace PmsBlog.Models.Article
{
    public class ArticleViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Topics { get; set; }
        public int AvgReadingMins { get; set; }
        public int ReadingCount { get; set; }

    }
}
