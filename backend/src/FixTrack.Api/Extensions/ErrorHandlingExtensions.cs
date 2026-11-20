using System.Text.Json;
using System.Text.Json.Serialization;
using FixTrack.Application.Common;

namespace FixTrack.Api.Extensions;

public static class ErrorHandlingExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static WebApplication UseFixTrackErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler(handler => handler.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteError(context, ApiErrors.Internal());
        }));

        app.UseStatusCodePages(async statusContext =>
        {
            var context = statusContext.HttpContext;
            var error = context.Response.StatusCode switch
            {
                StatusCodes.Status404NotFound => ApiErrors.NotFound(Messages.ResourceNotFound),
                StatusCodes.Status405MethodNotAllowed => ApiErrors.MethodNotAllowed(),
                StatusCodes.Status415UnsupportedMediaType => ApiErrors.UnsupportedMediaType(),
                _ => null
            };

            if (error is not null)
            {
                await WriteError(context, error);
            }
        });

        return app;
    }

    private static Task WriteError(HttpContext context, ApiErrorResponse error)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        return context.Response.WriteAsync(JsonSerializer.Serialize(error, JsonOptions));
    }
}
