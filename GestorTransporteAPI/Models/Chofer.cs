namespace GestorTransporteAPI.Models
{
    public class Chofer
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Licencia { get; set; } = string.Empty;
        public string? Telefono { get; set; }

        // Relación 1 a muchos: un chofer puede tener varios vehículos asignados
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}
