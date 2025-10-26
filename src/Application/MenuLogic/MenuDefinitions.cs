using Domain.Enums;
using Domain.ValueObjects;

namespace Application.MenuLogic;

/// <summary>
/// Defines all IVR menus in the system
/// Implements a hierarchical menu structure with parent-child relationships
/// </summary>
public static class MenuDefinitions
{
    // Main Menu
    public static readonly MenuState MainMenu = new(
        menuId: "MAIN",
        level: MenuLevel.Main,
        message: "Welcome to our IVR system. Press 1 for Sales, Press 2 for Support, Press 3 for Billing, Press 4 for Customer Service."
    );

    // Child Menus (Level 1)
    public static readonly MenuState SalesMenu = new(
        menuId: "SALES",
        level: MenuLevel.Child,
        message: "You have reached Sales. Press 1 for Product Information, Press 2 for New Orders, Press 3 for Sales Inquiries, Press 4 to return to the main menu.",
        parentMenuId: "MAIN"
    );

    public static readonly MenuState SupportMenu = new(
        menuId: "SUPPORT",
        level: MenuLevel.Child,
        message: "You have reached Support. Press 1 for Technical Support, Press 2 for Account Help, Press 3 for General Support, Press 4 to return to the main menu.",
        parentMenuId: "MAIN"
    );

    public static readonly MenuState BillingMenu = new(
        menuId: "BILLING",
        level: MenuLevel.Child,
        message: "You have reached Billing. Press 1 for Payment Options, Press 2 for Invoice Inquiries, Press 3 for Billing Disputes, Press 4 to return to the main menu.",
        parentMenuId: "MAIN"
    );

    public static readonly MenuState CustomerServiceMenu = new(
        menuId: "CUSTOMER_SERVICE",
        level: MenuLevel.Child,
        message: "You have reached Customer Service. Press 1 for General Inquiries, Press 2 for Feedback, Press 3 for Complaints, Press 4 to return to the main menu.",
        parentMenuId: "MAIN"
    );

    // Grandchild Menus (Level 2) - Sales Branch
    public static readonly MenuState ProductInfoMenu = new(
        menuId: "SALES_PRODUCT",
        level: MenuLevel.Grandchild,
        message: "Product Information. Press 1 to hear about our latest products, Press 2 for pricing details, Press 3 for product catalog, Press 4 to return to Sales menu.",
        parentMenuId: "SALES"
    );

    public static readonly MenuState NewOrdersMenu = new(
        menuId: "SALES_ORDERS",
        level: MenuLevel.Grandchild,
        message: "New Orders. Press 1 to place a new order, Press 2 to check order status, Press 3 for order modifications, Press 4 to return to Sales menu.",
        parentMenuId: "SALES"
    );

    public static readonly MenuState SalesInquiriesMenu = new(
        menuId: "SALES_INQUIRIES",
        level: MenuLevel.Grandchild,
        message: "Sales Inquiries. Press 1 for partnership opportunities, Press 2 for bulk orders, Press 3 for quotes, Press 4 to return to Sales menu.",
        parentMenuId: "SALES"
    );

    // Grandchild Menus (Level 2) - Support Branch
    public static readonly MenuState TechnicalSupportMenu = new(
        menuId: "SUPPORT_TECHNICAL",
        level: MenuLevel.Grandchild,
        message: "Technical Support. Press 1 for software issues, Press 2 for hardware issues, Press 3 for connectivity problems, Press 4 to return to Support menu.",
        parentMenuId: "SUPPORT"
    );

    public static readonly MenuState AccountHelpMenu = new(
        menuId: "SUPPORT_ACCOUNT",
        level: MenuLevel.Grandchild,
        message: "Account Help. Press 1 for password reset, Press 2 for account settings, Press 3 for profile updates, Press 4 to return to Support menu.",
        parentMenuId: "SUPPORT"
    );

    public static readonly MenuState GeneralSupportMenu = new(
        menuId: "SUPPORT_GENERAL",
        level: MenuLevel.Grandchild,
        message: "General Support. Press 1 for FAQs, Press 2 for user guides, Press 3 for tutorials, Press 4 to return to Support menu.",
        parentMenuId: "SUPPORT"
    );

    // Grandchild Menus (Level 2) - Billing Branch
    public static readonly MenuState PaymentOptionsMenu = new(
        menuId: "BILLING_PAYMENT",
        level: MenuLevel.Grandchild,
        message: "Payment Options. Press 1 for credit card payment, Press 2 for bank transfer, Press 3 for payment plans, Press 4 to return to Billing menu.",
        parentMenuId: "BILLING"
    );

    public static readonly MenuState InvoiceInquiriesMenu = new(
        menuId: "BILLING_INVOICE",
        level: MenuLevel.Grandchild,
        message: "Invoice Inquiries. Press 1 to request an invoice copy, Press 2 for payment history, Press 3 for invoice disputes, Press 4 to return to Billing menu.",
        parentMenuId: "BILLING"
    );

    public static readonly MenuState BillingDisputesMenu = new(
        menuId: "BILLING_DISPUTES",
        level: MenuLevel.Grandchild,
        message: "Billing Disputes. Press 1 to file a dispute, Press 2 to check dispute status, Press 3 for refund requests, Press 4 to return to Billing menu.",
        parentMenuId: "BILLING"
    );

