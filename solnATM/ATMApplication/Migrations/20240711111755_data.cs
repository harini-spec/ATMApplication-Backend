using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATMApplication.Migrations
{
    public partial class data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Accounts");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountNo", "Balance", "Id" },
                values: new object[] { 1, "1234567890", 10000.0, 1 });

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ExpiryDate" },
                values: new object[] { new DateTime(2024, 7, 11, 16, 47, 55, 283, DateTimeKind.Local).AddTicks(4367), new DateTime(2024, 7, 21, 16, 47, 55, 283, DateTimeKind.Local).AddTicks(4378) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ExpiryDate" },
                values: new object[] { new DateTime(2024, 7, 11, 16, 41, 25, 726, DateTimeKind.Local).AddTicks(470), new DateTime(2024, 7, 21, 16, 41, 25, 726, DateTimeKind.Local).AddTicks(488) });
        }
    }
}
