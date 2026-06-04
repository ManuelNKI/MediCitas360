using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoRepository _medicoRepository;

    public MedicosController(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _medicoRepository.GetMedicosAsync();
        return Ok(medicos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _medicoRepository.GetMedicoByIdAsync(id);
        if (medico == null)
        {
            return NotFound(new { error = "Médico no encontrado" });
        }
        return Ok(medico);
    }

    [HttpGet("{id}/horarios")]
    public async Task<IActionResult> GetHorarios(int id)
    {
        var horarios = await _medicoRepository.GetHorariosByMedicoIdAsync(id);
        return Ok(horarios);
    }
}
