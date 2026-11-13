using System.ComponentModel.DataAnnotations;

namespace TechStore.Models;

public class RegistroEvento
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    [StringLength(50)]
    public string Nivel { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Origen { get; set; } = string.Empty;

    [Required]
    public string Mensaje { get; set; } = string.Empty;

    public string? Usuario { get; set; }
}
