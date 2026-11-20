namespace FixTrack.Domain.Cases;

/// <summary>
/// The processing status of a case. New cases always start as <see cref="Open"/>.
/// </summary>
public enum CaseStatus
{
    /// <summary>The case has been created and is waiting to be worked on.</summary>
    Open,

    /// <summary>The case is currently being worked on.</summary>
    InProgress,

    /// <summary>Work on the case is finished.</summary>
    Completed
}
