using GraphUserManagement.Application.DTOs;
using GraphUserManagement.Application.Mappers;
using GraphUserManagement.Domain.Entities;
using GraphUserManagement.Domain.Exceptions;
using GraphUserManagement.Domain.Interfaces;
using GraphUserManagement.Domain.ValueObjects;
using MediatR;

namespace GraphUserManagement.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userRepository.UserExistsAsync(request.UserDto.Email, cancellationToken);
        if (existingUser)
        {
            throw new UserAlreadyExistsException(request.UserDto.Email);
        }

        // Create domain entity
        var email = new Email(request.UserDto.Email);
        var user = new User(
            request.UserDto.DisplayName,
            email,
            request.UserDto.GivenName,
            request.UserDto.Surname,
            request.UserDto.UserPrincipalName,
            request.UserDto.JobTitle,
            request.UserDto.Department,
            request.UserDto.MobilePhone,
            request.UserDto.AccountEnabled);

        // Create user through repository
        var createdUser = await _userRepository.CreateUserAsync(user, cancellationToken);

        // Map to DTO and return
        return UserMapper.ToDto(createdUser);
    }
}