using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisterUserNameIntoOfficeFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "display_order",
                table: "office_file_sheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "register_user_id",
                table: "office_file",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEOZKmGhF51c6c2IcjsTx9IKdg2nrSGRoOp4YwqkY9hTYeLGE9Im3vBMbwixDomMPhw==");

            migrationBuilder.CreateIndex(
                name: "IX_office_file_register_user_id",
                table: "office_file",
                column: "register_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeFile_ApplicationUser",
                table: "office_file",
                column: "register_user_id",
                principalTable: "application_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficeFile_ApplicationUser",
                table: "office_file");

            migrationBuilder.DropIndex(
                name: "IX_office_file_register_user_id",
                table: "office_file");

            migrationBuilder.DropColumn(
                name: "display_order",
                table: "office_file_sheet");

            migrationBuilder.DropColumn(
                name: "register_user_id",
                table: "office_file");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEGxiz0jpxI//UjlRAjgNKkhMejmJZlyM879dJM9NkahpMpSVki4Mq4mAZ7oW4QbsDA==");
        }
    }
}
