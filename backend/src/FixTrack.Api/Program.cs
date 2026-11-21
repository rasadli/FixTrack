// Composition root. Frozen after C0: change only via a PR reviewed by the integrator.
using FixTrack.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFixTrackApiBehavior();
builder.Services.AddMockAuthentication();
builder.Services.AddCaseServices();
builder.Services.AddFixTrackOpenApi();
var app = builder.Build();
app.UseFixTrackErrorHandling();
app.UseAuthentication();
app.UseAuthorization();
app.MapFixTrackOpenApi();
app.MapControllers();
app.MapGreeting();
app.Run();
