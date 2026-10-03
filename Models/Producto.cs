using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechStore.Models;

/// <summary>
/// Entidad Producto. Representa una fila de la tabla Productos. Cada producto
/// pertenece a una categoría mediante la clave foránea <see cref="CategoriaId"/>.
/// </summary>
public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(400, ErrorMessage = "La descripción no puede superar los 400 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, 1000000, ErrorMessage = "Ingrese un precio mayor que cero.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    [Range(0, 100000, ErrorMessage = "Ingrese un stock entre 0 y 100000.")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Display(Name = "Activo")]
    public bool Estado { get; set; } = true;

    [StringLength(100)]
    [Display(Name = "Imagen")]
    public string Imagen { get; set; } = string.Empty;

    // --- Relación con Categoria (uno-a-muchos) ---------------------------------

    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia la categoría a la que pertenece el producto.
    /// Entity Framework Core la rellena al hacer Include(p => p.Categoria).
    /// </summary>
    [ValidateNever]
    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    // --- Propiedad calculada (no se almacena en la base de datos) --------------

    [NotMapped]
    public bool Disponible => Estado && Stock > 0;
}
