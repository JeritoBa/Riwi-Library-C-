using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercise4.Migrations
{
    /// <inheritdoc />
    public partial class FixBookLoanRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_BookId1",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookId1",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId1",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId1",
                table: "Loans",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_BookId1",
                table: "Loans",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
