using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillerSimulator.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCustomer24091210301012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerNumber",
                table: "AppCustomers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNumber",
                table: "AppCustomers");
        }
    }
}
