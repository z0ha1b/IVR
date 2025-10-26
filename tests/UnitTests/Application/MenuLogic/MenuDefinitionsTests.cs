using Application.MenuLogic;
using Domain.Enums;
using Xunit;

namespace UnitTests.Application.MenuLogic;

public class MenuDefinitionsTests
{
    [Fact]
    public void GetMenuById_MainMenu_ReturnsCorrectMenu()
    {
        // Act
        var menu = MenuDefinitions.GetMenuById("MAIN");

        // Assert
        Assert.NotNull(menu);
        Assert.Equal("MAIN", menu.MenuId);
        Assert.Equal(MenuLevel.Main, menu.Level);
    }

    [Theory]
    [InlineData("MAIN", "1", "SALES")]
    [InlineData("MAIN", "2", "SUPPORT")]
    [InlineData("MAIN", "3", "BILLING")]
    [InlineData("MAIN", "4", "CUSTOMER_SERVICE")]
    [InlineData("SALES", "1", "SALES_PRODUCT")]
    [InlineData("SALES", "2", "SALES_ORDERS")]
    [InlineData("SALES", "3", "SALES_INQUIRIES")]
    [InlineData("SUPPORT", "1", "SUPPORT_TECHNICAL")]
    [InlineData("SUPPORT", "2", "SUPPORT_ACCOUNT")]
    [InlineData("SUPPORT", "3", "SUPPORT_GENERAL")]
    public void GetNextMenu_ValidTransition_ReturnsCorrectMenu(
        string currentMenuId,
        string digit,
        string expectedMenuId)
    {
        // Act
        var nextMenu = MenuDefinitions.GetNextMenu(currentMenuId, digit);

        // Assert
        Assert.NotNull(nextMenu);
        Assert.Equal(expectedMenuId, nextMenu.MenuId);
    }

    [Fact]
    public void GetNextMenu_BackNavigation_ReturnsNull()
    {
        // Act - Option 4 is now back navigation
        var nextMenu = MenuDefinitions.GetNextMenu("SALES", "4");

        // Assert
        Assert.Null(nextMenu);
    }

    [Fact]
    public void GetNextMenu_InvalidTransition_ReturnsNull()
    {
        // Act
        var nextMenu = MenuDefinitions.GetNextMenu("SALES_PRODUCT", "1");

        // Assert
        Assert.Null(nextMenu);
    }

    [Theory]
    [InlineData("MAIN", false)]
    [InlineData("SALES", false)]
    [InlineData("SALES_PRODUCT", true)]
    [InlineData("SUPPORT_TECHNICAL", true)]
    public void IsTerminalMenu_VariousMenus_ReturnsCorrectValue(string menuId, bool expectedIsTerminal)
    {
        // Act
        var isTerminal = MenuDefinitions.IsTerminalMenu(menuId);

        // Assert
        Assert.Equal(expectedIsTerminal, isTerminal);
    }

    [Fact]
    public void MainMenu_HasCorrectProperties()
    {
        // Act
        var mainMenu = MenuDefinitions.MainMenu;

        // Assert
        Assert.Equal("MAIN", mainMenu.MenuId);
        Assert.Equal(MenuLevel.Main, mainMenu.Level);
        Assert.Null(mainMenu.ParentMenuId);
        Assert.Contains("Welcome", mainMenu.Message);
    }

    [Theory]
    [InlineData("SALES")]
    [InlineData("SUPPORT")]
    [InlineData("BILLING")]
    [InlineData("CUSTOMER_SERVICE")]
    public void ChildMenus_HaveCorrectParent(string menuId)
    {
        // Act
        var menu = MenuDefinitions.GetMenuById(menuId);

        // Assert
        Assert.NotNull(menu);
        Assert.Equal(MenuLevel.Child, menu.Level);
        Assert.Equal("MAIN", menu.ParentMenuId);
    }
}
