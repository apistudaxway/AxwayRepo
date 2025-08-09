using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace GraphUserManagement.Infrastructure.Services;

public class GraphApiService : IGraphApiService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly ILogger<GraphApiService> _logger;

    public GraphApiService(GraphServiceClient graphServiceClient, ILogger<GraphApiService> logger)
    {
        _graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Microsoft.Graph.Models.User> CreateUserAsync(Microsoft.Graph.Models.User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating user with email: {Email}", user.Mail);
            
            var createdUser = await _graphServiceClient.Users.PostAsync(user, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully created user with ID: {UserId}", createdUser?.Id);
            return createdUser!;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}. Error code: {ErrorCode}", 
                user.Mail, ex.Error?.Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user with email: {Email}", user.Mail);
            throw;
        }
    }

    public async Task<Microsoft.Graph.Models.User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", userId);
            
            var user = await _graphServiceClient.Users[userId].GetAsync(cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully retrieved user by ID: {UserId}", userId);
            return user;
        }
        catch (ODataError ex) when (ex.Error?.Code == "Request_ResourceNotFound")
        {
            _logger.LogWarning("User not found with ID: {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<Microsoft.Graph.Models.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user by email: {Email}", email);
            
            var users = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"mail eq '{email}' or userPrincipalName eq '{email}'";
                requestConfiguration.QueryParameters.Top = 1;
            }, cancellationToken: cancellationToken);

            var user = users?.Value?.FirstOrDefault();
            
            if (user != null)
            {
                _logger.LogInformation("Successfully retrieved user by email: {Email}", email);
            }
            else
            {
                _logger.LogWarning("User not found with email: {Email}", email);
            }
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<Microsoft.Graph.Models.User> UpdateUserAsync(string userId, Microsoft.Graph.Models.User userUpdate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", userId);
            
            await _graphServiceClient.Users[userId].PatchAsync(userUpdate, cancellationToken: cancellationToken);
            
            // Get the updated user to return
            var updatedUser = await GetUserByIdAsync(userId, cancellationToken);
            
            _logger.LogInformation("Successfully updated user with ID: {UserId}", userId);
            return updatedUser!;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}. Error code: {ErrorCode}", 
                userId, ex.Error?.Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user with ID: {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", userId);
            
            await _graphServiceClient.Users[userId].DeleteAsync(cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully deleted user with ID: {UserId}", userId);
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}. Error code: {ErrorCode}", 
                userId, ex.Error?.Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting user with ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Microsoft.Graph.Models.User>> GetUsersAsync(int pageSize = 100, string? nextPageToken = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting users with page size: {PageSize}", pageSize);
            
            var users = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Top = pageSize;
                // Note: Skip token pagination might need to be implemented differently based on Graph API version
            }, cancellationToken: cancellationToken);

            var userList = users?.Value ?? new List<Microsoft.Graph.Models.User>();
            
            _logger.LogInformation("Successfully retrieved {Count} users", userList.Count);
            return userList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetUserByEmailAsync(email, cancellationToken);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists with email: {Email}", email);
            throw;
        }
    }
}