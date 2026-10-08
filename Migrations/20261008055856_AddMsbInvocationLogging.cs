using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSB.Test.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMsbInvocationLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MsbInvocationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    OrganId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OwTrakingCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MapConfirmationTrackingCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TaskId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemoteIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemotePort = table.Column<int>(type: "int", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentLength = table.Column<long>(type: "bigint", nullable: true),
                    ForwardedFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestBodyMasked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBodyMasked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsbInvocationLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_CreatedAt",
                table: "MsbInvocationLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_Direction",
                table: "MsbInvocationLogs",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_IsSuccess",
                table: "MsbInvocationLogs",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_MapConfirmationTrackingCode",
                table: "MsbInvocationLogs",
                column: "MapConfirmationTrackingCode");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_OrganId",
                table: "MsbInvocationLogs",
                column: "OrganId");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_OrganId_CreatedAt",
                table: "MsbInvocationLogs",
                columns: new[] { "OrganId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_OwTrakingCode",
                table: "MsbInvocationLogs",
                column: "OwTrakingCode");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_RequestId",
                table: "MsbInvocationLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_ServiceName",
                table: "MsbInvocationLogs",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_ServiceName_CreatedAt",
                table: "MsbInvocationLogs",
                columns: new[] { "ServiceName", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MsbInvocationLogs_TraceIdentifier",
                table: "MsbInvocationLogs",
                column: "TraceIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MsbInvocationLogs");
        }
    }
}
