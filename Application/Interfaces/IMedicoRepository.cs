using Domain.Entities;

namespace Application.Interfaces;

public interface IMedicoRepository
{
    Task<IEnumerable<Medico>> GetMedicosAsync();
    Task<Medico?> GetMedicoByIdAsync(int id);
    Task<IEnumerable<Horario>> GetHorariosByMedicoIdAsync(int medicoId);
}
