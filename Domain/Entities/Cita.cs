namespace Domain.Entities;

public class Cita
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public string Paciente { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public decimal ValorPagar { get; set; }
    public string CodigoPago { get; set; } = string.Empty;
}
