using GraphUserManagement.Application.DTOs;
using GraphUserManagement.Application.Mappers;
using GraphUserManagement.Domain.Interfaces;
using MediatR;

namespace GraphUserManagement.Application.Features.Users.Queries.GetUser;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        return user != null ? UserMapper.ToDto(user) : null;
    }
}