using System;
using System.Collections.Generic;
using Filehook.Abstractions;

namespace Filehook.Extensions.EntityFrameworkCore.Entities
{
    public class FilehookBlobEntity : FilehookBlob
    {
        public int Id { get; set; }

        public ICollection<FilehookAttachmentEntity> Attachments { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
