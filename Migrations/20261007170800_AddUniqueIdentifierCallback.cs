using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSB.Test.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIdentifierCallback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniqueIdentifierCallbacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MapConfirmationTrackingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueIdentifierCallbacks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniqueIdentifierCallbacks");
        }
    }
}
