using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Commands;

/// <summary>
/// PARSE command implementation for data extraction
/// </summary>
public class ParseCommand : IScriptCommand
{
    private readonly ILogger<ParseCommand> _logger;
    private readonly IParserFactory _parserFactory;
    private readonly IScriptParser _scriptParser;

    public string CommandName => "PARSE";
    public string Description => "Extracts data from text using various parsing methods (LR, Regex, CSS, JSON)";

    public ParseCommand(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<ParseCommand>>() 
                 ?? throw new ArgumentException("ILogger<ParseCommand> not found in service provider");
        _parserFactory = serviceProvider.GetService<IParserFactory>() 
                       ?? throw new ArgumentException("IParserFactory not found in service provider");
        _scriptParser = serviceProvider.GetService<IScriptParser>() 
                      ?? throw new ArgumentException("IScriptParser not found in service provider");
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        ArgumentNullException.ThrowIfNull(instruction);
        ArgumentNullException.ThrowIfNull(botData);

        try
        {
            _logger.LogTrace("Executing PARSE command on line {LineNumber}", instruction.LineNumber);

            // Parse parameters
            var parseParams = BuildParseParameters(instruction, botData);

            // Get the parser
            var parser = _parserFactory.CreateParser(parseParams.ParseType);

            // Build parse options
            var parseOptions = BuildParseOptions(parseParams, instruction);

            // Execute parsing
            var parseResult = parser.Parse(parseParams.Input, parseParams.Pattern, parseOptions);

            // Process results
            var commandResult = await ProcessParseResult(parseResult, parseParams, botData, instruction);

            _logger.LogTrace("PARSE command completed successfully with {MatchCount} matches", parseResult.Matches.Count);
            return commandResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PARSE command failed on line {LineNumber}", instruction.LineNumber);
            botData.Status = BotStatus.Error;
            botData.AddLogEntry($"PARSE failed: {ex.Message}");
            return new CommandResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        try
        {
            // Validate minimum arguments: TARGET TYPE PATTERN
            if (instruction.Arguments.Count < 3)
            {
                result.IsValid = false;
                result.Errors.Add("PARSE command requires at least TARGET, TYPE, and PATTERN arguments");
                return result;
            }

            // Validate parse type with proper mapping
            var typeArg = instruction.Arguments[1].ToUpperInvariant();
            ParseType parseType;
            
            switch (typeArg)
            {
                case "LR":
                    parseType = ParseType.LeftRight;
                    break;
                case "REGEX":
                    parseType = ParseType.Regex;
                    break;
                case "CSS":
                    parseType = ParseType.CSS;
                    break;
                case "JSON":
                    parseType = ParseType.Json;
                    break;
                case "XPATH":
                    parseType = ParseType.XPath;
                    break;
                default:
                    result.IsValid = false;
                    result.Errors.Add($"Invalid parse type: {typeArg}. Valid types are: LR, REGEX, CSS, JSON, XPATH");
                    return result;
            }

            // Validate that the parser type is available
            var availableTypes = _parserFactory.GetAvailableTypes();
            if (!availableTypes.Contains(parseType))
            {
                result.IsValid = false;
                result.Errors.Add($"Parser type {typeArg} is not available");
                return result;
            }

            // Validate pattern if possible
            var pattern = instruction.Arguments[2];
            if (!string.IsNullOrWhiteSpace(pattern) && !pattern.StartsWith("<"))
            {
                try
                {
                    var parser = _parserFactory.CreateParser(parseType);
                    if (!parser.IsValidPattern(pattern))
                    {
                        result.Warnings.Add($"Pattern may be invalid for {parseType} parser: {pattern}");
                    }
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Could not validate pattern: {ex.Message}");
                }
            }

            // Check for redirector
            var hasRedirector = instruction.Parameters.ContainsKey("RedirectorType") && 
                              instruction.Parameters.ContainsKey("RedirectorName");
            
            if (!hasRedirector)
            {
                result.Warnings.Add("PARSE command should specify output destination with -> VAR \"name\" or -> CAP \"name\"");
            }

            // Validate boolean parameters
            ValidateBooleanParameters(instruction, result);

            return result;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    private ParseParameters BuildParseParameters(ScriptInstruction instruction, BotData botData)
    {
        var parameters = new ParseParameters();

        // Get target (source data)
        parameters.Input = SubstituteVariables(instruction.Arguments[0], botData);
        
        // Handle special target values
        if (parameters.Input.Equals("<SOURCE>", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Input = botData.Source;
        }
        else if (parameters.Input.Equals("<ADDRESS>", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Input = botData.Address;
        }

        // Get parse type with proper mapping
        var typeString = instruction.Arguments[1].ToUpperInvariant();
        ParseType parseType = typeString switch
        {
            "LR" => ParseType.LeftRight,
            "REGEX" => ParseType.Regex,
            "CSS" => ParseType.CSS,
            "JSON" => ParseType.Json,
            "XPATH" => ParseType.XPath,
            _ => throw new ArgumentException($"Invalid parse type: {typeString}")
        };
        parameters.ParseType = parseType;

        // Get pattern
        parameters.Pattern = SubstituteVariables(instruction.Arguments[2], botData);

        // Parse additional arguments based on type
        ParseTypeSpecificArguments(instruction, parameters, botData);

        // Get redirector information
        if (instruction.Parameters.TryGetValue("RedirectorType", out var redirectorType))
        {
            parameters.RedirectorType = redirectorType.ToString();
        }
        
        if (instruction.Parameters.TryGetValue("RedirectorName", out var redirectorName))
        {
            parameters.RedirectorName = redirectorName.ToString();
        }

        if (instruction.Parameters.TryGetValue("RedirectorPrefix", out var prefix))
        {
            parameters.RedirectorPrefix = prefix.ToString();
        }

        if (instruction.Parameters.TryGetValue("RedirectorSuffix", out var suffix))
        {
            parameters.RedirectorSuffix = suffix.ToString();
        }

        return parameters;
    }

    private void ParseTypeSpecificArguments(ScriptInstruction instruction, ParseParameters parameters, BotData botData)
    {
        switch (parameters.ParseType)
        {
            case ParseType.LeftRight:
                // For LR, we expect: TARGET, TYPE, LEFT_DELIMITER, RIGHT_DELIMITER  
                // Pattern is already set to LEFT_DELIMITER (arguments[2])
                // Now set both delimiters for the options
                parameters.LeftDelimiter = parameters.Pattern; // Left delimiter is the pattern
                if (instruction.Arguments.Count >= 4)
                {
                    parameters.RightDelimiter = SubstituteVariables(instruction.Arguments[3], botData); // Right delimiter
                }
                break;

            case ParseType.CSS:
                // For CSS, we might have an attribute name
                if (instruction.Arguments.Count >= 4)
                {
                    parameters.AttributeName = SubstituteVariables(instruction.Arguments[3], botData);
                }
                break;

            case ParseType.Json:
                // JSON parsing might have additional path specifications
                // Pattern is already set, no additional arguments needed typically
                break;

            case ParseType.Regex:
                // Regex might have group specifications
                // Pattern is already set, group handling is in options
                break;
        }
    }

    private ParseOptions BuildParseOptions(ParseParameters parameters, ScriptInstruction instruction)
    {
        var options = new ParseOptions();

        // Set basic options from parameters
        if (!string.IsNullOrEmpty(parameters.LeftDelimiter))
        {
            options.LeftDelimiter = parameters.LeftDelimiter;
        }
        
        if (!string.IsNullOrEmpty(parameters.RightDelimiter))
        {
            options.RightDelimiter = parameters.RightDelimiter;
        }

        if (!string.IsNullOrEmpty(parameters.AttributeName))
        {
            options.AttributeName = parameters.AttributeName;
        }

        // Parse boolean parameters
        if (instruction.Parameters.TryGetValue("Recursive", out var recursive))
        {
            options.Recursive = Convert.ToBoolean(recursive);
        }

        if (instruction.Parameters.TryGetValue("IgnoreCase", out var ignoreCase))
        {
            options.IgnoreCase = Convert.ToBoolean(ignoreCase);
        }

        if (instruction.Parameters.TryGetValue("Multiline", out var multiline))
        {
            options.Multiline = Convert.ToBoolean(multiline);
        }

        // Parse additional options from custom parameters
        foreach (var param in instruction.Parameters)
        {
            if (!IsReservedParameter(param.Key))
            {
                options.CustomOptions[param.Key] = param.Value;
            }
        }

        return options;
    }

    private async Task<CommandResult> ProcessParseResult(ParseResult parseResult, ParseParameters parameters, BotData botData, ScriptInstruction instruction)
    {
        var commandResult = new CommandResult();

        if (!parseResult.Success)
        {
            commandResult.Success = false;
            commandResult.ErrorMessage = parseResult.ErrorMessage ?? "Parse operation failed";
            botData.AddLogEntry($"PARSE failed: {commandResult.ErrorMessage}");
            return commandResult;
        }

        // Log the results
        botData.AddLogEntry($"PARSE {parameters.ParseType} extracted {parseResult.Matches.Count} matches");

        // Handle output based on redirector
        if (!string.IsNullOrEmpty(parameters.RedirectorType) && !string.IsNullOrEmpty(parameters.RedirectorName))
        {
            await StoreResults(parseResult, parameters, botData, commandResult);
        }

        // Add metadata to command result
        foreach (var metadata in parseResult.Metadata)
        {
            commandResult.CapturedData[$"Parse_{metadata.Key}"] = metadata.Value;
        }

        commandResult.Success = true;
        return commandResult;
    }

    private async Task StoreResults(ParseResult parseResult, ParseParameters parameters, BotData botData, CommandResult commandResult)
    {
        var results = parseResult.Matches;
        
        // Apply prefix/suffix if specified
        if (!string.IsNullOrEmpty(parameters.RedirectorPrefix) || !string.IsNullOrEmpty(parameters.RedirectorSuffix))
        {
            results = results.Select(r => $"{parameters.RedirectorPrefix}{r}{parameters.RedirectorSuffix}").ToList();
        }

        var outputValue = results.Count switch
        {
            0 => string.Empty,
            1 => results[0],
            _ => string.Join("|", results) // Multiple results joined with pipe
        };

        switch (parameters.RedirectorType?.ToUpperInvariant())
        {
            case "VAR":
                botData.SetVariable(parameters.RedirectorName!, outputValue);
                commandResult.Variables[parameters.RedirectorName!] = outputValue;
                break;

            case "CAP":
                botData.SetCapture(parameters.RedirectorName!, outputValue);
                commandResult.CapturedData[parameters.RedirectorName!] = outputValue;
                break;

            default:
                // Default to variable if not specified
                botData.SetVariable(parameters.RedirectorName!, outputValue);
                commandResult.Variables[parameters.RedirectorName!] = outputValue;
                break;
        }

        // Also store all matches if more than one
        if (results.Count > 1)
        {
            var allMatchesKey = $"{parameters.RedirectorName}_ALL";
            botData.SetVariable(allMatchesKey, results);
            commandResult.Variables[allMatchesKey] = results;
        }
    }

    private void ValidateBooleanParameters(ScriptInstruction instruction, CommandValidationResult result)
    {
        var booleanParams = new[] { "Recursive", "IgnoreCase", "Multiline" };

        foreach (var param in booleanParams)
        {
            if (instruction.Parameters.TryGetValue(param, out var value))
            {
                if (value is not bool && !bool.TryParse(value?.ToString(), out _))
                {
                    result.Errors.Add($"{param} parameter must be true or false");
                    result.IsValid = false;
                }
            }
        }
    }

    private bool IsReservedParameter(string parameterName)
    {
        var reserved = new[] 
        { 
            "RedirectorType", "RedirectorName", "RedirectorPrefix", "RedirectorSuffix",
            "Recursive", "IgnoreCase", "Multiline"
        };
        
        return reserved.Contains(parameterName, StringComparer.OrdinalIgnoreCase);
    }

    private string SubstituteVariables(string input, BotData botData)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var variables = botData.Variables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return _scriptParser.SubstituteVariables(input, variables);
    }

    /// <summary>
    /// Parameters for parse operations
    /// </summary>
    private class ParseParameters
    {
        public string Input { get; set; } = string.Empty;
        public ParseType ParseType { get; set; }
        public string Pattern { get; set; } = string.Empty;
        public string? LeftDelimiter { get; set; }
        public string? RightDelimiter { get; set; }
        public string? AttributeName { get; set; }
        public string? RedirectorType { get; set; }
        public string? RedirectorName { get; set; }
        public string? RedirectorPrefix { get; set; }
        public string? RedirectorSuffix { get; set; }
    }
}
