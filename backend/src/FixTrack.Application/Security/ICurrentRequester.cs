namespace FixTrack.Application.Security;

/// <summary>
/// Provides the identity of the user making the current request.
/// </summary>
public interface ICurrentRequester
{
    /// <summary>Returns the authenticated requester.</summary>
    /// <exception cref="InvalidOperationException">The current request is not authenticated.</exception>
    Requester Get();
}
