# ICSharpCode.AvalonEdit.dll Documentation

## Overview
ICSharpCode.AvalonEdit is a powerful WPF-based text editor control with syntax highlighting, code folding, and IntelliSense support. It's the editor component used in SharpDevelop IDE and provides advanced text editing capabilities for OpenBullet's configuration editor.

## Purpose in OpenBullet
- Provide syntax highlighting for LoliScript/LoliCode
- Enable code folding and line numbers
- Support search and replace functionality
- Provide IntelliSense/auto-completion
- Enable bracket matching and indentation
- Display error markers and tooltips

## Key Components

### Core Editor

#### `TextEditor`
- **Purpose**: Main text editor control
- **Key Properties**:
  - `Document` - Text document model
  - `Text` - Editor text content
  - `SyntaxHighlighting` - Highlighting definition
  - `Options` - Editor options
  - `FontFamily` - Editor font
  - `FontSize` - Font size
  - `ShowLineNumbers` - Line number visibility
- **Key Methods**:
  - `Load()` - Load file content
  - `Save()` - Save to file
  - `SelectAll()` - Select all text
  - `Copy/Cut/Paste()` - Clipboard operations
  - `Undo/Redo()` - History operations

```csharp
// Initialize editor
var editor = new TextEditor();
editor.FontFamily = new FontFamily("Consolas");
editor.FontSize = 14;
editor.ShowLineNumbers = true;
editor.Options.EnableHyperlinks = true;
editor.Options.EnableEmailHyperlinks = true;
```

#### `TextDocument`
- **Purpose**: Document model holding text
- **Key Properties**:
  - `Text` - Document text
  - `LineCount` - Number of lines
  - `TextLength` - Total character count
  - `Version` - Document version for change tracking
- **Key Methods**:
  - `Insert()` - Insert text at offset
  - `Remove()` - Remove text range
  - `Replace()` - Replace text range
  - `GetText()` - Get text from range
  - `GetLineByNumber()` - Get specific line

### Syntax Highlighting

#### `IHighlightingDefinition`
- **Purpose**: Define syntax highlighting rules
- **Implementation**:
```csharp
public class LoliScriptHighlighting : IHighlightingDefinition
{
    public string Name => "LoliScript";
    
    public HighlightingRuleSet MainRuleSet { get; }
    
    public LoliScriptHighlighting()
    {
        MainRuleSet = new HighlightingRuleSet();
        
        // Keywords
        var keywordColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Blue) };
        var keywords = new[] { "REQUEST", "KEYCHECK", "PARSE", "FUNCTION", "IF", "ELSE" };
        
        foreach (var keyword in keywords)
        {
            MainRuleSet.Rules.Add(new HighlightingRule
            {
                Regex = new Regex($@"\b{keyword}\b", RegexOptions.IgnoreCase),
                Color = keywordColor
            });
        }
        
        // Strings
        var stringColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Red) };
        MainRuleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"""([^""]|"""")*"""),
            Color = stringColor
        });
        
        // Comments
        var commentColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Green) };
        MainRuleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"#.*$", RegexOptions.Multiline),
            Color = commentColor
        });
    }
}
```

#### `HighlightingManager`
- **Purpose**: Manage highlighting definitions
- **Usage**:
```csharp
// Register custom highlighting
var definition = new LoliScriptHighlighting();
HighlightingManager.Instance.RegisterHighlighting("LoliScript", new[] { ".loli" }, definition);

// Apply to editor
editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("LoliScript");
```

### Code Completion

#### `CompletionWindow`
- **Purpose**: IntelliSense popup window
- **Implementation**:
```csharp
public class LoliScriptCompletion
{
    private CompletionWindow completionWindow;
    private TextEditor editor;
    
    public void ShowCompletion()
    {
        completionWindow = new CompletionWindow(editor.TextArea);
        var data = completionWindow.CompletionList.CompletionData;
        
        // Add completion items
        data.Add(new CompletionData("REQUEST", "HTTP request block"));
        data.Add(new CompletionData("KEYCHECK", "Response validation block"));
        data.Add(new CompletionData("PARSE", "Data extraction block"));
        data.Add(new CompletionData("FUNCTION", "Function call block"));
        
        completionWindow.Show();
        completionWindow.Closed += (s, e) => completionWindow = null;
    }
}

public class CompletionData : ICompletionData
{
    public string Text { get; }
    public string Description { get; }
    
    public CompletionData(string text, string description)
    {
        Text = text;
        Description = description;
    }
    
    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs e)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
    
    public ImageSource Image => null;
    public object Content => Text;
    public object Description => Description;
    public double Priority => 0;
}
```

