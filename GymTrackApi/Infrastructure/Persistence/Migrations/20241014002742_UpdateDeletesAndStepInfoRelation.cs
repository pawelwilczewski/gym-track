using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeletesAndStepInfoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "ExerciseInfoId",
                principalSchema: "Workout",
                principalTable: "ExerciseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorkouts_Workouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "ExerciseInfoId",
                principalSchema: "Workout",
                principalTable: "ExerciseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
