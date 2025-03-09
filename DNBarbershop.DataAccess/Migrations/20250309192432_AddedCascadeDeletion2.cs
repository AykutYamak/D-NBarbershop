using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeletion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_barbers_BarberId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_barbers_BarberId",
                table: "feedbacks",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_barbers_BarberId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_barbers_BarberId",
                table: "feedbacks",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
