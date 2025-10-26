using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace IntegrationTests.Controllers;

public class IVRControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IVRControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HandleIncomingCall_ValidRequest_ReturnsXml()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "CallSid", "CA_TEST_123456" },
            { "From", "+15551234567" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/api/ivr/main", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);

        var xmlContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("<Response>", xmlContent);
        Assert.Contains("<Gather>", xmlContent);
    }

    [Fact]
    public async Task HandleIncomingCall_MissingCallSid_ReturnsBadRequest()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "From", "+15551234567" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/api/ivr/main", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleMenuSelection_ValidRequest_ReturnsXml()
    {
        // Arrange - First make an incoming call to create a session
        var incomingCallData = new Dictionary<string, string>
        {
            { "CallSid", "CA_TEST_MENU_123" },
            { "From", "+15559876543" }
        };
        await _client.PostAsync("/api/ivr/main", new FormUrlEncodedContent(incomingCallData));

        // Now send a menu selection
        var menuData = new Dictionary<string, string>
        {
            { "CallSid", "CA_TEST_MENU_123" },
            { "From", "+15559876543" },
            { "Digits", "1" }
        };
        var content = new FormUrlEncodedContent(menuData);

        // Act
        var response = await _client.PostAsync("/api/ivr/menu", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);

        var xmlContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("<Response>", xmlContent);
    }

    [Fact]
    public async Task HandleCallStatus_ValidRequest_ReturnsOk()
    {
        // Arrange
        var statusData = new Dictionary<string, string>
        {
            { "CallSid", "CA_TEST_STATUS_123" },
            { "CallStatus", "completed" }
        };
        var content = new FormUrlEncodedContent(statusData);

        // Act
        var response = await _client.PostAsync("/api/ivr/status", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
