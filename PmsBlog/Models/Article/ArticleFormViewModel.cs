using NuGet.Protocol.Plugins;
using PmsBlog.Data;

namespace PmsBlog.Models.Article
{
    public class ArticleFormViewModel
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> SelectedTopicIds { get; set; }
        public int AvgReadingMins { get; set; }
        public List<Topic>? Topics { get; set; }
    }
}
