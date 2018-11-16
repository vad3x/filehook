using System;
using Filehook.Abstractions;

namespace Filehook.Extensions.EntityFrameworkCore.Entities
{
    public class AttachmentEntity : FilehookAttachment
    {
        public int Id { get; set; }

        public int BlobId { get; set; }

        public new BlobEntity Blob { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
