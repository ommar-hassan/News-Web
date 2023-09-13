using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Models;

namespace NewsAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<Author>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<News> News { get; set; }
    }
}
