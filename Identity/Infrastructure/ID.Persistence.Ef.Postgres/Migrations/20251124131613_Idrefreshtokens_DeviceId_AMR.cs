using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Idrefreshtokens_DeviceId_AMR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                schema: "MyId",
                table: "trusted_device",
                type: "character varying(75)",
                maxLength: 75,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "auth_method_refs",
                schema: "MyId",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens",
                column: "trusted_device_id");

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_trusted_device_trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens",
                column: "trusted_device_id",
                principalSchema: "MyId",
                principalTable: "trusted_device",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_trusted_device_trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "auth_method_refs",
                schema: "MyId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "trusted_device_id",
                schema: "MyId",
                table: "refresh_tokens");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                schema: "MyId",
                table: "trusted_device",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(75)",
                oldMaxLength: 75);
        }
    }
}
