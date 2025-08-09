using GraphUserManagement.Domain.Entities;
using GraphUserManagement.Domain.Exceptions;
using GraphUserManagement.Domain.Interfaces;
using GraphUserManagement.Domain.ValueObjects;
using GraphUserManagement.Infrastructure.Mappers;
using GraphUserManagement.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace GraphUserManagement.Infrastructure.Repositories;

public class GraphUserRepository : IUserRepository
{
    private readonly IGraphApiService _graphApiService;
    private readonly ILogger<GraphUserRepository> _logger;

    public GraphUserRepository(IGraphApiService graphApiService, ILogger<GraphUserRepository> logger)
    {
        _graphApiService = graphApiService ?? throw new ArgumentNullException(nameof(graphApiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating user in Graph API: {Email}", user.Email.Value);

            // Map domain user to Graph API user
            var graphUser = GraphUserMapper.ToGraphUser(user);
            
            // Create user in Graph API
            var createdGraphUser = await _graphApiService.CreateUserAsync(graphUser, cancellationToken);
            
            // Map back to domain user and set the ID
            var domainUser = GraphUserMapper.ToDomainUser(createdGraphUser);
            
            _logger.LogInformation("Successfully created user with ID: {UserId}", domainUser.Id);
            return domainUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email.Value);
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", userId);

            var graphUser = await _graphApiService.GetUserByIdAsync(userId, cancellationToken);
            
            if (graphUser == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", userId);
                return null;
            }

            var domainUser = GraphUserMapper.ToDomainUser(graphUser);
            
            _logger.LogInformation("Successfully retrieved user: {UserId}", userId);
            return domainUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user by email: {Email}", email);

            var graphUser = await _graphApiService.GetUserByEmailAsync(email, cancellationToken);
            
            if (graphUser == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
                return null;
            }

            var domainUser = GraphUserMapper.ToDomainUser(graphUser);
            
            _logger.LogInformation("Successfully retrieved user by email: {Email}", email);
            return domainUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user: {UserId}", user.Id);

            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("User ID is required for update", nameof(user));
            }

            // Check if user exists
            var existingUser = await GetUserByIdAsync(user.Id, cancellationToken);
            if (existingUser == null)
            {
                throw new UserNotFoundException(user.Id);
            }

            // Map domain user to Graph API user for update
            var graphUserUpdate = GraphUserMapper.ToGraphUserUpdate(user);
            
            // Update user in Graph API
            var updatedGraphUser = await _graphApiService.UpdateUserAsync(user.Id, graphUserUpdate, cancellationToken);
            
            // Map back to domain user
            var domainUser = GraphUserMapper.ToDomainUser(updatedGraphUser);
            
            _logger.LogInformation("Successfully updated user: {UserId}", user.Id);
            return domainUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user: {UserId}", userId);

            // Check if user exists
            var existingUser = await GetUserByIdAsync(userId, cancellationToken);
            if (existingUser == null)
            {
                throw new UserNotFoundException(userId);
            }

            // Delete user from Graph API
            await _graphApiService.DeleteUserAsync(userId, cancellationToken);
            
            _logger.LogInformation("Successfully deleted user: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetUsersAsync(int pageSize = 100, string? nextPageToken = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting users with page size: {PageSize}", pageSize);

            var graphUsers = await _graphApiService.GetUsersAsync(pageSize, nextPageToken, cancellationToken);
            
            var domainUsers = graphUsers.Select(GraphUserMapper.ToDomainUser).ToList();
            
            _logger.LogInformation("Successfully retrieved {Count} users", domainUsers.Count);
            return domainUsers;
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
            return await _graphApiService.UserExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists: {Email}", email);
            throw;
        }
    }
}