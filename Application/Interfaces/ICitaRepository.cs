using Domain.Entities;

namespace Application.Interfaces;

public interface ICitaRepository
{
    Task<bool> CitaExistsAsync(int medicoId, DateTime fecha, TimeSpan hora);
    Task<int> GuardarCitaConReplicaAsync(Cita cita);
    Task<IEnumerable<Cita>> GetCitasAsync();
}
