using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BestFor.Data.Migrations
{
    public partial class A07 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowAvatar",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowCompanyName",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowDescription",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowEmail",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPhoneNumber",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowWebSite",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebSite",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowAvatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowCompanyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowDescription",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowPhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShowWebSite",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WebSite",
                table: "AspNetUsers");
        }
    }
}
