using EyeCare.Application.Identity.RefreshTokens.Revoke;
using EyeCare.Application.Identity.RefreshTokens.Rotate;
using EyeCareAPI.Contracts.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(
        ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                new RotateRefreshTokenCommand(
                    request.RefreshToken),
                cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> Revoke(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new RevokeRefreshTokenCommand(
                request.RefreshToken),
            cancellationToken);

        return NoContent();
    }
}
