using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class MultipleOfAttribute : ValidationAttribute
{
    private readonly int _divisor;

    public MultipleOfAttribute(int divisor)
    {
        if (divisor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(divisor));
        }

        _divisor = divisor;
        ErrorMessage = $"The {{0}} field must be a multiple of {_divisor}.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is int number && number % _divisor == 0;
    }
}
