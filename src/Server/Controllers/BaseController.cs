using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Core.Entities;
using Core.Persistence;
using Shared;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class BaseController : Controller
{
    public IMainRepository MainRepository =>
            HttpContext.RequestServices.GetRequiredService<IMainRepository>() ?? throw new ArgumentException();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(ModelState);
        }
        base.OnActionExecuting(context);
    }

    protected async Task<ResponseResult<User>> GetUser()
    {
        var userIdClaim = HttpContext?.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

        var user = await MainRepository.Query<User>().FirstOrDefaultAsync(t => t.Id == int.Parse(userIdClaim!));

        if (user == null)
            return ResponseResult<User>.CreateError("Unable find user");

        return ResponseResult<User>.CreateSuccess(user);
    }
    [NonAction]
    public IActionResult ReturnResponse<T>(ResponseResult<T> responseResult)
    {
        return responseResult.Success ? Ok(responseResult.Value) : BadRequest(responseResult);
    }
    [NonAction]
    public IActionResult ReturnResponse(ResponseResult responseResult)
    {
        return responseResult.Success ? Ok(responseResult) : BadRequest(responseResult);
    }
}
