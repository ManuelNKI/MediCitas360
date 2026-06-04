using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ICitaService
{
    Task<CitaResponseDto> RegistrarCitaAsync(RegistroCitaDto dto);
}
