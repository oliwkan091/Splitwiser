using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Splitwiser.EntityFramework.Migrations.Splitwiser
{
    /// <inheritdoc />
    public partial class addAddDate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddDate",
                table: "UserGroups",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddDate",
                table: "UserGroups");
        }
    }
}
