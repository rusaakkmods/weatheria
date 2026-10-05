using System.Data;
using FluentValidation;

namespace Weatheria.Application.Features.WeatherNotes;

public sealed class CreateWeatherNoteCommandValidator : AbstractValidator<CreateWeatherNoteCommand>
{
    public CreateWeatherNoteCommandValidator()
    {
        RuleFor(x => x.City)
        .NotEmpty()
        .MaximumLength(100);

        RuleFor(x => x.Content)
        .NotEmpty()
        .MaximumLength(1000);
    }
}