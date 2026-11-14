namespace GestorTransporteAPI.DTOs
{
    // DTO de respuesta: lo que la API devuelve al consultar vehículos
    public class VehiculoDTO
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public int ChoferId { get; set; }
        public string NombreChofer { get; set; } = string.Empty;
    }
}
