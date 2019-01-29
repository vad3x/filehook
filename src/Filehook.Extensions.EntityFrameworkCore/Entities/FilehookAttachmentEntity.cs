using System;

namespace Filehook.Extensions.EntityFrameworkCore.Entities
{
    public class FilehookAttachmentEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public int BlobId { get; set; }

        public virtual FilehookBlobEntity Blob { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
