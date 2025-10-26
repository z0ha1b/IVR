using Application.DTOs;
using Application.Interfaces;
using Application.MenuLogic;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service for managing IVR menu navigation and call state
/// Implements business logic for menu transitions and call logging
/// </summary>
public class IVRMenuService : IIVRMenuService
{
    private readonly ICallSessionRepository _sessionRepository;
    private readonly ICallLogRepository _callLogRepository;
    private readonly ISWMLGenerator _swmlGenerator;
    private readonly ILogger<IVRMenuService> _logger;

    public IVRMenuService(
        ICallSessionRepository sessionRepository,
        ICallLogRepository callLogRepository,
        ISWMLGenerator swmlGenerator,
        ILogger<IVRMenuService> logger)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _callLogRepository = callLogRepository ?? throw new ArgumentNullException(nameof(callLogRepository));
        _swmlGenerator = swmlGenerator ?? throw new ArgumentNullException(nameof(swmlGenerator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<MenuResponse> HandleIncomingCallAsync(
        string callSid,
        string callerNumber,
        string baseUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Incoming call from {CallerNumber} with CallSid {CallSid}", callerNumber, callSid);

            // Create phone number value object
            var phoneNumber = new PhoneNumber(callerNumber);

            // Create new call session with main menu
            var mainMenu = MenuDefinitions.MainMenu;
            var session = new CallSession(callSid, phoneNumber, mainMenu);

            // Save session to cache
            await _sessionRepository.SaveAsync(session, cancellationToken);

            // Log the initial call
            var callLog = new CallLog(
                callSid: callSid,
                callerNumber: phoneNumber,
                menuPath: mainMenu.MenuId,
                digitPressed: "INCOMING",
                currentMenuId: mainMenu.MenuId
            );
            await _callLogRepository.CreateAsync(callLog, cancellationToken);

            // Generate SWML response
            var actionUrl = $"{baseUrl}/api/ivr/menu";
            var swml = _swmlGenerator.GenerateMenuResponse(mainMenu, actionUrl);

            return new MenuResponse
            {
                SWMLContent = swml,
                MenuId = mainMenu.MenuId,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling incoming call for CallSid {CallSid}", callSid);
            return new MenuResponse
            {
                SWMLContent = _swmlGenerator.GenerateErrorResponse("An error occurred. Please try again later."),
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <inheritdoc/>
    public async Task<MenuResponse> HandleMenuSelectionAsync(
        MenuNavigationRequest request,
        string baseUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Menu selection for CallSid {CallSid}: Digit {Digit}",
                request.CallSid,
                request.Digits);

            // Retrieve call session
            var session = await _sessionRepository.GetByCallSidAsync(request.CallSid, cancellationToken);
            if (session == null)
            {
                _logger.LogWarning("Session not found for CallSid {CallSid}", request.CallSid);
                return new MenuResponse
                {
                    SWMLContent = _swmlGenerator.GenerateErrorResponse("Session expired. Please call again."),
                    IsSuccess = false,
                    ErrorMessage = "Session not found"
                };
            }

            // Validate digit input
            if (!IsValidDigit(request.Digits))
            {
                _logger.LogWarning("Invalid digit pressed: {Digit}", request.Digits);
                var currentMenu = session.GetCurrentMenu();
                var actionUrl = $"{baseUrl}/api/ivr/menu";
                var errorSwml = _swmlGenerator.GenerateMenuResponse(currentMenu, actionUrl);

                return new MenuResponse
                {
                    SWMLContent = errorSwml,
                    MenuId = currentMenu.MenuId,
                    IsSuccess = false,
                    ErrorMessage = "Invalid digit"
                };
            }

            MenuState nextMenu;
            var currentMenuState = session.GetCurrentMenu();

            // Handle Option 4 - Back navigation
            if (request.Digits == "4")
            {
                nextMenu = session.NavigateBack() ?? currentMenuState;
                _logger.LogInformation("Navigating back to menu {MenuId}", nextMenu.MenuId);
            }
            else
            {
                // Get next menu based on current menu and digit
                var nextMenuFromDefinition = MenuDefinitions.GetNextMenu(currentMenuState.MenuId, request.Digits);

                if (nextMenuFromDefinition != null)
                {
                    // Navigate forward to new menu
                    session.NavigateToMenu(nextMenuFromDefinition);
                    nextMenu = nextMenuFromDefinition;
                    _logger.LogInformation("Navigating forward to menu {MenuId}", nextMenu.MenuId);
                }
                else
                {
                    // No valid transition - either terminal menu or invalid selection
                    // For terminal menus (grandchild), repeat the current menu
                    if (MenuDefinitions.IsTerminalMenu(currentMenuState.MenuId))
                    {
                        nextMenu = currentMenuState;
                        _logger.LogInformation("Terminal menu - repeating current menu {MenuId}", nextMenu.MenuId);
                    }
                    else
                    {
                        // Invalid selection
                        _logger.LogWarning("Invalid menu transition from {CurrentMenu} with digit {Digit}",
                            currentMenuState.MenuId, request.Digits);
                        nextMenu = currentMenuState;
                    }
                }
            }

            // Save updated session
            await _sessionRepository.SaveAsync(session, cancellationToken);

            // Log the menu selection
            var callLog = new CallLog(
                callSid: request.CallSid,
                callerNumber: session.CallerNumber,
                menuPath: session.GetMenuPath(),
                digitPressed: request.Digits,
                currentMenuId: nextMenu.MenuId
            );
            await _callLogRepository.CreateAsync(callLog, cancellationToken);

            // Generate SWML response
            var nextActionUrl = $"{baseUrl}/api/ivr/menu";
            var swml = _swmlGenerator.GenerateMenuResponse(nextMenu, nextActionUrl);

            return new MenuResponse
            {
                SWMLContent = swml,
                MenuId = nextMenu.MenuId,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling menu selection for CallSid {CallSid}", request.CallSid);
            return new MenuResponse
            {
                SWMLContent = _swmlGenerator.GenerateErrorResponse("An error occurred. Please try again."),
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <inheritdoc/>
    public async Task EndCallSessionAsync(string callSid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Ending call session for CallSid {CallSid}", callSid);
            await _sessionRepository.RemoveAsync(callSid, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending call session for CallSid {CallSid}", callSid);
            // Don't throw - this is cleanup
        }
    }

    private static bool IsValidDigit(string digit)
    {
        return digit is "1" or "2" or "3" or "4";
    }
}
