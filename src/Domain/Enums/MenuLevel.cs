namespace Domain.Enums;

/// <summary>
/// Represents the hierarchical level of an IVR menu
/// </summary>
public enum MenuLevel
{
    /// <summary>
    /// The initial main menu presented to the caller
    /// </summary>
    Main = 0,

    /// <summary>
    /// First-level submenu (Child A, B, or C)
    /// </summary>
    Child = 1,

    /// <summary>
    /// Second-level submenu (Grandchild)
    /// </summary>
    Grandchild = 2
}
