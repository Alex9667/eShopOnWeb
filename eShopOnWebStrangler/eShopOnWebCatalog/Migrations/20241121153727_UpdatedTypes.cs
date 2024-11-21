using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopOnWebCatalog.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.RenameColumn(
                name: "ItemType",
                table: "CatalogTypes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TypeID",
                table: "CatalogTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BrandID",
                table: "CatalogBrands",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(6,2)", nullable: false),
                    PictureUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogTypeId = table.Column<int>(type: "int", nullable: false),
                    CatalogBrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogBrands_CatalogBrandId",
                        column: x => x.CatalogBrandId,
                        principalTable: "CatalogBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogTypes_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CatalogTypes",
                newName: "ItemType");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CatalogTypes",
                newName: "TypeID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CatalogBrands",
                newName: "BrandID");

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogBrandId = table.Column<int>(type: "int", nullable: false),
                    CatalogTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogBrands_CatalogBrandId",
                        column: x => x.CatalogBrandId,
                        principalTable: "CatalogBrands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogTypes_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogBrandId",
                table: "CatalogItems",
                column: "CatalogBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogTypeId",
                table: "CatalogItems",
                column: "CatalogTypeId");
        }
    }
}
