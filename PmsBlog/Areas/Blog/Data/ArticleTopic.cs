namespace PmsBlog.Areas.Blog.Data
{
    public class ArticleTopic
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public Article Article { get; set; }
        public string ArticleId { get; set; }
        public  Topic Topic { get; set; }
        public string TopicId { get; set; }
    }
}
