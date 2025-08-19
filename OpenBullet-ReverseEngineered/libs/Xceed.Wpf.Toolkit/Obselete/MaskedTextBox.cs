// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Obselete.MaskedTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit.Obselete;

[Obsolete("Legacy implementation of MaskedTextBox. Use Xceed.Wpf.Toolkit.MaskedTextBox instead.", true)]
public class MaskedTextBox : TextBox
{
  private bool _isSyncingTextAndValueProperties;
  private bool _isInitialized;
  private bool _convertExceptionOccurred;
  public static readonly DependencyProperty IncludePromptProperty = DependencyProperty.Register(nameof (IncludePrompt), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(MaskedTextBox.OnIncludePromptPropertyChanged)));
  public static readonly DependencyProperty IncludeLiteralsProperty = DependencyProperty.Register(nameof (IncludeLiterals), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.OnIncludeLiteralsPropertyChanged)));
  public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(nameof (Mask), typeof (string), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) "<>", new PropertyChangedCallback(MaskedTextBox.OnMaskPropertyChanged)));
  public static readonly DependencyProperty PromptCharProperty = DependencyProperty.Register(nameof (PromptChar), typeof (char), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) '_', new PropertyChangedCallback(MaskedTextBox.OnPromptCharChanged)));
  public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register(nameof (SelectAllOnGotFocus), typeof (bool), typeof (MaskedTextBox), new PropertyMetadata((object) false));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (object), typeof (MaskedTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(MaskedTextBox.OnValueChanged)));
  public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register(nameof (ValueType), typeof (Type), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) typeof (string), new PropertyChangedCallback(MaskedTextBox.OnValueTypeChanged)));
  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (MaskedTextBox));

  protected MaskedTextProvider MaskProvider { get; set; }

  public bool IncludePrompt
  {
    get => (bool) this.GetValue(MaskedTextBox.IncludePromptProperty);
    set => this.SetValue(MaskedTextBox.IncludePromptProperty, (object) value);
  }

  private static void OnIncludePromptPropertyChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnIncludePromptChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIncludePromptChanged(bool oldValue, bool newValue)
  {
    this.UpdateMaskProvider(this.Mask);
  }

  public bool IncludeLiterals
  {
    get => (bool) this.GetValue(MaskedTextBox.IncludeLiteralsProperty);
    set => this.SetValue(MaskedTextBox.IncludeLiteralsProperty, (object) value);
  }

  private static void OnIncludeLiteralsPropertyChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnIncludeLiteralsChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIncludeLiteralsChanged(bool oldValue, bool newValue)
  {
    this.UpdateMaskProvider(this.Mask);
  }

  public string Mask
  {
    get => (string) this.GetValue(MaskedTextBox.MaskProperty);
    set => this.SetValue(MaskedTextBox.MaskProperty, (object) value);
  }

  private static void OnMaskPropertyChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnMaskChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnMaskChanged(string oldValue, string newValue)
  {
    this.UpdateMaskProvider(newValue);
    this.UpdateText(0);
  }

  public char PromptChar
  {
    get => (char) this.GetValue(MaskedTextBox.PromptCharProperty);
    set => this.SetValue(MaskedTextBox.PromptCharProperty, (object) value);
  }

  private static void OnPromptCharChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnPromptCharChanged((char) e.OldValue, (char) e.NewValue);
  }

  protected virtual void OnPromptCharChanged(char oldValue, char newValue)
  {
    this.UpdateMaskProvider(this.Mask);
  }

  public bool SelectAllOnGotFocus
  {
    get => (bool) this.GetValue(MaskedTextBox.SelectAllOnGotFocusProperty);
    set => this.SetValue(MaskedTextBox.SelectAllOnGotFocusProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnTextChanged(string oldValue, string newValue)
  {
    if (!this._isInitialized)
      return;
    this.SyncTextAndValueProperties(TextBox.TextProperty, (object) newValue);
  }

  public object Value
  {
    get => this.GetValue(MaskedTextBox.ValueProperty);
    set => this.SetValue(MaskedTextBox.ValueProperty, value);
  }

  private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnValueChanged(e.OldValue, e.NewValue);
  }

  protected virtual void OnValueChanged(object oldValue, object newValue)
  {
    if (this._isInitialized)
      this.SyncTextAndValueProperties(MaskedTextBox.ValueProperty, newValue);
    RoutedPropertyChangedEventArgs<object> e = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue);
    e.RoutedEvent = MaskedTextBox.ValueChangedEvent;
    this.RaiseEvent((RoutedEventArgs) e);
  }

  public Type ValueType
  {
    get => (Type) this.GetValue(MaskedTextBox.ValueTypeProperty);
    set => this.SetValue(MaskedTextBox.ValueTypeProperty, (object) value);
  }

  private static void OnValueTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MaskedTextBox maskedTextBox))
      return;
    maskedTextBox.OnValueTypeChanged((Type) e.OldValue, (Type) e.NewValue);
  }

  protected virtual void OnValueTypeChanged(Type oldValue, Type newValue)
  {
    if (!this._isInitialized)
      return;
    this.SyncTextAndValueProperties(TextBox.TextProperty, (object) this.Text);
  }

  static MaskedTextBox()
  {
    TextBox.TextProperty.OverrideMetadata(typeof (MaskedTextBox), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MaskedTextBox.OnTextChanged)));
  }

  public MaskedTextBox()
  {
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Paste, new ExecutedRoutedEventHandler(this.Paste)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Cut, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanCut)));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.UpdateMaskProvider(this.Mask);
    this.UpdateText(0);
  }

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    if (this._isInitialized)
      return;
    this._isInitialized = true;
    this.SyncTextAndValueProperties(MaskedTextBox.ValueProperty, this.Value);
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    if (this.SelectAllOnGotFocus)
      this.SelectAll();
    base.OnGotKeyboardFocus(e);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (!e.Handled)
      this.HandlePreviewKeyDown(e);
    base.OnPreviewKeyDown(e);
  }

  protected override void OnPreviewTextInput(TextCompositionEventArgs e)
  {
    if (!e.Handled)
      this.HandlePreviewTextInput(e);
    base.OnPreviewTextInput(e);
  }

  public event RoutedPropertyChangedEventHandler<object> ValueChanged
  {
    add => this.AddHandler(MaskedTextBox.ValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(MaskedTextBox.ValueChangedEvent, (Delegate) value);
  }

  private void UpdateText() => this.UpdateText(this.SelectionStart);

  private void UpdateText(int position)
  {
    this.Text = (this.MaskProvider ?? throw new InvalidOperationException()).ToDisplayString();
    this.SelectionLength = 0;
    this.SelectionStart = position;
  }

  private int GetNextCharacterPosition(int startPosition)
  {
    int editPositionFrom = this.MaskProvider.FindEditPositionFrom(startPosition, true);
    return editPositionFrom != -1 ? editPositionFrom : startPosition;
  }

  private void UpdateMaskProvider(string mask)
  {
    if (string.IsNullOrEmpty(mask))
      return;
    this.MaskProvider = new MaskedTextProvider(mask)
    {
      IncludePrompt = this.IncludePrompt,
      IncludeLiterals = this.IncludeLiterals,
      PromptChar = this.PromptChar,
      ResetOnSpace = false
    };
  }

  private object ConvertTextToValue(string text)
  {
    object obj = (object) null;
    Type valueType = this.ValueType;
    string o = this.MaskProvider.ToString().Trim();
    try
    {
      if (o.GetType() == valueType || valueType.IsInstanceOfType((object) o))
        obj = (object) o;
      else if (string.IsNullOrWhiteSpace(o))
        obj = Activator.CreateInstance(valueType);
      else if (obj == null)
      {
        if (o != null)
          obj = Convert.ChangeType((object) o, valueType);
      }
    }
    catch
    {
      this._convertExceptionOccurred = true;
      return this.Value;
    }
    return obj;
  }

  private string ConvertValueToText(object value)
  {
    if (value == null)
      value = (object) string.Empty;
    if (this._convertExceptionOccurred)
    {
      value = this.Value;
      this._convertExceptionOccurred = false;
    }
    if (this.MaskProvider == null)
      return value.ToString();
    this.MaskProvider.Set(value.ToString());
    return this.MaskProvider.ToDisplayString();
  }

  private void SyncTextAndValueProperties(DependencyProperty p, object newValue)
  {
    if (this._isSyncingTextAndValueProperties)
      return;
    this._isSyncingTextAndValueProperties = true;
    if (TextBox.TextProperty == p && newValue != null)
      this.SetValue(MaskedTextBox.ValueProperty, this.ConvertTextToValue(newValue.ToString()));
    this.SetValue(TextBox.TextProperty, (object) this.ConvertValueToText(newValue));
    this._isSyncingTextAndValueProperties = false;
  }

  private void HandlePreviewTextInput(TextCompositionEventArgs e)
  {
    if (!this.IsReadOnly)
      this.InsertText(e.Text);
    e.Handled = true;
  }

  private void HandlePreviewKeyDown(KeyEventArgs e)
  {
    if (e.Key == Key.Delete)
      e.Handled = this.IsReadOnly || this.HandleKeyDownDelete();
    else if (e.Key == Key.Back)
      e.Handled = this.IsReadOnly || this.HandleKeyDownBack();
    else if (e.Key == Key.Space)
    {
      if (!this.IsReadOnly)
        this.InsertText(" ");
      e.Handled = true;
    }
    else if (e.Key == Key.Return || e.Key == Key.Return)
    {
      if (!this.IsReadOnly && this.AcceptsReturn)
        this.InsertText("\r");
      e.Handled = true;
    }
    else if (e.Key == Key.Escape)
    {
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Tab || !this.AcceptsTab)
        return;
      if (!this.IsReadOnly)
        this.InsertText("\t");
      e.Handled = true;
    }
  }

  private bool HandleKeyDownDelete()
  {
    ModifierKeys modifiers = Keyboard.Modifiers;
    bool flag = true;
    switch (modifiers)
    {
      case ModifierKeys.None:
        if (!this.RemoveSelectedText())
        {
          int selectionStart = this.SelectionStart;
          if (selectionStart < this.Text.Length)
          {
            this.RemoveText(selectionStart, 1);
            this.UpdateText(selectionStart);
            break;
          }
          break;
        }
        this.UpdateText();
        break;
      case ModifierKeys.Control:
        if (!this.RemoveSelectedText())
        {
          int selectionStart = this.SelectionStart;
          this.RemoveTextToEnd(selectionStart);
          this.UpdateText(selectionStart);
          break;
        }
        this.UpdateText();
        break;
      case ModifierKeys.Shift:
        if (this.RemoveSelectedText())
        {
          this.UpdateText();
          break;
        }
        flag = false;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  private bool HandleKeyDownBack()
  {
    ModifierKeys modifiers = Keyboard.Modifiers;
    bool flag = true;
    switch (modifiers)
    {
      case ModifierKeys.None:
      case ModifierKeys.Shift:
        if (!this.RemoveSelectedText())
        {
          int selectionStart = this.SelectionStart;
          if (selectionStart > 0)
          {
            int position = selectionStart - 1;
            this.RemoveText(position, 1);
            this.UpdateText(position);
            break;
          }
          break;
        }
        this.UpdateText();
        break;
      case ModifierKeys.Control:
        if (!this.RemoveSelectedText())
        {
          this.RemoveTextFromStart(this.SelectionStart);
          this.UpdateText(0);
          break;
        }
        this.UpdateText();
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  private void InsertText(string text)
  {
    int selectionStart = this.SelectionStart;
    MaskedTextProvider maskProvider = this.MaskProvider;
    int num = this.RemoveSelectedText() ? 1 : 0;
    int characterPosition = this.GetNextCharacterPosition(selectionStart);
    if (num == 0 && Keyboard.IsKeyToggled(Key.Insert))
    {
      if (maskProvider.Replace(text, characterPosition))
        characterPosition += text.Length;
    }
    else if (maskProvider.InsertAt(text, characterPosition))
      characterPosition += text.Length;
    this.UpdateText(this.GetNextCharacterPosition(characterPosition));
  }

  private void RemoveTextFromStart(int endPosition) => this.RemoveText(0, endPosition);

  private void RemoveTextToEnd(int startPosition)
  {
    this.RemoveText(startPosition, this.Text.Length - startPosition);
  }

  private void RemoveText(int position, int length)
  {
    if (length == 0)
      return;
    this.MaskProvider.RemoveAt(position, position + length - 1);
  }

  private bool RemoveSelectedText()
  {
    int selectionLength = this.SelectionLength;
    if (selectionLength == 0)
      return false;
    int selectionStart = this.SelectionStart;
    return this.MaskProvider.RemoveAt(selectionStart, selectionStart + selectionLength - 1);
  }

  private void Paste(object sender, RoutedEventArgs e)
  {
    if (this.IsReadOnly)
      return;
    object data = Clipboard.GetData(DataFormats.Text);
    if (data == null)
      return;
    string input = data.ToString().Trim();
    if (input.Length <= 0)
      return;
    int selectionStart = this.SelectionStart;
    this.MaskProvider.Set(input);
    this.UpdateText(selectionStart);
  }

  private void CanCut(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = false;
    e.Handled = true;
  }
}
