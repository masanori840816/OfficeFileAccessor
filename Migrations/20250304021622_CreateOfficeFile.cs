using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class CreateOfficeFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "office_file",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_name = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    mime_type = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    last_update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_office_file", x => x.id);
                });

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
                name: "office_file_data",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    file_data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_office_file_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_office_file_data_office_file_file_id",
                        column: x => x.file_id,
                        principalTable: "office_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAEOnwAUWEWyakcaJITTRW/1MFuCEsa/gmT89V2QnxBhRBvmePYGLReBJiLrSlm8zw+w==");

            migrationBuilder.CreateIndex(
                name: "IX_file_sheet_file_id",
                table: "file_sheet",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_office_file_data_file_id",
                table: "office_file_data",
                column: "file_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_sheet");

            migrationBuilder.DropTable(
                name: "office_file_data");

            migrationBuilder.DropTable(
                name: "office_file");

            migrationBuilder.UpdateData(
                table: "application_user",
                keyColumn: "id",
                keyValue: -1,
                column: "password",
                value: "AQAAAAIAAYagAAAAED0dsBqv4zQSw/iA9wWCJhc0VHzbUt6zWlVrpdNkByieuN2xhcbjlegCV5yUoL/D2g==");
        }
    }
}
