using System.Globalization;
using System.Text.Encodings.Web;

namespace FixTrack.Api.Greeting;

internal static class GreetingPage
{
    internal static string Render(DateTimeOffset generatedAt)
    {
        var encoder = HtmlEncoder.Default;
        var members = string.Join(Environment.NewLine, TeamMembers.All.Select(member => $"""
            <li><strong>{encoder.Encode(member.Name)}</strong><p>{encoder.Encode(member.Responsibility)}</p></li>
            """));
        var timestamp = encoder.Encode(generatedAt.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture));

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>FixTrack</title>
              <style>
                * { box-sizing: border-box; }
                body { margin: 0; background: #f1f5f4; color: #172b2a; font: 1rem/1.6 system-ui, sans-serif; }
                main { max-width: 68rem; margin: auto; padding: 3rem 2rem; }
                header { border-bottom: 2px solid #80aaa2; padding-bottom: 1.5rem; }
                h1 { margin: 0; font-size: clamp(2.5rem, 7vw, 4.5rem); line-height: 1.1; }
                h2 { margin-top: 2rem; }
                p { margin: .5rem 0; }
                ul { list-style: none; padding: 0; display: grid; grid-template-columns: repeat(3, 1fr); gap: .75rem; }
                li, .table-wrap { background: #fff; border: 1px solid #c4d6d2; border-radius: .5rem; }
                li { padding: 1rem; }
                .table-wrap { overflow-x: auto; }
                table { width: 100%; min-width: 40rem; border-collapse: collapse; text-align: left; }
                th, td { padding: .65rem .85rem; border-bottom: 1px solid #dbe6e3; }
                thead { background: #184a43; color: #fff; }
                tbody tr:last-child td { border-bottom: 0; }
                .status { margin-top: 1rem; color: #184a43; }
                @media (max-width: 42rem) {
                  main { padding: 1.5rem 1rem; }
                  ul { grid-template-columns: 1fr; }
                }
              </style>
            </head>
            <body>
              <main>
                <header>
                  <h1>FixTrack</h1>
                  <p>A repair, warranty, and return case management system.</p>
                  <p>CSCI 3509 · Introduction to Software Engineering</p>
                  <p class="status"><strong>Running</strong> · API v1</p>
                  <p>Generated at {{timestamp}} (UTC)</p>
                </header>
                <section>
                  <h2>Team</h2>
                  <ul>
                    {{members}}
                  </ul>
                </section>
                <section>
                  <h2 id="endpoints">API endpoints</h2>
                  <div class="table-wrap" role="region" aria-labelledby="endpoints" tabindex="0">
                    <table>
                      <thead><tr><th scope="col">Method</th><th scope="col">Path</th><th scope="col">Purpose</th><th scope="col">Access</th></tr></thead>
                      <tbody>
                        <tr><td>GET</td><td><code>/api/v1/cases</code></td><td>List visible cases with pagination</td><td>Bearer token</td></tr>
                        <tr><td>GET</td><td><code>/api/v1/cases/{id}</code></td><td>Get one case</td><td>Bearer token</td></tr>
                        <tr><td>POST</td><td><code>/api/v1/cases</code></td><td>Create a case owned by the requester</td><td>Bearer token</td></tr>
                        <tr><td>PUT</td><td><code>/api/v1/cases/{id}</code></td><td>Replace a case’s editable fields</td><td>Bearer token</td></tr>
                        <tr><td>DELETE</td><td><code>/api/v1/cases/{id}</code></td><td>Delete a case</td><td>Bearer token</td></tr>
                        <tr><td>GET</td><td><code>/api/v1/greeting</code></td><td>View the project and team page</td><td>Anonymous</td></tr>
                      </tbody>
                    </table>
                  </div>
                </section>
              </main>
            </body>
            </html>
            """;
    }
}
