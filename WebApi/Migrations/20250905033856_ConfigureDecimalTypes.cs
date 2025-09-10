using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSelfCheckout.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDecimalTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ITM2",
                table: "ITM2");

            migrationBuilder.DropIndex(
                name: "IX_ITM2_ProductItemCode",
                table: "ITM2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ITM1",
                table: "ITM1");

            migrationBuilder.DropIndex(
                name: "IX_ITM1_ProductItemCode",
                table: "ITM1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductGroup",
                table: "ProductGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerGroup",
                table: "CustomerGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.RenameTable(
                name: "ProductGroup",
                newName: "OITG");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "ODVC");

            migrationBuilder.RenameTable(
                name: "CustomerGroup",
                newName: "OCTG");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "OCTR");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "OPOS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EANCode",
                table: "OITM",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "AltText",
                table: "OIMG",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "DeviceCode",
                table: "OIMG",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ITM1",
                type: "decimal(19,6)",
                precision: 19,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "IsPrimary",
                table: "ITM1",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PosCode",
                table: "ODVC",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITM2",
                table: "ITM2",
                columns: new[] { "ProductItemCode", "ItemCode" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITM1",
                table: "ITM1",
                columns: new[] { "ProductItemCode", "ItemCode" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OITG",
                table: "OITG",
                column: "ProductGroupCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ODVC",
                table: "ODVC",
                column: "DeviceCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OCTG",
                table: "OCTG",
                column: "CustomerGroupCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OCTR",
                table: "OCTR",
                column: "CustomerCode");

            migrationBuilder.CreateTable(
                name: "ORDR",
                columns: table => new
                {
                    DocEntry = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolioPref = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    FolioNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocStatus = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    DocType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    PaidType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Transferred = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Printed = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    DocRate = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    DocTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    DocTotalFC = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDR", x => x.DocEntry);
                });

            migrationBuilder.CreateTable(
                name: "RDR1",
                columns: table => new
                {
                    DocEntry = table.Column<int>(type: "int", nullable: false),
                    LineId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LineStatus = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDR1", x => new { x.DocEntry, x.LineId });
                    table.ForeignKey(
                        name: "FK_RDR1_ORDR_DocEntry",
                        column: x => x.DocEntry,
                        principalTable: "ORDR",
                        principalColumn: "DocEntry",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RDR1");

            migrationBuilder.DropTable(
                name: "ORDR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ITM2",
                table: "ITM2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ITM1",
                table: "ITM1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OITG",
                table: "OITG");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ODVC",
                table: "ODVC");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OCTR",
                table: "OCTR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OCTG",
                table: "OCTG");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "OPOS");

            migrationBuilder.DropColumn(
                name: "DeviceCode",
                table: "OIMG");

            migrationBuilder.DropColumn(
                name: "PosCode",
                table: "ODVC");

            migrationBuilder.RenameTable(
                name: "OITG",
                newName: "ProductGroup");

            migrationBuilder.RenameTable(
                name: "ODVC",
                newName: "Device");

            migrationBuilder.RenameTable(
                name: "OCTR",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "OCTG",
                newName: "CustomerGroup");

            migrationBuilder.AlterColumn<string>(
                name: "EANCode",
                table: "OITM",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "OIMG",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AltText",
                table: "OIMG",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ITM1",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)",
                oldPrecision: 19,
                oldScale: 6);

            migrationBuilder.AlterColumn<string>(
                name: "IsPrimary",
                table: "ITM1",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITM2",
                table: "ITM2",
                column: "ItemCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITM1",
                table: "ITM1",
                column: "ItemCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductGroup",
                table: "ProductGroup",
                column: "ProductGroupCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "DeviceCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "CustomerCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerGroup",
                table: "CustomerGroup",
                column: "CustomerGroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ITM2_ProductItemCode",
                table: "ITM2",
                column: "ProductItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_ITM1_ProductItemCode",
                table: "ITM1",
                column: "ProductItemCode");
        }
    }
}
