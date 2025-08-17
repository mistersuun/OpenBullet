using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Extensions.DependencyInjection;
using OpenBullet.Core.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace OpenBullet.UI.Controls;

/// <summary>
/// Professional LoliScript editor with syntax highlighting and advanced features
/// </summary>
public partial class LoliScriptEditor : UserControl
{
    private readonly ISyntaxHighlightingService _syntaxService;
    private readonly AutoCompletionProvider _autoCompletionProvider;
    private readonly DispatcherTimer _validationTimer;
    private SearchPanel? _searchPanel;
    private bool _isTextChanging;
    private CompletionWindow? _completionWindow;

    public static readonly DependencyProperty ScriptTextProperty =
        DependencyProperty.Register(nameof(ScriptText), typeof(string), typeof(LoliScriptEditor),
            new PropertyMetadata(string.Empty, OnScriptTextChanged));

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LoliScriptEditor),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

    public static readonly DependencyProperty ShowLineNumbersProperty =
        DependencyProperty.Register(nameof(ShowLineNumbers), typeof(bool), typeof(LoliScriptEditor),
            new PropertyMetadata(true, OnShowLineNumbersChanged));

    /// <summary>
    /// Gets or sets the script text content
    /// </summary>
    public string ScriptText
    {
        get => (string)GetValue(ScriptTextProperty);
        set => SetValue(ScriptTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the editor is read-only
    /// </summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets whether line numbers are shown
    /// </summary>
    public bool ShowLineNumbers
    {
        get => (bool)GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    /// <summary>
    /// Event fired when the script is saved
    /// </summary>
    public event EventHandler<string>? ScriptSaved;

    /// <summary>
    /// Event fired when the script text changes
    /// </summary>
    public event EventHandler<string>? ScriptTextChanged;

    /// <summary>
    /// Event fired when syntax validation is complete
    /// </summary>
    public event EventHandler<bool>? ValidationCompleted;

    public LoliScriptEditor()
    {
        InitializeComponent();
        
        // Get services from DI container
        _syntaxService = App.ServiceProvider?.GetService<ISyntaxHighlightingService>() 
                         ?? throw new InvalidOperationException("SyntaxHighlightingService not registered");
        
        _autoCompletionProvider = App.ServiceProvider?.GetService<AutoCompletionProvider>() 
                                  ?? new AutoCompletionProvider();

        // Initialize validation timer
        _validationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1) // Validate 1 second after user stops typing
        };
        _validationTimer.Tick += ValidationTimer_Tick;

        Loaded += LoliScriptEditor_Loaded;
    }

    private void LoliScriptEditor_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
        InitializeEditor();
            SetupSyntaxHighlighting();
        SetupSearchPanel();
            SetupAutoCompletion();
            UpdateStatusBar();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error initializing editor: {ex.Message}";
        }
    }

    private void InitializeEditor()
    {
        // Configure editor settings
        ScriptEditor.Options.EnableEmailHyperlinks = false;
        ScriptEditor.Options.EnableHyperlinks = false;
        ScriptEditor.Options.HighlightCurrentLine = true;
        ScriptEditor.Options.ShowEndOfLine = false;
        ScriptEditor.Options.ShowSpaces = false;
        ScriptEditor.Options.ShowTabs = false;
        ScriptEditor.Options.ConvertTabsToSpaces = true;
        ScriptEditor.Options.IndentationSize = 4;
        ScriptEditor.TextArea.TextView.LinkTextForegroundBrush = Brushes.Blue;
        
        // Set initial theme
        ThemeComboBox.SelectedIndex = 0; // Dark theme by default
        ApplyTheme("Dark");
    }

    private void SetupSyntaxHighlighting()
    {
        try
        {
            // Create custom LoliScript highlighting definition
            var highlightingDefinition = CreateLoliScriptHighlighting();
            ScriptEditor.SyntaxHighlighting = highlightingDefinition;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Syntax highlighting setup failed: {ex.Message}";
        }
    }

    private IHighlightingDefinition CreateLoliScriptHighlighting()
    {
        var rules = _syntaxService.GetHighlightingRules("LoliScript");
        var scheme = _syntaxService.GetColorScheme("Dark");

        // Create XML for AvalonEdit highlighting definition
        var highlightingXml = $@"<?xml version='1.0'?>
<SyntaxDefinition name='LoliScript' xmlns='http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008'>
    <Color name='Command' foreground='{scheme.CommandColor}' fontWeight='bold' />
    <Color name='String' foreground='{scheme.StringColor}' />
    <Color name='Comment' foreground='{scheme.CommentColor}' fontStyle='italic' />
    <Color name='Variable' foreground='{scheme.VariableColor}' />
    <Color name='Number' foreground='{scheme.NumberColor}' />
    <Color name='Label' foreground='{scheme.LabelColor}' fontWeight='bold' />
    
    <RuleSet>
        <Span color='Comment' begin='#' />
        <Span color='Comment' begin='//' />
        <Span color='String' begin='&quot;' end='&quot;' />
        <Span color='String' begin=""'"" end=""'"" />
        
        <Keywords color='Command'>
            {string.Join("\n            ", rules.Keywords.Select(k => $"<Word>{k}</Word>"))}
        </Keywords>
        
        <Rule color='Variable'>
            &lt;[^&gt;]+&gt;|\$[a-zA-Z_][a-zA-Z0-9_]*
        </Rule>
        
        <Rule color='Number'>
            \b\d+(\.\d+)?\b
        </Rule>
        
        <Rule color='Label'>
            ^[a-zA-Z_][a-zA-Z0-9_]*:
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

        using var reader = new StringReader(highlightingXml);
        using var xmlReader = XmlReader.Create(reader);
        return HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
    }

    private void SetupSearchPanel()
    {
        _searchPanel = SearchPanel.Install(ScriptEditor);
    }

    private void SetupAutoCompletion()
    {
        try
        {
            // Add key handlers for auto-completion
            ScriptEditor.TextArea.KeyDown += OnKeyDown;
            ScriptEditor.TextArea.TextEntered += OnTextEntered;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Auto-completion setup failed: {ex.Message}";
        }
    }

    private static void OnScriptTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoliScriptEditor editor && !editor._isTextChanging)
        {
            editor._isTextChanging = true;
            editor.ScriptEditor.Text = e.NewValue?.ToString() ?? string.Empty;
            editor._isTextChanging = false;
        }
    }

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoliScriptEditor editor)
        {
            editor.ScriptEditor.IsReadOnly = (bool)e.NewValue;
        }
    }

    private static void OnShowLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoliScriptEditor editor)
        {
            editor.ScriptEditor.ShowLineNumbers = (bool)e.NewValue;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ScriptSaved?.Invoke(this, ScriptEditor.Text);
            StatusText.Text = "Script saved successfully";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Save failed: {ex.Message}";
        }
    }

    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        if (ScriptEditor.CanUndo)
        {
            ScriptEditor.Undo();
            StatusText.Text = "Undo completed";
        }
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        if (ScriptEditor.CanRedo)
        {
            ScriptEditor.Redo();
            StatusText.Text = "Redo completed";
        }
    }

    private void FindButton_Click(object sender, RoutedEventArgs e)
    {
        _searchPanel?.Open();
        StatusText.Text = "Search panel opened";
    }

    private void ValidateButton_Click(object sender, RoutedEventArgs e)
    {
        ValidateSyntaxNow();
    }

    private void FormatButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            FormatScript();
            StatusText.Text = "Script formatted successfully";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Format failed: {ex.Message}";
        }
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem item && item.Tag is string theme)
        {
            ApplyTheme(theme);
        }
    }

    private void ScriptEditor_TextChanged(object sender, EventArgs e)
    {
        if (!_isTextChanging)
        {
            _isTextChanging = true;
            ScriptText = ScriptEditor.Text;
            _isTextChanging = false;

            ScriptTextChanged?.Invoke(this, ScriptEditor.Text);
            
            // Restart validation timer
            _validationTimer.Stop();
            _validationTimer.Start();
            
            UpdateStatusBar();
        }
    }

    private void ScriptEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle keyboard shortcuts
        if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
            SaveButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
            FindButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        else if (e.Key == Key.F5)
            {
            ValidateButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }

        UpdateStatusBar();
    }

    private void ValidationTimer_Tick(object? sender, EventArgs e)
    {
        _validationTimer.Stop();
        ValidateSyntaxInBackground();
    }

    private async void ValidateSyntaxInBackground()
    {
        try
        {
            var validationResult = await Task.Run(() => _syntaxService.ValidateSyntax(ScriptEditor.Text));
            
            await Dispatcher.InvokeAsync(() =>
            {
                if (validationResult.IsValid)
                {
                    StatusText.Text = "Syntax validation passed";
                    StatusText.Foreground = Brushes.Green;
                }
                else
                {
                    var errorCount = validationResult.Errors.Count;
                    StatusText.Text = $"Syntax validation failed: {errorCount} error(s)";
                    StatusText.Foreground = Brushes.Red;
                }

                ValidationCompleted?.Invoke(this, validationResult.IsValid);
            });
        }
        catch (Exception ex)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                StatusText.Text = $"Validation error: {ex.Message}";
                StatusText.Foreground = Brushes.Red;
            });
        }
    }

    private void ValidateSyntaxNow()
    {
        try
        {
            var validationResult = _syntaxService.ValidateSyntax(ScriptEditor.Text);
            
            if (validationResult.IsValid)
            {
                StatusText.Text = "✓ Syntax is valid";
                StatusText.Foreground = Brushes.Green;
            }
            else
            {
                var firstError = validationResult.Errors.FirstOrDefault();
                if (firstError != null)
                {
                    StatusText.Text = $"✗ Line {firstError.LineNumber}: {firstError.Message}";
                    StatusText.Foreground = Brushes.Red;
                    
                    // Navigate to first error
                    if (firstError.LineNumber > 0 && firstError.LineNumber <= ScriptEditor.Document.LineCount)
                    {
                        var line = ScriptEditor.Document.GetLineByNumber(firstError.LineNumber);
                        ScriptEditor.CaretOffset = line.Offset + Math.Max(0, firstError.Column - 1);
                        ScriptEditor.ScrollToLine(firstError.LineNumber);
                    }
                }
                else
                {
                    StatusText.Text = "✗ Syntax validation failed";
                    StatusText.Foreground = Brushes.Red;
                }
            }

            ValidationCompleted?.Invoke(this, validationResult.IsValid);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Validation error: {ex.Message}";
            StatusText.Foreground = Brushes.Red;
        }
    }

    private void FormatScript()
    {
        var lines = ScriptEditor.Text.Split('\n');
            var formattedLines = new List<string>();
        int indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
            {
                formattedLines.Add(string.Empty);
                continue;
            }

            // Decrease indent for end keywords
            if (IsEndKeyword(trimmedLine))
                indentLevel = Math.Max(0, indentLevel - 1);

            // Add indentation
            var indentedLine = new string(' ', indentLevel * 4) + trimmedLine;
            formattedLines.Add(indentedLine);

            // Increase indent for start keywords
            if (IsStartKeyword(trimmedLine))
                    indentLevel++;
                }

        ScriptEditor.Text = string.Join("\n", formattedLines);
    }

    private bool IsStartKeyword(string line)
    {
        var firstWord = line.Split(' ')[0].ToUpper();
        return firstWord is "IF" or "WHILE" or "FOR" or "TRY" or "FUNCTION" or "BLOCK";
    }

    private bool IsEndKeyword(string line)
    {
        var firstWord = line.Split(' ')[0].ToUpper();
        return firstWord is "ENDIF" or "ENDWHILE" or "ENDFOR" or "ENDTRY" or "RETURN" or "ENDBLOCK";
    }

    private void ApplyTheme(string themeName)
    {
        try
        {
            var scheme = _syntaxService.GetColorScheme(themeName);
            
            // Update syntax highlighting
            SetupSyntaxHighlighting();
            
            // Update editor appearance based on theme
            switch (themeName)
            {
                case "Dark":
                    ScriptEditor.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    ScriptEditor.Foreground = new SolidColorBrush(Color.FromRgb(212, 212, 212));
                    break;
                case "Light":
                    ScriptEditor.Background = new SolidColorBrush(Colors.White);
                    ScriptEditor.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case "HighContrast":
                    ScriptEditor.Background = new SolidColorBrush(Colors.Black);
                    ScriptEditor.Foreground = new SolidColorBrush(Colors.White);
                    break;
            }

            StatusText.Text = $"Applied {themeName} theme";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Theme application failed: {ex.Message}";
        }
    }

    private void UpdateStatusBar()
    {
        try
        {
            var caretPosition = ScriptEditor.CaretOffset;
            var document = ScriptEditor.Document;
            var location = document.GetLocation(caretPosition);
            
            LineInfoText.Text = $"Line: {location.Line}";
            CharInfoText.Text = $"Col: {location.Column} | {document.TextLength} chars";
        }
        catch
        {
            LineInfoText.Text = "Line: -";
            CharInfoText.Text = "Col: - | 0 chars";
        }
    }

    /// <summary>
    /// Gets the current caret position
    /// </summary>
    public TextLocation GetCaretPosition()
    {
        return ScriptEditor.Document.GetLocation(ScriptEditor.CaretOffset);
    }

    /// <summary>
    /// Sets the caret position
    /// </summary>
    public void SetCaretPosition(int line, int column)
    {
        if (line > 0 && line <= ScriptEditor.Document.LineCount)
        {
            var documentLine = ScriptEditor.Document.GetLineByNumber(line);
            var offset = documentLine.Offset + Math.Max(0, Math.Min(column - 1, documentLine.Length));
            ScriptEditor.CaretOffset = offset;
            ScriptEditor.ScrollToLine(line);
        }
    }

    /// <summary>
    /// Inserts text at the current caret position
    /// </summary>
    public void InsertText(string text)
    {
        ScriptEditor.Document.Insert(ScriptEditor.CaretOffset, text);
    }

    /// <summary>
    /// Gets the currently selected text
    /// </summary>
    public string GetSelectedText()
    {
        return ScriptEditor.SelectedText;
    }

    /// <summary>
    /// Replaces the currently selected text
    /// </summary>
    public void ReplaceSelectedText(string newText)
    {
        ScriptEditor.SelectedText = newText;
    }

    #region Auto-Completion Event Handlers

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        // Close existing completion window if ESC is pressed
        if (e.Key == Key.Escape)
        {
            if (_completionWindow != null)
            {
                _completionWindow.Close();
                e.Handled = true;
            }
        }
        // Open completion window on Ctrl+Space
        else if (e.Key == Key.Space && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            ShowCompletionWindow();
            e.Handled = true;
        }
    }

    private void OnTextEntered(object sender, TextCompositionEventArgs e)
    {
        // Auto-trigger completion on certain characters
        if (e.Text?.Length == 1)
        {
            var character = e.Text[0];
            
            // Trigger completion on space after certain patterns
            if (character == ' ' && ShouldTriggerCompletion())
            {
                ShowCompletionWindow();
            }
            // Trigger completion when typing letters (for command suggestions)
            else if (char.IsLetter(character) && GetCurrentWord().Length >= 2)
            {
                ShowCompletionWindow();
            }
            // Trigger completion on '<' for variable suggestions
            else if (character == '<')
            {
                ShowCompletionWindow();
            }
            // Trigger completion on '"' for value suggestions
            else if (character == '"' && ShouldTriggerValueCompletion())
            {
                ShowCompletionWindow();
            }
        }
    }

    private bool ShouldTriggerCompletion()
    {
        var line = GetCurrentLine();
        var trimmedLine = line.Trim();
        
        // Trigger completion at the beginning of a line or after certain keywords
        return string.IsNullOrEmpty(trimmedLine) || 
               trimmedLine.EndsWith("->") || 
               trimmedLine.EndsWith("THEN") ||
               trimmedLine.EndsWith("ELSE");
    }

    private void ShowCompletionWindow()
    {
        try
        {
            // Close existing completion window
            if (_completionWindow != null)
            {
                _completionWindow.Close();
                _completionWindow = null;
            }

            // Get completion context
            var context = CreateCompletionContext();
            
            // Get completions from our provider
            var completions = _autoCompletionProvider.GetCompletions(context);
            
            if (completions.Any())
            {
                // Create completion window
                _completionWindow = new CompletionWindow(ScriptEditor.TextArea);
                
                // Sort completions by priority (highest first) then alphabetically
                var sortedCompletions = completions
                    .OrderByDescending(c => c.Priority)
                    .ThenBy(c => c.Text)
                    .ToList();
                
                // Add completion items
                foreach (var completion in sortedCompletions)
                {
                    _completionWindow.CompletionList.CompletionData.Add(
                        new LoliScriptCompletionData(completion));
                }
                
                // Configure window appearance
                _completionWindow.Width = 450;
                _completionWindow.Height = Math.Min(350, completions.Count * 22 + 50);
                _completionWindow.CloseWhenCaretAtBeginning = true;
                _completionWindow.Closed += (s, e) => _completionWindow = null;
                
                // Show window
                _completionWindow.Show();
                
                // Auto-select best match
                SelectBestCompletionMatch();
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Auto-completion error: {ex.Message}";
        }
    }

    private CompletionContext CreateCompletionContext()
    {
        var document = ScriptEditor.Document;
        var caretOffset = ScriptEditor.CaretOffset;
        var line = document.GetLineByOffset(caretOffset);
        
        var lineText = document.GetText(line.Offset, line.Length);
        var position = caretOffset - line.Offset;
        
        // Extract available variables from the script
        var availableVariables = ExtractVariables(document.Text);
        
        return new CompletionContext
        {
            Text = lineText,
            Position = position,
            LineNumber = line.LineNumber,
            CurrentCommand = ExtractCurrentCommand(lineText),
            AvailableVariables = availableVariables
        };
    }

    private string GetCurrentLine()
    {
        var line = ScriptEditor.Document.GetLineByOffset(ScriptEditor.CaretOffset);
        return ScriptEditor.Document.GetText(line.Offset, line.Length);
    }

    private string GetCurrentWord()
    {
        var document = ScriptEditor.Document;
        var caretOffset = ScriptEditor.CaretOffset;
        
        var start = caretOffset;
        var end = caretOffset;
        
        // Find word boundaries
        while (start > 0 && char.IsLetterOrDigit(document.GetCharAt(start - 1)))
            start--;
        
        while (end < document.TextLength && char.IsLetterOrDigit(document.GetCharAt(end)))
            end++;
        
        return document.GetText(start, end - start);
    }

    private string? ExtractCurrentCommand(string lineText)
    {
        var trimmed = lineText.Trim();
        var spaceIndex = trimmed.IndexOf(' ');
        return spaceIndex > 0 ? trimmed.Substring(0, spaceIndex) : trimmed;
    }

    private string[] ExtractVariables(string scriptText)
    {
        // Simple regex to extract variable names from the script
        var variables = new HashSet<string>();
        var lines = scriptText.Split('\n');
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Look for variable assignments (-> VAR)
            var arrowIndex = trimmedLine.IndexOf("->");
            if (arrowIndex >= 0)
            {
                var afterArrow = trimmedLine.Substring(arrowIndex + 2).Trim();
                var spaceIndex = afterArrow.IndexOf(' ');
                var variableName = spaceIndex > 0 ? afterArrow.Substring(0, spaceIndex) : afterArrow;
                
                if (!string.IsNullOrEmpty(variableName) && char.IsLetter(variableName[0]))
                {
                    variables.Add(variableName);
                }
            }
        }
        
        return variables.ToArray();
    }

    private void SelectBestCompletionMatch()
    {
        if (_completionWindow?.CompletionList.CompletionData.Count > 0)
        {
            var currentWord = GetCurrentWord();
            if (!string.IsNullOrEmpty(currentWord))
            {
                // Find exact match first
                var exactMatch = _completionWindow.CompletionList.CompletionData
                    .FirstOrDefault(c => c.Text.Equals(currentWord, StringComparison.OrdinalIgnoreCase));
                
                if (exactMatch != null)
                {
                    _completionWindow.CompletionList.SelectedItem = exactMatch;
                    return;
                }
                
                // Find best prefix match
                var prefixMatch = _completionWindow.CompletionList.CompletionData
                    .FirstOrDefault(c => c.Text.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase));
                
                if (prefixMatch != null)
                {
                    _completionWindow.CompletionList.SelectedItem = prefixMatch;
                    return;
                }
            }
            
            // Default to first item
            _completionWindow.CompletionList.SelectedItem = _completionWindow.CompletionList.CompletionData[0];
        }
    }

    private bool ShouldTriggerValueCompletion()
    {
        var line = GetCurrentLine();
        var position = ScriptEditor.CaretOffset - ScriptEditor.Document.GetLineByOffset(ScriptEditor.CaretOffset).Offset;
        
        // Check if we're inside a command parameter context
        var beforeCursor = line.Substring(0, Math.Min(position, line.Length));
        
        // Trigger value completion after certain patterns
        return beforeCursor.Contains("REQUEST ") ||
               beforeCursor.Contains("PARSE ") ||
               beforeCursor.Contains("KEYCHECK ") ||
               beforeCursor.Contains("-> ");
    }

    #endregion
}

/// <summary>
/// AvalonEdit completion data adapter for LoliScript completions
/// </summary>
public class LoliScriptCompletionData : ICompletionData
{
    private readonly CompletionItem _completionItem;

    public LoliScriptCompletionData(CompletionItem completionItem)
    {
        _completionItem = completionItem;
        Text = completionItem.Text;
        Description = completionItem.Description;
        Priority = _completionItem.Priority;
    }

    public string Text { get; }
    public object Content => Text;
    public object Description { get; }
    public ImageSource? Image => null;
    public double Priority { get; }

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        var insertText = _completionItem.InsertText ?? _completionItem.Text;
        
        // Replace the completion segment with our text
        textArea.Document.Replace(completionSegment, insertText);
        
        // Position cursor appropriately
        if (insertText.Contains('(') && insertText.EndsWith(')'))
        {
            // Position cursor between parentheses for function calls
            var openParenIndex = insertText.IndexOf('(');
            var newOffset = completionSegment.Offset + openParenIndex + 1;
            if (newOffset < textArea.Document.TextLength)
            {
                textArea.Caret.Offset = newOffset;
            }
        }
    }
}