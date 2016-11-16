using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BestFor.Data.Migrations
{
    public partial class A08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserDescription",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDescription",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);
        }
    }
}
