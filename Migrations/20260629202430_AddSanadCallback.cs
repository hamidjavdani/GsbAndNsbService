using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSB.Test.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSanadCallback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationCallbackLogs");

            migrationBuilder.CreateTable(
                name: "SanadCallbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    RawJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanadCallbacks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SanadCallbacks");

            migrationBuilder.CreateTable(
                name: "RegistrationCallbackLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CallbackCode = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RawJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiveDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCallbackLogs", x => x.Id);
                });
        }
    }
}
