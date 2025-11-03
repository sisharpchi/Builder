using Microsoft.AspNetCore.Mvc;
using Contract.Dto.Auth;
using Contract.Service;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] CreateUserDto dto)
    {
        var result = await _authService.SignUp(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmPhoneOrEmailDto dto)
    {
        var result = await _authService.ConfirmEmail(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("confirm-phone")]
    public async Task<IActionResult> ConfirmPhone([FromQuery] ConfirmPhoneOrEmailDto dto)
    {
        var result = await _authService.ConfirmPhone(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}