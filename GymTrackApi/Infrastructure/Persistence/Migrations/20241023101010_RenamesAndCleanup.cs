using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamesAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseStepInfos",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "ExerciseInfoSteps",
                schema: "Workout",
                columns: table => new
                {
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseInfoSteps", x => new { x.ExerciseInfoId, x.Index });
                    table.ForeignKey(
                        name: "FK_ExerciseInfoSteps_ExerciseInfos_ExerciseInfoId",
                        column: x => x.ExerciseInfoId,
                        principalSchema: "Workout",
                        principalTable: "ExerciseInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseInfoSteps",
                schema: "Workout");

            migrationBuilder.CreateTable(
                name: "ExerciseStepInfos",
                schema: "Workout",
                columns: table => new
                {
                    ExerciseInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
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
        }
    }
}
