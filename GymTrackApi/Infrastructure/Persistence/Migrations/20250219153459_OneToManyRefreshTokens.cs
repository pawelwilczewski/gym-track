using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "Authentication",
                table: "UserRefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "Authentication",
                table: "UserRefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "Authentication",
                table: "UserRefreshTokens",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserId",
                schema: "Authentication",
                table: "UserRefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "Authentication",
                table: "UserRefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_UserId",
                schema: "Authentication",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Authentication",
                table: "UserRefreshTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "Authentication",
                table: "UserRefreshTokens",
                column: "UserId");
        }
    }
}
