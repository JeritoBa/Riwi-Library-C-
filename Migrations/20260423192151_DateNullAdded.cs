using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercise4.Migrations
{
    /// <inheritdoc />
    public partial class DateNullAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReturnDate",
                table: "Loans",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LoanDate",
                table: "Loans",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2026, 4, 23),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2026, 4, 22));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReturnDate",
                table: "Loans",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LoanDate",
                table: "Loans",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2026, 4, 22),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2026, 4, 23));
        }
    }
}
