using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHandymanProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HandymanProfiles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandymanProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_HandymanProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandymanCities",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandymanCities", x => new { x.UserId, x.CityId });
                    table.ForeignKey(
                        name: "FK_HandymanCities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HandymanCities_HandymanProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "HandymanProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandymanSubCategories",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandymanSubCategories", x => new { x.UserId, x.SubCategoryId });
                    table.ForeignKey(
                        name: "FK_HandymanSubCategories_HandymanProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "HandymanProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandymanSubCategories_ServiceSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "ServiceSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandymanCities_CityId",
                table: "HandymanCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_HandymanSubCategories_SubCategoryId",
                table: "HandymanSubCategories",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandymanCities");

            migrationBuilder.DropTable(
                name: "HandymanSubCategories");

            migrationBuilder.DropTable(
                name: "HandymanProfiles");
        }
    }
}
