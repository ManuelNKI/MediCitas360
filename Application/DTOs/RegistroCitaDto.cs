namespace Application.DTOs;

public class RegistroCitaDto
{
    public string Paciente { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public int MedicoId { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
}
