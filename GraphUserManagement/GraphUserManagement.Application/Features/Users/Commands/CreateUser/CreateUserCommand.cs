using GraphUserManagement.Application.DTOs;
using MediatR;

namespace GraphUserManagement.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(CreateUserDto UserDto) : IRequest<UserDto>;