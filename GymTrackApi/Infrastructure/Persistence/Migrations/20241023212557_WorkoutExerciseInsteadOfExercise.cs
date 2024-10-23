using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutExerciseInsteadOfExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSets_Exercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets");

            migrationBuilder.DropTable(
                name: "Exercises",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                schema: "Workout",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercises", x => new { x.WorkoutId, x.Index });
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_ExerciseInfos_ExerciseInfoId",
                        column: x => x.ExerciseInfoId,
                        principalSchema: "Workout",
                        principalTable: "ExerciseInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "Workout",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "ExerciseInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets",
                columns: new[] { "WorkoutId", "ExerciseIndex" },
                principalSchema: "Workout",
                principalTable: "WorkoutExercises",
                principalColumns: new[] { "WorkoutId", "Index" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                schema: "Workout",
                table: "ExerciseSets");

            migrationBuilder.DropTable(
                name: "WorkoutExercises",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "Exercises",
                schema: "Workout",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => new { x.WorkoutId, x.Index });
                    table.ForeignKey(
                        name: "FK_Exercises_ExerciseInfos_ExerciseInfoId",
                        column: x => x.ExerciseInfoId,
                        principalSchema: "Workout",
                        principalTable: "ExerciseInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "Workout",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ExerciseInfoId",
                schema: "Workout",
                table: "Exercises",
                column: "ExerciseInfoId");

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
    }
}
