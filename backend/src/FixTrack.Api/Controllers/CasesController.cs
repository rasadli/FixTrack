using System.Globalization;
using FixTrack.Application.Cases;
using FixTrack.Application.Cases.Contracts;
using FixTrack.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixTrack.Api.Controllers;

/// <summary>
/// CRUD endpoints for repair cases. See docs/API_CONTRACT.md.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/cases")]
[Produces("application/json")]
public sealed class CasesController(ICaseService cases) : ControllerBase
{
    private const int DefaultPage = 1;
    private const int DefaultPerPage = 10;
    private const int MaxPerPage = 50;

    /// <summary>Lists the requester's cases (admin: all cases), paginated.</summary>
    [HttpGet]
    [ProducesResponseType<PagedApiResponse<CaseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public IActionResult List([FromQuery] string? page, [FromQuery] string? perPage)
    {
        var errors = new List<FieldError>();
        var pageNumber = ParseQuery(page, nameof(page), DefaultPage, 1, int.MaxValue, errors);
        var pageSize = ParseQuery(perPage, nameof(perPage), DefaultPerPage, 1, MaxPerPage, errors);
        if (errors.Count > 0)
        {
            return BadRequest(ApiErrors.Validation(errors));
        }

        var result = cases.List(pageNumber, pageSize);
        return Ok(ApiResponses.Paged("Cases retrieved.", result.Items, result.Total, pageNumber, pageSize));
    }

    /// <summary>Gets one case.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType<ApiResponse<CaseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult GetById(string id)
    {
        var found = cases.Get(id);
        return found is null
            ? CaseNotFound()
            : Ok(ApiResponses.Ok("Case retrieved.", found));
    }

    /// <summary>Creates a case owned by the requester.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<ApiResponse<CaseResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public IActionResult Create(CreateCaseRequest request)
    {
        var created = cases.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponses.Ok("Case created.", created));
    }

    /// <summary>Replaces a case's editable fields.</summary>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType<ApiResponse<CaseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult Update(string id, UpdateCaseRequest request)
    {
        var updated = cases.Update(id, request);
        return updated is null
            ? CaseNotFound()
            : Ok(ApiResponses.Ok("Case updated.", updated));
    }

    /// <summary>Deletes a case.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string id)
    {
        return cases.Delete(id)
            ? Ok(ApiResponses.Ok<object>("Case deleted.", null))
            : CaseNotFound();
    }

    // Cross-user access deliberately gets the same 404 as a missing case (docs/API_CONTRACT.md §5).
    private NotFoundObjectResult CaseNotFound() => NotFound(ApiErrors.NotFound(Messages.CaseNotFound));

    private static int ParseQuery(string? raw, string field, int defaultValue, int min, int max, List<FieldError> errors)
    {
        if (raw is null)
        {
            return defaultValue;
        }

        if (int.TryParse(raw, NumberStyles.None, CultureInfo.InvariantCulture, out var value) && value >= min && value <= max)
        {
            return value;
        }

        errors.Add(new FieldError(field, max == int.MaxValue
            ? $"{field} must be an integer greater than or equal to {min}."
            : $"{field} must be an integer between {min} and {max}."));
        return defaultValue;
    }
}
