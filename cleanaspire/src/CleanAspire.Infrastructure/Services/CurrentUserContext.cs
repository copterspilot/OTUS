using System.Security.Claims;
using CleanAspire.Application.Common.Services;

namespace CleanAspire.Infrastructure.Services;

/// <summary>
/// Represents the current user context, holding session information.
/// </summary>
public class CurrentUserContext : ICurrentUserContext
{
    private static AsyncLocal<ClaimsPrincipal?> _currentUser = new AsyncLocal<ClaimsPrincipal?>();

    public ClaimsPrincipal? GetCurrentUser() => _currentUser.Value;

    public void Set(ClaimsPrincipal? user) => _currentUser.Value = user;
    public void Clear() => _currentUser.Value = null;
}
