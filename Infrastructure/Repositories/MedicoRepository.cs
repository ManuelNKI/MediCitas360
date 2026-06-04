using Application.Interfaces;
using Domain.Entities;
using Npgsql;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly NpgsqlConnection _connection;

    public MedicoRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Medico>> GetMedicosAsync()
    {
        var medicos = new List<Medico>();
        await _connection.OpenAsync();
        
        using var cmd = new NpgsqlCommand("SELECT id, nombre, especialidad, consultorio, costo_consulta, duracion_turno FROM medicos", _connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            medicos.Add(new Medico
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Especialidad = reader.GetString(2),
                Consultorio = reader.GetString(3),
                CostoConsulta = reader.GetDecimal(4),
                DuracionTurno = reader.GetInt32(5)
            });
        }
        
        await _connection.CloseAsync();
        return medicos;
    }

    public async Task<Medico?> GetMedicoByIdAsync(int id)
    {
        Medico? medico = null;
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, nombre, especialidad, consultorio, costo_consulta, duracion_turno FROM medicos WHERE id = @id", _connection);
        cmd.Parameters.AddWithValue("id", id);
        
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            medico = new Medico
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Especialidad = reader.GetString(2),
                Consultorio = reader.GetString(3),
                CostoConsulta = reader.GetDecimal(4),
                DuracionTurno = reader.GetInt32(5)
            };
        }

        await _connection.CloseAsync();
        return medico;
    }

    public async Task<IEnumerable<Horario>> GetHorariosByMedicoIdAsync(int medicoId)
    {
        var horarios = new List<Horario>();
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, medico_id, fecha, hora, disponible FROM horarios WHERE medico_id = @medicoId ORDER BY fecha ASC, hora ASC", _connection);
        cmd.Parameters.AddWithValue("medicoId", medicoId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            horarios.Add(new Horario
            {
                Id = reader.GetInt32(0),
                MedicoId = reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                Hora = reader.GetTimeSpan(3),
                Disponible = reader.GetBoolean(4)
            });
        }

        await _connection.CloseAsync();
        return horarios;
    }
}
