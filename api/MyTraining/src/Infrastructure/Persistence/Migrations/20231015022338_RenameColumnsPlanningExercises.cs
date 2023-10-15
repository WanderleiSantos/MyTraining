using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class RenameColumnsPlanningExercises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_exercise_ExercisesId",
                table: "planning_exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlanningsId",
                table: "planning_exercises");

            migrationBuilder.RenameColumn(
                name: "SeriesPlanningsId",
                table: "planning_exercises",
                newName: "series_planning_id");

            migrationBuilder.RenameColumn(
                name: "ExercisesId",
                table: "planning_exercises",
                newName: "exercise_id");

            migrationBuilder.RenameIndex(
                name: "IX_planning_exercises_SeriesPlanningsId",
                table: "planning_exercises",
                newName: "IX_planning_exercises_series_planning_id");

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_exercise_exercise_id",
                table: "planning_exercises",
                column: "exercise_id",
                principalTable: "exercise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_series_planning_series_planning_id",
                table: "planning_exercises",
                column: "series_planning_id",
                principalTable: "series_planning",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_exercise_exercise_id",
                table: "planning_exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_series_planning_series_planning_id",
                table: "planning_exercises");

            migrationBuilder.RenameColumn(
                name: "series_planning_id",
                table: "planning_exercises",
                newName: "SeriesPlanningsId");

            migrationBuilder.RenameColumn(
                name: "exercise_id",
                table: "planning_exercises",
                newName: "ExercisesId");

            migrationBuilder.RenameIndex(
                name: "IX_planning_exercises_series_planning_id",
                table: "planning_exercises",
                newName: "IX_planning_exercises_SeriesPlanningsId");

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_exercise_ExercisesId",
                table: "planning_exercises",
                column: "ExercisesId",
                principalTable: "exercise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlanningsId",
                table: "planning_exercises",
                column: "SeriesPlanningsId",
                principalTable: "series_planning",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
