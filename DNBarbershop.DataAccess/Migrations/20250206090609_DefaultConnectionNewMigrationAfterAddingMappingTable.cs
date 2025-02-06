using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DefaultConnectionNewMigrationAfterAddingMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_appointments_AppointmentId",
                table: "AppointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_services_ServiceId",
                table: "AppointmentServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentServices",
                table: "AppointmentServices");

            migrationBuilder.RenameTable(
                name: "AppointmentServices",
                newName: "appointmentServices");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentServices_ServiceId",
                table: "appointmentServices",
                newName: "IX_appointmentServices_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_appointmentServices",
                table: "appointmentServices",
                columns: new[] { "AppointmentId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_appointments_AppointmentId",
                table: "appointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_appointmentServices_services_ServiceId",
                table: "appointmentServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_appointmentServices",
                table: "appointmentServices");

            migrationBuilder.RenameTable(
                name: "appointmentServices",
                newName: "AppointmentServices");

            migrationBuilder.RenameIndex(
                name: "IX_appointmentServices_ServiceId",
                table: "AppointmentServices",
                newName: "IX_AppointmentServices_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentServices",
                table: "AppointmentServices",
                columns: new[] { "AppointmentId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServices_appointments_AppointmentId",
                table: "AppointmentServices",
                column: "AppointmentId",
                principalTable: "appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServices_services_ServiceId",
                table: "AppointmentServices",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id");
        }
    }
}
