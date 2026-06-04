using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class RegistroCitaDtoValidator : AbstractValidator<RegistroCitaDto>
{
    public RegistroCitaDtoValidator()
    {
        RuleFor(x => x.Paciente)
            .NotEmpty().WithMessage("El nombre del paciente es obligatorio.");

        RuleFor(x => x.Cedula)
            .NotEmpty().WithMessage("La cédula es obligatoria.");

        RuleFor(x => x.MedicoId)
            .GreaterThan(0).WithMessage("El ID del médico debe ser válido.");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha de la cita es obligatoria y no puede estar vacía.")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("La fecha debe tener un formato válido (AAAA-MM-DD).");

        RuleFor(x => x.Hora)
            .NotEmpty().WithMessage("La hora de la cita es obligatoria y no puede estar vacía.")
            .Matches(@"^\d{2}:\d{2}(:\d{2})?$").WithMessage("La hora debe tener un formato válido de 24 horas (HH:MM).");
    }
}
