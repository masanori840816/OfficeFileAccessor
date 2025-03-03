using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableCells : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "table_cell",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    column = table.Column<int>(type: "integer", nullable: false),
                    row = table.Column<int>(type: "integer", nullable: false),
                    vertical_length = table.Column<int>(type: "integer", nullable: false),
                    horizontal_length = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    background_color = table.Column<string>(type: "text", nullable: true),
                    editabled = table.Column<bool>(type: "boolean", nullable: false),
                    vertical_writing = table.Column<bool>(type: "boolean", nullable: false),
                    text_rotation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_cell", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "merged_table_cell",
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
                    table.PrimaryKey("PK_merged_table_cell", x => x.id);
                    table.ForeignKey(
                        name: "FK_merged_table_cell_table_cell_table_cell_id",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_cell_borders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    left = table.Column<int>(type: "integer", nullable: false),
                    top = table.Column<int>(type: "integer", nullable: false),
                    right = table.Column<int>(type: "integer", nullable: false),
                    bottom = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_cell_borders", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_cell_borders_table_cell_table_cell_id",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_cell_font_format",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    font_name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    font_size = table.Column<double>(type: "double precision", nullable: true),
                    font_color = table.Column<string>(type: "text", nullable: true),
                    bold = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_cell_font_format", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_cell_font_format_table_cell_table_cell_id",
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
                value: "AQAAAAIAAYagAAAAEGN+Xv9z6+dg973rBpvmrnOK49Sr3TvCnOYHABJlD4UW0BEebYopfLCxD8yRsl+h7Q==");

            migrationBuilder.CreateIndex(
                name: "IX_merged_table_cell_table_cell_id",
                table: "merged_table_cell",
                column: "table_cell_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_table_cell_borders_table_cell_id",
                table: "table_cell_borders",
                column: "table_cell_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_table_cell_font_format_table_cell_id",
                table: "table_cell_font_format",
                column: "table_cell_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "merged_table_cell");

            migrationBuilder.DropTable(
                name: "table_cell_borders");

            migrationBuilder.DropTable(
                name: "table_cell_font_format");

            migrationBuilder.DropTable(
                name: "table_cell");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEIhh1d8rSTiUjaMMxAm5xmLICEwkX798BQFJh3jEd2UfZLe0os7y6wrMQj9K+yhvpA==");
        }
    }
}
