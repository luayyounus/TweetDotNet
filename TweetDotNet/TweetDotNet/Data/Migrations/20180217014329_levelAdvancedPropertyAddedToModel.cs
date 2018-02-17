using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TweetDotNet.Data.Migrations
{
    public partial class levelAdvancedPropertyAddedToModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LevelAdvanced",
                table: "TweetBlogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LevelBeginner",
                table: "TweetBlogs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LevelAdvanced",
                table: "TweetBlogs");

            migrationBuilder.DropColumn(
                name: "LevelBeginner",
                table: "TweetBlogs");
        }
    }
}
