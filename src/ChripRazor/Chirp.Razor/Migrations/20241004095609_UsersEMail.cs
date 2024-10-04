using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class UsersEMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Timestamp",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Messages");
        }
    }
}
