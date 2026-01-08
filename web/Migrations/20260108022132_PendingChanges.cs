using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRequestMessages_AspNetUsers_SenderUserId",
                table: "TutorRequestMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRequestMessages_AspNetUsers_SenderUserId",
                table: "TutorRequestMessages",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRequestMessages_AspNetUsers_SenderUserId",
                table: "TutorRequestMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRequestMessages_AspNetUsers_SenderUserId",
                table: "TutorRequestMessages",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
