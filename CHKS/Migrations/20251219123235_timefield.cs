using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class timefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "history",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "history",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "dailyexpense",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "dailyexpense",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "history");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "history");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "dailyexpense");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "dailyexpense");
        }
    }
}
