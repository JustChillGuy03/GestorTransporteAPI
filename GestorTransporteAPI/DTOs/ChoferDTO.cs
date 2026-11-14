namespace GestorTransporteAPI.DTOs
{
    // DTO de respuesta: lo que la API devuelve al consultar choferes
    public class ChoferDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Licencia { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public int CantidadVehiculos { get; set; }
    }
}
