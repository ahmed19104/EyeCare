using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Application.Authorization.Commands.AssignUserToBranch;
using EyeCare.Application.Identity.Commands.AssignRole;
using EyeCare.Application.Identity.Commands.LoginUser;
using EyeCare.Application.Identity.Commands.RegisterUser;
using EyeCareAPI.Contracts.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public IdentityController(
        ISender sender,
        ICurrentUser currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command =
            new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password);

        var userId =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(new
        {
            UserId = userId
        });
    }
    
    [HttpPost("login")]
    [EnableRateLimiting("login-limit")]
    public async Task<IActionResult> Login(
    [FromBody] LoginUserRequest request,
    CancellationToken cancellationToken)
    {
        var command =
            new LoginUserCommand(
                request.Email,
                request.Password);

        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = _currentUser.UserId
        });
    }

    [HttpPost("users/{userId}/roles")]
    public async Task<IActionResult> AssignRole(
    string userId,
    [FromBody] AssignRoleRequest request,
    CancellationToken cancellationToken)
    {
        var command =
            new AssignRoleCommand(
                userId,
                request.Role);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = Roles.Doctor)]
    [HttpGet("doctor-test")]
    public IActionResult DoctorTest()
    {
        return Ok(new
        {
            Message = "You are a doctor."
        });
    }

    [HttpPost("users/{userId}/branches/{branchId:guid}")]
    public async Task<IActionResult> AssignUserToBranch(
    string userId,
    Guid branchId,
    CancellationToken cancellationToken)
    {
        var command =
            new AssignUserToBranchCommand(
                userId,
                branchId);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }
}
