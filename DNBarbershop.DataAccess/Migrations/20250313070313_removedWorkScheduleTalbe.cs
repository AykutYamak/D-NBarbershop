using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class removedWorkScheduleTalbe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.DropTable(
                name: "workSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.CreateTable(
                name: "workSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BarberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workSchedules_barbers_BarberId",
                        column: x => x.BarberId,
                        principalTable: "barbers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_workSchedules_BarberId",
                table: "workSchedules",
                column: "BarberId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
