using System.Threading;
using System.Threading.Tasks;

namespace ExternalId.Application.Abstractions;

public interface IExternalUserService
{
    Task<string> CreateUserAsync(CreateExternalUserCommand command, CancellationToken cancellationToken);
}