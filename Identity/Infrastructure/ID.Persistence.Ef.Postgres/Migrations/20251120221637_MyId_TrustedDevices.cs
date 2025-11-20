using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MyId_TrustedDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trusted_device",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_fingerprint = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    trusted_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_used_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trusted_device", x => x.id);
                    table.ForeignKey(
                        name: "fk_trusted_device_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_trusted_device_user_id_device_fingerprint",
                schema: "MyId",
                table: "trusted_device",
                columns: new[] { "user_id", "device_fingerprint" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trusted_device",
                schema: "MyId");
        }
    }
}
