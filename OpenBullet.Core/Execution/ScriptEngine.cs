using Microsoft.Extensions.Logging;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using System.Diagnostics;

namespace OpenBullet.Core.Execution;

/// <summary>
/// Implementation of script execution engine
/// </summary>
public class ScriptEngine : IScriptEngine
{
    private readonly ILogger<ScriptEngine> _logger;
    private readonly ICommandFactory _commandFactory;
    private readonly IScriptParser _scriptParser;
    private readonly ExecutionStatistics _statistics = new();

    public ScriptEngine(
        ILogger<ScriptEngine> logger,
        ICommandFactory commandFactory,
        IScriptParser scriptParser)
    {
        _logger = logger;
        _commandFactory = commandFactory;
        _scriptParser = scriptParser;
        // TODO: Fix ExecutionStatistics to have StartTime property
        // _statistics.StartTime = DateTime.UtcNow;
    }

    public async Task<ScriptExecutionResult> ExecuteAsync(ConfigModel config, BotData botData, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ScriptExecutionResult();

        try
        {
            // Check for cancellation at the start
            cancellationToken.ThrowIfCancellationRequested();
            
            _logger.LogDebug("Starting script execution for bot {BotId} with config {ConfigName}", 
                botData.Id, config.Name);

            // Parse script instructions
            var parseResult = _scriptParser.ParseScript(config.Script);
            if (parseResult.Instructions.Count == 0)
            {
                _logger.LogWarning("No instructions found in script for config {ConfigName}", config.Name);
                result.Success = true;
                result.Status = BotStatus.Success;
                return result;
            }

            // Create execution context
            var context = new ScriptExecutionContext
            {
                Config = config,
                BotData = botData,
                Instructions = parseResult.Instructions,
                CancellationToken = cancellationToken,
                StartTime = DateTime.UtcNow
            };

            // Build label map
            BuildLabelMap(context);

            // Execute instructions sequentially
            while (context.ShouldContinue())
            {
                // Check for cancellation before each instruction
                cancellationToken.ThrowIfCancellationRequested();
                
                var instruction = context.GetCurrentInstruction();
                if (instruction == null)
                {
                    break;
                }

                var commandResult = await ExecuteInstructionAsync(instruction, botData, cancellationToken);
                context.CommandResults.Add(commandResult);
                result.CommandResults.Add(commandResult);

                if (!commandResult.Success)
                {
                    _logger.LogWarning("Command {CommandName} failed on line {LineNumber}: {ErrorMessage}", 
                        commandResult.CommandName, commandResult.LineNumber, commandResult.ErrorMessage);

                    // Handle command failure based on configuration
                    if (config.Settings?.IgnoreResponseErrors != true)
                    {
                        result.Success = false;
                        result.ErrorMessage = commandResult.ErrorMessage;
                        result.Exception = commandResult.Exception;
                        break;
                    }
                }

                // Handle flow control
                var shouldContinue = await HandleFlowControl(context, commandResult);
                if (!shouldContinue)
                {
                    break;
                }

                // Move to next instruction if continuing normally
                if (commandResult.FlowControl == FlowControl.Continue)
                {
                    context.MoveNext();
                }
            }

            // Finalize result
            stopwatch.Stop();
            
            // Set success based on execution results:
            // If ErrorMessage is set, a command failed, keep Success = false
            // Otherwise, script executed successfully
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                result.Success = true;
                // Set bot status to Success if it's still None
                if (botData.Status == BotStatus.None)
                {
                    botData.Status = BotStatus.Success;
                }
            }
            
            result.Status = botData.Status;
            result.CustomStatus = botData.CustomStatus;
            result.ExecutionTime = stopwatch.Elapsed;
            result.CommandsExecuted = context.CommandResults.Count;
            result.Variables = new Dictionary<string, object>(botData.Variables);
            result.CapturedData = new Dictionary<string, object>(botData.CapturedData);
            result.Logs = new List<string>(botData.Log);

            // Update statistics
            _statistics.TotalScriptsExecuted++;
            _statistics.TotalCommandsExecuted += result.CommandsExecuted;
            _statistics.TotalExecutionTime = _statistics.TotalExecutionTime.Add(result.ExecutionTime);
            
            if (result.Success)
            {
                _statistics.SuccessfulExecutions++;
            }
            else
            {
                _statistics.FailedExecutions++;
            }
            
            _statistics.LogEntryCount += result.Logs.Count;
            _statistics.TotalResponseTime += (long)result.ExecutionTime.TotalMilliseconds;

            _logger.LogDebug("Script execution completed for bot {BotId} in {ExecutionTime}ms with status {Status}", 
                botData.Id, result.ExecutionTime.TotalMilliseconds, result.Status);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Script execution cancelled for bot {BotId}", botData.Id);
            result.Success = false;
            result.Status = BotStatus.Error;
            result.ErrorMessage = "Script execution was cancelled";
            result.ExecutionTime = stopwatch.Elapsed;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Script execution failed for bot {BotId}", botData.Id);
            result.Success = false;
            result.Status = BotStatus.Error;
            result.ErrorMessage = ex.Message;
            result.Exception = ex;
            result.ExecutionTime = stopwatch.Elapsed;
            return result;
        }
    }

    public async Task<CommandExecutionResult> ExecuteInstructionAsync(ScriptInstruction instruction, BotData botData, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new CommandExecutionResult
        {
            CommandName = instruction.CommandName,
            LineNumber = instruction.LineNumber
        };

        try
        {
            _logger.LogTrace("Executing command {CommandName} on line {LineNumber}", 
                instruction.CommandName, instruction.LineNumber);

            // Skip comments and empty lines
            if (instruction.CommandName.StartsWith("#") || string.IsNullOrWhiteSpace(instruction.CommandName))
            {
                result.Success = true;
                result.FlowControl = FlowControl.Continue;
                return result;
            }

            // Handle labels
            if (!string.IsNullOrEmpty(instruction.Label))
            {
                result.Success = true;
                result.FlowControl = FlowControl.Continue;
                return result;
            }

            // Create command instance
            var command = _commandFactory.CreateCommand(instruction.CommandName);
            if (command == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Unknown command: {instruction.CommandName}";
                _logger.LogError("Unknown command {CommandName} on line {LineNumber}", 
                    instruction.CommandName, instruction.LineNumber);
                return result;
            }

            // Execute command
            var commandResult = await command.ExecuteAsync(instruction, botData);

            // Map command result to execution result
            result.Success = commandResult.Success;
            result.ErrorMessage = commandResult.ErrorMessage;
            result.FlowControl = commandResult.FlowControl;
            result.OutputData = new Dictionary<string, object>(commandResult.Variables);
            result.OutputData.AddRange(commandResult.CapturedData);

            // Handle status updates from command
            if (commandResult.NewStatus.HasValue)
            {
                botData.Status = commandResult.NewStatus.Value;
            }

            if (!string.IsNullOrEmpty(commandResult.CustomStatus))
            {
                botData.CustomStatus = commandResult.CustomStatus;
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            result.Success = false;
            result.ErrorMessage = "Command execution was cancelled";
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Command {CommandName} failed on line {LineNumber}", 
                instruction.CommandName, instruction.LineNumber);
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Exception = ex;
            return result;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }
    }

    public ScriptValidationResult ValidateScript(ConfigModel config)
    {
        var result = new ScriptValidationResult { IsValid = true };

        try
        {
            _logger.LogTrace("Validating script for config {ConfigName}", config.Name);

            if (string.IsNullOrWhiteSpace(config.Script))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    LineNumber = 0,
                    CommandName = "SCRIPT",
                    Message = "Script is empty or null",
                    Severity = ValidationSeverity.Critical
                });
                return result;
            }

            // Parse script instructions
            var parseResult = _scriptParser.ParseScript(config.Script);
            result.TotalCommands = parseResult.Instructions.Count;

            // Validate each instruction
            var labels = new HashSet<string>();
            var referencedLabels = new HashSet<string>();
            var variables = new HashSet<string>();

            foreach (var instruction in parseResult.Instructions)
            {
                // Collect labels
                if (!string.IsNullOrEmpty(instruction.Label))
                {
                    if (labels.Contains(instruction.Label))
                    {
                        result.Errors.Add(new ValidationError
                        {
                            LineNumber = instruction.LineNumber,
                            CommandName = instruction.CommandName,
                            Message = $"Duplicate label: {instruction.Label}",
                            Severity = ValidationSeverity.Error
                        });
                        result.IsValid = false;
                    }
                    else
                    {
                        labels.Add(instruction.Label);
                    }
                }

                // Skip comments and empty lines
                if (instruction.CommandName.StartsWith("#") || string.IsNullOrWhiteSpace(instruction.CommandName))
                {
                    continue;
                }

                // Count commands
                if (!result.CommandCounts.ContainsKey(instruction.CommandName))
                {
                    result.CommandCounts[instruction.CommandName] = 0;
                }
                result.CommandCounts[instruction.CommandName]++;

                // Validate command exists
                try
                {
                    var command = _commandFactory.CreateCommand(instruction.CommandName);
                    if (command == null)
                    {
                        result.Errors.Add(new ValidationError
                        {
                            LineNumber = instruction.LineNumber,
                            CommandName = instruction.CommandName,
                            Message = $"Unknown command: {instruction.CommandName}",
                            Severity = ValidationSeverity.Error
                        });
                        result.IsValid = false;
                        continue;
                    }

                    // Validate command syntax
                    var commandValidation = command.ValidateInstruction(instruction);
                    if (!commandValidation.IsValid)
                    {
                        foreach (var error in commandValidation.Errors)
                        {
                            result.Errors.Add(new ValidationError
                            {
                                LineNumber = instruction.LineNumber,
                                CommandName = instruction.CommandName,
                                Message = error,
                                Severity = ValidationSeverity.Error
                            });
                        }
                        result.IsValid = false;
                    }

                    foreach (var warning in commandValidation.Warnings)
                    {
                        result.Warnings.Add(new ValidationWarning
                        {
                            LineNumber = instruction.LineNumber,
                            CommandName = instruction.CommandName,
                            Message = warning
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ValidationError
                    {
                        LineNumber = instruction.LineNumber,
                        CommandName = instruction.CommandName,
                        Message = $"Command validation error: {ex.Message}",
                        Severity = ValidationSeverity.Error
                    });
                    result.IsValid = false;
                }

                // Extract variable references
                ExtractVariableReferences(instruction, variables, referencedLabels);
            }

            // Check for undefined label references
            foreach (var referencedLabel in referencedLabels)
            {
                if (!labels.Contains(referencedLabel))
                {
                    result.Warnings.Add(new ValidationWarning
                    {
                        CommandName = "JUMP",
                        Message = $"Referenced label '{referencedLabel}' is not defined",
                        Suggestion = $"Define label '{referencedLabel}' or remove the reference"
                    });
                }
            }

            result.UsedVariables = variables.ToList();
            result.DefinedLabels = labels.ToList();
            result.ReferencedLabels = referencedLabels.ToList();

            _logger.LogTrace("Script validation completed for config {ConfigName}: {IsValid}, {ErrorCount} errors, {WarningCount} warnings", 
                config.Name, result.IsValid, result.Errors.Count, result.Warnings.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Script validation failed for config {ConfigName}", config.Name);
            result.IsValid = false;
            result.Errors.Add(new ValidationError
            {
                CommandName = "VALIDATION",
                Message = $"Validation error: {ex.Message}",
                Severity = ValidationSeverity.Critical
            });
            return result;
        }
    }

    public ScriptExecutionStatistics GetExecutionStatistics()
    {
        // Convert the detailed ExecutionStatistics to ScriptExecutionStatistics
        return new ScriptExecutionStatistics
        {
            TotalScriptsExecuted = _statistics.TotalScriptsExecuted,
            TotalCommandsExecuted = _statistics.TotalCommandsExecuted,
            SuccessfulExecutions = _statistics.SuccessfulExecutions,
            FailedExecutions = _statistics.FailedExecutions,
            TotalExecutionTime = _statistics.TotalExecutionTime,
            CommandExecutionCounts = _statistics.CommandCounts,
            StartTime = DateTime.UtcNow.Subtract(_statistics.TotalExecutionTime),
            LastExecutionTime = _statistics.LastHttpRequest
        };
    }

    public void ResetStatistics() 
    { 
        // TODO: Implement ExecutionStatistics.Reset method
        _statistics.HttpRequestCount = 0;
        _statistics.VariableSetCount = 0;
        _statistics.VariableGetCount = 0;
        _statistics.DataCaptureCount = 0;
        _statistics.LogEntryCount = 0;
        _statistics.TotalResponseTime = 0;
        _statistics.ErrorCount = 0;
        _statistics.WarningCount = 0;
        _statistics.CommandCounts.Clear();
    }

    private void BuildLabelMap(ScriptExecutionContext context)
    {
        context.Labels.Clear();
        for (int i = 0; i < context.Instructions.Count; i++)
        {
            var instruction = context.Instructions[i];
            if (!string.IsNullOrEmpty(instruction.Label))
            {
                context.Labels[instruction.Label] = i;
            }
        }
    }

    private async Task<bool> HandleFlowControl(ScriptExecutionContext context, CommandExecutionResult commandResult)
    {
        switch (commandResult.FlowControl)
        {
            case FlowControl.Continue:
                return true;

            case FlowControl.Stop:
                _logger.LogTrace("Script execution stopped by command on line {LineNumber}", commandResult.LineNumber);
                context.ShouldStop = true;
                return false;

            case FlowControl.Jump:
                if (!string.IsNullOrEmpty(commandResult.JumpLabel))
                {
                    if (context.JumpToLabel(commandResult.JumpLabel))
                    {
                        _logger.LogTrace("Jumped to label {Label} from line {LineNumber}", 
                            commandResult.JumpLabel, commandResult.LineNumber);
                        return true;
                    }
                    else
                    {
                        _logger.LogError("Jump to undefined label {Label} from line {LineNumber}", 
                            commandResult.JumpLabel, commandResult.LineNumber);
                        context.ShouldStop = true;
                        return false;
                    }
                }
                return true;

            case FlowControl.Break:
                _logger.LogTrace("Break encountered on line {LineNumber}", commandResult.LineNumber);
                // For now, treat break like stop - could be enhanced for block structures
                context.ShouldStop = true;
                return false;

            case FlowControl.Return:
                if (context.CallStack.Count > 0)
                {
                    var returnAddress = context.CallStack.Pop();
                    context.CurrentInstructionIndex = returnAddress;
                    _logger.LogTrace("Returned to line {LineNumber} from line {SourceLine}", 
                        returnAddress, commandResult.LineNumber);
                    return true;
                }
                else
                {
                    _logger.LogTrace("Return encountered with empty call stack on line {LineNumber}, stopping execution", 
                        commandResult.LineNumber);
                    context.ShouldStop = true;
                    return false;
                }

            case FlowControl.Retry:
                _logger.LogTrace("Retry flow control on line {LineNumber} - continuing to next instruction", 
                    commandResult.LineNumber);
                // For now, just continue - retry logic would be handled by the specific command
                return true;

            default:
                _logger.LogWarning("Unknown flow control {FlowControl} on line {LineNumber}", 
                    commandResult.FlowControl, commandResult.LineNumber);
                return true;
        }
    }

    private void ExtractVariableReferences(ScriptInstruction instruction, HashSet<string> variables, HashSet<string> labels)
    {
        // Extract variables from arguments
        foreach (var arg in instruction.Arguments)
        {
            ExtractVariablesFromString(arg, variables);
            
            // Check for label references (simple heuristic)
            if (instruction.CommandName.Equals("JUMP", StringComparison.OrdinalIgnoreCase) && !arg.Contains("<"))
            {
                labels.Add(arg);
            }
        }

        // Extract variables from parameters
        foreach (var param in instruction.Parameters)
        {
            if (param.Value is string stringValue)
            {
                ExtractVariablesFromString(stringValue, variables);
            }
        }

        // Recursively process sub-instructions
        foreach (var subInstruction in instruction.SubInstructions)
        {
            ExtractVariableReferences(subInstruction, variables, labels);
        }
    }

    private void ExtractVariablesFromString(string input, HashSet<string> variables)
    {
        if (string.IsNullOrEmpty(input))
            return;

        var regex = new System.Text.RegularExpressions.Regex(@"<([^>]+)>");
        var matches = regex.Matches(input);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var variableName = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(variableName))
            {
                variables.Add(variableName);
            }
        }
    }
}

/// <summary>
/// Extension methods for bot status
/// </summary>
public static class BotStatusExtensions
{
    /// <summary>
    /// Checks if the bot status represents an error condition
    /// </summary>
    public static bool IsError(this BotStatus status)
    {
        return status == BotStatus.Error;
    }

    /// <summary>
    /// Checks if the bot status represents a success condition
    /// </summary>
    public static bool IsSuccess(this BotStatus status)
    {
        return status == BotStatus.Success || status == BotStatus.Custom;
    }

    /// <summary>
    /// Checks if the bot status requires retry
    /// </summary>
    public static bool ShouldRetry(this BotStatus status)
    {
        return status == BotStatus.Retry || status == BotStatus.Ban;
    }
}

/// <summary>
/// Extension methods for dictionary
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Adds all items from another dictionary
    /// </summary>
    public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other) where TKey : notnull
    {
        foreach (var kvp in other)
        {
            dictionary[kvp.Key] = kvp.Value;
        }
    }
}
