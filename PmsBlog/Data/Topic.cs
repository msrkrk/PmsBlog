namespace PmsBlog.Data
{
    public class Topic
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<UserTopic>? UserTopics { get; set; }
        public List<ArticleTopic>? ArticleTopics { get; set; }

    }
}
