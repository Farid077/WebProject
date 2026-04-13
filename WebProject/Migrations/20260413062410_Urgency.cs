using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject.Migrations
{
    /// <inheritdoc />
    public partial class Urgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Urgency_UrgencyId",
                table: "Issues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Urgency",
                table: "Urgency");

            migrationBuilder.RenameTable(
                name: "Urgency",
                newName: "Urgencies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Urgencies",
                table: "Urgencies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Urgencies_UrgencyId",
                table: "Issues",
                column: "UrgencyId",
                principalTable: "Urgencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Urgencies_UrgencyId",
                table: "Issues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Urgencies",
                table: "Urgencies");

            migrationBuilder.RenameTable(
                name: "Urgencies",
                newName: "Urgency");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Urgency",
                table: "Urgency",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Urgency_UrgencyId",
                table: "Issues",
                column: "UrgencyId",
                principalTable: "Urgency",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
