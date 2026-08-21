using System.ComponentModel.DataAnnotations;

namespace TechStore.Models;

public class Contacto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El asunto es obligatorio.")]
    public string Asunto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El mensaje es obligatorio.")]
    public string Mensaje { get; set; } = string.Empty;
}
