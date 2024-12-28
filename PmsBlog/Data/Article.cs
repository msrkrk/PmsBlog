using Microsoft.EntityFrameworkCore.Storage.Json;
using NuGet.Protocol.Core.Types;

namespace PmsBlog.Data
{
    public class Article
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string Content { get; set; }
        public int AvgReadingMins { get; set; }
        public int ReadingCount { get; set; }
        public List<ArticleTopic> ArticleTopics { get; set; }

        public string AuthorId { get; set; }
        public PmsBlogUser Author { get; set; }

    }
}
