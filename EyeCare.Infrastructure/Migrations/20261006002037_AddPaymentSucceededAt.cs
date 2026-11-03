using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentSucceededAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Refunds_IdempotencyKey",
                table: "Refunds",
                newName: "UX_Refunds_IdempotencyKey");

            migrationBuilder.RenameIndex(
                name: "IX_RefundAttempts_AttemptKey",
                table: "RefundAttempts",
                newName: "UX_RefundAttempts_AttemptKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_Refunds_IdempotencyKey",
                table: "Refunds",
                newName: "IX_Refunds_IdempotencyKey");

            migrationBuilder.RenameIndex(
                name: "UX_RefundAttempts_AttemptKey",
                table: "RefundAttempts",
                newName: "IX_RefundAttempts_AttemptKey");
        }
    }
}
