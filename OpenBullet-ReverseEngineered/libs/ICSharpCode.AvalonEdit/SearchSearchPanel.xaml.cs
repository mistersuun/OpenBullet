// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchPanel
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public partial class SearchPanel : Control
{
  private TextArea textArea;
  private SearchInputHandler handler;
  private TextDocument currentDocument;
  private SearchResultBackgroundRenderer renderer;
  private TextBox searchTextBox;
  private SearchPanelAdorner adorner;
  public static readonly DependencyProperty UseRegexProperty = DependencyProperty.Register(nameof (UseRegex), typeof (bool), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(SearchPanel.SearchPatternChangedCallback)));
  public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register(nameof (MatchCase), typeof (bool), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(SearchPanel.SearchPatternChangedCallback)));
  public static readonly DependencyProperty WholeWordsProperty = DependencyProperty.Register(nameof (WholeWords), typeof (bool), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(SearchPanel.SearchPatternChangedCallback)));
  public static readonly DependencyProperty SearchPatternProperty = DependencyProperty.Register(nameof (SearchPattern), typeof (string), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) "", new PropertyChangedCallback(SearchPanel.SearchPatternChangedCallback)));
  public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register(nameof (MarkerBrush), typeof (Brush), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.LightGreen, new PropertyChangedCallback(SearchPanel.MarkerBrushChangedCallback)));
  public static readonly DependencyProperty LocalizationProperty = DependencyProperty.Register(nameof (Localization), typeof (Localization), typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Localization()));
  private ISearchStrategy strategy;
  private System.Windows.Controls.ToolTip messageView;

  public bool UseRegex
  {
    get => (bool) this.GetValue(SearchPanel.UseRegexProperty);
    set => this.SetValue(SearchPanel.UseRegexProperty, (object) value);
  }

  public bool MatchCase
  {
    get => (bool) this.GetValue(SearchPanel.MatchCaseProperty);
    set => this.SetValue(SearchPanel.MatchCaseProperty, (object) value);
  }

  public bool WholeWords
  {
    get => (bool) this.GetValue(SearchPanel.WholeWordsProperty);
    set => this.SetValue(SearchPanel.WholeWordsProperty, (object) value);
  }

  public string SearchPattern
  {
    get => (string) this.GetValue(SearchPanel.SearchPatternProperty);
    set => this.SetValue(SearchPanel.SearchPatternProperty, (object) value);
  }

  public Brush MarkerBrush
  {
    get => (Brush) this.GetValue(SearchPanel.MarkerBrushProperty);
    set => this.SetValue(SearchPanel.MarkerBrushProperty, (object) value);
  }

  public Localization Localization
  {
    get => (Localization) this.GetValue(SearchPanel.LocalizationProperty);
    set => this.SetValue(SearchPanel.LocalizationProperty, (object) value);
  }

  private static void MarkerBrushChangedCallback(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is SearchPanel searchPanel))
      return;
    searchPanel.renderer.MarkerBrush = (Brush) e.NewValue;
  }

  static SearchPanel()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SearchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SearchPanel)));
  }

  private static void SearchPatternChangedCallback(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is SearchPanel searchPanel))
      return;
    searchPanel.ValidateSearchText();
    searchPanel.UpdateSearch();
  }

  private void UpdateSearch()
  {
    if (this.renderer.CurrentResults.Any<SearchResult>())
      this.messageView.IsOpen = false;
    this.strategy = SearchStrategyFactory.Create(this.SearchPattern ?? "", !this.MatchCase, this.WholeWords, this.UseRegex ? SearchMode.RegEx : SearchMode.Normal);
    this.OnSearchOptionsChanged(new SearchOptionsChangedEventArgs(this.SearchPattern, this.MatchCase, this.UseRegex, this.WholeWords));
    this.DoSearch(true);
  }

  private SearchPanel()
  {
    System.Windows.Controls.ToolTip toolTip = new System.Windows.Controls.ToolTip();
    toolTip.Placement = PlacementMode.Bottom;
    toolTip.StaysOpen = true;
    toolTip.Focusable = false;
    this.messageView = toolTip;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use the Install method instead")]
  public void Attach(TextArea textArea)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    this.AttachInternal(textArea);
  }

  public static SearchPanel Install(ICSharpCode.AvalonEdit.TextEditor editor)
  {
    return editor != null ? SearchPanel.Install(editor.TextArea) : throw new ArgumentNullException(nameof (editor));
  }

  public static SearchPanel Install(TextArea textArea)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    SearchPanel panel = new SearchPanel();
    panel.AttachInternal(textArea);
    panel.handler = new SearchInputHandler(textArea, panel);
    textArea.DefaultInputHandler.NestedInputHandlers.Add((ITextAreaInputHandler) panel.handler);
    return panel;
  }

  public void RegisterCommands(CommandBindingCollection commandBindings)
  {
    this.handler.RegisterGlobalCommands(commandBindings);
  }

  public void Uninstall()
  {
    this.CloseAndRemove();
    this.textArea.DefaultInputHandler.NestedInputHandlers.Remove((ITextAreaInputHandler) this.handler);
  }

  private void AttachInternal(TextArea textArea)
  {
    this.textArea = textArea;
    this.adorner = new SearchPanelAdorner(textArea, this);
    this.DataContext = (object) this;
    this.renderer = new SearchResultBackgroundRenderer();
    this.currentDocument = textArea.Document;
    if (this.currentDocument != null)
      this.currentDocument.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
    textArea.DocumentChanged += new EventHandler(this.textArea_DocumentChanged);
    this.KeyDown += new KeyEventHandler(this.SearchLayerKeyDown);
    this.CommandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindNext, (ExecutedRoutedEventHandler) ((sender, e) => this.FindNext())));
    this.CommandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindPrevious, (ExecutedRoutedEventHandler) ((sender, e) => this.FindPrevious())));
    this.CommandBindings.Add(new CommandBinding((ICommand) SearchCommands.CloseSearchPanel, (ExecutedRoutedEventHandler) ((sender, e) => this.Close())));
    this.IsClosed = true;
  }

  private void textArea_DocumentChanged(object sender, EventArgs e)
  {
    if (this.currentDocument != null)
      this.currentDocument.TextChanged -= new EventHandler(this.textArea_Document_TextChanged);
    this.currentDocument = this.textArea.Document;
    if (this.currentDocument == null)
      return;
    this.currentDocument.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
    this.DoSearch(false);
  }

  private void textArea_Document_TextChanged(object sender, EventArgs e) => this.DoSearch(false);

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.searchTextBox = this.Template.FindName("PART_searchTextBox", (FrameworkElement) this) as TextBox;
  }

  private void ValidateSearchText()
  {
    if (this.searchTextBox == null)
      return;
    BindingExpression bindingExpression = this.searchTextBox.GetBindingExpression(TextBox.TextProperty);
    try
    {
      Validation.ClearInvalid((BindingExpressionBase) bindingExpression);
      this.UpdateSearch();
    }
    catch (SearchPatternException ex)
    {
      ValidationError validationError = new ValidationError(bindingExpression.ParentBinding.ValidationRules[0], (object) bindingExpression, (object) ex.Message, (Exception) ex);
      Validation.MarkInvalid((BindingExpressionBase) bindingExpression, validationError);
    }
  }

  public void Reactivate()
  {
    if (this.searchTextBox == null)
      return;
    this.searchTextBox.Focus();
    this.searchTextBox.SelectAll();
  }

  public void FindNext()
  {
    SearchResult result = this.renderer.CurrentResults.FindFirstSegmentWithStartAfter(this.textArea.Caret.Offset + 1) ?? this.renderer.CurrentResults.FirstSegment;
    if (result == null)
      return;
    this.SelectResult(result);
  }

  public void FindPrevious()
  {
    SearchResult searchResult = this.renderer.CurrentResults.FindFirstSegmentWithStartAfter(this.textArea.Caret.Offset);
    if (searchResult != null)
      searchResult = this.renderer.CurrentResults.GetPreviousSegment(searchResult);
    if (searchResult == null)
      searchResult = this.renderer.CurrentResults.LastSegment;
    if (searchResult == null)
      return;
    this.SelectResult(searchResult);
  }

  private void DoSearch(bool changeSelection)
  {
    if (this.IsClosed)
      return;
    this.renderer.CurrentResults.Clear();
    if (!string.IsNullOrEmpty(this.SearchPattern))
    {
      int offset = this.textArea.Caret.Offset;
      if (changeSelection)
        this.textArea.ClearSelection();
      foreach (SearchResult result in this.strategy.FindAll((ITextSource) this.textArea.Document, 0, this.textArea.Document.TextLength))
      {
        if (changeSelection && result.StartOffset >= offset)
        {
          this.SelectResult(result);
          changeSelection = false;
        }
        this.renderer.CurrentResults.Add(result);
      }
      if (!this.renderer.CurrentResults.Any<SearchResult>())
      {
        this.messageView.IsOpen = true;
        this.messageView.Content = (object) this.Localization.NoMatchesFoundText;
        this.messageView.PlacementTarget = (UIElement) this.searchTextBox;
      }
      else
        this.messageView.IsOpen = false;
    }
    this.textArea.TextView.InvalidateLayer(KnownLayer.Selection);
  }

  private void SelectResult(SearchResult result)
  {
    this.textArea.Caret.Offset = result.StartOffset;
    this.textArea.Selection = Selection.Create(this.textArea, result.StartOffset, result.EndOffset);
    this.textArea.Caret.BringCaretToView();
    this.textArea.Caret.Show();
  }

  private void SearchLayerKeyDown(object sender, KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Return:
        e.Handled = true;
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
          this.FindPrevious();
        else
          this.FindNext();
        if (this.searchTextBox == null)
          break;
        ValidationError validationError = Validation.GetErrors((DependencyObject) this.searchTextBox).FirstOrDefault<ValidationError>();
        if (validationError == null)
          break;
        this.messageView.Content = (object) $"{this.Localization.ErrorText} {validationError.ErrorContent}";
        this.messageView.PlacementTarget = (UIElement) this.searchTextBox;
        this.messageView.IsOpen = true;
        break;
      case Key.Escape:
        e.Handled = true;
        this.Close();
        break;
    }
  }

  public bool IsClosed { get; private set; }

  public void Close()
  {
    bool keyboardFocusWithin = this.IsKeyboardFocusWithin;
    AdornerLayer.GetAdornerLayer((Visual) this.textArea)?.Remove((Adorner) this.adorner);
    this.messageView.IsOpen = false;
    this.textArea.TextView.BackgroundRenderers.Remove((IBackgroundRenderer) this.renderer);
    if (keyboardFocusWithin)
      this.textArea.Focus();
    this.IsClosed = true;
    this.renderer.CurrentResults.Clear();
  }

  [Obsolete("Use the Uninstall method instead!")]
  public void CloseAndRemove()
  {
    this.Close();
    this.textArea.DocumentChanged -= new EventHandler(this.textArea_DocumentChanged);
    if (this.currentDocument == null)
      return;
    this.currentDocument.TextChanged -= new EventHandler(this.textArea_Document_TextChanged);
  }

  public void Open()
  {
    if (!this.IsClosed)
      return;
    AdornerLayer.GetAdornerLayer((Visual) this.textArea)?.Add((Adorner) this.adorner);
    this.textArea.TextView.BackgroundRenderers.Add((IBackgroundRenderer) this.renderer);
    this.IsClosed = false;
    this.DoSearch(false);
  }

  public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged;

  protected virtual void OnSearchOptionsChanged(SearchOptionsChangedEventArgs e)
  {
    if (this.SearchOptionsChanged == null)
      return;
    this.SearchOptionsChanged((object) this, e);
  }
}
