using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "UserExerciseInfos",
                column: "ExerciseInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts");

            migrationBuilder.DropTable(
                name: "UserExerciseInfos",
                schema: "Workout");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
