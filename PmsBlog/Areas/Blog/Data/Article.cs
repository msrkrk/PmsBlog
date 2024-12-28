using Microsoft.EntityFrameworkCore.Storage.Json;

namespace PmsBlog.Areas.Blog.Data
{
    public class Article
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string Content { get; set; }
        public double AvgReadingTime { get; set; }
        public int ReadingCount { get; set; }
        public List<ArticleTopic>? ArticleTopics { get; set; }

    }
}
