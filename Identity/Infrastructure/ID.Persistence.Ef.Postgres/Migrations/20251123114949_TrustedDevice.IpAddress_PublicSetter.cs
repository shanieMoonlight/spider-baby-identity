using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class TrustedDeviceIpAddress_PublicSetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                schema: "MyId",
                table: "trusted_device",
                type: "character varying(75)",
                maxLength: 75,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip_address",
                schema: "MyId",
                table: "trusted_device");
        }
    }
}
