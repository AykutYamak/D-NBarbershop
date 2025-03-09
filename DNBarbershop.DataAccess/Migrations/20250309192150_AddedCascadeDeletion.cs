using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_appointments_AppointmentId",
                table: "appointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_services_ServiceId",
                table: "appointmentServices");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointmentServices_appointments_AppointmentId",
                table: "appointmentServices",
                column: "AppointmentId",
                principalTable: "appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointmentServices_services_ServiceId",
                table: "appointmentServices",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_appointments_AppointmentId",
                table: "appointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_services_ServiceId",
                table: "appointmentServices");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointmentServices_appointments_AppointmentId",
                table: "appointmentServices",
                column: "AppointmentId",
                principalTable: "appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointmentServices_services_ServiceId",
                table: "appointmentServices",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id");
        }
    }
}
