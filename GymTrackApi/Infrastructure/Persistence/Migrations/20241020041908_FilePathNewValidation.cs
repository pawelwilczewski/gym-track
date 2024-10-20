using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FilePathNewValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile_Path",
                schema: "Workout",
                table: "ExerciseStepInfos");

            migrationBuilder.RenameColumn(
                name: "ThumbnailImage_Path",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "ThumbnailImage");

            migrationBuilder.AddColumn<string>(
                name: "ImageFile",
                schema: "Workout",
                table: "ExerciseStepInfos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                schema: "Workout",
                table: "ExerciseStepInfos");

            migrationBuilder.RenameColumn(
                name: "ThumbnailImage",
                schema: "Workout",
                table: "ExerciseInfos",
                newName: "ThumbnailImage_Path");

            migrationBuilder.AddColumn<string>(
                name: "ImageFile_Path",
                schema: "Workout",
                table: "ExerciseStepInfos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
