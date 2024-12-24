using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMRMobile.Migrations
{
    /// <inheritdoc />
    public partial class updateworkContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "WorkContacts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsCome",
                table: "WorkContacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WorkContacts");

            migrationBuilder.DropColumn(
                name: "IsCome",
                table: "WorkContacts");
        }
    }
}
