using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSB.Test.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPoaEvaluationCallback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoaEvaluationCallbacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwTrakingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Succseed = table.Column<bool>(type: "bit", nullable: true),
                    NationalRegisterNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasPermission = table.Column<bool>(type: "bit", nullable: true),
                    ExistDoc = table.Column<bool>(type: "bit", nullable: true),
                    AdvocacyEndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorCode = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoaEvaluationCallbacks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoaEvaluationCallbacks");
        }
    }
}
