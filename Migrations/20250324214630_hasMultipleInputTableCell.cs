using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class hasMultipleInputTableCell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_input_table_cell_table_cell_id",
                table: "input_table_cell");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEM1VcOGAunvvqHNBcr0obSVLFb0UPlZwui8GLsE6BuURe79L1jyi+wwlFuurFiBeqw==");

            migrationBuilder.CreateIndex(
                name: "IX_input_table_cell_table_cell_id",
                table: "input_table_cell",
                column: "table_cell_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_input_table_cell_table_cell_id",
                table: "input_table_cell");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEDGU2tOlkHe1T0NSmzfwBTmHnuGI7403W0asrqH4Tg034SzbLKGZk+DLepuOpOXLxQ==");

            migrationBuilder.CreateIndex(
                name: "IX_input_table_cell_table_cell_id",
                table: "input_table_cell",
                column: "table_cell_id",
                unique: true);
        }
    }
}
