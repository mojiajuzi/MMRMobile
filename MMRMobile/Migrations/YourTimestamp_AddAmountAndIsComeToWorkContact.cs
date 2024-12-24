using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddAmountAndIsComeToWorkContact : Migration
{
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
            nullable: false,
            defaultValue: false);
    }

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