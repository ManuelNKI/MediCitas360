using API.Filters;
using Application.DTOs;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnforceMedicitasToken]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;
    private readonly IValidator<RegistroCitaDto> _validator;
    private readonly ICitaRepository _citaRepository;

    public CitasController(ICitaService citaService, IValidator<RegistroCitaDto> validator, ICitaRepository citaRepository)
    {
        _citaService = citaService;
        _validator = validator;
        _citaRepository = citaRepository;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarCita([FromBody] RegistroCitaDto dto)
    {
        // 1. Validar DTO con FluentValidation
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
        }

        try
        {
            // 2. Invocar al Caso de Uso
            var response = await _citaService.RegistrarCitaAsync(dto);
            return StatusCode(201, response); // O 200 Ok, según preferencia, 201 Created es más RESTful
        }
        catch (InvalidOperationException ex) // Maneja horario ocupado u otros similares
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex) // Maneja médico u horario no existente
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ocurrió un error interno al registrar la cita", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCitas()
    {
        var citas = await _citaRepository.GetCitasAsync();
        return Ok(citas);
    }
}
