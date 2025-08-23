
using System.Security.Claims;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanAspire.Application.Common.Services;

namespace CleanAspire.Infrastructure.Services;

/// <summary>
/// Provides access to the current user's session information.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessor"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserAccessor(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// Gets the session information of the current user.
    /// </summary>
    public ClaimsPrincipal? User => _currentUserContext.GetCurrentUser();

    public string? UserId => User?.GetUserId();

    public string? TenantId => User?.GetTenantId();
}

