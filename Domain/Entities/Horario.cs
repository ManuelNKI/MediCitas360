namespace Domain.Entities;

public class Horario
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public bool Disponible { get; set; }
}
