namespace Application.DTOs;

public class CitaResponseDto
{
    public string Mensaje { get; set; } = string.Empty;
    public int CitaId { get; set; }
    public decimal ValorPagar { get; set; }
}