### Code Folding

#### `FoldingManager`
- **Purpose**: Manage code folding regions
- **Implementation**:
```csharp
public class LoliScriptFoldingStrategy
{
    public void UpdateFoldings(FoldingManager manager, TextDocument document)
    {
        var foldings = new List<NewFolding>();
        
        // Find blocks to fold
        var text = document.Text;
        var blockStarts = new Stack<int>();
        
        for (int i = 0; i < document.LineCount; i++)
        {
            var line = document.GetLineByNumber(i + 1);
            var lineText = document.GetText(line.Offset, line.Length);
            
            if (lineText.TrimStart().StartsWith("BLOCK"))
            {
                blockStarts.Push(line.Offset);
            }
            else if (lineText.TrimStart().StartsWith("ENDBLOCK"))
            {
                if (blockStarts.Count > 0)
                {
                    var startOffset = blockStarts.Pop();
                    var endOffset = line.EndOffset;
                    foldings.Add(new NewFolding(startOffset, endOffset));
                }
            }
        }
        
        manager.UpdateFoldings(foldings, -1);
    }
}

// Usage
var foldingManager = FoldingManager.Install(editor.TextArea);
var foldingStrategy = new LoliScriptFoldingStrategy();
foldingStrategy.UpdateFoldings(foldingManager, editor.Document);
```

## Implementation Examples

### Custom Editor Setup
```csharp
public class ConfigEditor
{
    private TextEditor editor;
    private SearchPanel searchPanel;
    
    public ConfigEditor()
    {
        editor = new TextEditor();
        ConfigureEditor();
        SetupSearchPanel();
        SetupContextMenu();
    }
    
    private void ConfigureEditor()
    {
        // Basic settings
        editor.ShowLineNumbers = true;
        editor.FontFamily = new FontFamily("Consolas");
        editor.FontSize = 12;
        
        // Options
        editor.Options.EnableHyperlinks = true;
        editor.Options.EnableEmailHyperlinks = false;
        editor.Options.ConvertTabsToSpaces = true;
        editor.Options.IndentationSize = 2;
        editor.Options.ShowSpaces = false;
        editor.Options.ShowTabs = false;
        editor.Options.ShowEndOfLine = false;
        
        // Highlighting
        editor.SyntaxHighlighting = LoadLoliScriptHighlighting();
        
        // Events
        editor.TextChanged += OnTextChanged;
        editor.TextArea.TextEntering += OnTextEntering;
        editor.TextArea.TextEntered += OnTextEntered;
    }
    
    private void SetupSearchPanel()
    {
        searchPanel = SearchPanel.Install(editor);
        searchPanel.MarkerBrush = new SolidColorBrush(Colors.Yellow);
    }
    
    private void SetupContextMenu()
    {
        var menu = new ContextMenu();
        
        menu.Items.Add(new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut });
        menu.Items.Add(new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy });
        menu.Items.Add(new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem { Header = "Select All", Command = ApplicationCommands.SelectAll });
        
        editor.ContextMenu = menu;
    }
}
```

### Error Marking
```csharp
public class ErrorMarker
{
    private TextEditor editor;
    private TextMarkerService markerService;
    
    public ErrorMarker(TextEditor editor)
    {
        this.editor = editor;
        markerService = new TextMarkerService(editor);
    }
    
    public void MarkError(int line, int column, int length, string message)
    {
        var offset = editor.Document.GetOffset(line, column);
        var marker = markerService.Create(offset, length);
        
        marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
        marker.MarkerColor = Colors.Red;
        marker.ToolTip = message;
    }
    
    public void ClearErrors()
    {
        markerService.Clear();
    }
    
    public void HighlightLine(int lineNumber, Color color)
    {
        var line = editor.Document.GetLineByNumber(lineNumber);
        var marker = markerService.Create(line.Offset, line.Length);
        
        marker.BackgroundColor = color;
        marker.MarkerTypes = TextMarkerTypes.None;
    }
}
```

