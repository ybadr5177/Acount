using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountDAL.Data.MigrationCoza
{
    public partial class Fnew6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionSizes_DifferentSizes_DifferentSizeId",
                table: "DimensionSizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DimensionSizes",
                table: "DimensionSizes");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "DimensionSizes",
                newName: "DimensionSize");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "DifferentSizes",
                newName: "SizeNameId");

            migrationBuilder.RenameIndex(
                name: "IX_DimensionSizes_DifferentSizeId",
                table: "DimensionSize",
                newName: "IX_DimensionSize_DifferentSizeId");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductImageId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DimensionSize",
                table: "DimensionSize",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SizeNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeNames = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeNames", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DifferentSizes_SizeNameId",
                table: "DifferentSizes",
                column: "SizeNameId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DifferentSizes_SizeNames_SizeNameId",
                table: "DifferentSizes",
                column: "SizeNameId",
                principalTable: "SizeNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionSize_DifferentSizes_DifferentSizeId",
                table: "DimensionSize",
                column: "DifferentSizeId",
                principalTable: "DifferentSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DifferentSizes_SizeNames_SizeNameId",
                table: "DifferentSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_DimensionSize_DifferentSizes_DifferentSizeId",
                table: "DimensionSize");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "SizeNames");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_DifferentSizes_SizeNameId",
                table: "DifferentSizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DimensionSize",
                table: "DimensionSize");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductImageId",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "DimensionSize",
                newName: "DimensionSizes");

            migrationBuilder.RenameColumn(
                name: "SizeNameId",
                table: "DifferentSizes",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_DimensionSize_DifferentSizeId",
                table: "DimensionSizes",
                newName: "IX_DimensionSizes_DifferentSizeId");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DimensionSizes",
                table: "DimensionSizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionSizes_DifferentSizes_DifferentSizeId",
                table: "DimensionSizes",
                column: "DifferentSizeId",
                principalTable: "DifferentSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
