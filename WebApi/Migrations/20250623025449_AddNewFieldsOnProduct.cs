using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSelfCheckout.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsOnProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "OITM",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaitingTime",
                table: "OITM",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "OITM");

            migrationBuilder.DropColumn(
                name: "WaitingTime",
                table: "OITM");
        }
    }
}
