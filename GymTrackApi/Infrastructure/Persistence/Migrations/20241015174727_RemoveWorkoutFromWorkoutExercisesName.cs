using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkoutFromWorkoutExercisesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId",
                schema: "Workout",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercises",
                schema: "Workout",
                table: "WorkoutExercises");

            migrationBuilder.RenameTable(
                name: "WorkoutExercises",
                schema: "Workout",
                newName: "Exercises",
                newSchema: "Workout");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_ExerciseInfoId",
                schema: "Workout",
                table: "Exercises",
                newName: "IX_Exercises_ExerciseInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                schema: "Workout",
                table: "Exercises",
                columns: new[] { "WorkoutId", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "Exercises",
                column: "ExerciseInfoId",
                principalSchema: "Workout",
                principalTable: "ExerciseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Workouts_WorkoutId",
                schema: "Workout",
                table: "Exercises",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSets_Exercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets",
                columns: new[] { "WorkoutId", "ExerciseIndex" },
                principalSchema: "Workout",
                principalTable: "Exercises",
                principalColumns: new[] { "WorkoutId", "Index" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Workouts_WorkoutId",
                schema: "Workout",
                table: "Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSets_Exercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                schema: "Workout",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "Exercises",
                schema: "Workout",
                newName: "WorkoutExercises",
                newSchema: "Workout");

            migrationBuilder.RenameIndex(
                name: "IX_Exercises_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_ExerciseInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercises",
                schema: "Workout",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutId", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets",
                columns: new[] { "WorkoutId", "ExerciseIndex" },
                principalSchema: "Workout",
                principalTable: "WorkoutExercises",
                principalColumns: new[] { "WorkoutId", "Index" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "ExerciseInfoId",
                principalSchema: "Workout",
                principalTable: "ExerciseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalSchema: "Workout",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
