using System.Text.Json;
using System.Text.Json.Serialization;
using FixTrack.Api.Validation;
using FixTrack.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace FixTrack.Api.Extensions;

public static class ApiBehaviorExtensions
{
    // OWNER: Khavar. Keep JSON names camelCase and omit null response properties.
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
                        return new FieldError(field, field == "body" ? "The request body is invalid." : $"{field} is invalid.");
                    })
                    .ToList();
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

    private static string FieldName(string key)
    {
        var name = key[(key.LastIndexOf('.') + 1)..];
        return name is "" or "$" or "request" ? "body" : JsonNamingPolicy.CamelCase.ConvertName(name);
    }
}
