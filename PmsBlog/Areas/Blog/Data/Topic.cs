namespace PmsBlog.Areas.Blog.Data
{
    public class Topic
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<UserTopic>? UserTopics { get; set; }
        public List<ArticleTopic>? ArticleTopics { get; set; }

    }
}
