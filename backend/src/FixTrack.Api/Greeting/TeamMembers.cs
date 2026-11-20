namespace FixTrack.Api.Greeting;

internal static class TeamMembers
{
    internal sealed record Member(string Name, string Responsibility);

    internal static IReadOnlyList<Member> All { get; } =
    [
        new("Rashad Asadli", "API contract, project integration, and repair case CRUD."),
        new("Khavar Jabbarli", "Mock authentication, request validation, and API error responses."),
        new("Faig Almuradli", "Greeting page, project documentation, example requests, and demo script.")
    ];
}
