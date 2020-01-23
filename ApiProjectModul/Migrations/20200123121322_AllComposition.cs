using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiProjectModul.Migrations
{
    public partial class AllComposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Calories = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compositions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Compositions",
                columns: new[] { "Id", "Calories", "Created", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 1000, new DateTime(2020, 1, 23, 16, 13, 21, 729, DateTimeKind.Local).AddTicks(4663), "Qanburger", "Starter" },
                    { 2, 1100, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1416), "Hamburger", "Main" },
                    { 3, 1200, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1465), "Spaghetti", "Dessert" },
                    { 4, 1500, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1469), "Pizza", "Starter" },
                    { 5, 2000, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1472), "Doner", "Doner" },
                    { 6, 2500, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1475), "Sushi", "Sushi" },
                    { 7, 3000, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1477), "Tonbalik", "Tonbalik" },
                    { 8, 2000, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1480), "Qutab", "Qutab" },
                    { 9, 2500, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1483), "Perashki", "Perashki" },
                    { 10, 1500, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1485), "Corekarasi", "Corekarasi" },
                    { 11, 2500, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1488), "Qogal", "Qogal" },
                    { 12, 1000, new DateTime(2020, 1, 23, 16, 13, 21, 731, DateTimeKind.Local).AddTicks(1491), "Kasbmali", "Kasbmali" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compositions");
        }
    }
}
