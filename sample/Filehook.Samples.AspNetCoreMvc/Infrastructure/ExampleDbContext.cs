using Microsoft.EntityFrameworkCore;

namespace Filehook.Samples.AspNetCoreMvc.Infrastructure
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
