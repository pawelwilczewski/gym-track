using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PasswordResetCodeLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPasswordResetCodes",
                schema: "Authentication",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordResetCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordResetCodes", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPasswordResetCodes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authentication",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswordResetCodes",
                schema: "Authentication");
        }
    }
}
