using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChirpIntegration.Migrations
{
    /// <inheritdoc />
    public partial class GifMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorLikeList",
                table: "Cheeps",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GifId",
                table: "Cheeps",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GifId",
                table: "Cheeps");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorLikeList",
                table: "Cheeps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
