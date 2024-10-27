using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutExerciseSetIndexRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SetIndex",
                schema: "Workout",
                table: "WorkoutExerciseSets",
                newName: "Index");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Index",
                schema: "Workout",
                table: "WorkoutExerciseSets",
                newName: "SetIndex");
        }
    }
}
