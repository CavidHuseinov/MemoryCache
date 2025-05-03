using MemoryCache.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemoryCache.Context
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Blog> Blogs { get; set; }
    }
}
