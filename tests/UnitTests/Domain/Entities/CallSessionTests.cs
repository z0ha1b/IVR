using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain.Entities;

public class CallSessionTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesSession()
    {
        // Arrange
        var callSid = "CA1234567890";
        var phoneNumber = new PhoneNumber("+15551234567");
        var mainMenu = new MenuState("MAIN", MenuLevel.Main, "Welcome");

        // Act
        var session = new CallSession(callSid, phoneNumber, mainMenu);

        // Assert
        Assert.Equal(callSid, session.CallSid);
        Assert.Equal(phoneNumber, session.CallerNumber);
        Assert.Equal("MAIN", session.CurrentMenuId);
        Assert.Single(session.MenuStack);
    }

    [Fact]
    public void NavigateToMenu_NewMenu_AddsToStack()
    {
        // Arrange
        var session = CreateTestSession();
        var salesMenu = new MenuState("SALES", MenuLevel.Child, "Sales menu", "MAIN");

        // Act
        session.NavigateToMenu(salesMenu);

        // Assert
        Assert.Equal(2, session.MenuStack.Count);
        Assert.Equal("SALES", session.CurrentMenuId);
    }

    [Fact]
    public void NavigateBack_MultipleMenus_ReturnsToParent()
    {
        // Arrange
        var session = CreateTestSession();
        var salesMenu = new MenuState("SALES", MenuLevel.Child, "Sales menu", "MAIN");
        var productMenu = new MenuState("SALES_PRODUCT", MenuLevel.Grandchild, "Product menu", "SALES");

        session.NavigateToMenu(salesMenu);
        session.NavigateToMenu(productMenu);

        // Act
        var previousMenu = session.NavigateBack();

        // Assert
        Assert.NotNull(previousMenu);
        Assert.Equal("SALES", previousMenu.MenuId);
        Assert.Equal("SALES", session.CurrentMenuId);
        Assert.Equal(2, session.MenuStack.Count);
    }

    [Fact]
    public void NavigateBack_AtMainMenu_StaysAtMainMenu()
    {
        // Arrange
        var session = CreateTestSession();

        // Act
        var previousMenu = session.NavigateBack();

        // Assert
        Assert.NotNull(previousMenu);
        Assert.Equal("MAIN", previousMenu.MenuId);
        Assert.Single(session.MenuStack);
    }

    [Fact]
    public void GetMenuPath_MultipleMenus_ReturnsCorrectPath()
    {
        // Arrange
        var session = CreateTestSession();
        var salesMenu = new MenuState("SALES", MenuLevel.Child, "Sales menu", "MAIN");
        var productMenu = new MenuState("SALES_PRODUCT", MenuLevel.Grandchild, "Product menu", "SALES");

        session.NavigateToMenu(salesMenu);
        session.NavigateToMenu(productMenu);

        // Act
        var path = session.GetMenuPath();

        // Assert
        Assert.Equal("MAIN > SALES > SALES_PRODUCT", path);
    }

    [Fact]
    public void GetCurrentMenu_ReturnsTopOfStack()
    {
        // Arrange
        var session = CreateTestSession();
        var salesMenu = new MenuState("SALES", MenuLevel.Child, "Sales menu", "MAIN");
        session.NavigateToMenu(salesMenu);

        // Act
        var currentMenu = session.GetCurrentMenu();

        // Assert
        Assert.Equal("SALES", currentMenu.MenuId);
    }

    private static CallSession CreateTestSession()
    {
        var callSid = "CA1234567890";
        var phoneNumber = new PhoneNumber("+15551234567");
        var mainMenu = new MenuState("MAIN", MenuLevel.Main, "Welcome");
        return new CallSession(callSid, phoneNumber, mainMenu);
    }
}
