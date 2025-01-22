using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedTrackedWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tracking");

            migrationBuilder.CreateTable(
                name: "TrackedWorkouts",
                schema: "Tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedWorkouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackedWorkouts_Workouts_WorkoutId",
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
                name: "IX_TrackedWorkouts_WorkoutId",
                schema: "Tracking",
                table: "TrackedWorkouts",
                column: "WorkoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedWorkouts",
                schema: "Tracking");
        }
    }
}
