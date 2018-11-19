using System.Collections.Generic;

using Filehook.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

namespace Filehook.Extensions.EntityFrameworkCore
{
    public class BlobEntityTypeConfiguration : IEntityTypeConfiguration<FilehookBlob>
    {
        public void Configure(EntityTypeBuilder<FilehookBlob> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => x.Key);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.FileName)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(64);

            builder
                .Property(b => b.Metadata)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));

            builder.Property(x => x.ByteSize)
                .IsRequired();

            builder.Property(x => x.Checksum)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(x => x.Key).IsUnique();
        }
    }
}
