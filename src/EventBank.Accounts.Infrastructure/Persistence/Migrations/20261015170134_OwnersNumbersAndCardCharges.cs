using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBank.Accounts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OwnersNumbersAndCardCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "account_numbers",
                startValue: 100001L);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "account_transactions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "holder",
                table: "account_summaries",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "branch",
                table: "account_summaries",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "account_summaries",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "account_summaries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "approved_customers",
                columns: table => new
                {
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approved_customers", x => x.customer_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_summaries_owner_id",
                table: "account_summaries",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approved_customers");

            migrationBuilder.DropIndex(
                name: "ix_account_summaries_owner_id",
                table: "account_summaries");

            migrationBuilder.DropColumn(
                name: "description",
                table: "account_transactions");

            migrationBuilder.DropColumn(
                name: "branch",
                table: "account_summaries");

            migrationBuilder.DropColumn(
                name: "number",
                table: "account_summaries");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "account_summaries");

            migrationBuilder.DropSequence(
                name: "account_numbers");

            migrationBuilder.AlterColumn<string>(
                name: "holder",
                table: "account_summaries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);
        }
    }
}
