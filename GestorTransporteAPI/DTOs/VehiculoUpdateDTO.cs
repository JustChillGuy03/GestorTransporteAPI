using System.ComponentModel.DataAnnotations;

namespace GestorTransporteAPI.DTOs
{
    // DTO de entrada para actualizar un vehículo (PUT)
    public class VehiculoUpdateDTO
    {
        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50, ErrorMessage = "La marca no puede pasar de 50 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(50, ErrorMessage = "El modelo no puede pasar de 50 caracteres.")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "La placa debe tener entre 5 y 10 caracteres.")]
        public string Placa { get; set; } = string.Empty;

        [Range(1, 60, ErrorMessage = "La capacidad debe estar entre 1 y 60 asientos.")]
        public int Capacidad { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el Id de un chofer válido.")]
        public int ChoferId { get; set; }
    }
}
