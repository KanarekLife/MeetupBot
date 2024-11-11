using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetupBot.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetupGroupsAndMeetupEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetupGroups",
                columns: table => new
                {
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Published = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetupGroups", x => x.Url);
                });

            migrationBuilder.CreateTable(
                name: "MeetupEvents",
                columns: table => new
                {
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Published = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MeetupGroupUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetupEvents", x => x.Url);
                    table.ForeignKey(
                        name: "FK_MeetupEvents_MeetupGroups_MeetupGroupUrl",
                        column: x => x.MeetupGroupUrl,
                        principalTable: "MeetupGroups",
                        principalColumn: "Url");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetupEvents_MeetupGroupUrl",
                table: "MeetupEvents",
                column: "MeetupGroupUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetupEvents");

            migrationBuilder.DropTable(
                name: "MeetupGroups");
        }
    }
}
