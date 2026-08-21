namespace TechStore.Models;

// Datos de ejemplo en memoria. No se usa base de datos: los productos y las
// categorías viven en estas listas mientras la aplicación está en ejecución.
public static class TiendaDatos
{
    public static List<Producto> Productos { get; } = new()
    {
        new Producto
        {
            Id = 1,
            Nombre = "Laptop Lenovo IdeaPad 3",
            Descripcion = "Ryzen 5, 16 GB RAM y SSD de 512 GB. Ideal para estudio y trabajo.",
            Precio = 649.99m,
            Categoria = "Computadoras",
            Imagen = "laptop.svg",
            Stock = 8,
            Estado = true
        },
        new Producto
        {
            Id = 2,
            Nombre = "Smartphone Samsung Galaxy A54",
            Descripcion = "Pantalla AMOLED de 6.4\", cámara de 50 MP y batería de 5000 mAh.",
            Precio = 379.00m,
            Categoria = "Smartphones",
            Imagen = "smartphone.svg",
            Stock = 12,
            Estado = true
        },
        new Producto
        {
            Id = 3,
            Nombre = "Audífonos Sony WH-CH520",
            Descripcion = "Inalámbricos, con hasta 50 horas de batería y buen aislamiento.",
            Precio = 59.99m,
            Categoria = "Accesorios",
            Imagen = "headphones.svg",
            Stock = 25,
            Estado = true
        },
        new Producto
        {
            Id = 4,
            Nombre = "Teclado Mecánico Redragon Kumara",
            Descripcion = "Switches rojos, retroiluminación e ideal para escribir y jugar.",
            Precio = 42.50m,
            Categoria = "Gaming",
            Imagen = "keyboard.svg",
            Stock = 15,
            Estado = true
        },
        new Producto
        {
            Id = 5,
            Nombre = "Mouse Gamer Logitech G203",
            Descripcion = "Sensor de 8000 DPI, RGB configurable y diseño ligero.",
            Precio = 29.99m,
            Categoria = "Gaming",
            Imagen = "mouse.svg",
            Stock = 0,
            Estado = true
        },
        new Producto
        {
            Id = 6,
            Nombre = "Smartwatch Amazfit Bip 5",
            Descripcion = "GPS, monitor de salud y hasta 10 días de batería.",
            Precio = 89.90m,
            Categoria = "Accesorios",
            Imagen = "smartwatch.svg",
            Stock = 10,
            Estado = true
        },
        new Producto
        {
            Id = 7,
            Nombre = "Monitor LG UltraGear 24\"",
            Descripcion = "Full HD, 144 Hz y 1 ms. Pensado para gaming fluido.",
            Precio = 179.00m,
            Categoria = "Computadoras",
            Imagen = "monitor.svg",
            Stock = 6,
            Estado = true
        },
        new Producto
        {
            Id = 8,
            Nombre = "Tablet Xiaomi Redmi Pad SE",
            Descripcion = "Pantalla de 11\", 128 GB de almacenamiento y sonido cuádruple.",
            Precio = 199.99m,
            Categoria = "Smartphones",
            Imagen = "tablet.svg",
            Stock = 9,
            Estado = true
        }
    };

    public static List<Categoria> Categorias { get; } = new()
    {
        new Categoria
        {
            Id = 1,
            Nombre = "Computadoras",
            Descripcion = "Laptops, monitores y equipos para estudio y trabajo.",
            Imagen = "cat-computadoras.svg"
        },
        new Categoria
        {
            Id = 2,
            Nombre = "Smartphones",
            Descripcion = "Teléfonos y tablets de las marcas más buscadas.",
            Imagen = "cat-smartphones.svg"
        },
        new Categoria
        {
            Id = 3,
            Nombre = "Accesorios",
            Descripcion = "Audífonos, smartwatches y complementos para tu día a día.",
            Imagen = "cat-accesorios.svg"
        },
        new Categoria
        {
            Id = 4,
            Nombre = "Gaming",
            Descripcion = "Teclados, mouse y periféricos para jugar mejor.",
            Imagen = "cat-gaming.svg"
        }
    };
}
