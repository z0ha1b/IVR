using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Entity representing an active call session
/// Maintains state and navigation history for a caller
/// </summary>
public class CallSession
{
    public string CallSid { get; private set; }
    public PhoneNumber CallerNumber { get; private set; }
    public Stack<MenuState> MenuStack { get; private set; }
    public string CurrentMenuId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime LastActivityTime { get; private set; }

    // Required for serialization
    private CallSession()
    {
        MenuStack = new Stack<MenuState>();
    }

    public CallSession(string callSid, PhoneNumber callerNumber, MenuState initialMenu)
    {
        CallSid = callSid ?? throw new ArgumentNullException(nameof(callSid));
        CallerNumber = callerNumber ?? throw new ArgumentNullException(nameof(callerNumber));
        MenuStack = new Stack<MenuState>();
        MenuStack.Push(initialMenu);
        CurrentMenuId = initialMenu.MenuId;
        StartTime = DateTime.UtcNow;
        LastActivityTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Navigates to a new menu by pushing it onto the stack
    /// </summary>
    public void NavigateToMenu(MenuState menuState)
    {
        if (menuState == null)
            throw new ArgumentNullException(nameof(menuState));

        MenuStack.Push(menuState);
        CurrentMenuId = menuState.MenuId;
        LastActivityTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Returns to the previous menu by popping from the stack
    /// </summary>
    /// <returns>The previous menu state, or null if at main menu</returns>
    public MenuState? NavigateBack()
    {
        // Don't pop if we're at the main menu (only one item in stack)
        if (MenuStack.Count <= 1)
            return MenuStack.Peek();

        // Pop current menu
        MenuStack.Pop();

        // Return to previous menu
        var previousMenu = MenuStack.Peek();
        CurrentMenuId = previousMenu.MenuId;
        LastActivityTime = DateTime.UtcNow;

        return previousMenu;
    }

    /// <summary>
    /// Gets the current menu without modifying the stack
    /// </summary>
    public MenuState GetCurrentMenu()
    {
        if (MenuStack.Count == 0)
            throw new InvalidOperationException("Menu stack is empty");

        return MenuStack.Peek();
    }

    /// <summary>
    /// Builds a path string representing the menu navigation history
    /// </summary>
    public string GetMenuPath()
    {
        return string.Join(" > ", MenuStack.Reverse().Select(m => m.MenuId));
    }
}
