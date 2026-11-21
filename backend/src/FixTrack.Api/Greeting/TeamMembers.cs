namespace FixTrack.Api.Greeting;

internal static class TeamMembers
{
    internal sealed record Member(string Name, string StudentId, string Responsibility);

    internal static IReadOnlyList<Member> All { get; } =
    [
        new("Rashad Asadli", "17960", "API contract, project integration, and repair case CRUD."),
        new("Khavar Jabbarli", "18167", "Mock authentication, request validation, and API error responses."),
        new("Faig Almuradli", "23168", "Greeting page, project documentation, example requests, and demo script.")
    ];
}
