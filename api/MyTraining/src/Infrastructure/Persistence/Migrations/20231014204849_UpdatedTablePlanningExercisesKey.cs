using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class UpdatedTablePlanningExercisesKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SeriesPlanningId",
                table: "planning_exercises",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_planning_exercises_ExerciseId",
                table: "planning_exercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_planning_exercises_SeriesPlanningId",
                table: "planning_exercises",
                column: "SeriesPlanningId");

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_exercise_ExerciseId",
                table: "planning_exercises",
                column: "ExerciseId",
                principalTable: "exercise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlanningId",
                table: "planning_exercises",
                column: "SeriesPlanningId",
                principalTable: "series_planning",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_exercise_ExerciseId",
                table: "planning_exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlanningId",
                table: "planning_exercises");

            migrationBuilder.DropIndex(
                name: "IX_planning_exercises_ExerciseId",
                table: "planning_exercises");

            migrationBuilder.DropIndex(
                name: "IX_planning_exercises_SeriesPlanningId",
                table: "planning_exercises");

            migrationBuilder.DropColumn(
                name: "SeriesPlanningId",
                table: "planning_exercises");
        }
    }
}
