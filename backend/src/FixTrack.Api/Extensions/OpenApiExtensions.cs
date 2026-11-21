using System.Text.Json.Serialization;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace FixTrack.Api.Extensions;

public static class OpenApiExtensions
{
    private const string DocumentUrl = "/openapi/v1.json";
    private const string BearerSchemeId = "Bearer";

    public static IServiceCollection AddFixTrackOpenApi(this IServiceCollection services)
    {
        // The OpenAPI generator reads these options, not the MVC ones, so enums must be strings here too.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

        services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Title = "FixTrack API";
            document.Info.Description = "Repair, warranty, and return case management (CSCI 3509, Assignment 2).";

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[BearerSchemeId] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Description = $"Mock token: {FixTrackClaims.UserAToken}, {FixTrackClaims.UserBToken} or {FixTrackClaims.AdminToken}."
            };

            document.Security ??= [];
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerSchemeId, document)] = []
            });

            return Task.CompletedTask;
        }));

        return services;
    }

    // Swagger UI at /swagger, reading the built-in OpenAPI document. Development only.
    public static WebApplication MapFixTrackOpenApi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(DocumentUrl, "FixTrack API v1");
            options.DocumentTitle = "FixTrack API";
        });

        return app;
    }
}
