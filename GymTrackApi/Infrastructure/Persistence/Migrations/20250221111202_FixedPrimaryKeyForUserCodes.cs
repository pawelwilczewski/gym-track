using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedPrimaryKeyForUserCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPasswordResetCodes",
                schema: "Authentication",
                table: "UserPasswordResetCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailConfirmationCodes",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPasswordResetCodes",
                schema: "Authentication",
                table: "UserPasswordResetCodes",
                column: "PasswordResetCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailConfirmationCodes",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes",
                column: "EmailConfirmationCode");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetCodes_UserId",
                schema: "Authentication",
                table: "UserPasswordResetCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailConfirmationCodes_UserId",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPasswordResetCodes",
                schema: "Authentication",
                table: "UserPasswordResetCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserPasswordResetCodes_UserId",
                schema: "Authentication",
                table: "UserPasswordResetCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailConfirmationCodes",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailConfirmationCodes_UserId",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPasswordResetCodes",
                schema: "Authentication",
                table: "UserPasswordResetCodes",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailConfirmationCodes",
                schema: "Authentication",
                table: "UserEmailConfirmationCodes",
                column: "UserId");
        }
    }
}
