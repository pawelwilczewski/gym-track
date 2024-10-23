using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutExerciseSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSets",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "WorkoutExerciseSets",
                schema: "Workout",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseIndex = table.Column<int>(type: "integer", nullable: false),
                    SetIndex = table.Column<int>(type: "integer", nullable: false),
                    Metric = table.Column<string>(type: "jsonb", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExerciseSets", x => new { x.WorkoutId, x.ExerciseIndex, x.SetIndex });
                    table.ForeignKey(
                        name: "FK_WorkoutExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                        columns: x => new { x.WorkoutId, x.ExerciseIndex },
                        principalSchema: "Workout",
                        principalTable: "WorkoutExercises",
                        principalColumns: new[] { "WorkoutId", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutExerciseSets",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "ExerciseSets",
                schema: "Workout",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseIndex = table.Column<int>(type: "integer", nullable: false),
                    SetIndex = table.Column<int>(type: "integer", nullable: false),
                    Metric = table.Column<string>(type: "jsonb", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSets", x => new { x.WorkoutId, x.ExerciseIndex, x.SetIndex });
                    table.ForeignKey(
                        name: "FK_ExerciseSets_WorkoutExercises_WorkoutId_ExerciseIndex",
                        columns: x => new { x.WorkoutId, x.ExerciseIndex },
                        principalSchema: "Workout",
                        principalTable: "WorkoutExercises",
                        principalColumns: new[] { "WorkoutId", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
