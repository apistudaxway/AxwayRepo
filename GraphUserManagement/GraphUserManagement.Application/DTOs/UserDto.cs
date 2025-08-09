namespace GraphUserManagement.Application.DTOs;

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string GivenName { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string UserPrincipalName { get; init; } = string.Empty;
    public string? JobTitle { get; init; }
    public string? Department { get; init; }
    public string? MobilePhone { get; init; }
    public bool AccountEnabled { get; init; }
    public DateTime CreatedDateTime { get; init; }
    public DateTime? LastModifiedDateTime { get; init; }
}