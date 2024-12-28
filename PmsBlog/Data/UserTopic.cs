namespace PmsBlog.Data
{
    public class UserTopic
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PmsBlogUser PmsBlogUser { get; set; }
        public string PmsBlogUserId { get; set; }

        public Topic Topic { get; set; }
        public string TopicId { get; set; }
    }
}
