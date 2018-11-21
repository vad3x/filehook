using Filehook.Abstractions;
using Filehook.Extensions.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Filehook.Extensions.EntityFrameworkCore
{
    public class FilehookDbContext : DbContext
    {
        public FilehookDbContext(DbContextOptions<FilehookDbContext> options)
            : base(options)
        {
        }

        public DbSet<FilehookAttachmentEntity> FilehookAttachments { get; set; }

        public DbSet<FilehookBlobEntity> FilehookBlobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlobEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentEntityTypeConfiguration());
        }
    }
}
