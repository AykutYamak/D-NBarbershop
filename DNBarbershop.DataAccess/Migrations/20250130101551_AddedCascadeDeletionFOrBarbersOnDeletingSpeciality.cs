using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNBarbershop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeletionFOrBarbersOnDeletingSpeciality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers");

            migrationBuilder.AddForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers",
                column: "SpecialityId",
                principalTable: "speciality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers");

            migrationBuilder.AddForeignKey(
                name: "FK_barbers_speciality_SpecialityId",
                table: "barbers",
                column: "SpecialityId",
                principalTable: "speciality",
                principalColumn: "Id");
        }
    }
}
