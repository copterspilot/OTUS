using System.Security.Claims;

namespace CleanAspire.Application.Common.Services;

/// <summary>
/// Interface to access the current user's session information.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets the current session information of the user.
    /// </summary>
    ClaimsPrincipal? User { get; }
    string? UserId { get; }
    string? TenantId { get; }
}
