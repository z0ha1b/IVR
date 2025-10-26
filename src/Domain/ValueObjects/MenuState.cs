using Domain.Enums;

namespace Domain.ValueObjects;

/// <summary>
/// Value object representing the current state of a menu in the IVR system
/// Immutable and contains all information needed to render a menu
/// </summary>
public sealed class MenuState : IEquatable<MenuState>
{
    public string MenuId { get; }
    public MenuLevel Level { get; }
    public string Message { get; }
    public string? ParentMenuId { get; }

    public MenuState(string menuId, MenuLevel level, string message, string? parentMenuId = null)
    {
        if (string.IsNullOrWhiteSpace(menuId))
            throw new ArgumentException("Menu ID cannot be empty", nameof(menuId));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Menu message cannot be empty", nameof(message));

        MenuId = menuId;
        Level = level;
        Message = message;
        ParentMenuId = parentMenuId;
    }

    public bool Equals(MenuState? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return MenuId == other.MenuId && Level == other.Level;
    }

    public override bool Equals(object? obj)
    {
        return obj is MenuState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MenuId, Level);
    }
}
