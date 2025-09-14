using Microsoft.EntityFrameworkCore;

namespace TinyUrlApi
{
    public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UrlMapping> UrlMappings { get; set; } 
}
}