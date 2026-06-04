using Application.Interfaces;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionSupabase = configuration.GetConnectionString("SupabaseConnection")!;
        string connectionLocal = configuration.GetConnectionString("LocalConnection")!;

        // Conexión principal (Supabase)
        services.AddTransient<NpgsqlConnection>(sp => new NpgsqlConnection(connectionSupabase));

        // Conexión secundaria mapeada con llave para la Réplica Local
        services.AddKeyedTransient<NpgsqlConnection>("LocalDb", (sp, key) => new NpgsqlConnection(connectionLocal));

        // Repositorios
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<ICitaRepository, CitaRepository>();

        // Servicios de Aplicación
        services.AddScoped<ICitaService, CitaService>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<RegistroCitaDtoValidator>();

        return services;
    }
}