### Bracket Matching
```csharp
public class BracketHighlighter
{
    private TextEditor editor;
    private BracketHighlightRenderer renderer;
    
    public BracketHighlighter(TextEditor editor)
    {
        this.editor = editor;
        renderer = new BracketHighlightRenderer();
        editor.TextArea.TextView.BackgroundRenderers.Add(renderer);
        editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
    }
    
    private void OnCaretPositionChanged(object sender, EventArgs e)
    {
        var caretOffset = editor.TextArea.Caret.Offset;
        var document = editor.Document;
        
        if (caretOffset > 0 && caretOffset <= document.TextLength)
        {
            var currentChar = document.GetCharAt(caretOffset - 1);
            var matchingBracket = FindMatchingBracket(currentChar, caretOffset - 1);
            
            if (matchingBracket >= 0)
            {
                renderer.SetHighlight(new BracketSearchResult(
                    caretOffset - 1, 1, matchingBracket, 1));
            }
            else
            {
                renderer.SetHighlight(null);
            }
        }
    }
    
    private int FindMatchingBracket(char bracket, int offset)
    {
        // Implementation for finding matching brackets
        var pairs = new Dictionary<char, char>
        {
            { '(', ')' }, { '[', ']' }, { '{', '}' },
            { ')', '(' }, { ']', '[' }, { '}', '{' }
        };
        
        if (!pairs.ContainsKey(bracket))
            return -1;
            
        // Search logic here...
        return -1;
    }
}
```

## Integration with OpenBullet

### Config Editor Implementation
```csharp
public class OpenBulletConfigEditor : UserControl
{
    private TextEditor editor;
    private FoldingManager foldingManager;
    private ErrorMarker errorMarker;
    
    public string ConfigText
    {
        get => editor.Text;
        set => editor.Text = value;
    }
    
    public OpenBulletConfigEditor()
    {
        InitializeEditor();
        SetupLoliScriptSupport();
    }
    
    private void InitializeEditor()
    {
        editor = new TextEditor
        {
            ShowLineNumbers = true,
            FontFamily = new FontFamily("Courier New"),
            FontSize = 11,
            SyntaxHighlighting = GetLoliScriptHighlighting()
        };
        
        // Install features
        foldingManager = FoldingManager.Install(editor.TextArea);
        SearchPanel.Install(editor);
        errorMarker = new ErrorMarker(editor);
        
        // Auto-completion
        editor.TextArea.TextEntered += (s, e) =>
        {
            if (e.Text == " " && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ShowAutoComplete();
            }
        };
    }
    
    private void SetupLoliScriptSupport()
    {
        // Update foldings on text change
        editor.TextChanged += (s, e) =>
        {
            UpdateFoldings();
            ValidateSyntax();
        };
    }
    
    private void ValidateSyntax()
    {
        errorMarker.ClearErrors();
        
        var errors = LoliScriptValidator.Validate(editor.Text);
        foreach (var error in errors)
        {
            errorMarker.MarkError(error.Line, error.Column, error.Length, error.Message);
        }
    }
}
```

### Syntax Highlighting for LoliScript
```xml
<!-- LoliScript.xshd -->
<SyntaxDefinition name="LoliScript" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
    <Color name="Keyword" foreground="Blue" fontWeight="Bold" />
    <Color name="String" foreground="Red" />
    <Color name="Comment" foreground="Green" fontStyle="Italic" />
    <Color name="Variable" foreground="Purple" />
    
    <RuleSet>
        <!-- Keywords -->
        <Keywords color="Keyword">
            <Word>REQUEST</Word>
            <Word>KEYCHECK</Word>
            <Word>PARSE</Word>
            <Word>FUNCTION</Word>
            <Word>IF</Word>
            <Word>ELSE</Word>
            <Word>WHILE</Word>
        </Keywords>
        
        <!-- Strings -->
        <Span color="String">
            <Begin>"</Begin>
            <End>"</End>
            <RuleSet>
                <Span begin="\\" end="." />
            </RuleSet>
        </Span>
        
        <!-- Comments -->
        <Span color="Comment">
            <Begin>#</Begin>
        </Span>
        
        <!-- Variables -->
        <Rule color="Variable">
            &lt;[A-Z_]+&gt;
        </Rule>
    </RuleSet>
</SyntaxDefinition>
```

## Best Practices
1. Load highlighting definitions once and reuse
2. Update foldings asynchronously for large documents
3. Implement throttling for real-time validation
4. Cache completion data for performance
5. Dispose of text markers properly
6. Use virtual scrolling for large files
7. Implement proper undo/redo grouping

## Performance Optimization
- Use TextDocument.BeginUpdate/EndUpdate for bulk changes
- Implement lazy loading for large files
- Cache syntax highlighting results
- Use background threads for validation
- Limit real-time features for very large documents

## Limitations
- WPF-only (no WinForms support)
- Memory usage for very large files
- Performance with complex highlighting rules
- Limited built-in language definitions

## Dependencies
- .NET Framework 4.5+ or .NET Core 3.0+ (WPF)
- WPF (Windows Presentation Foundation)
- System.Xaml