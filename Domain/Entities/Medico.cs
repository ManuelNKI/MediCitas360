namespace Domain.Entities;

public class Medico
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Consultorio { get; set; } = string.Empty;
    public decimal CostoConsulta { get; set; }
    public int DuracionTurno { get; set; } // En minutos, según sea aplicable
}
