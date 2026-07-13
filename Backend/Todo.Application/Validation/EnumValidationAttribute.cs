using System.ComponentModel.DataAnnotations;

namespace Todo.Application.Validation;

public sealed class EnumValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValidationAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return false;

        return Enum.IsDefined(_enumType, value);
    }
}