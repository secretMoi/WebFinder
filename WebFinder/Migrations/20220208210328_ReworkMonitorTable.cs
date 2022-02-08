using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebFinder.Migrations
{
    public partial class ReworkMonitorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isMonitoringProduct",
                table: "Monitor",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isMonitoringProduct",
                table: "Monitor");
        }
    }
}
