using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace drv_next_api.Migrations
{
    public partial class trippaidlabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Paid",
                table: "Trips",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Trips");
        }
    }
}
