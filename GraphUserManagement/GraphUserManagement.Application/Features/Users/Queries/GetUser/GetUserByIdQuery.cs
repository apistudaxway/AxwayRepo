using GraphUserManagement.Application.DTOs;
using MediatR;

namespace GraphUserManagement.Application.Features.Users.Queries.GetUser;

public record GetUserByIdQuery(string UserId) : IRequest<UserDto?>;