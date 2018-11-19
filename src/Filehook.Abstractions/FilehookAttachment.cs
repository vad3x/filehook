using System;

namespace Filehook.Abstractions
{
    public class FilehookAttachment
    {
        public int Id { get; set; }

        public int BlobId { get; set; }

        public string Name { get; set; }

        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public virtual FilehookBlob Blob { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
