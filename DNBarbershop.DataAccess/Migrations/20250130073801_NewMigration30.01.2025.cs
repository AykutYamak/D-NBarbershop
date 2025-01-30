using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration30012025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_appointments_AppointmentId",
                table: "AppointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_services_ServiceId",
                table: "AppointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orders_OrderId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_products_ProductId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_AspNetUsers_UserId",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_workSchedules_barbers_BarberId",
                table: "workSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers",
                column: "SpecialityId",
                principalTable: "speciality",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orders_OrderId",
                table: "orderDetails",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_products_ProductId",
                table: "orderDetails",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_AspNetUsers_UserId",
                table: "orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_workSchedules_barbers_BarberId",
                table: "workSchedules",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_appointments_AppointmentId",
                table: "AppointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServices_services_ServiceId",
                table: "AppointmentServices");

            migrationBuilder.DropForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orders_OrderId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_products_ProductId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_AspNetUsers_UserId",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_workSchedules_barbers_BarberId",
                table: "workSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_AspNetUsers_UserId",
                table: "appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_barbers_BarberId",
                table: "appointments",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServices_appointments_AppointmentId",
                table: "AppointmentServices",
                column: "AppointmentId",
                principalTable: "appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServices_services_ServiceId",
                table: "AppointmentServices",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers",
                column: "SpecialityId",
                principalTable: "speciality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_AspNetUsers_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orders_OrderId",
                table: "orderDetails",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_products_ProductId",
                table: "orderDetails",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_AspNetUsers_UserId",
                table: "orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workSchedules_barbers_BarberId",
                table: "workSchedules",
                column: "BarberId",
                principalTable: "barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
