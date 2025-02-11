using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedServiceIdFromFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_services_ServiceId",
                table: "feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_feedbacks_ServiceId",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "feedbacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "feedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_ServiceId",
                table: "feedbacks",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_services_ServiceId",
                table: "feedbacks",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id");
        }
    }
}
