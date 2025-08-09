using System.Threading;
using System.Threading.Tasks;
using ExternalId.Application.Abstractions;
using MediatR;

namespace ExternalId.Application;

public sealed class CreateExternalUserHandler : IRequestHandler<CreateExternalUserCommand, CreateExternalUserResult>
{
    private readonly IExternalUserService _externalUserService;

    public CreateExternalUserHandler(IExternalUserService externalUserService)
    {
        _externalUserService = externalUserService;
    }

    public async Task<CreateExternalUserResult> Handle(CreateExternalUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _externalUserService.CreateUserAsync(request, cancellationToken);
        return new CreateExternalUserResult(userId);
    }
}