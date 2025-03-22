using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    office_file_id = table.Column<long>(type: "bigint", nullable: false),
                    working_user_id = table.Column<int>(type: "integer", nullable: true),
                    finish_working_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_WorkRecord_ApplicationUser",
                        column: x => x.working_user_id,
                        principalTable: "application_user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_WorkRecord_OfficeFile",
                        column: x => x.office_file_id,
                        principalTable: "office_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "input_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_record_id = table.Column<long>(type: "bigint", nullable: false),
                    table_cell_id = table.Column<long>(type: "bigint", nullable: false),
                    result = table.Column<string>(type: "text", nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    update_user_id = table.Column<int>(type: "integer", nullable: false),
                    last_update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_input_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_InputRecord_ApplicationUser",
                        column: x => x.update_user_id,
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InputRecord_TableCell",
                        column: x => x.table_cell_id,
                        principalTable: "table_cell",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InputRecord_WorkRecord",
                        column: x => x.work_record_id,
                        principalTable: "work_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEJwGfBgUuOP8V7tK/Up7FfFPvHT+kBz7bzJySmvCDpfJzvW58IiRx7lJhD6tCkHMTg==");

            migrationBuilder.CreateIndex(
                name: "IX_input_record_table_cell_id",
                table: "input_record",
                column: "table_cell_id");

            migrationBuilder.CreateIndex(
                name: "IX_input_record_update_user_id",
                table: "input_record",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_input_record_work_record_id",
                table: "input_record",
                column: "work_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_record_office_file_id",
                table: "work_record",
                column: "office_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_record_working_user_id",
                table: "work_record",
                column: "working_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "input_record");

            migrationBuilder.DropTable(
                name: "work_record");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEP0hMm6kXSNJY49lciGU5V3xjAI6JFszcuPMv7VZB26OKHW/od59X8CGdAVOZSEfjQ==");
        }
    }
}
