using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "table_group",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_group_cell",
                columns: table => new
                {
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    table_group_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_group_cell", x => new { x.table_cell_id, x.table_group_id });
                    table.ForeignKey(
                        name: "FK_table_group_cell_table_cell_table_cell_id",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_group_cell_table_group_table_group_id",
                        column: x => x.table_group_id,
                        principalTable: "table_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEBnlLz94rig3PhWRgNIfS7oMLoAaLokoQ87H3Lx8T0SwgxkXaMsOt8Q3YX+IskiXfQ==");

            migrationBuilder.CreateIndex(
                name: "IX_table_group_cell_table_group_id",
                table: "table_group_cell",
                column: "table_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "table_group_cell");

            migrationBuilder.DropTable(
                name: "table_group");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEGN+Xv9z6+dg973rBpvmrnOK49Sr3TvCnOYHABJlD4UW0BEebYopfLCxD8yRsl+h7Q==");
        }
    }
}
