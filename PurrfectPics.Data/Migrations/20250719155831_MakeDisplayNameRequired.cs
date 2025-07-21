using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurrfectPics.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeDisplayNameRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_CatImages_CatImageId",
                table: "Favorites");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileBio",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_CatImages_CatImageId",
                table: "Favorites",
                column: "CatImageId",
                principalTable: "CatImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_CatImages_CatImageId",
                table: "Favorites");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileBio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_CatImages_CatImageId",
                table: "Favorites",
                column: "CatImageId",
                principalTable: "CatImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
