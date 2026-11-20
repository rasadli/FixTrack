namespace FixTrack.Domain.Cases;

/// <summary>
/// The kind of service request a case represents.
/// </summary>
public enum CaseType
{
    /// <summary>The device needs to be repaired.</summary>
    Repair,

    /// <summary>The device is being serviced under warranty.</summary>
    Warranty,

    /// <summary>The device is being returned.</summary>
    Return
}
