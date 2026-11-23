using System.Text.Json;
using System.Text.Json.Serialization;
using FixTrack.Api.Validation;
using FixTrack.Application.Common;
using FixTrack.Domain.Cases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FixTrack.Api.Extensions;

public static class ApiBehaviorExtensions
{
    private const string InvalidBodyMessage = "The request body must be a valid JSON object.";

    // Keep JSON names camelCase and omit null response properties.
    public static IServiceCollection AddFixTrackApiBehavior(this IServiceCollection services)
    {
        services.PostConfigure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = true;
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(item => item.Value?.Errors.Count > 0)
                    .Select(item =>
                    {
                        var field = FieldName(item.Key);
                        var message = InvalidFieldMessage(field, item.Value!.Errors);
                        return new FieldError(message == InvalidBodyMessage ? "body" : field, message);
                    })
                    .DistinctBy(error => error.Field)
                    .ToList();
                if (errors.Count > 1) errors.RemoveAll(error => error.Field == "body");
                return new BadRequestObjectResult(ApiErrors.Validation(errors));
            };
        });

        services.AddControllers(options => options.Filters.Add<CaseRequestValidationFilter>()).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        });

        return services;
    }

    // Turns System.Text.Json binding errors into client-friendly messages without exposing internal type names.
    private static string InvalidFieldMessage(string field, ModelErrorCollection errors)
    {
        var detail = string.Join(" ", errors.Select(error => error.Exception?.Message ?? error.ErrorMessage));

        if (detail.Contains("could not be mapped", StringComparison.OrdinalIgnoreCase))
        {
            return $"{field} is not an allowed field.";
        }

        if (field != "body" && detail.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
        {
            return field switch
            {
                "caseType" => $"caseType must be one of: {string.Join(", ", Enum.GetNames<CaseType>())}.",
                "status" => $"status must be one of: {string.Join(", ", Enum.GetNames<CaseStatus>())}.",
                _ => $"{field} must be a string."
            };
        }

        return InvalidBodyMessage;
    }

    private static string FieldName(string key)
    {
        var name = key[(key.LastIndexOf('.') + 1)..];
        return name is "" or "$" or "request" ? "body" : JsonNamingPolicy.CamelCase.ConvertName(name);
    }
}
