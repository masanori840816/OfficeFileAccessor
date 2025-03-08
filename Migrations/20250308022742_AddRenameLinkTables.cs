using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddRenameLinkTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_sheet");

            migrationBuilder.DropTable(
                name: "sheet_column_width");

            migrationBuilder.DropTable(
                name: "sheet_row_height");

            migrationBuilder.DropTable(
                name: "sheet_table_group");

            migrationBuilder.DropTable(
                name: "table_group_cell");

            migrationBuilder.DropPrimaryKey(
                name: "PK_table_colmun_width",
                table: "table_colmun_width");

            migrationBuilder.RenameTable(
                name: "table_colmun_width",
                newName: "table_column_width");

            migrationBuilder.AddPrimaryKey(
                name: "PK_table_column_width",
                table: "table_column_width",
                column: "id");

            migrationBuilder.CreateTable(
                name: "link_file_sheet",
                columns: table => new
                {
                    sheet_id = table.Column<long>(type: "bigint", nullable: false),
                    file_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_file_sheet", x => new { x.sheet_id, x.file_id });
                    table.ForeignKey(
                        name: "FK_link_file_sheet_office_file_file_id",
                        column: x => x.file_id,
                        principalTable: "office_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_link_file_sheet_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "link_group_cell",
                columns: table => new
                {
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    table_group_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_group_cell", x => new { x.table_cell_id, x.table_group_id });
                    table.ForeignKey(
                        name: "FK_link_group_cell_table_cell_table_cell_id",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_link_group_cell_table_group_table_group_id",
                        column: x => x.table_group_id,
                        principalTable: "table_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "link_sheet_column_width",
                columns: table => new
                {
                    column_width_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_sheet_column_width", x => new { x.column_width_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_link_sheet_column_width_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_link_sheet_column_width_table_column_width_column_width_id",
                        column: x => x.column_width_id,
                        principalTable: "table_column_width",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "link_sheet_group",
                columns: table => new
                {
                    table_group_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_sheet_group", x => new { x.table_group_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_link_sheet_group_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_link_sheet_group_table_group_table_group_id",
                        column: x => x.table_group_id,
                        principalTable: "table_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "link_sheet_row_height",
                columns: table => new
                {
                    row_height_id = table.Column<long>(type: "bigint", nullable: false),
                    sheet_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_sheet_row_height", x => new { x.row_height_id, x.sheet_id });
                    table.ForeignKey(
                        name: "FK_link_sheet_row_height_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_link_sheet_row_height_table_row_height_row_height_id",
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
                value: "AQAAAAIAAYagAAAAEM2vBpvCb/v9+tSEnWdDn34+U+MxV8ZIsfrSe9kC4p6prOmnKBgrcYFnkkWOKEig4w==");

            migrationBuilder.CreateIndex(
                name: "IX_link_file_sheet_file_id",
                table: "link_file_sheet",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_link_group_cell_table_group_id",
                table: "link_group_cell",
                column: "table_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_link_sheet_column_width_sheet_id",
                table: "link_sheet_column_width",
                column: "sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_link_sheet_group_sheet_id",
                table: "link_sheet_group",
                column: "sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_link_sheet_row_height_sheet_id",
                table: "link_sheet_row_height",
                column: "sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "link_file_sheet");

            migrationBuilder.DropTable(
                name: "link_group_cell");

            migrationBuilder.DropTable(
                name: "link_sheet_column_width");

            migrationBuilder.DropTable(
                name: "link_sheet_group");

            migrationBuilder.DropTable(
                name: "link_sheet_row_height");

            migrationBuilder.DropPrimaryKey(
                name: "PK_table_column_width",
                table: "table_column_width");

            migrationBuilder.RenameTable(
                name: "table_column_width",
                newName: "table_colmun_width");

            migrationBuilder.AddPrimaryKey(
                name: "PK_table_colmun_width",
                table: "table_colmun_width",
                column: "id");

            migrationBuilder.CreateTable(
                name: "file_sheet",
                columns: table => new
                {
                    sheet_id = table.Column<long>(type: "bigint", nullable: false),
                    file_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_sheet", x => new { x.sheet_id, x.file_id });
                    table.ForeignKey(
                        name: "FK_file_sheet_office_file_file_id",
                        column: x => x.file_id,
                        principalTable: "office_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_sheet_office_file_sheet_sheet_id",
                        column: x => x.sheet_id,
                        principalTable: "office_file_sheet",
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
                value: "AQAAAAIAAYagAAAAEG4STBsC1UpSZwAiK5ed24z1O3mdLvFyPf95Il3d3gcP25u14ib/Az9xI/uDOZW11Q==");

            migrationBuilder.CreateIndex(
                name: "IX_file_sheet_file_id",
                table: "file_sheet",
                column: "file_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_table_group_cell_table_group_id",
                table: "table_group_cell",
                column: "table_group_id");
        }
    }
}
