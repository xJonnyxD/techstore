using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Migrations
{
    /// <inheritdoc />
    public partial class InicialTechStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descripcion", "Imagen", "Nombre" },
                values: new object[,]
                {
                    { 1, "Laptops, monitores y equipos para estudio y trabajo.", "cat-computadoras.svg", "Computadoras" },
                    { 2, "Teléfonos y tablets de las marcas más buscadas.", "cat-smartphones.svg", "Smartphones" },
                    { 3, "Audífonos, smartwatches y complementos para tu día a día.", "cat-accesorios.svg", "Accesorios" },
                    { 4, "Teclados, mouse y periféricos para jugar mejor.", "cat-gaming.svg", "Gaming" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "Imagen", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, 1, "Ryzen 5, 16 GB RAM y SSD de 512 GB. Ideal para estudio y trabajo.", true, "laptop.svg", "Laptop Lenovo IdeaPad 3", 649.99m, 8 },
                    { 2, 2, "Pantalla AMOLED de 6.4\", cámara de 50 MP y batería de 5000 mAh.", true, "smartphone.svg", "Smartphone Samsung Galaxy A54", 379.00m, 12 },
                    { 3, 3, "Inalámbricos, con hasta 50 horas de batería y buen aislamiento.", true, "headphones.svg", "Audífonos Sony WH-CH520", 59.99m, 25 },
                    { 4, 4, "Switches rojos, retroiluminación e ideal para escribir y jugar.", true, "keyboard.svg", "Teclado Mecánico Redragon Kumara", 42.50m, 15 },
                    { 5, 4, "Sensor de 8000 DPI, RGB configurable y diseño ligero.", true, "mouse.svg", "Mouse Gamer Logitech G203", 29.99m, 0 },
                    { 6, 3, "GPS, monitor de salud y hasta 10 días de batería.", true, "smartwatch.svg", "Smartwatch Amazfit Bip 5", 89.90m, 10 },
                    { 7, 1, "Full HD, 144 Hz y 1 ms. Pensado para gaming fluido.", true, "monitor.svg", "Monitor LG UltraGear 24\"", 179.00m, 6 },
                    { 8, 2, "Pantalla de 11\", 128 GB de almacenamiento y sonido cuádruple.", true, "tablet.svg", "Tablet Xiaomi Redmi Pad SE", 199.99m, 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
