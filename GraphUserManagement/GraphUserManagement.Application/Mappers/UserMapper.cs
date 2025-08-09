using GraphUserManagement.Application.DTOs;
using GraphUserManagement.Domain.Entities;

namespace GraphUserManagement.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email.Value,
            GivenName = user.GivenName,
            Surname = user.Surname,
            UserPrincipalName = user.UserPrincipalName,
            JobTitle = user.JobTitle,
            Department = user.Department,
            MobilePhone = user.MobilePhone,
            AccountEnabled = user.AccountEnabled,
            CreatedDateTime = user.CreatedDateTime,
            LastModifiedDateTime = user.LastModifiedDateTime
        };
    }

    public static IEnumerable<UserDto> ToDtoList(IEnumerable<User> users)
    {
        return users.Select(ToDto);
    }
}