using System.ComponentModel.DataAnnotations;

namespace TechStore.Models;

/// <summary>
/// Entidad Categoría. Representa una fila de la tabla Categorias en la base de
/// datos. Una categoría agrupa varios productos (relación uno-a-muchos).
/// </summary>
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Imagen")]
    public string Imagen { get; set; } = string.Empty;

    /// <summary>
    /// Propiedad de navegación: lista de productos que pertenecen a esta
    /// categoría. Es el lado "muchos" de la relación uno-a-muchos.
    /// </summary>
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
