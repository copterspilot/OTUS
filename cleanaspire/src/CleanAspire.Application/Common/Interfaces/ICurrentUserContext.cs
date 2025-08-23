using System.Security.Claims;

namespace CleanAspire.Application.Common.Services;

/// <summary>
/// Interface representing the current user context.
/// </summary>
public interface ICurrentUserContext
{

    ClaimsPrincipal? GetCurrentUser();

    void Set(ClaimsPrincipal? user);
    void Clear();
}
