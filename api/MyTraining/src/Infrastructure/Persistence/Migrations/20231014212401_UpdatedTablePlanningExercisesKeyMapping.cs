using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class UpdatedTablePlanningExercisesKeyMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_exercise_ExerciseId",
                table: "planning_exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlaningId",
                table: "planning_exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_planning_exercises",
                table: "planning_exercises");

            migrationBuilder.DropIndex(
                name: "IX_planning_exercises_ExerciseId",
                table: "planning_exercises");

            migrationBuilder.DropColumn(
                name: "id",
                table: "planning_exercises");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "planning_exercises");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "planning_exercises");

            migrationBuilder.RenameColumn(
                name: "SeriesPlaningId",
                table: "planning_exercises",
                newName: "SeriesPlanningsId");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "planning_exercises",
                newName: "ExercisesId");

            migrationBuilder.RenameIndex(
                name: "IX_planning_exercises_SeriesPlaningId",
                table: "planning_exercises",
                newName: "IX_planning_exercises_SeriesPlanningsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_planning_exercises",
                table: "planning_exercises",
                columns: new[] { "ExercisesId", "SeriesPlanningsId" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_exercise_ExercisesId",
                table: "planning_exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlanningsId",
                table: "planning_exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_planning_exercises",
                table: "planning_exercises");

            migrationBuilder.RenameColumn(
                name: "SeriesPlanningsId",
                table: "planning_exercises",
                newName: "SeriesPlaningId");

            migrationBuilder.RenameColumn(
                name: "ExercisesId",
                table: "planning_exercises",
                newName: "ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_planning_exercises_SeriesPlanningsId",
                table: "planning_exercises",
                newName: "IX_planning_exercises_SeriesPlaningId");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "planning_exercises",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "planning_exercises",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "planning_exercises",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_planning_exercises",
                table: "planning_exercises",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_planning_exercises_ExerciseId",
                table: "planning_exercises",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_exercise_ExerciseId",
                table: "planning_exercises",
                column: "ExerciseId",
                principalTable: "exercise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planning_exercises_series_planning_SeriesPlaningId",
                table: "planning_exercises",
                column: "SeriesPlaningId",
                principalTable: "series_planning",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
