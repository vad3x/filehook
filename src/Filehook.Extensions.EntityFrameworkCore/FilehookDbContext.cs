using Filehook.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Filehook.Extensions.EntityFrameworkCore
{
    public class FilehookDbContext : DbContext
    {
        public FilehookDbContext(DbContextOptions<FilehookDbContext> options)
            : base(options)
        {
        }

        public DbSet<FilehookAttachment> FilehookAttachments { get; set; }

        public DbSet<FilehookBlob> FilehookBlobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlobEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentEntityTypeConfiguration());
        }
    }
}
