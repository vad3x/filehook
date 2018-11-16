using System;
using System.Collections.Generic;
using System.Text;
using Filehook.Extensions.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Filehook.Extensions.EntityFrameworkCore
{

    public class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<AttachmentEntity>
    {
        public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.EntityId)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.EntityType)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasOne(x => x.Blob)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.BlobId);

            builder.HasIndex(x => new { x.Name, x.EntityId, x.EntityType });
            builder.HasIndex(x => new { x.Name, x.EntityId, x.EntityType, x.BlobId }).IsUnique();
        }
    }
}
