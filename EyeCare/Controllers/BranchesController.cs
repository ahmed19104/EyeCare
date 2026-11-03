
using EyeCare.Application.Branches.Commands.CreateBranch;
using EyeCareAPI.Contracts.Branches;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareAPI.Controllers;

[ApiController]
[Route("api/branches")]
public class BranchesController : ControllerBase
{
    private readonly ISender _sender;

    public BranchesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBranchCommand(
            request.Name,
            request.Address,
            request.PhoneNumber);

        var branchId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/branches/{branchId}",
            new
            {
                BranchId = branchId
            });
    }
}
