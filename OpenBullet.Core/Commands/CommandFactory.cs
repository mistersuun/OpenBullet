using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using System.Collections.Concurrent;

namespace OpenBullet.Core.Commands;

/// <summary>
/// Command factory implementation
/// </summary>
public class CommandFactory : ICommandFactory
{
    private readonly ILogger<CommandFactory> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, Type> _commandTypes = new();
    private readonly ConcurrentDictionary<string, IScriptCommand> _commandInstances = new();
    private readonly ConcurrentDictionary<string, CommandMetadata> _commandMetadata = new();

    public CommandFactory(ILogger<CommandFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        RegisterDefaultCommands();
    }

    public IScriptCommand? CreateCommand(string commandName)
    {
        ArgumentException.ThrowIfNullOrEmpty(commandName);

        var normalizedName = commandName.ToUpperInvariant();

        try
        {
            // Check for singleton instances first
            if (_commandInstances.TryGetValue(normalizedName, out var instance))
            {
                return instance;
            }

            // Create new instance from registered type
            if (_commandTypes.TryGetValue(normalizedName, out var commandType))
            {
                // Try service provider first (for DI-registered commands)
                var command = (IScriptCommand?)_serviceProvider.GetService(commandType);
                
                // Fall back to direct instantiation if not in DI container
                if (command == null)
                {
                    try
                    {
                        // Try constructor with IServiceProvider
                        command = (IScriptCommand?)Activator.CreateInstance(commandType, _serviceProvider);
                    }
                    catch
                    {
                        // Fall back to parameterless constructor
                        command = (IScriptCommand?)Activator.CreateInstance(commandType);
                    }
                }
                
                if (command != null)
                {
                    _logger.LogTrace("Created command instance for {CommandName}", commandName);
                    return command;
                }
            }

            _logger.LogWarning("Unknown command: {CommandName}", commandName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating command {CommandName}", commandName);
            return null;
        }
    }

    public IEnumerable<string> GetAvailableCommands()
    {
        var commands = new HashSet<string>();
        commands.UnionWith(_commandTypes.Keys);
        commands.UnionWith(_commandInstances.Keys);
        return commands.OrderBy(c => c);
    }

    public void RegisterCommand<T>(string commandName) where T : class, IScriptCommand
    {
        ArgumentException.ThrowIfNullOrEmpty(commandName);

        var normalizedName = commandName.ToUpperInvariant();
        _commandTypes[normalizedName] = typeof(T);

        _logger.LogDebug("Registered command type {CommandName} -> {TypeName}", commandName, typeof(T).Name);
    }

    public void RegisterCommand(string commandName, IScriptCommand command)
    {
        ArgumentException.ThrowIfNullOrEmpty(commandName);
        ArgumentNullException.ThrowIfNull(command);

        var normalizedName = commandName.ToUpperInvariant();
        _commandInstances[normalizedName] = command;

        _logger.LogDebug("Registered command instance {CommandName} -> {TypeName}", commandName, command.GetType().Name);
    }

    public bool IsCommandRegistered(string commandName)
    {
        if (string.IsNullOrEmpty(commandName))
            return false;

        var normalizedName = commandName.ToUpperInvariant();
        return _commandTypes.ContainsKey(normalizedName) || _commandInstances.ContainsKey(normalizedName);
    }

    public CommandMetadata? GetCommandMetadata(string commandName)
    {
        if (string.IsNullOrEmpty(commandName))
            return null;

        var normalizedName = commandName.ToUpperInvariant();
        return _commandMetadata.TryGetValue(normalizedName, out var metadata) ? metadata : null;
    }

    /// <summary>
    /// Registers command metadata
    /// </summary>
    public void RegisterCommandMetadata(string commandName, CommandMetadata metadata)
    {
        ArgumentException.ThrowIfNullOrEmpty(commandName);
        ArgumentNullException.ThrowIfNull(metadata);

        var normalizedName = commandName.ToUpperInvariant();
        _commandMetadata[normalizedName] = metadata;
    }

    private void RegisterDefaultCommands()
    {
        // Register REQUEST command
        RegisterCommand<RequestCommand>("REQUEST");
        
        // Register PARSE command
        RegisterCommand<ParseCommand>("PARSE");
        
        // Register KEYCHECK command
        RegisterCommand<KeyCheckCommand>("KEYCHECK");
        
        // Register Step 14: Advanced Commands
        RegisterCommand<FunctionCommand>("FUNCTION");
        RegisterCommand<UtilityCommand>("UTILITY");
        RegisterCommand<FlowControlCommand>("FLOWCONTROL");
        RegisterCommandMetadata("REQUEST", new CommandMetadata
        {
            Name = "REQUEST",
            Description = "Sends an HTTP request to a specified URL",
            Category = "HTTP",
            Syntax = "REQUEST <METHOD> \"<URL>\" [CONTENT \"<content>\"] [CONTENTTYPE \"<type>\"] [HEADER \"<name>\" \"<value>\"] [Cookie \"<name>\" \"<value>\"] [AutoRedirect=<true/false>] [Timeout=<seconds>]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "METHOD", Description = "HTTP method", Type = ParameterType.HttpMethod, Required = true, AllowedValues = { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" } },
                new() { Name = "URL", Description = "Target URL", Type = ParameterType.Url, Required = true },
                new() { Name = "CONTENT", Description = "Request body content", Type = ParameterType.String, Required = false },
                new() { Name = "CONTENTTYPE", Description = "Content-Type header", Type = ParameterType.ContentType, Required = false, DefaultValue = "application/x-www-form-urlencoded" },
                new() { Name = "HEADER", Description = "Custom header", Type = ParameterType.String, Required = false },
                new() { Name = "Cookie", Description = "Custom cookie", Type = ParameterType.String, Required = false },
                new() { Name = "AutoRedirect", Description = "Follow redirects", Type = ParameterType.Boolean, Required = false, DefaultValue = true },
                new() { Name = "Timeout", Description = "Request timeout in seconds", Type = ParameterType.Integer, Required = false, DefaultValue = 10 }
            },
            Examples = new List<string>
            {
                "REQUEST GET \"https://example.com\"",
                "REQUEST POST \"https://api.example.com/login\" CONTENT \"username=<USER>&password=<PASS>\" CONTENTTYPE \"application/x-www-form-urlencoded\"",
                "REQUEST GET \"https://example.com/api\" HEADER \"Authorization\" \"Bearer <TOKEN>\" AutoRedirect=false"
            }
        });

        // Register PARSE command metadata
        RegisterCommandMetadata("PARSE", new CommandMetadata
        {
            Name = "PARSE",
            Description = "Extracts data from text using various parsing methods (LR, Regex, CSS, JSON)",
            Category = "Data",
            Syntax = "PARSE \"<TARGET>\" <TYPE> \"<PATTERN>\" [\"<ATTRIBUTE>\"] [Recursive=<true/false>] [-> VAR/CAP \"<NAME>\"]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "TARGET", Description = "Source data to parse", Type = ParameterType.String, Required = true },
                new() { Name = "TYPE", Description = "Parse method", Type = ParameterType.Enum, Required = true, AllowedValues = { "LR", "REGEX", "CSS", "JSON", "XPATH" } },
                new() { Name = "PATTERN", Description = "Parsing pattern or selector", Type = ParameterType.String, Required = true },
                new() { Name = "ATTRIBUTE", Description = "Attribute name for CSS parsing", Type = ParameterType.String, Required = false },
                new() { Name = "Recursive", Description = "Enable recursive parsing", Type = ParameterType.Boolean, Required = false, DefaultValue = false },
                new() { Name = "IgnoreCase", Description = "Ignore case in parsing", Type = ParameterType.Boolean, Required = false, DefaultValue = false },
                new() { Name = "Multiline", Description = "Enable multiline mode", Type = ParameterType.Boolean, Required = false, DefaultValue = false }
            },
            Examples = new List<string>
            {
                "PARSE \"<SOURCE>\" LR \"<title>\" \"</title>\" -> VAR \"TITLE\"",
                "PARSE \"<SOURCE>\" CSS \"input[name='token']\" \"value\" -> VAR \"TOKEN\"",
                "PARSE \"<SOURCE>\" REGEX \"\\d+\" -> VAR \"NUMBERS\"",
                "PARSE \"<SOURCE>\" JSON \"$.data[0].name\" -> CAP \"USERNAME\""
            }
        });

        // Register KEYCHECK command metadata
        RegisterCommandMetadata("KEYCHECK", new CommandMetadata
        {
            Name = "KEYCHECK",
            Description = "Analyzes response data using key chains to determine bot status",
            Category = "Logic",
            Syntax = "KEYCHECK\n  KEYCHAIN <STATUS> [<LOGIC>]\n    KEY \"<SOURCE>\" <CONDITION> \"<VALUE>\" [CaseSensitive=<true/false>]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "STATUS", Description = "Key chain status", Type = ParameterType.Enum, Required = true, AllowedValues = { "Success", "Failure", "Ban", "Retry", "Custom" } },
                new() { Name = "LOGIC", Description = "Chain logic operator", Type = ParameterType.Enum, Required = false, AllowedValues = { "OR", "AND" }, DefaultValue = "OR" },
                new() { Name = "SOURCE", Description = "Data source to check", Type = ParameterType.String, Required = true },
                new() { Name = "CONDITION", Description = "Comparison condition", Type = ParameterType.Enum, Required = true, AllowedValues = { "Contains", "DoesNotContain", "EqualTo", "NotEqualTo", "MatchesRegex", "StartsWith", "EndsWith", "IsEmpty", "IsNotEmpty" } },
                new() { Name = "VALUE", Description = "Value to compare against", Type = ParameterType.String, Required = true },
                new() { Name = "CaseSensitive", Description = "Enable case-sensitive comparison", Type = ParameterType.Boolean, Required = false, DefaultValue = false }
            },
            Examples = new List<string>
            {
                "KEYCHECK\n  KEYCHAIN Success OR\n    KEY \"<SOURCE>\" Contains \"Welcome\"\n    KEY \"<SOURCE>\" Contains \"Dashboard\"",
                "KEYCHECK\n  KEYCHAIN Failure OR\n    KEY \"<SOURCE>\" Contains \"Invalid\"\n    KEY \"<SOURCE>\" Contains \"Error\"",
                "KEYCHECK\n  KEYCHAIN Ban AND\n    KEY \"<RESPONSECODE>\" EqualTo \"429\"\n    KEY \"<SOURCE>\" Contains \"Rate limit\"",
                "KEYCHECK\n  KEYCHAIN Custom \"Premium User\" OR\n    KEY \"<SOURCE>\" Contains \"Premium\"\n    KEY \"<USER_TYPE>\" EqualTo \"premium\""
            }
        });

        // Register FUNCTION command metadata
        RegisterCommandMetadata("FUNCTION", new CommandMetadata
        {
            Name = "FUNCTION",
            Description = "Defines and calls reusable code blocks with parameters",
            Category = "Advanced",
            Syntax = "FUNCTION <COMMAND> \"<NAME>\" [PARAMS \"<param1,param2>\"] [BODY \"<code>\"]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "COMMAND", Description = "Function operation", Type = ParameterType.Enum, Required = true, AllowedValues = { "DEFINE", "CALL", "LIST", "DELETE" } },
                new() { Name = "NAME", Description = "Function name", Type = ParameterType.String, Required = true },
                new() { Name = "PARAMS", Description = "Function parameters", Type = ParameterType.String, Required = false },
                new() { Name = "BODY", Description = "Function code body", Type = ParameterType.String, Required = false }
            },
            Examples = new List<string>
            {
                "FUNCTION DEFINE \"greet\" PARAMS \"name\" BODY \"SET 'message' 'Hello <name>!'\"",
                "FUNCTION CALL \"greet\" PARAMS \"John\"",
                "FUNCTION LIST",
                "FUNCTION DELETE \"greet\""
            }
        });

        // Register UTILITY command metadata
        RegisterCommandMetadata("UTILITY", new CommandMetadata
        {
            Name = "UTILITY",
            Description = "Provides utility operations for strings, lists, math, dates, and encoding",
            Category = "Advanced",
            Syntax = "UTILITY <OPERATION> [TARGET \"<variable>\"] [VALUE \"<value>\"] [RESULT \"<variable>\"]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "OPERATION", Description = "Utility operation", Type = ParameterType.Enum, Required = true, AllowedValues = { "REPLACE", "SUBSTRING", "SPLIT", "JOIN", "UPPER", "LOWER", "TRIM", "LENGTH", "ADD", "REMOVE", "SORT", "SHUFFLE", "CALCULATE", "RANDOM", "ROUND", "NOW", "FORMAT", "BASE64", "URL", "HASH" } },
                new() { Name = "TARGET", Description = "Target variable or value", Type = ParameterType.String, Required = false },
                new() { Name = "VALUE", Description = "Operation value", Type = ParameterType.String, Required = false },
                new() { Name = "RESULT", Description = "Result variable", Type = ParameterType.String, Required = false }
            },
            Examples = new List<string>
            {
                "UTILITY UPPER TARGET \"text\" RESULT \"uppercase\"",
                "UTILITY SPLIT TARGET \"csv\" VALUE \",\" RESULT \"parts\"",
                "UTILITY CALCULATE TARGET \"5+3\" RESULT \"sum\"",
                "UTILITY NOW RESULT \"timestamp\"",
                "UTILITY BASE64 TARGET \"hello\" VALUE \"encode\" RESULT \"encoded\""
            }
        });

        // Register FLOWCONTROL command metadata
        RegisterCommandMetadata("FLOWCONTROL", new CommandMetadata
        {
            Name = "FLOWCONTROL",
            Description = "Provides flow control structures like IF/ELSE, WHILE, FOR, and exception handling",
            Category = "Advanced",
            Syntax = "FLOWCONTROL <COMMAND> [CONDITION \"<condition>\"] [BODY \"<code>\"] [ELSE \"<code>\"]",
            RequiresVariableSubstitution = true,
            Parameters = new List<CommandParameter>
            {
                new() { Name = "COMMAND", Description = "Flow control operation", Type = ParameterType.Enum, Required = true, AllowedValues = { "IF", "WHILE", "FOR", "BREAK", "CONTINUE", "RETURN", "TRY" } },
                new() { Name = "CONDITION", Description = "Control condition", Type = ParameterType.String, Required = false },
                new() { Name = "BODY", Description = "Code body to execute", Type = ParameterType.String, Required = false },
                new() { Name = "ELSE", Description = "Else code body", Type = ParameterType.String, Required = false }
            },
            Examples = new List<string>
            {
                "FLOWCONTROL IF CONDITION \"status == 'success'\" BODY \"SET 'result' 'Success!'\" ELSE \"SET 'result' 'Failed!'\"",
                "FLOWCONTROL WHILE CONDITION \"count < 5\" BODY \"INCREMENT 'count'\"",
                "FLOWCONTROL FOR CONDITION \"i,1,10\" BODY \"SET 'sum' '<sum> + <i>'\"",
                "FLOWCONTROL TRY BODY \"REQUEST GET '<url>'\" ELSE \"SET 'error' 'Request failed'\""
            }
        });

        _logger.LogInformation("Registered {Count} default commands", _commandTypes.Count + _commandInstances.Count);
    }
}
