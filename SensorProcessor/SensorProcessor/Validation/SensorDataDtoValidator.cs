using FluentValidation;
using SensorProcessor.DTOs;

namespace SensorProcessor.Validation;

public class SensorDataDtoValidator : AbstractValidator<SensorDataDto>
{
    public SensorDataDtoValidator()
    {
        RuleFor(x => x.SensorId)
            .InclusiveBetween(1, 10).WithMessage("Sensor ID must be between 1 and 10");

        RuleFor(x => x.Value)
            .InclusiveBetween(0, 100).WithMessage("Value must be between 0 and 100");

        RuleFor(x => x.Timestamp)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Timestamp cannot be in the future")
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-1)).WithMessage("Timestamp cannot be older than 1 year");
    }
}