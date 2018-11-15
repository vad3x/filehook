using Filehook.Extensions.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Filehook.Samples.AspNetCoreMvc.Infrastructure
{
    public class ExampleDbContext : DbContext, IFilehookDbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            : base(options)
        {
        }

        public DbSet<FilehookAttachmentEntity> FilehookAttachments { get; set; }
        public DbSet<FilehookBlobEntity> FilehookBlobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasFilehook();
        }
    }
}