    // Grandchild Menus (Level 2) - Customer Service Branch
    public static readonly MenuState GeneralInquiriesMenu = new(
        menuId: "CS_INQUIRIES",
        level: MenuLevel.Grandchild,
        message: "General Inquiries. Press 1 for business hours, Press 2 for locations, Press 3 for contact information, Press 4 to return to Customer Service menu.",
        parentMenuId: "CUSTOMER_SERVICE"
    );

    public static readonly MenuState FeedbackMenu = new(
        menuId: "CS_FEEDBACK",
        level: MenuLevel.Grandchild,
        message: "Feedback. Press 1 to leave a compliment, Press 2 for suggestions, Press 3 for service improvement ideas, Press 4 to return to Customer Service menu.",
        parentMenuId: "CUSTOMER_SERVICE"
    );

    public static readonly MenuState ComplaintsMenu = new(
        menuId: "CS_COMPLAINTS",
        level: MenuLevel.Grandchild,
        message: "Complaints. Press 1 to file a complaint, Press 2 to check complaint status, Press 3 to escalate, Press 4 to return to Customer Service menu.",
        parentMenuId: "CUSTOMER_SERVICE"
    );

    /// <summary>
    /// Dictionary mapping menu IDs to their MenuState objects for quick lookup
    /// </summary>
    private static readonly Dictionary<string, MenuState> MenuMap = new()
    {
        { MainMenu.MenuId, MainMenu },
        { SalesMenu.MenuId, SalesMenu },
        { SupportMenu.MenuId, SupportMenu },
        { BillingMenu.MenuId, BillingMenu },
        { CustomerServiceMenu.MenuId, CustomerServiceMenu },
        { ProductInfoMenu.MenuId, ProductInfoMenu },
        { NewOrdersMenu.MenuId, NewOrdersMenu },
        { SalesInquiriesMenu.MenuId, SalesInquiriesMenu },
        { TechnicalSupportMenu.MenuId, TechnicalSupportMenu },
        { AccountHelpMenu.MenuId, AccountHelpMenu },
        { GeneralSupportMenu.MenuId, GeneralSupportMenu },
        { PaymentOptionsMenu.MenuId, PaymentOptionsMenu },
        { InvoiceInquiriesMenu.MenuId, InvoiceInquiriesMenu },
        { BillingDisputesMenu.MenuId, BillingDisputesMenu },
        { GeneralInquiriesMenu.MenuId, GeneralInquiriesMenu },
        { FeedbackMenu.MenuId, FeedbackMenu },
        { ComplaintsMenu.MenuId, ComplaintsMenu }
    };

    /// <summary>
    /// Dictionary defining child menu transitions based on parent menu and digit pressed
    /// Key format: "PARENT_MENU_ID:DIGIT" -> Child MenuState
    /// </summary>
    private static readonly Dictionary<string, MenuState> MenuTransitions = new()
    {
        // Main Menu transitions
        { "MAIN:1", SalesMenu },
        { "MAIN:2", SupportMenu },
        { "MAIN:3", BillingMenu },
        { "MAIN:4", CustomerServiceMenu },

        // Sales Menu transitions
        { "SALES:1", ProductInfoMenu },
        { "SALES:2", NewOrdersMenu },
        { "SALES:3", SalesInquiriesMenu },
        // "SALES:4" is handled by back navigation

        // Support Menu transitions
        { "SUPPORT:1", TechnicalSupportMenu },
        { "SUPPORT:2", AccountHelpMenu },
        { "SUPPORT:3", GeneralSupportMenu },
        // "SUPPORT:4" is handled by back navigation

        // Billing Menu transitions
        { "BILLING:1", PaymentOptionsMenu },
        { "BILLING:2", InvoiceInquiriesMenu },
        { "BILLING:3", BillingDisputesMenu },
        // "BILLING:4" is handled by back navigation

        // Customer Service Menu transitions
        { "CUSTOMER_SERVICE:1", GeneralInquiriesMenu },
        { "CUSTOMER_SERVICE:2", FeedbackMenu },
        { "CUSTOMER_SERVICE:3", ComplaintsMenu },
        // "CUSTOMER_SERVICE:4" is handled by back navigation

        // Grandchild menus - options 1, 2, and 3 could lead to actions or repeat messages
        // For this implementation, we'll treat them as terminal actions (repeat same menu)
        // Option 4 always goes back
    };

    /// <summary>
    /// Gets a menu by its ID
    /// </summary>
    public static MenuState? GetMenuById(string menuId)
    {
        return MenuMap.TryGetValue(menuId, out var menu) ? menu : null;
    }

    /// <summary>
    /// Gets the next menu based on current menu and digit pressed
    /// Returns null if the transition doesn't exist or if digit is 4 (back) and no explicit transition exists
    /// </summary>
    public static MenuState? GetNextMenu(string currentMenuId, string digit)
    {
        // Check if there's an explicit transition defined for this menu+digit combination
        var key = $"{currentMenuId}:{digit}";
        if (MenuTransitions.TryGetValue(key, out var nextMenu))
        {
            return nextMenu;
        }

        // If no explicit transition and digit is 4, treat as back navigation
        // This allows special cases like MAIN:4 to be defined explicitly
        // while child/grandchild menus use 4 for back by default
        if (digit == "4")
            return null;

        // No valid transition found
        return null;
    }

    /// <summary>
    /// Determines if a menu is a terminal menu (no further navigation)
    /// Grandchild menus are typically terminal
    /// </summary>
    public static bool IsTerminalMenu(string menuId)
    {
        var menu = GetMenuById(menuId);
        return menu?.Level == MenuLevel.Grandchild;
    }
}
