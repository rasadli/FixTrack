using System.Text.Json;
using System.Text.Json.Serialization;

namespace FixTrack.Api.Extensions;

public static class ApiBehaviorExtensions
{
    // OWNER: Khavar - implement in feature/mock-auth-validation
    // (validation response shape, unknown-property handling, 405/415 envelopes).
    // The JSON options below are part of the shared contract: camelCase names,
    // enums as strings, and null properties omitted.
    public static IServiceCollection AddFixTrackApiBehavior(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }
}
