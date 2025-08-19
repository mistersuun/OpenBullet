// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.MaskedTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class MaskedTextBox : ValueRangeTextBox
{
  private static readonly char[] MaskChars = new char[18]
  {
    '0',
    '9',
    '#',
    'L',
    '?',
    '&',
    'C',
    'A',
    'a',
    '.',
    ',',
    ':',
    '/',
    '$',
    '<',
    '>',
    '|',
    '\\'
  };
  private static char DefaultPasswordChar = char.MinValue;
  private static string NullMaskString = "<>";
  public static readonly DependencyProperty AllowPromptAsInputProperty = DependencyProperty.Register(nameof (AllowPromptAsInput), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.AllowPromptAsInputPropertyChangedCallback)));
  public static readonly DependencyProperty ClipboardMaskFormatProperty = DependencyProperty.Register(nameof (ClipboardMaskFormat), typeof (MaskFormat), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) MaskFormat.IncludeLiterals));
  public static readonly DependencyProperty HidePromptOnLeaveProperty = DependencyProperty.Register(nameof (HidePromptOnLeave), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IncludeLiteralsInValueProperty = DependencyProperty.Register(nameof (IncludeLiteralsInValue), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.InlcudeLiteralsInValuePropertyChangedCallback)));
  public static readonly DependencyProperty IncludePromptInValueProperty = DependencyProperty.Register(nameof (IncludePromptInValue), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(MaskedTextBox.IncludePromptInValuePropertyChangedCallback)));
  public static readonly DependencyProperty InsertKeyModeProperty = DependencyProperty.Register(nameof (InsertKeyMode), typeof (InsertKeyMode), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) InsertKeyMode.Default));
  private static readonly DependencyPropertyKey IsMaskCompletedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsMaskCompleted), typeof (bool), typeof (MaskedTextBox), new PropertyMetadata((object) false));
  public static readonly DependencyProperty IsMaskCompletedProperty = MaskedTextBox.IsMaskCompletedPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsMaskFullPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsMaskFull), typeof (bool), typeof (MaskedTextBox), new PropertyMetadata((object) false));
  public static readonly DependencyProperty IsMaskFullProperty = MaskedTextBox.IsMaskFullPropertyKey.DependencyProperty;
  public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(nameof (Mask), typeof (string), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty, new PropertyChangedCallback(MaskedTextBox.MaskPropertyChangedCallback), new CoerceValueCallback(MaskedTextBox.MaskCoerceValueCallback)));
  public static readonly DependencyProperty PromptCharProperty = DependencyProperty.Register(nameof (PromptChar), typeof (char), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) '_', new PropertyChangedCallback(MaskedTextBox.PromptCharPropertyChangedCallback), new CoerceValueCallback(MaskedTextBox.PromptCharCoerceValueCallback)));
  public static readonly DependencyProperty RejectInputOnFirstFailureProperty = DependencyProperty.Register(nameof (RejectInputOnFirstFailure), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ResetOnPromptProperty = DependencyProperty.Register(nameof (ResetOnPrompt), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.ResetOnPromptPropertyChangedCallback)));
  public static readonly DependencyProperty ResetOnSpaceProperty = DependencyProperty.Register(nameof (ResetOnSpace), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.ResetOnSpacePropertyChangedCallback)));
  public static readonly DependencyProperty RestrictToAsciiProperty = DependencyProperty.Register(nameof (RestrictToAscii), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(MaskedTextBox.RestrictToAsciiPropertyChangedCallback), new CoerceValueCallback(MaskedTextBox.RestrictToAsciiCoerceValueCallback)));
  public static readonly DependencyProperty SkipLiteralsProperty = DependencyProperty.Register(nameof (SkipLiterals), typeof (bool), typeof (MaskedTextBox), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(MaskedTextBox.SkipLiteralsPropertyChangedCallback)));
  private MaskedTextProvider m_maskedTextProvider;
  private bool m_insertToggled;
  private bool m_maskIsNull = true;
  private bool m_forcingMask;
  private List<int> m_unhandledLiteralsPositions;
  private string m_formatSpecifier;
  private MethodInfo m_valueToStringMethodInfo;

  private static string GetRawText(MaskedTextProvider provider)
  {
    return provider.ToString(true, false, false, 0, provider.Length);
  }

  public static string GetFormatSpecifierFromMask(string mask, IFormatProvider formatProvider)
  {
    return MaskedTextBox.GetFormatSpecifierFromMask(mask, MaskedTextBox.MaskChars, formatProvider, true, out List<int> _);
  }

  private static string GetFormatSpecifierFromMask(
    string mask,
    char[] maskChars,
    IFormatProvider formatProvider,
    bool includeNonSeparatorLiteralsInValue,
    out List<int> unhandledLiteralsPositions)
  {
    unhandledLiteralsPositions = new List<int>();
    NumberFormatInfo instance = NumberFormatInfo.GetInstance(formatProvider);
    StringBuilder stringBuilder = new StringBuilder(32 /*0x20*/);
    bool flag = false;
    int index1 = 0;
    int num = 0;
    for (; index1 < mask.Length; ++index1)
    {
      char ch = mask[index1];
      if (ch == '\\' && !flag)
        flag = true;
      else if (flag || Array.IndexOf<char>(maskChars, ch) < 0)
      {
        flag = false;
        stringBuilder.Append('\\');
        stringBuilder.Append(ch);
        if (!includeNonSeparatorLiteralsInValue && ch != ' ')
          unhandledLiteralsPositions.Add(num);
        ++num;
      }
      else
      {
        switch (ch)
        {
          case '#':
          case '0':
          case '9':
            stringBuilder.Append('0');
            ++num;
            continue;
          case '$':
            string currencySymbol = instance.CurrencySymbol;
            stringBuilder.Append('"');
            stringBuilder.Append(currencySymbol);
            stringBuilder.Append('"');
            for (int index2 = 0; index2 < currencySymbol.Length; ++index2)
            {
              if (!includeNonSeparatorLiteralsInValue)
                unhandledLiteralsPositions.Add(num);
              ++num;
            }
            continue;
          case ',':
            stringBuilder.Append(',');
            num += instance.NumberGroupSeparator.Length;
            continue;
          case '.':
            stringBuilder.Append('.');
            num += instance.NumberDecimalSeparator.Length;
            continue;
          default:
            stringBuilder.Append(ch);
            if (!includeNonSeparatorLiteralsInValue && ch != ' ')
              unhandledLiteralsPositions.Add(num);
            ++num;
            continue;
        }
      }
    }
    return stringBuilder.ToString();
  }

  static MaskedTextBox()
  {
    TextBox.TextProperty.OverrideMetadata(typeof (MaskedTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(MaskedTextBox.TextCoerceValueCallback)));
  }

  public MaskedTextBox()
  {
    CommandManager.AddPreviewCanExecuteHandler((UIElement) this, new CanExecuteRoutedEventHandler(this.OnPreviewCanExecuteCommands));
    CommandManager.AddPreviewExecutedHandler((UIElement) this, new ExecutedRoutedEventHandler(this.OnPreviewExecutedCommands));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Paste, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecutePaste)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Cut, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteCut)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Copy, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteCopy)));
    this.CommandBindings.Add(new CommandBinding((ICommand) EditingCommands.ToggleInsert, new ExecutedRoutedEventHandler(this.ToggleInsertExecutedCallback)));
    this.CommandBindings.Add(new CommandBinding((ICommand) EditingCommands.Delete, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteDelete)));
    this.CommandBindings.Add(new CommandBinding((ICommand) EditingCommands.DeletePreviousWord, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteDeletePreviousWord)));
    this.CommandBindings.Add(new CommandBinding((ICommand) EditingCommands.DeleteNextWord, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteDeleteNextWord)));
    this.CommandBindings.Add(new CommandBinding((ICommand) EditingCommands.Backspace, (ExecutedRoutedEventHandler) null, new CanExecuteRoutedEventHandler(this.CanExecuteBackspace)));
    DragDrop.AddPreviewQueryContinueDragHandler((DependencyObject) this, new QueryContinueDragEventHandler(this.PreviewQueryContinueDragCallback));
    this.AllowDrop = false;
  }

  private void InitializeMaskedTextProvider()
  {
    string text = this.Text;
    string mask = this.Mask;
    if (mask == string.Empty)
    {
      this.m_maskedTextProvider = this.CreateMaskedTextProvider(MaskedTextBox.NullMaskString);
      this.m_maskIsNull = true;
    }
    else
    {
      this.m_maskedTextProvider = this.CreateMaskedTextProvider(mask);
      this.m_maskIsNull = false;
    }
    if (!this.m_maskIsNull && text != string.Empty && !this.m_maskedTextProvider.Add(text) && !DesignerProperties.GetIsInDesignMode((DependencyObject) this))
      throw new InvalidOperationException("An attempt was made to apply a new mask that cannot be applied to the current text.");
  }

  protected override void OnInitialized(EventArgs e)
  {
    this.InitializeMaskedTextProvider();
    this.SetIsMaskCompleted(this.m_maskedTextProvider.MaskCompleted);
    this.SetIsMaskFull(this.m_maskedTextProvider.MaskFull);
    base.OnInitialized(e);
  }

  public bool AllowPromptAsInput
  {
    get => (bool) this.GetValue(MaskedTextBox.AllowPromptAsInputProperty);
    set => this.SetValue(MaskedTextBox.AllowPromptAsInputProperty, (object) value);
  }

  private static void AllowPromptAsInputPropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider = maskedTextBox.CreateMaskedTextProvider(maskedTextBox.Mask);
  }

  public MaskFormat ClipboardMaskFormat
  {
    get => (MaskFormat) this.GetValue(MaskedTextBox.ClipboardMaskFormatProperty);
    set => this.SetValue(MaskedTextBox.ClipboardMaskFormatProperty, (object) value);
  }

  public bool HidePromptOnLeave
  {
    get => (bool) this.GetValue(MaskedTextBox.HidePromptOnLeaveProperty);
    set => this.SetValue(MaskedTextBox.HidePromptOnLeaveProperty, (object) value);
  }

  public bool IncludeLiteralsInValue
  {
    get => (bool) this.GetValue(MaskedTextBox.IncludeLiteralsInValueProperty);
    set => this.SetValue(MaskedTextBox.IncludeLiteralsInValueProperty, (object) value);
  }

  private static void InlcudeLiteralsInValuePropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return;
    maskedTextBox.RefreshConversionHelpers();
    maskedTextBox.RefreshValue();
  }

  public bool IncludePromptInValue
  {
    get => (bool) this.GetValue(MaskedTextBox.IncludePromptInValueProperty);
    set => this.SetValue(MaskedTextBox.IncludePromptInValueProperty, (object) value);
  }

  private static void IncludePromptInValuePropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return;
    maskedTextBox.RefreshValue();
  }

  public InsertKeyMode InsertKeyMode
  {
    get => (InsertKeyMode) this.GetValue(MaskedTextBox.InsertKeyModeProperty);
    set => this.SetValue(MaskedTextBox.InsertKeyModeProperty, (object) value);
  }

  public bool IsMaskCompleted => (bool) this.GetValue(MaskedTextBox.IsMaskCompletedProperty);

  private void SetIsMaskCompleted(bool value)
  {
    this.SetValue(MaskedTextBox.IsMaskCompletedPropertyKey, (object) value);
  }

  public bool IsMaskFull => (bool) this.GetValue(MaskedTextBox.IsMaskFullProperty);

  private void SetIsMaskFull(bool value)
  {
    this.SetValue(MaskedTextBox.IsMaskFullPropertyKey, (object) value);
  }

  public string Mask
  {
    get => (string) this.GetValue(MaskedTextBox.MaskProperty);
    set => this.SetValue(MaskedTextBox.MaskProperty, (object) value);
  }

  private static object MaskCoerceValueCallback(DependencyObject sender, object value)
  {
    if (value == null)
      value = (object) string.Empty;
    if (value.Equals((object) string.Empty))
      return value;
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return value;
    bool flag;
    try
    {
      flag = maskedTextBox.CreateMaskedTextProvider((string) value).VerifyString(MaskedTextBox.GetRawText(maskedTextBox.m_maskedTextProvider));
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("An error occured while testing the current text against the new mask.", ex);
    }
    if (!flag)
      throw new ArgumentException("The mask cannot be applied to the current text.", "Mask");
    return value;
  }

  private static void MaskPropertyChangedCallback(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return;
    string newValue = (string) e.NewValue;
    MaskedTextProvider maskedTextProvider;
    if (newValue == string.Empty)
    {
      maskedTextProvider = maskedTextBox.CreateMaskedTextProvider(MaskedTextBox.NullMaskString);
      maskedTextBox.m_maskIsNull = true;
      maskedTextBox.Text = "";
    }
    else
    {
      maskedTextProvider = maskedTextBox.CreateMaskedTextProvider(newValue);
      maskedTextBox.m_maskIsNull = false;
    }
    maskedTextBox.m_maskedTextProvider = maskedTextProvider;
    maskedTextBox.RefreshConversionHelpers();
    if (maskedTextBox.ValueDataType != (Type) null)
    {
      string textFromValue = maskedTextBox.GetTextFromValue(maskedTextBox.Value);
      maskedTextBox.m_maskedTextProvider.Set(textFromValue);
    }
    maskedTextBox.RefreshCurrentText(true);
  }

  public MaskedTextProvider MaskedTextProvider
  {
    get
    {
      return !this.m_maskIsNull ? this.m_maskedTextProvider.Clone() as MaskedTextProvider : (MaskedTextProvider) null;
    }
  }

  public char PromptChar
  {
    get => (char) this.GetValue(MaskedTextBox.PromptCharProperty);
    set => this.SetValue(MaskedTextBox.PromptCharProperty, (object) value);
  }

  private static object PromptCharCoerceValueCallback(object sender, object value)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return value;
    MaskedTextProvider maskedTextProvider = maskedTextBox.m_maskedTextProvider.Clone() as MaskedTextProvider;
    try
    {
      maskedTextProvider.PromptChar = (char) value;
    }
    catch (Exception ex)
    {
      throw new ArgumentException("The prompt character is invalid.", ex);
    }
    return value;
  }

  private static void PromptCharPropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider.PromptChar = (char) e.NewValue;
    maskedTextBox.RefreshCurrentText(true);
  }

  public bool RejectInputOnFirstFailure
  {
    get => (bool) this.GetValue(MaskedTextBox.RejectInputOnFirstFailureProperty);
    set => this.SetValue(MaskedTextBox.RejectInputOnFirstFailureProperty, (object) value);
  }

  public bool ResetOnPrompt
  {
    get => (bool) this.GetValue(MaskedTextBox.ResetOnPromptProperty);
    set => this.SetValue(MaskedTextBox.ResetOnPromptProperty, (object) value);
  }

  private static void ResetOnPromptPropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider.ResetOnPrompt = (bool) e.NewValue;
  }

  public bool ResetOnSpace
  {
    get => (bool) this.GetValue(MaskedTextBox.ResetOnSpaceProperty);
    set => this.SetValue(MaskedTextBox.ResetOnSpaceProperty, (object) value);
  }

  private static void ResetOnSpacePropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider.ResetOnSpace = (bool) e.NewValue;
  }

  public bool RestrictToAscii
  {
    get => (bool) this.GetValue(MaskedTextBox.RestrictToAsciiProperty);
    set => this.SetValue(MaskedTextBox.RestrictToAsciiProperty, (object) value);
  }

  private static object RestrictToAsciiCoerceValueCallback(object sender, object value)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return value;
    bool restrictToAscii = (bool) value;
    if (!restrictToAscii)
      return value;
    if (!maskedTextBox.CreateMaskedTextProvider(maskedTextBox.Mask, maskedTextBox.GetCultureInfo(), maskedTextBox.AllowPromptAsInput, maskedTextBox.PromptChar, MaskedTextBox.DefaultPasswordChar, restrictToAscii).VerifyString(maskedTextBox.Text))
      throw new ArgumentException("The current text cannot be restricted to ASCII characters. The RestrictToAscii property is set to true.", "RestrictToAscii");
    return (object) restrictToAscii;
  }

  private static void RestrictToAsciiPropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider = maskedTextBox.CreateMaskedTextProvider(maskedTextBox.Mask);
    maskedTextBox.RefreshCurrentText(true);
  }

  public bool SkipLiterals
  {
    get => (bool) this.GetValue(MaskedTextBox.SkipLiteralsProperty);
    set => this.SetValue(MaskedTextBox.SkipLiteralsProperty, (object) value);
  }

  private static void SkipLiteralsPropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized || maskedTextBox.m_maskIsNull)
      return;
    maskedTextBox.m_maskedTextProvider.SkipLiterals = (bool) e.NewValue;
  }

  private static object TextCoerceValueCallback(DependencyObject sender, object value)
  {
    MaskedTextBox maskedTextBox = sender as MaskedTextBox;
    if (!maskedTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    if (maskedTextBox.IsInIMEComposition)
      return value;
    if (value == null)
      value = (object) string.Empty;
    return maskedTextBox.IsForcingText || maskedTextBox.m_maskIsNull ? value : (object) maskedTextBox.ValidateText((string) value);
  }

  private string ValidateText(string text)
  {
    string formattedString;
    if (this.RejectInputOnFirstFailure)
    {
      MaskedTextProvider provider = this.m_maskedTextProvider.Clone() as MaskedTextProvider;
      if (provider.Set(text, out int _, out MaskedTextResultHint _) || provider.Mask.StartsWith(">") || provider.Mask.StartsWith("<"))
      {
        formattedString = this.GetFormattedString(provider, text);
      }
      else
      {
        formattedString = this.GetFormattedString(this.m_maskedTextProvider, text);
        this.m_maskedTextProvider.Set(formattedString);
      }
    }
    else
    {
      MaskedTextProvider provider = (MaskedTextProvider) this.m_maskedTextProvider.Clone();
      if (this.CanReplace(provider, text, 0, this.m_maskedTextProvider.Length, this.RejectInputOnFirstFailure, out int _))
      {
        formattedString = this.GetFormattedString(provider, text);
      }
      else
      {
        formattedString = this.GetFormattedString(this.m_maskedTextProvider, text);
        this.m_maskedTextProvider.Set(formattedString);
      }
    }
    return formattedString;
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    if (!this.m_maskIsNull && (this.IsInValueChanged || !this.IsForcingText))
    {
      string text = this.Text;
      if (this.m_maskIsNull)
      {
        this.CaretIndex = text.Length;
      }
      else
      {
        this.m_maskedTextProvider.Set(text);
        if (this.m_maskedTextProvider.Mask.StartsWith(">") || this.m_maskedTextProvider.Mask.StartsWith("<"))
        {
          this.CaretIndex = text.Length;
        }
        else
        {
          int num = this.m_maskedTextProvider.FindUnassignedEditPositionFrom(0, true);
          if (num == -1)
            num = this.m_maskedTextProvider.Length;
          this.CaretIndex = num;
        }
      }
    }
    if (this.m_maskedTextProvider != null)
    {
      this.SetIsMaskCompleted(this.m_maskedTextProvider.MaskCompleted);
      this.SetIsMaskFull(this.m_maskedTextProvider.MaskFull);
    }
    base.OnTextChanged(e);
  }

  private void OnPreviewCanExecuteCommands(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    if (e.Command is RoutedUICommand command && (command.Name == "Space" || command.Name == "ShiftSpace"))
    {
      if (this.IsReadOnly)
      {
        e.CanExecute = false;
      }
      else
      {
        MaskedTextProvider provider = (MaskedTextProvider) this.m_maskedTextProvider.Clone();
        e.CanExecute = this.CanReplace(provider, " ", this.SelectionStart, this.SelectionLength, this.RejectInputOnFirstFailure, out int _);
      }
      e.Handled = true;
    }
    else
    {
      if (e.Command != ApplicationCommands.Undo && e.Command != ApplicationCommands.Redo)
        return;
      e.CanExecute = false;
      e.Handled = true;
    }
  }

  private void OnPreviewExecutedCommands(object sender, ExecutedRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    if (e.Command == EditingCommands.Delete)
    {
      e.Handled = true;
      this.Delete(this.SelectionStart, this.SelectionLength, true);
    }
    else if (e.Command == EditingCommands.DeleteNextWord)
    {
      e.Handled = true;
      EditingCommands.SelectRightByWord.Execute((object) null, (IInputElement) this);
      this.Delete(this.SelectionStart, this.SelectionLength, true);
    }
    else if (e.Command == EditingCommands.DeletePreviousWord)
    {
      e.Handled = true;
      EditingCommands.SelectLeftByWord.Execute((object) null, (IInputElement) this);
      this.Delete(this.SelectionStart, this.SelectionLength, false);
    }
    else if (e.Command == EditingCommands.Backspace)
    {
      e.Handled = true;
      this.Delete(this.SelectionStart, this.SelectionLength, false);
    }
    else if (e.Command == ApplicationCommands.Cut)
    {
      e.Handled = true;
      if (ApplicationCommands.Copy.CanExecute((object) null, (IInputElement) this))
        ApplicationCommands.Copy.Execute((object) null, (IInputElement) this);
      this.Delete(this.SelectionStart, this.SelectionLength, true);
    }
    else if (e.Command == ApplicationCommands.Copy)
    {
      e.Handled = true;
      this.ExecuteCopy();
    }
    else if (e.Command == ApplicationCommands.Paste)
    {
      e.Handled = true;
      this.Replace((string) Clipboard.GetDataObject().GetData("System.String"), this.SelectionStart, this.SelectionLength);
    }
    else
    {
      if (!(e.Command is RoutedUICommand command) || !(command.Name == "Space") && !(command.Name == "ShiftSpace"))
        return;
      e.Handled = true;
      this.ProcessTextInput(" ");
    }
  }

  private void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    e.CanExecute = this.CanDelete(this.SelectionStart, this.SelectionLength, true, this.MaskedTextProvider.Clone() as MaskedTextProvider);
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecuteDeletePreviousWord(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    bool flag = !this.IsReadOnly && EditingCommands.SelectLeftByWord.CanExecute((object) null, (IInputElement) this);
    if (flag)
    {
      int selectionStart = this.SelectionStart;
      int selectionLength = this.SelectionLength;
      EditingCommands.SelectLeftByWord.Execute((object) null, (IInputElement) this);
      flag = this.CanDelete(this.SelectionStart, this.SelectionLength, false, this.MaskedTextProvider.Clone() as MaskedTextProvider);
      if (!flag)
      {
        this.SelectionStart = selectionStart;
        this.SelectionLength = selectionLength;
      }
    }
    e.CanExecute = flag;
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecuteDeleteNextWord(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    bool flag = !this.IsReadOnly && EditingCommands.SelectRightByWord.CanExecute((object) null, (IInputElement) this);
    if (flag)
    {
      int selectionStart = this.SelectionStart;
      int selectionLength = this.SelectionLength;
      EditingCommands.SelectRightByWord.Execute((object) null, (IInputElement) this);
      flag = this.CanDelete(this.SelectionStart, this.SelectionLength, true, this.MaskedTextProvider.Clone() as MaskedTextProvider);
      if (!flag)
      {
        this.SelectionStart = selectionStart;
        this.SelectionLength = selectionLength;
      }
    }
    e.CanExecute = flag;
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecuteBackspace(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    e.CanExecute = this.CanDelete(this.SelectionStart, this.SelectionLength, false, this.MaskedTextProvider.Clone() as MaskedTextProvider);
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecuteCut(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    bool flag = !this.IsReadOnly && this.SelectionLength > 0;
    if (flag)
    {
      int endPosition = this.SelectionLength > 0 ? this.SelectionStart + this.SelectionLength - 1 : this.SelectionStart;
      flag = (this.m_maskedTextProvider.Clone() as MaskedTextProvider).RemoveAt(this.SelectionStart, endPosition);
    }
    e.CanExecute = flag;
    e.Handled = true;
    if (flag || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    bool flag = false;
    if (!this.IsReadOnly)
    {
      string empty = string.Empty;
      try
      {
        string data = (string) Clipboard.GetDataObject().GetData("System.String");
        if (data != null)
          flag = this.CanReplace((MaskedTextProvider) this.m_maskedTextProvider.Clone(), data, this.SelectionStart, this.SelectionLength, this.RejectInputOnFirstFailure, out int _);
      }
      catch
      {
      }
    }
    e.CanExecute = flag;
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void CanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    e.CanExecute = !this.m_maskedTextProvider.IsPassword;
    e.Handled = true;
    if (e.CanExecute || !this.BeepOnError)
      return;
    SystemSounds.Beep.Play();
  }

  private void ExecuteCopy()
  {
    string selectedText = this.GetSelectedText();
    try
    {
      new UIPermission(UIPermissionClipboard.AllClipboard).Demand();
      if (selectedText.Length == 0)
        Clipboard.Clear();
      else
        Clipboard.SetText(selectedText);
    }
    catch (SecurityException ex)
    {
    }
  }

  private void ToggleInsertExecutedCallback(object sender, ExecutedRoutedEventArgs e)
  {
    this.m_insertToggled = !this.m_insertToggled;
  }

  private void PreviewQueryContinueDragCallback(object sender, QueryContinueDragEventArgs e)
  {
    if (this.m_maskIsNull)
      return;
    e.Action = DragAction.Cancel;
    e.Handled = true;
  }

  protected override void OnDragEnter(DragEventArgs e)
  {
    if (!this.m_maskIsNull)
    {
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }
    base.OnDragEnter(e);
  }

  protected override void OnDragOver(DragEventArgs e)
  {
    if (!this.m_maskIsNull)
    {
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }
    base.OnDragOver(e);
  }

  protected override bool QueryValueFromTextCore(string text, out object value)
  {
    if (this.ValueDataType != (Type) null && this.m_unhandledLiteralsPositions != null && this.m_unhandledLiteralsPositions.Count > 0)
    {
      text = this.m_maskedTextProvider.ToString(false, false, true, 0, this.m_maskedTextProvider.Length);
      for (int index = this.m_unhandledLiteralsPositions.Count - 1; index >= 0; --index)
        text = text.Remove(this.m_unhandledLiteralsPositions[index], 1);
    }
    return base.QueryValueFromTextCore(text, out value);
  }

  protected override string QueryTextFromValueCore(object value)
  {
    if (this.m_valueToStringMethodInfo != (MethodInfo) null)
    {
      if (value != null)
      {
        try
        {
          return (string) this.m_valueToStringMethodInfo.Invoke(value, new object[2]
          {
            (object) this.m_formatSpecifier,
            (object) this.GetActiveFormatProvider()
          });
        }
        catch
        {
        }
      }
    }
    return base.QueryTextFromValueCore(value);
  }

  protected virtual char[] GetMaskCharacters() => MaskedTextBox.MaskChars;

  private MaskedTextProvider CreateMaskedTextProvider(string mask)
  {
    return this.CreateMaskedTextProvider(mask, this.GetCultureInfo(), this.AllowPromptAsInput, this.PromptChar, MaskedTextBox.DefaultPasswordChar, this.RestrictToAscii);
  }

  protected virtual MaskedTextProvider CreateMaskedTextProvider(
    string mask,
    CultureInfo cultureInfo,
    bool allowPromptAsInput,
    char promptChar,
    char passwordChar,
    bool restrictToAscii)
  {
    return new MaskedTextProvider(mask, cultureInfo, allowPromptAsInput, promptChar, passwordChar, restrictToAscii)
    {
      ResetOnPrompt = this.ResetOnPrompt,
      ResetOnSpace = this.ResetOnSpace,
      SkipLiterals = this.SkipLiterals,
      IncludeLiterals = true,
      IncludePrompt = true,
      IsPassword = false
    };
  }

  internal override void OnIMECompositionEnded(CachedTextInfo cachedTextInfo)
  {
    this.ForceText(cachedTextInfo.Text, false);
    this.CaretIndex = cachedTextInfo.CaretIndex;
    this.SelectionStart = cachedTextInfo.SelectionStart;
    this.SelectionLength = cachedTextInfo.SelectionLength;
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    if (this.IsInIMEComposition)
      this.EndIMEComposition();
    if (this.m_maskIsNull || this.m_maskedTextProvider == null || this.IsReadOnly)
    {
      base.OnTextInput(e);
    }
    else
    {
      e.Handled = true;
      if (this.CharacterCasing == CharacterCasing.Upper)
        this.ProcessTextInput(e.Text.ToUpper());
      else if (this.CharacterCasing == CharacterCasing.Lower)
        this.ProcessTextInput(e.Text.ToLower());
      else
        this.ProcessTextInput(e.Text);
      base.OnTextInput(e);
    }
  }

  private void ProcessTextInput(string text)
  {
    if (text.Length == 1)
    {
      string maskedTextOutput = this.MaskedTextOutput;
      int caretIndex;
      if (this.PlaceChar(text[0], this.SelectionStart, this.SelectionLength, this.IsOverwriteMode, out caretIndex))
      {
        if (this.MaskedTextOutput != maskedTextOutput)
          this.RefreshCurrentText(false);
        this.SelectionStart = caretIndex + 1;
      }
      else if (this.BeepOnError)
        SystemSounds.Beep.Play();
      if (this.SelectionLength <= 0)
        return;
      this.SelectionLength = 0;
    }
    else
      this.Replace(text, this.SelectionStart, this.SelectionLength);
  }

  protected override void ValidateValue(object value)
  {
    base.ValidateValue(value);
    if (this.m_maskIsNull)
      return;
    string textFromValue = this.GetTextFromValue(value);
    if (!(this.m_maskedTextProvider.Clone() as MaskedTextProvider).VerifyString(textFromValue))
      throw new ArgumentException($"The value representation '{textFromValue}' does not match the mask.", nameof (value));
  }

  internal bool IsForcingMask => this.m_forcingMask;

  internal string FormatSpecifier
  {
    get => this.m_formatSpecifier;
    set => this.m_formatSpecifier = value;
  }

  internal override bool IsTextReadyToBeParsed => this.IsMaskCompleted;

  internal override bool GetIsEditTextEmpty()
  {
    return this.MaskedTextProvider.AssignedEditPositionCount == 0;
  }

  internal override string GetCurrentText()
  {
    return this.m_maskIsNull ? base.GetCurrentText() : this.GetFormattedString(this.m_maskedTextProvider, this.Text);
  }

  internal override string GetParsableText()
  {
    if (this.m_maskIsNull)
      return base.GetParsableText();
    bool includePrompt = false;
    bool includeLiterals = true;
    if (this.ValueDataType == typeof (string))
    {
      includePrompt = this.IncludePromptInValue;
      includeLiterals = this.IncludeLiteralsInValue;
    }
    return this.m_maskedTextProvider.ToString(false, includePrompt, includeLiterals, 0, this.m_maskedTextProvider.Length);
  }

  internal override void OnFormatProviderChanged()
  {
    this.m_maskedTextProvider = new MaskedTextProvider(this.Mask);
    this.RefreshConversionHelpers();
    this.RefreshCurrentText(true);
    base.OnFormatProviderChanged();
  }

  internal override void RefreshConversionHelpers()
  {
    Type valueDataType = this.ValueDataType;
    if (valueDataType == (Type) null || !this.IsNumericValueDataType)
    {
      this.m_formatSpecifier = (string) null;
      this.m_valueToStringMethodInfo = (MethodInfo) null;
      this.m_unhandledLiteralsPositions = (List<int>) null;
    }
    else
    {
      this.m_valueToStringMethodInfo = valueDataType.GetMethod("ToString", new Type[2]
      {
        typeof (string),
        typeof (IFormatProvider)
      });
      string mask = this.m_maskedTextProvider.Mask;
      IFormatProvider activeFormatProvider = this.GetActiveFormatProvider();
      char[] maskCharacters = this.GetMaskCharacters();
      List<int> unhandledLiteralsPositions;
      this.m_formatSpecifier = MaskedTextBox.GetFormatSpecifierFromMask(mask, maskCharacters, activeFormatProvider, this.IncludeLiteralsInValue, out unhandledLiteralsPositions);
      if (activeFormatProvider.GetFormat(typeof (NumberFormatInfo)) is NumberFormatInfo format && this.m_formatSpecifier.Contains(format.NegativeSign))
        this.m_formatSpecifier = $"{this.m_formatSpecifier};{this.m_formatSpecifier};{this.m_formatSpecifier}";
      this.m_unhandledLiteralsPositions = unhandledLiteralsPositions;
    }
  }

  internal void SetValueToStringMethodInfo(MethodInfo valueToStringMethodInfo)
  {
    this.m_valueToStringMethodInfo = valueToStringMethodInfo;
  }

  internal void ForceMask(string mask)
  {
    this.m_forcingMask = true;
    try
    {
      this.Mask = mask;
    }
    finally
    {
      this.m_forcingMask = false;
    }
  }

  private bool IsOverwriteMode
  {
    get
    {
      if (!this.m_maskIsNull)
      {
        switch (this.InsertKeyMode)
        {
          case InsertKeyMode.Default:
            return this.m_insertToggled;
          case InsertKeyMode.Insert:
            return false;
          case InsertKeyMode.Overwrite:
            return true;
        }
      }
      return false;
    }
  }

  private bool PlaceChar(
    char ch,
    int startPosition,
    int length,
    bool overwrite,
    out int caretIndex)
  {
    return this.PlaceChar(this.m_maskedTextProvider, ch, startPosition, length, overwrite, out caretIndex);
  }

  private bool PlaceChar(
    MaskedTextProvider provider,
    char ch,
    int startPosition,
    int length,
    bool overwrite,
    out int caretPosition)
  {
    if (this.ShouldQueryAutoCompleteMask(provider.Clone() as MaskedTextProvider, ch, startPosition))
    {
      AutoCompletingMaskEventArgs e = new AutoCompletingMaskEventArgs(this.m_maskedTextProvider.Clone() as MaskedTextProvider, startPosition, length, ch.ToString());
      this.OnAutoCompletingMask(e);
      if (!e.Cancel && e.AutoCompleteStartPosition > -1)
      {
        caretPosition = startPosition;
        for (int index = 0; index < e.AutoCompleteText.Length; ++index)
        {
          if (!this.PlaceCharCore(provider, e.AutoCompleteText[index], e.AutoCompleteStartPosition + index, 0, true, out caretPosition))
            return false;
        }
        caretPosition = e.AutoCompleteStartPosition + e.AutoCompleteText.Length;
        return true;
      }
    }
    return this.PlaceCharCore(provider, ch, startPosition, length, overwrite, out caretPosition);
  }

  private bool ShouldQueryAutoCompleteMask(MaskedTextProvider provider, char ch, int startPosition)
  {
    if (provider.IsEditPosition(startPosition))
    {
      int editPositionFrom1 = provider.FindNonEditPositionFrom(startPosition, true);
      if (editPositionFrom1 != -1 && provider[editPositionFrom1].Equals(ch))
      {
        int editPositionFrom2 = provider.FindNonEditPositionFrom(startPosition, false);
        if (provider.FindUnassignedEditPositionInRange(editPositionFrom2, editPositionFrom1, true) != -1)
          return true;
      }
    }
    return false;
  }

  protected virtual void OnAutoCompletingMask(AutoCompletingMaskEventArgs e)
  {
    if (this.AutoCompletingMask == null)
      return;
    this.AutoCompletingMask((object) this, e);
  }

  public event EventHandler<AutoCompletingMaskEventArgs> AutoCompletingMask;

  private bool PlaceCharCore(
    MaskedTextProvider provider,
    char ch,
    int startPosition,
    int length,
    bool overwrite,
    out int caretPosition)
  {
    caretPosition = startPosition;
    if (startPosition >= this.m_maskedTextProvider.Length)
      return false;
    MaskedTextResultHint resultHint;
    if (length > 0)
    {
      int endPosition = startPosition + length - 1;
      return provider.Replace(ch, startPosition, endPosition, out caretPosition, out resultHint);
    }
    return overwrite ? provider.Replace(ch, startPosition, out caretPosition, out resultHint) : provider.InsertAt(ch, startPosition, out caretPosition, out resultHint);
  }

  internal void Replace(string text, int startPosition, int selectionLength)
  {
    MaskedTextProvider provider = (MaskedTextProvider) this.m_maskedTextProvider.Clone();
    int tentativeCaretIndex;
    if (this.CanReplace(provider, text, startPosition, selectionLength, this.RejectInputOnFirstFailure, out tentativeCaretIndex))
    {
      int num = this.MaskedTextOutput != provider.ToString() ? 1 : 0;
      this.m_maskedTextProvider = provider;
      if (num != 0)
        this.RefreshCurrentText(false);
      this.CaretIndex = tentativeCaretIndex + 1;
    }
    else
    {
      if (!this.BeepOnError)
        return;
      SystemSounds.Beep.Play();
    }
  }

  internal virtual bool CanReplace(
    MaskedTextProvider provider,
    string text,
    int startPosition,
    int selectionLength,
    bool rejectInputOnFirstFailure,
    out int tentativeCaretIndex)
  {
    int endPosition = startPosition + selectionLength - 1;
    tentativeCaretIndex = -1;
    bool flag = false;
    foreach (char ch in text)
    {
      if (!this.m_maskedTextProvider.VerifyEscapeChar(ch, startPosition))
      {
        int editPositionFrom = provider.FindEditPositionFrom(startPosition, true);
        if (editPositionFrom != MaskedTextProvider.InvalidIndex)
          startPosition = editPositionFrom;
        else
          break;
      }
      int length = endPosition >= startPosition ? 1 : 0;
      bool overwrite = length > 0;
      if (this.PlaceChar(provider, ch, startPosition, length, overwrite, out tentativeCaretIndex))
      {
        flag = true;
        startPosition = tentativeCaretIndex + 1;
      }
      else if (rejectInputOnFirstFailure)
        return false;
    }
    if (selectionLength > 0 && startPosition <= endPosition && !provider.RemoveAt(startPosition, endPosition, out int _, out MaskedTextResultHint _))
      flag = false;
    return flag;
  }

  private bool CanDelete(
    int startPosition,
    int selectionLength,
    bool deleteForward,
    MaskedTextProvider provider)
  {
    if (this.IsReadOnly)
      return false;
    if (selectionLength == 0)
    {
      if (!deleteForward)
      {
        if (startPosition == 0)
          return false;
        --startPosition;
      }
      else if (startPosition + selectionLength == provider.Length)
        return false;
    }
    int testPosition = startPosition;
    int endPosition = selectionLength > 0 ? startPosition + selectionLength - 1 : startPosition;
    return provider.RemoveAt(startPosition, endPosition, out testPosition, out MaskedTextResultHint _);
  }

  private void Delete(int startPosition, int selectionLength, bool deleteForward)
  {
    if (this.IsReadOnly)
      return;
    if (selectionLength == 0)
    {
      if (!deleteForward)
      {
        if (startPosition == 0)
          return;
        --startPosition;
      }
      else if (startPosition + selectionLength == this.m_maskedTextProvider.Length)
        return;
    }
    int testPosition = startPosition;
    int endPosition = selectionLength > 0 ? startPosition + selectionLength - 1 : startPosition;
    string maskedTextOutput = this.MaskedTextOutput;
    MaskedTextResultHint resultHint;
    if (!this.m_maskedTextProvider.RemoveAt(startPosition, endPosition, out testPosition, out resultHint))
    {
      if (!this.BeepOnError)
        return;
      SystemSounds.Beep.Play();
    }
    else
    {
      if (this.MaskedTextOutput != maskedTextOutput)
        this.RefreshCurrentText(false);
      else if (selectionLength > 0)
        testPosition = startPosition;
      else if (resultHint == MaskedTextResultHint.NoEffect)
      {
        if (deleteForward)
        {
          testPosition = this.m_maskedTextProvider.FindEditPositionFrom(startPosition, true);
        }
        else
        {
          testPosition = this.m_maskedTextProvider.FindAssignedEditPositionFrom(startPosition, true) != MaskedTextProvider.InvalidIndex ? this.m_maskedTextProvider.FindEditPositionFrom(startPosition, false) : this.m_maskedTextProvider.FindAssignedEditPositionFrom(startPosition, false);
          if (testPosition != MaskedTextProvider.InvalidIndex)
            ++testPosition;
        }
        if (testPosition == MaskedTextProvider.InvalidIndex)
          testPosition = startPosition;
      }
      else if (!deleteForward)
        testPosition = startPosition;
      this.CaretIndex = testPosition;
    }
  }

  private string MaskedTextOutput => this.m_maskedTextProvider.ToString();

  private string GetRawText()
  {
    return this.m_maskIsNull ? this.Text : MaskedTextBox.GetRawText(this.m_maskedTextProvider);
  }

  private string GetFormattedString(MaskedTextProvider provider, string text)
  {
    if (provider.Mask.StartsWith(">"))
      return text.ToUpper();
    if (provider.Mask.StartsWith("<"))
      return text.ToLower();
    bool includePrompt = !this.IsReadOnly && (!this.HidePromptOnLeave || this.IsFocused);
    return provider.ToString(false, includePrompt, true, 0, this.m_maskedTextProvider.Length);
  }

  private string GetSelectedText()
  {
    int selectionLength = this.SelectionLength;
    return selectionLength == 0 ? string.Empty : this.m_maskedTextProvider.ToString(true, (this.ClipboardMaskFormat & MaskFormat.IncludePrompt) != 0, (this.ClipboardMaskFormat & MaskFormat.IncludeLiterals) != 0, this.SelectionStart, selectionLength);
  }
}
