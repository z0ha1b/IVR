using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace IntegrationTests.Controllers;

public class CallLogsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public CallLogsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetCallLogs_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/calllogs");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetCallLogs_ReturnsValidCallLogListResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/calllogs?skip=0&take=10");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CallLogListResponse>(content, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CallLogs);
        Assert.True(result.TotalCount >= 0);
        Assert.Equal(0, result.Skip);
        Assert.Equal(10, result.Take);
    }

    [Fact]
    public async Task GetCallLogs_WithPagination_ReturnsCorrectPageSize()
    {
        // Act
        var response = await _client.GetAsync("/api/calllogs?skip=0&take=5");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CallLogListResponse>(content, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CallLogs.Count() <= 5);
    }

    [Fact]
    public async Task GetCallLogsByCallSid_InvalidCallSid_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/calllogs/session/CA_INVALID_123");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCallLogsByPhoneNumber_ValidRequest_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/calllogs/phone/+15551234567");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
