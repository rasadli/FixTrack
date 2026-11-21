using FixTrack.Application.Cases.Contracts;
using FixTrack.Application.Cases.Validation;
using FixTrack.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FixTrack.Api.Validation;

public sealed class CaseRequestValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            IReadOnlyList<FieldError> errors = argument switch
            {
                CreateCaseRequest create => CaseRequestValidator.Validate(create),
                UpdateCaseRequest update => CaseRequestValidator.Validate(update),
                _ => Array.Empty<FieldError>()
            };

            if (errors.Count > 0)
            {
                context.Result = new BadRequestObjectResult(ApiErrors.Validation(errors));
                return;
            }
        }
    }
}
