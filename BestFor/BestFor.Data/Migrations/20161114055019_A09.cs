using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BestFor.Data.Migrations
{
    public partial class A09 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowDescription",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "ShowUserDescription",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowUserDescription",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "ShowDescription",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }
    }
}
