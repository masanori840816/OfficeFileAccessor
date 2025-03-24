using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddInputTableCell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "input_table_cell",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    start_column = table.Column<int>(type: "integer", nullable: false),
                    start_row = table.Column<int>(type: "integer", nullable: false),
                    end_column = table.Column<int>(type: "integer", nullable: false),
                    end_row = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_input_table_cell", x => x.id);
                    table.ForeignKey(
                        name: "FK_input_table_cell_table_cell_table_cell_id",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "input_table_cell");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEJwGfBgUuOP8V7tK/Up7FfFPvHT+kBz7bzJySmvCDpfJzvW58IiRx7lJhD6tCkHMTg==");
        }
    }
}
