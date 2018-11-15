using System;
using Filehook.Abstractions;

namespace Filehook.Extensions.EntityFrameworkCore.Entities
{
    public class FilehookAttachmentEntity : FilehookAttachment
    {
        public int Id { get; set; }

        public int BlobId { get; set; }

        public new FilehookBlobEntity Blob { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
