using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Application.Abstractions;

public interface IAccountService
{
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
