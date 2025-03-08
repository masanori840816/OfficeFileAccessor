using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddCellValueTypeAndFormula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "formula",
                table: "table_cell",
                type: "varchar(512)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "value_type",
                table: "table_cell",
                type: "varchar(32)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEG4STBsC1UpSZwAiK5ed24z1O3mdLvFyPf95Il3d3gcP25u14ib/Az9xI/uDOZW11Q==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "formula",
                table: "table_cell");

            migrationBuilder.DropColumn(
                name: "value_type",
                table: "table_cell");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEOnwAUWEWyakcaJITTRW/1MFuCEsa/gmT89V2QnxBhRBvmePYGLReBJiLrSlm8zw+w==");
        }
    }
}
