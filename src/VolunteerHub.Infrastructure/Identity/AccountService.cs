using Microsoft.AspNetCore.Identity;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Infrastructure.Identity;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly INotificationService _notificationService;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        INotificationService notificationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _notificationService = notificationService;
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var allowedRoles = new[]
        {
            AppRoles.Volunteer,
            AppRoles.Organizer,
            AppRoles.Sponsor
        };

        var normalizedRole = allowedRoles
            .FirstOrDefault(x => x.Equals(request.Role, StringComparison.OrdinalIgnoreCase));

        if (normalizedRole is null)
        {
            return Result.Failure(new Error(
                "Auth.InvalidRole",
                "Role must be Volunteer, Organizer, or Sponsor."));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Auth.UserRegistrationFailed", errors));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Auth.AssignRoleFailed", errors));
        }

        await _notificationService.SendWelcomeNotificationAsync(
            user.Id,
            user.Email!,
            request.FirstName,
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _signInManager.PasswordSignInAsync(
            request.Email, 
            request.Password, 
            request.RememberMe, 
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Result.Failure(Error.InvalidCredentials);
        }

        return Result.Success();
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Auth.PasswordChangeFailed", errors));
        }

        return Result.Success();
    }
}
