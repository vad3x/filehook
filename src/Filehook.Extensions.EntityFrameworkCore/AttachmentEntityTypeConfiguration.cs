using Filehook.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Filehook.Extensions.EntityFrameworkCore
{

    public class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<FilehookAttachment>
    {
        public void Configure(EntityTypeBuilder<FilehookAttachment> builder)
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
                .WithMany()
                .HasForeignKey(x => x.BlobId);

            builder.HasIndex(x => new { x.Name, x.EntityId, x.EntityType });
            builder.HasIndex(x => new { x.Name, x.EntityId, x.EntityType, x.BlobId }).IsUnique();
        }
    }
}
