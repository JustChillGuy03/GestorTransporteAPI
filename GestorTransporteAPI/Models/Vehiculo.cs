namespace GestorTransporteAPI.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public int Capacidad { get; set; }

        // Llave foránea hacia el chofer asignado
        public int ChoferId { get; set; }
        public Chofer? Chofer { get; set; }
    }
}
