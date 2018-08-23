using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.DAL
{
    public class UrlDbContext : DbContext
    {
        public UrlDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<TheUrl> SavedUrls { get; set; }
    }
}