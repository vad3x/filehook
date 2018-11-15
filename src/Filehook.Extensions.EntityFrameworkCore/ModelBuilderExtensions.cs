using System.Collections.Generic;

using Filehook.Extensions.EntityFrameworkCore.Entities;

using Newtonsoft.Json;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder HasFilehook(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilehookBlobEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasAlternateKey(x => x.Key);

                entity.Property(x => x.Key)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(x => x.FileName)
                    .IsRequired();

                entity.Property(x => x.ContentType)
                    .HasMaxLength(64);

                entity
                    .Property(b => b.Metadata)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));

                entity.Property(x => x.ByteSize)
                    .IsRequired();

                entity.Property(x => x.Checksum)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasIndex(x => x.Key).IsUnique();
            });

            modelBuilder.Entity<FilehookAttachmentEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(x => x.EntityId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(x => x.EntityType)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(x => x.Blob)
                    .WithMany(x => x.Attachments)
                    .HasForeignKey(x => x.BlobId);

                entity.HasIndex(x => new { x.Name, x.EntityId, x.EntityType });
                entity.HasIndex(x => new { x.Name, x.EntityId, x.EntityType, x.BlobId }).IsUnique();
            });

            return modelBuilder;
        }
    }
}
