using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSelfCheckout.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class update_image_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "OIMG");

            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "OIMG",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "OIMG",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "OIMG",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OIMG",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OIMG",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "OIMG",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "OIMG",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "OIMG",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "OIMG",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OIMG",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "OIMG",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicUrl",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OIMG",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltText",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "PublicUrl",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OIMG");

            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "OIMG",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OIMG",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Enabled",
                table: "OIMG",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "OIMG",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
