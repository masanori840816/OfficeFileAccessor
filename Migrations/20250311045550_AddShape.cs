using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddShape : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "bordered_group",
                table: "table_group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "shape",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    office_file_sheet_id = table.Column<long>(type: "bigint", nullable: false),
                    start_column = table.Column<int>(type: "integer", nullable: false),
                    start_row = table.Column<int>(type: "integer", nullable: false),
                    start_offset_x = table.Column<double>(type: "double precision", nullable: false),
                    start_offset_y = table.Column<double>(type: "double precision", nullable: false),
                    end_column = table.Column<int>(type: "integer", nullable: true),
                    end_row = table.Column<int>(type: "integer", nullable: true),
                    end_offset_x = table.Column<double>(type: "double precision", nullable: true),
                    end_offset_y = table.Column<double>(type: "double precision", nullable: true),
                    shape_type = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shape", x => x.id);
                    table.ForeignKey(
                        name: "FK_Shape_OfficeFileSheet",
                        column: x => x.office_file_sheet_id,
                        principalTable: "office_file_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEP0hMm6kXSNJY49lciGU5V3xjAI6JFszcuPMv7VZB26OKHW/od59X8CGdAVOZSEfjQ==");

            migrationBuilder.CreateIndex(
                name: "IX_shape_office_file_sheet_id",
                table: "shape",
                column: "office_file_sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shape");

            migrationBuilder.DropColumn(
                name: "bordered_group",
                table: "table_group");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEOZKmGhF51c6c2IcjsTx9IKdg2nrSGRoOp4YwqkY9hTYeLGE9Im3vBMbwixDomMPhw==");
        }
    }
}
