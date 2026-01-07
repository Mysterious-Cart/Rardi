using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class AddTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "historyconnector",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "historyconnector");
        }
    }
}
