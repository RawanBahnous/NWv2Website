using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsAPIProject.Models;

namespace NewsAPIProject.Data
{
    public class NewsDbContext : IdentityDbContext<User>
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }
        public DbSet<Author> Authors { get; set; }
        public DbSet<News> News { get; set; }
    }
}
