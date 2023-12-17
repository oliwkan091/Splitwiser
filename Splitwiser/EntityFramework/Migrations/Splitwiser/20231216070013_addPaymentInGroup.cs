using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Splitwiser.EntityFramework.Migrations.Splitwiser
{
    /// <inheritdoc />
    public partial class addPaymentInGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentInGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserWhoReturnsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserToBePaidId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountToPay = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInGroups", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentInGroups");
        }
    }
}
