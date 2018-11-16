using System;
using System.Collections.Generic;
using Filehook.Abstractions;

namespace Filehook.Extensions.EntityFrameworkCore.Entities
{
    public class BlobEntity : FilehookBlob
    {
        public int Id { get; set; }

        public ICollection<AttachmentEntity> Attachments { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
