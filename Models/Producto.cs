using System.ComponentModel.DataAnnotations;

namespace TechStore.Models;

public class Producto
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    [Display(Name = "Categoría")]
    public string Categoria { get; set; } = string.Empty;

    [Display(Name = "Imagen")]
    public string Imagen { get; set; } = string.Empty;

    public int Stock { get; set; }

    public bool Estado { get; set; }

    // Ayudas para la vista
    public bool Disponible => Estado && Stock > 0;
}
