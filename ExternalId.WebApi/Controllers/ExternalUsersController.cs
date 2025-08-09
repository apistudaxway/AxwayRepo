using System.Threading;
using System.Threading.Tasks;
using ExternalId.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExternalId.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExternalUsersController : ControllerBase
{
    private readonly ISender _sender;

    public ExternalUsersController(ISender sender)
    {
        _sender = sender;
    }

    public sealed class CreateExternalUserRequest
    {
        public string Email { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string GivenName { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public bool AccountEnabled { get; init; } = true;
        public bool ForceChangePasswordNextSignIn { get; init; } = false;
    }

    [HttpPost]
    public async Task<ActionResult<CreateExternalUserResult>> Create([FromBody] CreateExternalUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateExternalUserCommand(
            request.Email,
            request.DisplayName,
            request.GivenName,
            request.Surname,
            request.Password,
            request.AccountEnabled,
            request.ForceChangePasswordNextSignIn);

        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = result.UserId }, result);
    }
}