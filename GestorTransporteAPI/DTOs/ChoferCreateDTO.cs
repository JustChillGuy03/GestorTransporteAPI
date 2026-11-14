using System.ComponentModel.DataAnnotations;

namespace GestorTransporteAPI.DTOs
{
    // DTO de entrada para registrar un chofer (POST)
    public class ChoferCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La licencia es obligatoria.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "La licencia debe tener entre 5 y 20 caracteres.")]
        public string Licencia { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede pasar de 20 caracteres.")]
        public string? Telefono { get; set; }
    }
}
