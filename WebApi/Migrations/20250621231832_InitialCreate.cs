using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSelfCheckout.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TaxIdentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CellPhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Datasource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CustomerGroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerCode);
                });

            migrationBuilder.CreateTable(
                name: "CustomerGroup",
                columns: table => new
                {
                    CustomerGroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerGroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Datasource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGroup", x => x.CustomerGroupCode);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.DeviceCode);
                });

            migrationBuilder.CreateTable(
                name: "OIMG",
                columns: table => new
                {
                    ImageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OIMG", x => x.ImageCode);
                });

            migrationBuilder.CreateTable(
                name: "OITC",
                columns: table => new
                {
                    CategoryItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FrgnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FrgnDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VisOrder = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    GroupItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OITC", x => x.CategoryItemCode);
                });

            migrationBuilder.CreateTable(
                name: "OITM",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EANCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FrgnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FrgnDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SellItem = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Available = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    GroupItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OITM", x => x.ItemCode);
                });

            migrationBuilder.CreateTable(
                name: "OITT",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OITT", x => x.ItemCode);
                });

            migrationBuilder.CreateTable(
                name: "ONMN",
                columns: table => new
                {
                    ObjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InitialNum = table.Column<int>(type: "int", nullable: false),
                    NextNumber = table.Column<int>(type: "int", nullable: false),
                    LastNum = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PeriodCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ONMN", x => x.ObjectCode);
                });

            migrationBuilder.CreateTable(
                name: "OPOS",
                columns: table => new
                {
                    PosCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PosName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Datasource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    SISCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxIdentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPOS", x => x.PosCode);
                });

            migrationBuilder.CreateTable(
                name: "OSHP",
                columns: table => new
                {
                    ShippingCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ShippingName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OSHP", x => x.ShippingCode);
                });

            migrationBuilder.CreateTable(
                name: "OSTC",
                columns: table => new
                {
                    TaxCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TaxName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OSTC", x => x.TaxCode);
                });

            migrationBuilder.CreateTable(
                name: "OWOR",
                columns: table => new
                {
                    DocEntry = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNum = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkToObj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndTime = table.Column<int>(type: "int", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Printed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OWOR", x => x.DocEntry);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroup",
                columns: table => new
                {
                    ProductGroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductGroupName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FrgnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FrgnDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Enabled = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VisOrder = table.Column<int>(type: "int", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ProductGroupCodeERP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductGroupCodePOS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroup", x => x.ProductGroupCode);
                });

            migrationBuilder.CreateTable(
                name: "ITM1",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductItemCode = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITM1", x => x.ItemCode);
                    table.ForeignKey(
                        name: "FK_ITM1_OITM_ProductItemCode",
                        column: x => x.ProductItemCode,
                        principalTable: "OITM",
                        principalColumn: "ItemCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITM2",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PriceOld = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductItemCode = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITM2", x => x.ItemCode);
                    table.ForeignKey(
                        name: "FK_ITM2_OITM_ProductItemCode",
                        column: x => x.ProductItemCode,
                        principalTable: "OITM",
                        principalColumn: "ItemCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITT1",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTreeItemCode = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITT1", x => x.ItemCode);
                    table.ForeignKey(
                        name: "FK_ITT1_OITT_ProductTreeItemCode",
                        column: x => x.ProductTreeItemCode,
                        principalTable: "OITT",
                        principalColumn: "ItemCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WOR1",
                columns: table => new
                {
                    WorkOrderDocEntry = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    VisOrder = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkOrderDocEntry1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOR1", x => x.WorkOrderDocEntry);
                    table.ForeignKey(
                        name: "FK_WOR1_OWOR_WorkOrderDocEntry1",
                        column: x => x.WorkOrderDocEntry1,
                        principalTable: "OWOR",
                        principalColumn: "DocEntry",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITM1_ProductItemCode",
                table: "ITM1",
                column: "ProductItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_ITM2_ProductItemCode",
                table: "ITM2",
                column: "ProductItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_ITT1_ProductTreeItemCode",
                table: "ITT1",
                column: "ProductTreeItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_WOR1_WorkOrderDocEntry1",
                table: "WOR1",
                column: "WorkOrderDocEntry1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "CustomerGroup");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "ITM1");

            migrationBuilder.DropTable(
                name: "ITM2");

            migrationBuilder.DropTable(
                name: "ITT1");

            migrationBuilder.DropTable(
                name: "OIMG");

            migrationBuilder.DropTable(
                name: "OITC");

            migrationBuilder.DropTable(
                name: "ONMN");

            migrationBuilder.DropTable(
                name: "OPOS");

            migrationBuilder.DropTable(
                name: "OSHP");

            migrationBuilder.DropTable(
                name: "OSTC");

            migrationBuilder.DropTable(
                name: "ProductGroup");

            migrationBuilder.DropTable(
                name: "WOR1");

            migrationBuilder.DropTable(
                name: "OITM");

            migrationBuilder.DropTable(
                name: "OITT");

            migrationBuilder.DropTable(
                name: "OWOR");
        }
    }
}
