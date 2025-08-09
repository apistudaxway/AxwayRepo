namespace GraphUserManagement.Application.DTOs;

public record UpdateUserDto
{
    public string? DisplayName { get; init; }
    public string? GivenName { get; init; }
    public string? Surname { get; init; }
    public string? JobTitle { get; init; }
    public string? Department { get; init; }
    public string? MobilePhone { get; init; }
    public bool? AccountEnabled { get; init; }
}