using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddRenameSheetColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "office_file_sheet",
                newName: "name");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "office_file_sheet",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEGxiz0jpxI//UjlRAjgNKkhMejmJZlyM879dJM9NkahpMpSVki4Mq4mAZ7oW4QbsDA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "office_file_sheet",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "office_file_sheet",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEM2vBpvCb/v9+tSEnWdDn34+U+MxV8ZIsfrSe9kC4p6prOmnKBgrcYFnkkWOKEig4w==");
        }
    }
}
