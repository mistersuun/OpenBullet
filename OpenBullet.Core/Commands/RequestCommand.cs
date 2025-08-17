using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Commands;

/// <summary>
/// REQUEST command implementation for HTTP requests
/// </summary>
public class RequestCommand : IScriptCommand
{
    private readonly ILogger<RequestCommand> _logger;
    private readonly IHttpClientService _httpClientService;
    private readonly IScriptParser _scriptParser;

    public string CommandName => "REQUEST";
    public string Description => "Sends an HTTP request to a specified URL with optional headers, content, and cookies";

    public RequestCommand(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<RequestCommand>>() 
                 ?? throw new ArgumentException("ILogger<RequestCommand> not found in service provider");
        _httpClientService = serviceProvider.GetService<IHttpClientService>() 
                           ?? throw new ArgumentException("IHttpClientService not found in service provider");
        _scriptParser = serviceProvider.GetService<IScriptParser>() 
                      ?? throw new ArgumentException("IScriptParser not found in service provider");
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        ArgumentNullException.ThrowIfNull(instruction);
        ArgumentNullException.ThrowIfNull(botData);

        try
        {
            _logger.LogTrace("Executing REQUEST command on line {LineNumber}", instruction.LineNumber);

            // Parse request parameters
            var requestParams = ParseRequestParameters(instruction, botData);

            // Create HTTP request message
            var request = CreateHttpRequestMessage(requestParams, botData);

            // Configure HTTP client
            var clientConfig = CreateHttpClientConfiguration(requestParams, botData);

            // Execute request
            var response = await _httpClientService.SendAsync(request, botData.Proxy, clientConfig);

            // Process response
            await ProcessResponse(response, botData, requestParams);

            _logger.LogTrace("REQUEST command completed successfully");
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "REQUEST command failed on line {LineNumber}", instruction.LineNumber);
            botData.Status = BotStatus.Error;
            botData.AddLogEntry($"REQUEST failed: {ex.Message}");
            return new CommandResult { Success = false };
        }
    }

    private RequestParameters ParseRequestParameters(ScriptInstruction instruction, BotData botData)
    {
        var parameters = new RequestParameters();

        if (instruction.Arguments.Count < 2)
        {
            throw new ArgumentException("REQUEST command requires at least METHOD and URL arguments");
        }

        // Parse method and URL
        parameters.Method = SubstituteVariables(instruction.Arguments[0], botData).ToUpperInvariant();
        parameters.Url = SubstituteVariables(instruction.Arguments[1], botData);

        // Validate method
        if (!IsValidHttpMethod(parameters.Method))
        {
            throw new ArgumentException($"Invalid HTTP method: {parameters.Method}");
        }

        // Parse optional arguments
        for (int i = 2; i < instruction.Arguments.Count; i++)
        {
            var arg = instruction.Arguments[i];

            if (arg.StartsWith("CONTENT", StringComparison.OrdinalIgnoreCase) && i + 1 < instruction.Arguments.Count)
            {
                parameters.Content = SubstituteVariables(instruction.Arguments[i + 1], botData);
                i++; // Skip next argument
            }
            else if (arg.StartsWith("CONTENTTYPE", StringComparison.OrdinalIgnoreCase) && i + 1 < instruction.Arguments.Count)
            {
                parameters.ContentType = SubstituteVariables(instruction.Arguments[i + 1], botData);
                i++; // Skip next argument
            }
            else if (arg.StartsWith("HEADER", StringComparison.OrdinalIgnoreCase) && i + 2 < instruction.Arguments.Count)
            {
                var headerName = SubstituteVariables(instruction.Arguments[i + 1], botData);
                var headerValue = SubstituteVariables(instruction.Arguments[i + 2], botData);
                parameters.Headers[headerName] = headerValue;
                i += 2; // Skip next two arguments
            }
            else if (arg.StartsWith("Cookie", StringComparison.OrdinalIgnoreCase) && i + 2 < instruction.Arguments.Count)
            {
                var cookieName = SubstituteVariables(instruction.Arguments[i + 1], botData);
                var cookieValue = SubstituteVariables(instruction.Arguments[i + 2], botData);
                parameters.Cookies[cookieName] = cookieValue;
                i += 2; // Skip next two arguments
            }
        }

        // Parse boolean parameters
        if (instruction.Parameters.TryGetValue("AutoRedirect", out var autoRedirectValue))
        {
            parameters.AutoRedirect = Convert.ToBoolean(autoRedirectValue);
        }

        if (instruction.Parameters.TryGetValue("Timeout", out var timeoutValue))
        {
            parameters.TimeoutSeconds = Convert.ToInt32(timeoutValue);
        }

        // Parse sub-instructions (multi-line format)
        foreach (var subInstruction in instruction.SubInstructions)
        {
            ProcessSubInstruction(subInstruction, parameters, botData);
        }

        return parameters;
    }

    private void ProcessSubInstruction(ScriptInstruction subInstruction, RequestParameters parameters, BotData botData)
    {
        var commandName = subInstruction.CommandName.ToUpperInvariant();

        switch (commandName)
        {
            case "CONTENT":
                if (subInstruction.Arguments.Count > 0)
                {
                    parameters.Content = SubstituteVariables(subInstruction.Arguments[0], botData);
                }
                break;

            case "CONTENTTYPE":
                if (subInstruction.Arguments.Count > 0)
                {
                    parameters.ContentType = SubstituteVariables(subInstruction.Arguments[0], botData);
                }
                break;

            case "HEADER":
                if (subInstruction.Arguments.Count >= 2)
                {
                    var headerName = SubstituteVariables(subInstruction.Arguments[0], botData);
                    var headerValue = SubstituteVariables(subInstruction.Arguments[1], botData);
                    parameters.Headers[headerName] = headerValue;
                }
                break;

            case "COOKIE":
                if (subInstruction.Arguments.Count >= 2)
                {
                    var cookieName = SubstituteVariables(subInstruction.Arguments[0], botData);
                    var cookieValue = SubstituteVariables(subInstruction.Arguments[1], botData);
                    parameters.Cookies[cookieName] = cookieValue;
                }
                break;

            default:
                _logger.LogWarning("Unknown REQUEST sub-command: {CommandName}", commandName);
                break;
        }
    }

    private HttpRequestMessage CreateHttpRequestMessage(RequestParameters parameters, BotData botData)
    {
        var method = new HttpMethod(parameters.Method);
        var request = new HttpRequestMessage(method, parameters.Url);

        // Add content if specified
        if (!string.IsNullOrEmpty(parameters.Content) && method != HttpMethod.Get && method != HttpMethod.Head)
        {
            var content = Encoding.UTF8.GetBytes(parameters.Content);
            request.Content = new ByteArrayContent(content);

            if (!string.IsNullOrEmpty(parameters.ContentType))
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(parameters.ContentType);
            }
        }

        // Add custom headers
        foreach (var header in parameters.Headers)
        {
            try
            {
                if (IsContentHeader(header.Key))
                {
                    request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                else
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add header {HeaderName}: {HeaderValue}", header.Key, header.Value);
            }
        }

        // Add custom cookies to the request
        if (parameters.Cookies.Count > 0)
        {
            var cookieHeader = string.Join("; ", parameters.Cookies.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        return request;
    }

    private HttpClientConfiguration CreateHttpClientConfiguration(RequestParameters parameters, BotData botData)
    {
        var config = new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromSeconds(parameters.TimeoutSeconds),
            AllowAutoRedirect = parameters.AutoRedirect,
            UseCookies = true,
            CookieContainer = botData.Cookies,
            UserAgent = "OpenBullet/2.0" // TODO: Add ConfigSettings to BotData
        };

        // TODO: Apply configuration settings from BotData when ConfigSettings is added
        // For now using defaults
        config.MaxRedirects = 8;
        config.IgnoreSslErrors = false;

        return config;
    }

    private async Task ProcessResponse(HttpResponseWrapper response, BotData botData, RequestParameters parameters)
    {
        // Update BotData with response information
        botData.Source = response.Content;
        botData.ResponseCode = (int)response.StatusCode;
        botData.Address = response.RequestUri ?? parameters.Url;

        // Update cookies from response
        if (response.Cookies?.Count > 0)
        {
            foreach (System.Net.Cookie cookie in response.Cookies)
            {
                try
                {
                    botData.Cookies.Add(cookie);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add response cookie: {CookieName}", cookie.Name);
                }
            }
        }

        // Log response details
        botData.AddLogEntry($"REQUEST {parameters.Method} {parameters.Url} -> {response.StatusCode} ({response.ResponseTime}ms)");

        // Handle specific status codes
        if (!response.IsSuccessStatusCode)
        {
            if ((int)response.StatusCode >= (int)System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("HTTP request returned error status: {StatusCode} for {Url}", 
                    response.StatusCode, parameters.Url);
                
                // Don't set error status for 4xx errors by default (configurable behavior)
                // TODO: Add ConfigSettings to BotData
                bool ignoreResponseErrors = false; // botData.ConfigSettings?.IgnoreResponseErrors ?? false;
                if (!ignoreResponseErrors)
                {
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        botData.Status = BotStatus.Failure;
                    }
                    else if ((int)response.StatusCode >= 500)
                    {
                        botData.Status = BotStatus.Retry;
                    }
                }
            }
        }
        else
        {
            _logger.LogTrace("HTTP request successful: {StatusCode} for {Url}", 
                response.StatusCode, parameters.Url);
        }

        // Handle exceptions
        if (response.Exception != null)
        {
            _logger.LogError(response.Exception, "HTTP request exception for {Url}", parameters.Url);
            botData.Status = BotStatus.Error;
            botData.AddLogEntry($"REQUEST error: {response.Exception.Message}");
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        // Validate minimum arguments
        if (instruction.Arguments.Count < 2)
        {
            result.IsValid = false;
            result.Errors.Add("REQUEST command requires at least METHOD and URL arguments");
            return result;
        }

        // Validate HTTP method
        var method = instruction.Arguments[0].ToUpperInvariant();
        if (!IsValidHttpMethod(method))
        {
            result.IsValid = false;
            result.Errors.Add($"Invalid HTTP method: {method}. Valid methods are: GET, POST, PUT, DELETE, PATCH, HEAD, OPTIONS");
        }

        // Validate URL format (basic check)
        var url = instruction.Arguments[1];
        if (string.IsNullOrWhiteSpace(url))
        {
            result.IsValid = false;
            result.Errors.Add("URL cannot be empty");
        }
        else if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("<"))
        {
            result.Warnings.Add($"URL should start with http:// or https:// (found: {url})");
        }

        // Validate argument pairs
        for (int i = 2; i < instruction.Arguments.Count; i++)
        {
            var arg = instruction.Arguments[i];
            
            if ((arg.Equals("CONTENT", StringComparison.OrdinalIgnoreCase) ||
                 arg.Equals("CONTENTTYPE", StringComparison.OrdinalIgnoreCase)) && 
                i + 1 >= instruction.Arguments.Count)
            {
                result.Errors.Add($"{arg} parameter requires a value");
                result.IsValid = false;
            }
            else if ((arg.Equals("HEADER", StringComparison.OrdinalIgnoreCase) ||
                      arg.Equals("Cookie", StringComparison.OrdinalIgnoreCase)) && 
                     i + 2 >= instruction.Arguments.Count)
            {
                result.Errors.Add($"{arg} parameter requires name and value");
                result.IsValid = false;
            }
        }

        // Validate boolean parameters
        foreach (var param in instruction.Parameters)
        {
            if (param.Key == "AutoRedirect" && param.Value is not bool)
            {
                if (!bool.TryParse(param.Value?.ToString(), out _))
                {
                    result.Errors.Add("AutoRedirect parameter must be true or false");
                    result.IsValid = false;
                }
            }
            else if (param.Key == "Timeout" && param.Value is not int)
            {
                if (!int.TryParse(param.Value?.ToString(), out var timeout) || timeout <= 0)
                {
                    result.Errors.Add("Timeout parameter must be a positive integer");
                    result.IsValid = false;
                }
            }
        }

        return result;
    }

    private string SubstituteVariables(string input, BotData botData)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Use the script parser's variable substitution
        var variables = botData.Variables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return _scriptParser.SubstituteVariables(input, variables);
    }

    private static bool IsValidHttpMethod(string method)
    {
        var validMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
        return validMethods.Contains(method, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsContentHeader(string headerName)
    {
        var contentHeaders = new[] 
        { 
            "Content-Type", "Content-Length", "Content-Encoding", "Content-Language", 
            "Content-Location", "Content-MD5", "Content-Range", "Expires", "Last-Modified" 
        };
        return contentHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Request parameters container
    /// </summary>
    private class RequestParameters
    {
        public string Method { get; set; } = "GET";
        public string Url { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";
        public Dictionary<string, string> Headers { get; set; } = new();
        public Dictionary<string, string> Cookies { get; set; } = new();
        public bool AutoRedirect { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 10;
    }
}
