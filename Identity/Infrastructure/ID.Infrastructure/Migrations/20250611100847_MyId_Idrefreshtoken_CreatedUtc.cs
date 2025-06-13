using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ID.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MyId_Idrefreshtoken_CreatedUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                schema: "MyId",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                schema: "MyId",
                table: "RefreshTokens");
        }
    }
}
