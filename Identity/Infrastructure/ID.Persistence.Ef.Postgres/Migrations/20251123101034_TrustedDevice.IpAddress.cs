using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class TrustedDeviceIpAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_team_device_subscription_id",
                schema: "MyId",
                table: "team_device");

            migrationBuilder.RenameColumn(
                name: "device_fingerprint",
                schema: "MyId",
                table: "trusted_device",
                newName: "fingerprint");

            migrationBuilder.RenameIndex(
                name: "ix_trusted_device_user_id_device_fingerprint",
                schema: "MyId",
                table: "trusted_device",
                newName: "ix_trusted_device_user_id_fingerprint");

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                schema: "MyId",
                table: "trusted_device",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_device_subscription_id_unique_id",
                schema: "MyId",
                table: "team_device",
                columns: new[] { "subscription_id", "unique_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_team_device_subscription_id_unique_id",
                schema: "MyId",
                table: "team_device");

            migrationBuilder.RenameColumn(
                name: "fingerprint",
                schema: "MyId",
                table: "trusted_device",
                newName: "device_fingerprint");

            migrationBuilder.RenameIndex(
                name: "ix_trusted_device_user_id_fingerprint",
                schema: "MyId",
                table: "trusted_device",
                newName: "ix_trusted_device_user_id_device_fingerprint");

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                schema: "MyId",
                table: "trusted_device",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "ix_team_device_subscription_id",
                schema: "MyId",
                table: "team_device",
                column: "subscription_id");
        }
    }
}
