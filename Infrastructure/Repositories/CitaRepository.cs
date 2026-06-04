using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly NpgsqlConnection _supabaseConnection;
    private readonly NpgsqlConnection _localConnection;

    public CitaRepository(NpgsqlConnection supabaseConnection, [FromKeyedServices("LocalDb")] NpgsqlConnection localConnection)
    {
        _supabaseConnection = supabaseConnection;
        _localConnection = localConnection;
    }

    public async Task<bool> CitaExistsAsync(int medicoId, DateTime fecha, TimeSpan hora)
    {
        await _supabaseConnection.OpenAsync();
        
        using var cmd = new NpgsqlCommand("SELECT COUNT(1) FROM citas WHERE medico_id = @medicoId AND fecha = @fecha AND hora = @hora", _supabaseConnection);
        cmd.Parameters.AddWithValue("medicoId", medicoId);
        cmd.Parameters.AddWithValue("fecha", fecha);
        cmd.Parameters.AddWithValue("hora", hora);
        
        long count = (long)(await cmd.ExecuteScalarAsync() ?? 0);
        await _supabaseConnection.CloseAsync();
        
        return count > 0;
    }

    public async Task<int> GuardarCitaConReplicaAsync(Cita cita)
    {
        int generatedId = 0;

        // 1. Inserción en Supabase (Master)
        await _supabaseConnection.OpenAsync();
        using (var cmd = new NpgsqlCommand(
            "INSERT INTO citas (medico_id, paciente, cedula, fecha, hora, valor_pagar, codigo_pago) " +
            "VALUES (@medicoId, @paciente, @cedula, @fecha, @hora, @valorPagar, @codigoPago) RETURNING id;", _supabaseConnection))
        {
            cmd.Parameters.AddWithValue("medicoId", cita.MedicoId);
            cmd.Parameters.AddWithValue("paciente", cita.Paciente);
            cmd.Parameters.AddWithValue("cedula", cita.Cedula);
            cmd.Parameters.AddWithValue("fecha", cita.Fecha);
            cmd.Parameters.AddWithValue("hora", cita.Hora);
            cmd.Parameters.AddWithValue("valorPagar", cita.ValorPagar);
            cmd.Parameters.AddWithValue("codigoPago", cita.CodigoPago);

            generatedId = (int)(await cmd.ExecuteScalarAsync() ?? throw new Exception("No se pudo obtener el ID autogenerado."));
        }
        await _supabaseConnection.CloseAsync();

        // 2. Réplica Local
        try
        {
            await _localConnection.OpenAsync();
            using (var localCmd = new NpgsqlCommand(
                "INSERT INTO citas (id, medico_id, paciente, cedula, fecha, hora, valor_pagar, codigo_pago) " +
                "VALUES (@id, @medicoId, @paciente, @cedula, @fecha, @hora, @valorPagar, @codigoPago);", _localConnection))
            {
                localCmd.Parameters.AddWithValue("id", generatedId);
                localCmd.Parameters.AddWithValue("medicoId", cita.MedicoId);
                localCmd.Parameters.AddWithValue("paciente", cita.Paciente);
                localCmd.Parameters.AddWithValue("cedula", cita.Cedula);
                localCmd.Parameters.AddWithValue("fecha", cita.Fecha);
                localCmd.Parameters.AddWithValue("hora", cita.Hora);
                localCmd.Parameters.AddWithValue("valorPagar", cita.ValorPagar);
                localCmd.Parameters.AddWithValue("codigoPago", cita.CodigoPago);

                await localCmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            // Log local replica error, but DO NOT block the flow
            Console.WriteLine($"[ADVERTENCIA] Error guardando cita {generatedId} en la réplica local: {ex.Message}");
        }
        finally
        {
            if (_localConnection.State == System.Data.ConnectionState.Open)
            {
                await _localConnection.CloseAsync();
            }
        }

        return generatedId;
    }

    public async Task<IEnumerable<Cita>> GetCitasAsync()
    {
        var citas = new List<Cita>();
        await _supabaseConnection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, medico_id, paciente, cedula, fecha, hora, valor_pagar, codigo_pago FROM citas ORDER BY fecha DESC, hora DESC", _supabaseConnection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            citas.Add(new Cita
            {
                Id = reader.GetInt32(0),
                MedicoId = reader.GetInt32(1),
                Paciente = reader.GetString(2),
                Cedula = reader.GetString(3),
                Fecha = reader.GetDateTime(4),
                Hora = reader.GetTimeSpan(5),
                ValorPagar = reader.GetDecimal(6),
                CodigoPago = reader.GetString(7)
            });
        }

        await _supabaseConnection.CloseAsync();
        return citas;
    }
}
