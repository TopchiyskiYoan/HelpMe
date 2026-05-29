using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHandymanIsVerified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "HandymanProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "HandymanProfiles");
        }
    }
}
