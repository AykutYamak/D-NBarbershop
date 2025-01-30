using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTheAppointmentEntityAppointmentTimepropertyfromDateTimetoTimeSpan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AppointmentTime",
                table: "appointments",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_appointments_ServiceId",
                table: "appointments",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_services_ServiceId",
                table: "appointments",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_services_ServiceId",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_ServiceId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "appointments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentTime",
                table: "appointments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }
    }
}
