using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class CitaService : ICitaService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly ICitaRepository _citaRepository;

    public CitaService(IMedicoRepository medicoRepository, ICitaRepository citaRepository)
    {
        _medicoRepository = medicoRepository;
        _citaRepository = citaRepository;
    }

    public async Task<CitaResponseDto> RegistrarCitaAsync(RegistroCitaDto dto)
    {
        // 1. Validar que el médico exista
        var medico = await _medicoRepository.GetMedicoByIdAsync(dto.MedicoId);
        if (medico == null)
        {
            throw new ArgumentException("El médico no existe.");
        }

        // Parseo seguro garantizado por FluentValidation
        var fechaParsed = DateTime.Parse(dto.Fecha);
        var horaParsed = TimeSpan.Parse(dto.Hora);

        // 2. Validar que el horario esté disponible
        var horarios = await _medicoRepository.GetHorariosByMedicoIdAsync(dto.MedicoId);
        var horarioDisponible = horarios.FirstOrDefault(h => h.Fecha.Date == fechaParsed.Date && h.Hora == horaParsed);
        
        if (horarioDisponible == null)
        {
            throw new ArgumentException("El horario seleccionado no existe para el médico indicado.");
        }

        if (!horarioDisponible.Disponible)
        {
            throw new InvalidOperationException("El médico ya tiene una cita registrada en ese horario");
        }

        // 3. Validar que no exista ya una cita para ese horario (redundancia de seguridad)
        var citaExistente = await _citaRepository.CitaExistsAsync(dto.MedicoId, fechaParsed, horaParsed);
        if (citaExistente)
        {
            throw new InvalidOperationException("El médico ya tiene una cita registrada en ese horario");
        }

        // 4. Generar código de pago y armar la entidad
        var randomCode = new Random().Next(1000, 9999);
        var codigoPago = $"PAGO-CITA-{DateTime.Now.Year}-{randomCode}";

        var cita = new Cita
        {
            MedicoId = dto.MedicoId,
            Paciente = dto.Paciente,
            Cedula = dto.Cedula,
            Fecha = fechaParsed,
            Hora = horaParsed,
            ValorPagar = medico.CostoConsulta,
            CodigoPago = codigoPago
        };

        // 5. Invocar abstracción de guardado con réplica (Dual Persistence coordinada por Infrastructure)
        int idCita = await _citaRepository.GuardarCitaConReplicaAsync(cita);

        // 6. Retornar DTO de éxito
        return new CitaResponseDto
        {
            Mensaje = "Cita registrada correctamente",
            CitaId = idCita,
            ValorPagar = cita.ValorPagar
        };
    }
}
