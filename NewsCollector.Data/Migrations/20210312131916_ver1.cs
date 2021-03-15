using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace NewsCollector.Data.Migrations
{
    public partial class ver1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewsContent",
                table: "News",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "News",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "turkish")
                .Annotation("Npgsql:TsVectorProperties", new[] { "NewsTitle", "NewsContent" });

            migrationBuilder.CreateIndex(
                name: "IX_News_SearchVector",
                table: "News",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_News_SearchVector",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsContent",
                table: "News");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "News");
        }
    }
}
