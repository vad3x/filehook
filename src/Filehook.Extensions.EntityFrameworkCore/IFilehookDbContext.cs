using Filehook.Extensions.EntityFrameworkCore.Entities;

namespace Microsoft.EntityFrameworkCore
{
    public interface IFilehookDbContext
    {
        DbSet<FilehookAttachmentEntity> FilehookAttachments { get; set; }

        DbSet<FilehookBlobEntity> FilehookBlobs { get; set; }
    }
}
