using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DddRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_UserId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropTable(
                name: "UserExerciseInfos",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "UserWorkouts",
                schema: "Workout");

            migrationBuilder.DropIndex(
                name: "IX_TrackedWorkouts_UserId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.AddColumn<Guid>(
                name: "ownerId",
                schema: "Workout",
                table: "Workouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ownerId",
                schema: "Workout",
                table: "ExerciseInfos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ownerId",
                schema: "Workout",
                table: "Workouts",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedWorkouts_ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInfos_ownerId",
                schema: "Workout",
                table: "ExerciseInfos",
                column: "ownerId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Workouts_ownerId",
                schema: "Workout",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_TrackedWorkouts_ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseInfos_ownerId",
                schema: "Workout",
                table: "ExerciseInfos");

            migrationBuilder.DropColumn(
                name: "ownerId",
                schema: "Workout",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "ownerId",
                schema: "Tracking",
                table: "TrackedWorkouts");

            migrationBuilder.DropColumn(
                name: "ownerId",
                schema: "Workout",
                table: "ExerciseInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserExerciseInfos",
                schema: "Workout",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExerciseInfos", x => new { x.UserId, x.ExerciseInfoId });
                    table.ForeignKey(
                        name: "FK_UserExerciseInfos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExerciseInfos_ExerciseInfos_ExerciseInfoId",
                        column: x => x.ExerciseInfoId,
                        principalSchema: "Workout",
                        principalTable: "ExerciseInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkouts",
                schema: "Workout",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkouts", x => new { x.UserId, x.WorkoutId });
                    table.ForeignKey(
                        name: "FK_UserWorkouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkouts_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "Workout",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedWorkouts_UserId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "UserExerciseInfos",
                column: "ExerciseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedWorkouts_AspNetUsers_UserId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
