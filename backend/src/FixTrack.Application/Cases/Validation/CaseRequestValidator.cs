using FixTrack.Application.Cases.Contracts;
using FixTrack.Application.Common;

namespace FixTrack.Application.Cases.Validation;

public static class CaseRequestValidator
{
    public const int CustomerNameMinLength = 2;
    public const int CustomerNameMaxLength = 100;
    public const int DeviceMinLength = 2;
    public const int DeviceMaxLength = 100;
    public const int IssueDescriptionMinLength = 10;
    public const int IssueDescriptionMaxLength = 1000;

    public static IReadOnlyList<FieldError> Validate(CreateCaseRequest request)
    {
        var errors = ValidateTextFields(request.CustomerName, request.Device, request.IssueDescription);
        ValidateEnum(errors, "caseType", request.CaseType);
        return errors;
    }

    public static IReadOnlyList<FieldError> Validate(UpdateCaseRequest request)
    {
        var errors = ValidateTextFields(request.CustomerName, request.Device, request.IssueDescription);
        ValidateEnum(errors, "caseType", request.CaseType);
        ValidateEnum(errors, "status", request.Status);
        return errors;
    }

    private static List<FieldError> ValidateTextFields(string? customerName, string? device, string? issueDescription)
    {
        var errors = new List<FieldError>();
        ValidateText(errors, "customerName", customerName, CustomerNameMinLength, CustomerNameMaxLength);
        ValidateText(errors, "device", device, DeviceMinLength, DeviceMaxLength);
        ValidateText(errors, "issueDescription", issueDescription, IssueDescriptionMinLength, IssueDescriptionMaxLength);
        return errors;
    }

    private static void ValidateText(List<FieldError> errors, string field, string? value, int min, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new FieldError(field, $"{field} is required."));
            return;
        }

        var length = value.Trim().Length;
        if (length < min || length > max)
        {
            errors.Add(new FieldError(field, $"{field} must be between {min} and {max} characters."));
        }
    }

    private static void ValidateEnum<TEnum>(List<FieldError> errors, string field, TEnum? value)
        where TEnum : struct, Enum
    {
        if (value is null)
        {
            errors.Add(new FieldError(field, $"{field} is required."));
        }
        else if (!Enum.IsDefined(value.Value))
        {
            errors.Add(new FieldError(field, $"{field} must be one of: {string.Join(", ", Enum.GetNames<TEnum>())}."));
        }
    }
}
