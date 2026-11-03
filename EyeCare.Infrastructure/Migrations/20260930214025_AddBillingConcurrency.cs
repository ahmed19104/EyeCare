using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Payments_IdempotencyKey",
                table: "Payments",
                newName: "UX_Payments_IdempotencyKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_Payments_IdempotencyKey",
                table: "Payments",
                newName: "IX_Payments_IdempotencyKey");
        }
    }
}
