using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Filehook.Samples.AspNetCoreMvc.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilehookBlobs",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 64, nullable: true),
                    Metadata = table.Column<string>(nullable: true),
                    ByteSize = table.Column<long>(nullable: false),
                    Checksum = table.Column<string>(maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilehookBlobs", x => x.Id);
                    table.UniqueConstraint("AK_FilehookBlobs_Key", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "FilehookAttachments",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(maxLength: 32, nullable: false),
                    EntityType = table.Column<string>(maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BlobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilehookAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilehookAttachments_FilehookBlobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "FilehookBlobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilehookAttachments_BlobId",
                table: "FilehookAttachments",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_FilehookAttachments_Name_EntityId_EntityType_BlobId",
                table: "FilehookAttachments",
                columns: new[] { "Name", "EntityId", "EntityType", "BlobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilehookBlobs_Key",
                table: "FilehookBlobs",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilehookAttachments");

            migrationBuilder.DropTable(
                name: "FilehookBlobs");
        }
    }
}
