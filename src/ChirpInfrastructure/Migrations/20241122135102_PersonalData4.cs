using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChirpIntegration.Migrations
{
    /// <inheritdoc />
    public partial class PersonalData4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FollowingList",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowingList",
                table: "AspNetUsers");
        }
    }
}
