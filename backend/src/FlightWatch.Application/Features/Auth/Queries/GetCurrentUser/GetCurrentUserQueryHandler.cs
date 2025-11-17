using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<GetCurrentUserQueryHandler> logger) : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger = logger;

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result.Failure<UserDto>(
                Error.NotFound("USER_NOT_FOUND", "User not found"));
        }

        return Result.Success(_mapper.Map<UserDto>(user));
    }
}
