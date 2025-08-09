using GraphUserManagement.Application.DTOs;
using GraphUserManagement.Application.Features.Users.Commands.CreateUser;
using GraphUserManagement.Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphUserManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new user in Azure Entra External ID
    /// </summary>
    /// <param name="createUserDto">User details for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromBody] CreateUserDto createUserDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating user with email: {Email}", createUserDto.Email);

            var command = new CreateUserCommand(createUserDto);
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully created user with ID: {UserId}", result.Id);

            return CreatedAtAction(
                nameof(GetUserById),
                new { userId = result.Id },
                result);
        }
        catch (Domain.Exceptions.UserAlreadyExistsException ex)
        {
            _logger.LogWarning(ex, "User already exists: {Email}", createUserDto.Email);
            return Conflict(new ProblemDetails
            {
                Title = "User Already Exists",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", createUserDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while creating the user",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetUserById(
        [FromRoute] string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", userId);

            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", userId);
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = $"User with ID '{userId}' was not found",
                    Status = StatusCodes.Status404NotFound
                });
            }

            _logger.LogInformation("Successfully retrieved user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while retrieving the user",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Gets a list of users with pagination
    /// </summary>
    /// <param name="pageSize">Number of users per page (default: 50, max: 100)</param>
    /// <param name="nextPageToken">Token for the next page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<IEnumerable<UserDto>>> GetUsers(
        [FromQuery] int pageSize = 50,
        [FromQuery] string? nextPageToken = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate page size
            if (pageSize <= 0 || pageSize > 100)
            {
                return Task.FromResult<ActionResult<IEnumerable<UserDto>>>(BadRequest(new ProblemDetails
                {
                    Title = "Invalid Page Size",
                    Detail = "Page size must be between 1 and 100",
                    Status = StatusCodes.Status400BadRequest
                }));
            }

            _logger.LogInformation("Getting users with page size: {PageSize}", pageSize);

            // For now, we'll just return an empty list since we don't have a GetUsers query implemented yet
            // In a complete implementation, you would create a GetUsersQuery
            var users = new List<UserDto>();

            _logger.LogInformation("Successfully retrieved {Count} users", users.Count);
            return Task.FromResult<ActionResult<IEnumerable<UserDto>>>(Ok(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return Task.FromResult<ActionResult<IEnumerable<UserDto>>>(StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while retrieving users",
                Status = StatusCodes.Status500InternalServerError
            }));
        }
    }
}