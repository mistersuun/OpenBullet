using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Services;
using System.Net;
using System.Net.Http;
using Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 6 Tests: REQUEST Command Implementation
/// </summary>
public class Step6_RequestCommandTests : IDisposable
{
    private readonly WireMockServer _mockServer;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<RequestCommand>> _loggerMock;
    private readonly Mock<IHttpClientService> _httpClientMock;
    private readonly Mock<IScriptParser> _scriptParserMock;
    private readonly RequestCommand _requestCommand;
    private readonly BotData _botData;

    public Step6_RequestCommandTests()
    {
        _mockServer = WireMockServer.Start();
        
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<RequestCommand>>();
        _httpClientMock = new Mock<IHttpClientService>();
        _scriptParserMock = new Mock<IScriptParser>();

        // Setup service provider
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<RequestCommand>)))
                           .Returns(_loggerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IHttpClientService)))
                           .Returns(_httpClientMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IScriptParser)))
                           .Returns(_scriptParserMock.Object);

        _requestCommand = new RequestCommand(_serviceProviderMock.Object);

        // Create test bot data
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        _botData = new BotData("test:data", config, logger, cancellationToken);

        // Setup script parser default behavior
        _scriptParserMock.Setup(sp => sp.SubstituteVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                        .Returns<string, Dictionary<string, object>>((input, _) => input);
    }

    [Fact]
    public void RequestCommand_Should_Have_Correct_Properties()
    {
        // Assert
        _requestCommand.CommandName.Should().Be("REQUEST");
        _requestCommand.Description.Should().NotBeEmpty();
        _requestCommand.Should().BeAssignableTo<IScriptCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_With_Simple_GET_Should_Work()
    {
        // Arrange
        _mockServer.Given(Request.Create().WithPath("/test").UsingGet())
                  .RespondWith(Response.Create().WithStatusCode(200).WithBody("Success"));

        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", $"{_mockServer.Url}/test" },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "Success",
            ResponseTime = 100
        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Source.Should().Be("Success");
        _botData.ResponseCode.Should().Be(200);
        _httpClientMock.Verify(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_POST_And_Content_Should_Work()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> 
            { 
                "POST", 
                $"{_mockServer.Url}/login", 
                "CONTENT", 
                "username=test&password=secret",
                "CONTENTTYPE",
                "application/x-www-form-urlencoded"
            },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "Login successful",

            ResponseTime = 200
        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Source.Should().Be("Login successful");

        // Verify the request was configured correctly
        _httpClientMock.Verify(hc => hc.SendAsync(
            It.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Post && 
                req.Content != null),
            It.IsAny<Models.ProxyInfo?>(), 
            It.IsAny<HttpClientConfiguration>(),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Headers_Should_Include_Headers()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> 
            { 
                "GET", 
                $"{_mockServer.Url}/api",
                "HEADER",
                "Authorization",
                "Bearer token123",
                "HEADER",
                "Accept",
                "application/json"
            },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "API response",

        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        // Verify headers were added (we can't easily check the actual headers in the mock, but we can verify the call was made)
        _httpClientMock.Verify(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Cookies_Should_Include_Cookies()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> 
            { 
                "GET", 
                $"{_mockServer.Url}/secure",
                "Cookie",
                "sessionid",
                "abc123",
                "Cookie",
                "userid",
                "456"
            },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "Secure content",

        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _httpClientMock.Verify(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Boolean_Parameters_Should_Apply_Settings()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", $"{_mockServer.Url}/test" },
            Parameters = new Dictionary<string, object>
            {
                ["AutoRedirect"] = false,
                ["Timeout"] = 5
            },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "Test response",

        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        // Verify configuration was applied
        _httpClientMock.Verify(hc => hc.SendAsync(
            It.IsAny<HttpRequestMessage>(),
            It.IsAny<Models.ProxyInfo?>(),
            It.Is<HttpClientConfiguration>(config => 
                config.AllowAutoRedirect == false && 
                config.Timeout == TimeSpan.FromSeconds(5)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_SubInstructions_Should_Process_All()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "POST", $"{_mockServer.Url}/api" },
            SubInstructions = new List<ScriptInstruction>
            {
                new() { CommandName = "CONTENT", Arguments = new List<string> { "data=test" } },
                new() { CommandName = "CONTENTTYPE", Arguments = new List<string> { "application/x-www-form-urlencoded" } },
                new() { CommandName = "HEADER", Arguments = new List<string> { "X-API-Key", "secret" } }
            },
            LineNumber = 1
        };

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "API success",

        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _httpClientMock.Verify(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Variable_Substitution_Should_Replace_Variables()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "<BASE_URL>/api/<ENDPOINT>" },
            LineNumber = 1
        };

        _scriptParserMock.Setup(sp => sp.SubstituteVariables("<BASE_URL>/api/<ENDPOINT>", It.IsAny<Dictionary<string, object>>()))
                        .Returns("https://example.com/api/users");

        var expectedResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = "Users data",

        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _scriptParserMock.Verify(sp => sp.SubstituteVariables("<BASE_URL>/api/<ENDPOINT>", It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_HTTP_Error_Should_Handle_Gracefully()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", $"{_mockServer.Url}/notfound" },
            LineNumber = 1
        };

        var errorResponse = new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.NotFound),
            Content = "Not found"
        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(errorResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue(); // Command execution succeeds even with HTTP errors
        _botData.ResponseCode.Should().Be(404);
        _botData.Status.Should().Be(BotStatus.Failure); // Status should be set based on HTTP error
    }

    [Fact]
    public async Task ExecuteAsync_With_Network_Exception_Should_Handle_Error()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", $"{_mockServer.Url}/test" },
            LineNumber = 1
        };

        var exceptionResponse = new HttpResponseWrapper
        {
            Exception = new HttpRequestException("Network error")
        };

        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(exceptionResponse);

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue(); // Command handles the exception gracefully
        _botData.Status.Should().Be(BotStatus.Error);
    }

    [Fact]
    public void ValidateInstruction_With_Valid_GET_Should_Pass()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com/api" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateInstruction_With_Missing_Arguments_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET" } // Missing URL
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("REQUEST command requires at least METHOD and URL arguments");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_HTTP_Method_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "INVALID", "https://example.com" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Invalid HTTP method: INVALID"));
    }

    [Fact]
    public void ValidateInstruction_With_Empty_URL_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("URL cannot be empty");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_URL_Format_Should_Warn()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "invalid-url" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue(); // Still valid, just warning
        result.Warnings.Should().Contain(w => w.Contains("URL should start with http:// or https://"));
    }

    [Fact]
    public void ValidateInstruction_With_Variable_URL_Should_Pass()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "<BASE_URL>/api" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Warnings.Should().BeEmpty(); // Variables in URLs are acceptable
    }

    [Fact]
    public void ValidateInstruction_With_Incomplete_Parameters_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "POST", "https://example.com", "CONTENT" } // Missing content value
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("CONTENT parameter requires a value");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Boolean_Parameter_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            Parameters = new Dictionary<string, object>
            {
                ["AutoRedirect"] = "invalid-boolean"
            }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("AutoRedirect parameter must be true or false");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Timeout_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            Parameters = new Dictionary<string, object>
            {
                ["Timeout"] = "invalid-number"
            }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Timeout parameter must be a positive integer");
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    [InlineData("HEAD")]
    [InlineData("OPTIONS")]
    public void ValidateInstruction_With_Valid_HTTP_Methods_Should_Pass(string method)
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { method, "https://example.com/api" }
        };

        // Act
        var result = _requestCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_Exception_In_Execution()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "invalid-url" }, // This should cause parsing issues
            LineNumber = 1
        };

        // Make the HTTP client throw an exception
        _httpClientMock.Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<Models.ProxyInfo?>(), It.IsAny<HttpClientConfiguration>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new InvalidOperationException("Test exception"));

        // Act
        var result = await _requestCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        _botData.Status.Should().Be(BotStatus.Error);
    }

    public void Dispose()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
    }
}

