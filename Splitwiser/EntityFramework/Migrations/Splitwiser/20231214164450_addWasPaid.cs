using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Splitwiser.EntityFramework.Migrations.Splitwiser
{
    /// <inheritdoc />
    public partial class addWasPaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "wasPaid",
                table: "PaymentMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wasPaid",
                table: "PaymentMembers");
        }
    }
}
