using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenHashing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payload",
                schema: "MyId",
                table: "refresh_tokens",
                newName: "payload_hash");

            migrationBuilder.AddColumn<string>(
                name: "selector",
                schema: "MyId",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selector",
                schema: "MyId",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "payload_hash",
                schema: "MyId",
                table: "refresh_tokens",
                newName: "payload");
        }
    }
}
