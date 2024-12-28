using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PmsBlog.Data;

public class PmsBlogContext : IdentityDbContext<PmsBlogUser>
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<ArticleTopic> ArticleTopics { get; set; }
    public DbSet<UserTopic> UserTopics { get; set; }
    public PmsBlogContext(DbContextOptions<PmsBlogContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
