using Microsoft.Graph.Models;

namespace GraphUserManagement.Infrastructure.Services;

public interface IGraphApiService
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(string userId, User userUpdate, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersAsync(int pageSize = 100, string? nextPageToken = null, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
}