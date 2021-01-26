using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Propmaster.Migrations.PropmasterModel
{
    public partial class AddPropertyModelContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    PropertySize = table.Column<int>(type: "int", nullable: false),
                    PropertyLocation = table.Column<string>(nullable: false),
                    PropertyType = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Furnished = table.Column<string>(nullable: false),
                    Bedroom = table.Column<int>(type: "int", nullable: false),
                    Bathroom = table.Column<int>(type: "int", nullable: false),
                    Carpark = table.Column<int>(type: "int", nullable: false),
                    PicUrl = table.Column<string>(nullable: false),
                    PropertyStatus = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Property");
        }
    }
}
