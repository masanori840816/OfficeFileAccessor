using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    /// <inheritdoc />
    public partial class CreateApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    organization = table.Column<string>(type: "text", nullable: true),
                    mail = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    last_update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_user", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "application_user",
                columns: new[] { "id", "mail", "organization", "password", "user_name" },
                values: new object[] { -1, "default@example.com", null, "AQAAAAIAAYagAAAAEIhh1d8rSTiUjaMMxAm5xmLICEwkX798BQFJh3jEd2UfZLe0os7y6wrMQj9K+yhvpA==", "DefaultUser" });

            migrationBuilder.CreateIndex(
                name: "IX_application_user_mail",
                table: "application_user",
                column: "mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_user");
        }
    }
}
