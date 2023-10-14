using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class CreateTableSeriesPlanning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "series_planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    series_number = table.Column<int>(type: "integer", nullable: false),
                    repetitions = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    charge = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    interval = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    training_sheet_series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_series_planning", x => x.id);
                    table.ForeignKey(
                        name: "FK_series_planning_training_sheet_series_training_sheet_series~",
                        column: x => x.training_sheet_series_id,
                        principalTable: "training_sheet_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_series_planning_training_sheet_series_id",
                table: "series_planning",
                column: "training_sheet_series_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "series_planning");
        }
    }
}
