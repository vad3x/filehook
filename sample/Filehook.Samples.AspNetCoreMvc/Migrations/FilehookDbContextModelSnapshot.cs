﻿// <auto-generated />
using System;
using Filehook.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Filehook.Samples.AspNetCoreMvc.Migrations
{
    [DbContext(typeof(FilehookDbContext))]
    partial class FilehookDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Filehook.Extensions.EntityFrameworkCore.Entities.FilehookAttachmentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlobId");

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("BlobId");

                    b.HasIndex("Name", "EntityId", "EntityType");

                    b.HasIndex("Name", "EntityId", "EntityType", "BlobId")
                        .IsUnique();

                    b.ToTable("FilehookAttachments");
                });

            modelBuilder.Entity("Filehook.Extensions.EntityFrameworkCore.Entities.FilehookBlobEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ByteSize");

                    b.Property<string>("Checksum")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("ContentType")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("FileName")
                        .IsRequired();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Metadata");

                    b.HasKey("Id");

                    b.HasAlternateKey("Key");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("FilehookBlobs");
                });

            modelBuilder.Entity("Filehook.Extensions.EntityFrameworkCore.Entities.FilehookAttachmentEntity", b =>
                {
                    b.HasOne("Filehook.Extensions.EntityFrameworkCore.Entities.FilehookBlobEntity", "Blob")
                        .WithMany()
                        .HasForeignKey("BlobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
