using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class CreateOfficeFileSheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "office_file_sheet",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_office_file_sheet", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_colmun_width",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    column = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_colmun_width", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_row_height",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    row = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_row_height", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sheet_table_group",
                columns: table => new
                {
                    table_group_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheet_table_group", x => new { x.table_group_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_sheet_table_group_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_table_group_table_group_table_group_id",
                        column: x => x.table_group_id,
                        principalTable: "table_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sheet_column_width",
                columns: table => new
                {
                    column_width_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheet_column_width", x => new { x.column_width_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_sheet_column_width_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_column_width_table_colmun_width_column_width_id",
                        column: x => x.column_width_id,
                        principalTable: "table_colmun_width",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sheet_row_height",
                columns: table => new
                {
                    row_height_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheet_row_height", x => new { x.row_height_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_sheet_row_height_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sheet_row_height_table_row_height_row_height_id",
                        column: x => x.row_height_id,
                        principalTable: "table_row_height",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAED0dsBqv4zQSw/iA9wWCJhc0VHzbUt6zWlVrpdNkByieuN2xhcbjlegCV5yUoL/D2g==");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_column_width_sheet_id",
                table: "sheet_column_width",
                column: "sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_row_height_sheet_id",
                table: "sheet_row_height",
                column: "sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheet_table_group_sheet_id",
                table: "sheet_table_group",
                column: "sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sheet_column_width");

            migrationBuilder.DropTable(
                name: "sheet_row_height");

            migrationBuilder.DropTable(
                name: "sheet_table_group");

            migrationBuilder.DropTable(
                name: "table_colmun_width");

            migrationBuilder.DropTable(
                name: "table_row_height");

            migrationBuilder.DropTable(
                name: "office_file_sheet");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEBnlLz94rig3PhWRgNIfS7oMLoAaLokoQ87H3Lx8T0SwgxkXaMsOt8Q3YX+IskiXfQ==");
        }
    }
}
