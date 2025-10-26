using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests.Application.Services;

public class IVRMenuServiceTests
{
    private readonly Mock<ICallSessionRepository> _mockSessionRepository;
    private readonly Mock<ICallLogRepository> _mockLogRepository;
    private readonly Mock<ISWMLGenerator> _mockSWMLGenerator;
    private readonly Mock<ILogger<IVRMenuService>> _mockLogger;
    private readonly IVRMenuService _service;

    public IVRMenuServiceTests()
    {
        _mockSessionRepository = new Mock<ICallSessionRepository>();
        _mockLogRepository = new Mock<ICallLogRepository>();
        _mockSWMLGenerator = new Mock<ISWMLGenerator>();
        _mockLogger = new Mock<ILogger<IVRMenuService>>();

        _service = new IVRMenuService(
            _mockSessionRepository.Object,
            _mockLogRepository.Object,
            _mockSWMLGenerator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleIncomingCallAsync_ValidCall_CreatesSessionAndReturnsMainMenu()
    {
        // Arrange
        var callSid = "CA1234567890";
        var callerNumber = "+15551234567";
        var baseUrl = "http://localhost:5000";
        var expectedSwml = "<Response><Gather></Gather></Response>";

        _mockSWMLGenerator
            .Setup(x => x.GenerateMenuResponse(It.IsAny<MenuState>(), It.IsAny<string>()))
            .Returns(expectedSwml);

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<CallLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CallLog log, CancellationToken ct) => log);

        // Act
        var result = await _service.HandleIncomingCallAsync(callSid, callerNumber, baseUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedSwml, result.SWMLContent);
        Assert.Equal("MAIN", result.MenuId);

        _mockSessionRepository.Verify(
            x => x.SaveAsync(It.IsAny<CallSession>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockLogRepository.Verify(
            x => x.CreateAsync(It.IsAny<CallLog>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleMenuSelectionAsync_ValidDigit1_NavigatesToChildMenu()
    {
        // Arrange
        var callSid = "CA1234567890";
        var phoneNumber = new PhoneNumber("+15551234567");
        var mainMenu = new MenuState("MAIN", MenuLevel.Main, "Welcome");
        var session = new CallSession(callSid, phoneNumber, mainMenu);

        var request = new MenuNavigationRequest
        {
            CallSid = callSid,
            From = "+15551234567",
            Digits = "1"
        };

        _mockSessionRepository
            .Setup(x => x.GetByCallSidAsync(callSid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _mockSWMLGenerator
            .Setup(x => x.GenerateMenuResponse(It.IsAny<MenuState>(), It.IsAny<string>()))
            .Returns("<Response><Gather></Gather></Response>");

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<CallLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CallLog log, CancellationToken ct) => log);

        // Act
        var result = await _service.HandleMenuSelectionAsync(request, "http://localhost:5000");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("SALES", result.MenuId);

        _mockSessionRepository.Verify(
            x => x.SaveAsync(It.IsAny<CallSession>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleMenuSelectionAsync_Digit4_NavigatesBack()
    {
        // Arrange
        var callSid = "CA1234567890";
        var phoneNumber = new PhoneNumber("+15551234567");
        var mainMenu = new MenuState("MAIN", MenuLevel.Main, "Welcome");
        var session = new CallSession(callSid, phoneNumber, mainMenu);

        // Navigate to a child menu first
        var salesMenu = new MenuState("SALES", MenuLevel.Child, "Sales", "MAIN");
        session.NavigateToMenu(salesMenu);

        var request = new MenuNavigationRequest
        {
            CallSid = callSid,
            From = "+15551234567",
            Digits = "4"
        };

        _mockSessionRepository
            .Setup(x => x.GetByCallSidAsync(callSid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _mockSWMLGenerator
            .Setup(x => x.GenerateMenuResponse(It.IsAny<MenuState>(), It.IsAny<string>()))
            .Returns("<Response><Gather></Gather></Response>");

        _mockLogRepository
            .Setup(x => x.CreateAsync(It.IsAny<CallLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CallLog log, CancellationToken ct) => log);

        // Act
        var result = await _service.HandleMenuSelectionAsync(request, "http://localhost:5000");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("MAIN", result.MenuId); // Should be back at main menu
    }

    [Fact]
    public async Task HandleMenuSelectionAsync_SessionNotFound_ReturnsError()
    {
        // Arrange
        var request = new MenuNavigationRequest
        {
            CallSid = "CA_NOTFOUND",
            From = "+15551234567",
            Digits = "1"
        };

        _mockSessionRepository
            .Setup(x => x.GetByCallSidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CallSession?)null);

        _mockSWMLGenerator
            .Setup(x => x.GenerateErrorResponse(It.IsAny<string>()))
            .Returns("<Response><Say>Error</Say></Response>");

        // Act
        var result = await _service.HandleMenuSelectionAsync(request, "http://localhost:5000");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Session", result.ErrorMessage!);
    }

    [Fact]
    public async Task EndCallSessionAsync_ValidCallSid_RemovesSession()
    {
        // Arrange
        var callSid = "CA1234567890";

        // Act
        await _service.EndCallSessionAsync(callSid);

        // Assert
        _mockSessionRepository.Verify(
            x => x.RemoveAsync(callSid, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
