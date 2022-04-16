using Bookalytics.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Bookalytics.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
