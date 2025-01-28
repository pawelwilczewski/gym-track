using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseInfos_AspNetUsers_ownerId",
                schema: "Workout",
                table: "ExerciseInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_AspNetUsers_ownerId",
                schema: "Workout",
                table: "Workouts");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                schema: "Workout",
                table: "Workouts",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_ownerId",
                schema: "Workout",
                table: "Workouts",
                newName: "IX_Workouts_OwnerId");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackedWorkouts_ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                newName: "IX_TrackedWorkouts_OwnerId");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseInfos_ownerId",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "IX_ExerciseInfos_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseInfos_AspNetUsers_OwnerId",
                schema: "Workout",
                table: "ExerciseInfos",
                column: "OwnerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_OwnerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "OwnerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_AspNetUsers_OwnerId",
                schema: "Workout",
                table: "Workouts",
                column: "OwnerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseInfos_AspNetUsers_OwnerId",
                schema: "Workout",
                table: "ExerciseInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_OwnerId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_AspNetUsers_OwnerId",
                schema: "Workout",
                table: "Workouts");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "Workout",
                table: "Workouts",
                newName: "ownerId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_OwnerId",
                schema: "Workout",
                table: "Workouts",
                newName: "IX_Workouts_ownerId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                newName: "ownerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackedWorkouts_OwnerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                newName: "IX_TrackedWorkouts_ownerId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "ownerId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseInfos_OwnerId",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "IX_ExerciseInfos_ownerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseInfos_AspNetUsers_ownerId",
                schema: "Workout",
                table: "ExerciseInfos",
                column: "ownerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "ownerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_AspNetUsers_ownerId",
                schema: "Workout",
                table: "Workouts",
                column: "ownerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
