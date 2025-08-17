using OpenBullet.Core.Models;

namespace OpenBullet.Core.Services;

/// <summary>
/// Interface for LoliScript syntax highlighting service
/// </summary>
public interface ISyntaxHighlightingService
{
    /// <summary>
    /// Whether the service has been initialized
    /// </summary>
    bool IsInitialized { get; }
    
    /// <summary>
    /// Supported programming languages
    /// </summary>
    string[] SupportedLanguages { get; }
    
    /// <summary>
    /// Tokenize LoliScript code into structured tokens for analysis
    /// </summary>
    List<SyntaxToken> TokenizeScript(string script);
    
    /// <summary>
    /// Generate syntax-highlighted text with color formatting
    /// </summary>
    SyntaxHighlightResult HighlightSyntax(string script);
    
    /// <summary>
    /// Get color scheme for specified theme
    /// </summary>
    ColorScheme GetColorScheme(string themeName);
    
    /// <summary>
    /// Get available color scheme names
    /// </summary>
    string[] GetAvailableThemes();
    
    /// <summary>
    /// Validate LoliScript syntax and return validation results
    /// </summary>
    SyntaxValidationResult ValidateSyntax(string script);
    
    /// <summary>
    /// Get syntax highlighting rules for a specific language
    /// </summary>
    SyntaxHighlightingRules GetHighlightingRules(string language);
}
