namespace FixTrack.Application.Common;

/// <summary>
/// A single validation problem for one request field.
/// </summary>
/// <param name="Field">The camelCase name of the field, as it appears in the JSON body or query string.</param>
/// <param name="Message">What is wrong with the field.</param>
public sealed record FieldError(string Field, string Message);
