using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelsSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Workout");

            migrationBuilder.CreateTable(
                name: "ExerciseInfos",
                schema: "Workout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AllowedMetricTypes = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailImage_Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                schema: "Workout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseStepInfos",
                schema: "Workout",
                columns: table => new
                {
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageFile_Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseStepInfos", x => new { x.ExerciseInfoId, x.Index });
                    table.ForeignKey(
                        name: "FK_ExerciseStepInfos_ExerciseInfos_ExerciseInfoId",
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalSchema: "Workout",
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkouts_WorkoutId",
                schema: "Workout",
                table: "UserWorkouts",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseInfoId",
                schema: "Workout",
                table: "WorkoutExercises",
                column: "ExerciseInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSets",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "ExerciseStepInfos",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "UserWorkouts",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "WorkoutExercises",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "ExerciseInfos",
                schema: "Workout");

            migrationBuilder.DropTable(
                name: "Workouts",
                schema: "Workout");
        }
    }
}
