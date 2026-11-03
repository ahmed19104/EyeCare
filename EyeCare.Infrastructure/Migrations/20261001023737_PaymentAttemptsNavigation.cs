using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentAttemptsNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Invoices_AppointmentId",
                table: "Invoices",
                newName: "UX_Invoices_AppointmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_Invoices_AppointmentId",
                table: "Invoices",
                newName: "IX_Invoices_AppointmentId");
        }
    }
}
