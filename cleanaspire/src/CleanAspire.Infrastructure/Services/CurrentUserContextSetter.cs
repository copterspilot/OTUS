
using System.Security.Claims;
using CleanAspire.Application.Common.Services;

namespace CleanAspire.Infrastructure.Services;

/// <summary>
/// Service for setting and clearing the current user context.
/// </summary>
public class CurrentUserContextSetter : ICurrentUserContextSetter
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserContextSetter"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserContextSetter(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// Sets the current user context with the provided session information.
    /// </summary>
    /// <param name="user">The session information of the current user.</param>
    public void SetCurrentUser(ClaimsPrincipal user)
    {
        _currentUserContext.Set(user);
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void Clear()
    {
        _currentUserContext.Clear();
    }
}

