using MediatR;

namespace ExternalId.Application;

public sealed record CreateExternalUserCommand(
    string Email,
    string DisplayName,
    string GivenName,
    string Surname,
    string Password,
    bool AccountEnabled = true,
    bool ForceChangePasswordNextSignIn = false
) : IRequest<CreateExternalUserResult>;

public sealed record CreateExternalUserResult(string UserId);