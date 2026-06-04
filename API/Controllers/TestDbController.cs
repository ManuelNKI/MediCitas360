using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API.Controllers;

[ApiController]
[Route("api/test-db")]
public class TestDbController : ControllerBase
{
    private readonly NpgsqlConnection _supabase;
    private readonly NpgsqlConnection _localDb;

    public TestDbController(
        NpgsqlConnection supabase,
        [FromKeyedServices("LocalDb")] NpgsqlConnection localDb)
    {
        _supabase = supabase;
        _localDb = localDb;
    }

    [HttpGet]
    public async Task<IActionResult> CheckConnections()
    {
        var statusSupabase = "Desconectado";
        var statusLocal = "Desconectado";

        // 1. Probar Nube (Supabase)
        try
        {
            await _supabase.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM medicos", _supabase);
            var count = await cmd.ExecuteScalarAsync();
            statusSupabase = $"Conectado exitosamente. Médicos en nube: {count}";
        }
        catch (Exception ex)
        {
            statusSupabase = $"Error: {ex.Message}";
        }
        finally
        {
            await _supabase.CloseAsync();
        }

        // 2. Probar Réplica Local
        try
        {
            await _localDb.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM medicos", _localDb);
            var count = await cmd.ExecuteScalarAsync();
            statusLocal = $"Conectado exitosamente. Médicos en local: {count}";
        }
        catch (Exception ex)
        {
            statusLocal = $"Error: {ex.Message}";
        }
        finally
        {
            await _localDb.CloseAsync();
        }

        return Ok(new
        {
            Base_Principal_Supabase = statusSupabase,
            Replica_Local = statusLocal,
            Fecha_Verificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }
}
