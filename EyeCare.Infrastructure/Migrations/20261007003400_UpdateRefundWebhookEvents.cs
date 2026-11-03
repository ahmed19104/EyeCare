using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefundWebhookEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalTransactionId",
                table: "RefundWebhookEvents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "RefundWebhookEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "RefundWebhookEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "RefundWebhookEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedAt",
                table: "RefundWebhookEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalTransactionId",
                table: "RefundWebhookEvents");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "RefundWebhookEvents");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "RefundWebhookEvents");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "RefundWebhookEvents");

            migrationBuilder.DropColumn(
                name: "ReceivedAt",
                table: "RefundWebhookEvents");
        }
    }
}