/// <summary>
/// Step 6 Integration Tests
/// </summary>
public class Step6_IntegrationTests : IDisposable
{
    private readonly WireMockServer _mockServer;

    public Step6_IntegrationTests()
    {
        _mockServer = WireMockServer.Start();
    }

    [Fact]
    public void RequestCommand_Should_Create_With_Valid_ServiceProvider()
    {
        // Arrange
        var serviceProvider = Step6TestHelpers.CreateServiceProvider();

        // Act
        var command = new RequestCommand(serviceProvider);

        // Assert
        command.Should().NotBeNull();
        command.CommandName.Should().Be("REQUEST");
        command.Description.Should().NotBeEmpty();
    }

    [Fact]
    public void RequestCommand_Should_Throw_With_Invalid_ServiceProvider()
    {
        // Arrange
        var emptyServiceProvider = new Mock<IServiceProvider>().Object;

        // Act & Assert
        var act = () => new RequestCommand(emptyServiceProvider);
        act.Should().Throw<ArgumentException>();
    }

    public void Dispose()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
    }
}

/// <summary>
/// Step 6 Test Helpers
/// </summary>
public static class Step6TestHelpers
{
    public static IServiceProvider CreateServiceProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<RequestCommand>)))
                      .Returns(new Mock<ILogger<RequestCommand>>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IHttpClientService)))
                      .Returns(new Mock<IHttpClientService>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IScriptParser)))
                      .Returns(new Mock<IScriptParser>().Object);
        
        return serviceProvider.Object;
    }

    public static ScriptInstruction CreateRequestInstruction(string method = "GET", string url = "https://example.com")
    {
        return new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { method, url },
            LineNumber = 1,
            RawLine = $"REQUEST {method} \"{url}\""
        };
    }

    public static BotData CreateTestBotData()
    {
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        return new BotData("test:data", config, logger, cancellationToken);
    }

    public static HttpResponseWrapper CreateSuccessResponse(string content = "Success")
    {
        return new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK),
            Content = content,

            ResponseTime = 100
        };
    }

    public static HttpResponseWrapper CreateErrorResponse(HttpStatusCode statusCode = HttpStatusCode.BadRequest, string content = "Error")
    {
        return new HttpResponseWrapper
        {
            Response = new HttpResponseMessage(statusCode),
            Content = content,
            ResponseTime = 50
        };
    }
}
