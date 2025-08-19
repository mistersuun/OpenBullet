// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridTextColumn
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridTextColumn : DataGridBoundColumn
{
  public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(DataGridColumn.NotifyPropertyChangeForRefreshContent)));
  private static Style _defaultElementStyle;
  private static Style _defaultEditingElementStyle;

  static DataGridTextColumn()
  {
    DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridTextColumn.DefaultElementStyle));
    DataGridBoundColumn.EditingElementStyleProperty.OverrideMetadata(typeof (DataGridTextColumn), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridTextColumn.DefaultEditingElementStyle));
  }

  public static Style DefaultElementStyle
  {
    get
    {
      if (DataGridTextColumn._defaultElementStyle == null)
      {
        Style style = new Style(typeof (TextBlock));
        style.Setters.Add((SetterBase) new Setter(FrameworkElement.MarginProperty, (object) new Thickness(2.0, 0.0, 2.0, 0.0)));
        style.Seal();
        DataGridTextColumn._defaultElementStyle = style;
      }
      return DataGridTextColumn._defaultElementStyle;
    }
  }

  public static Style DefaultEditingElementStyle
  {
    get
    {
      if (DataGridTextColumn._defaultEditingElementStyle == null)
      {
        Style style = new Style(typeof (TextBox));
        style.Setters.Add((SetterBase) new Setter(Control.BorderThicknessProperty, (object) new Thickness(0.0)));
        style.Setters.Add((SetterBase) new Setter(Control.PaddingProperty, (object) new Thickness(0.0)));
        style.Seal();
        DataGridTextColumn._defaultEditingElementStyle = style;
      }
      return DataGridTextColumn._defaultEditingElementStyle;
    }
  }

  protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
  {
    TextBlock element = new TextBlock();
    this.SyncProperties((FrameworkElement) element);
    this.ApplyStyle(false, false, (FrameworkElement) element);
    this.ApplyBinding((DependencyObject) element, TextBlock.TextProperty);
    return (FrameworkElement) element;
  }

  protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
  {
    TextBox editingElement = new TextBox();
    this.SyncProperties((FrameworkElement) editingElement);
    this.ApplyStyle(true, false, (FrameworkElement) editingElement);
    this.ApplyBinding((DependencyObject) editingElement, TextBox.TextProperty);
    return (FrameworkElement) editingElement;
  }

  private void SyncProperties(FrameworkElement e)
  {
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) e, TextElement.FontFamilyProperty, DataGridTextColumn.FontFamilyProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) e, TextElement.FontSizeProperty, DataGridTextColumn.FontSizeProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) e, TextElement.FontStyleProperty, DataGridTextColumn.FontStyleProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) e, TextElement.FontWeightProperty, DataGridTextColumn.FontWeightProperty);
    DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) e, TextElement.ForegroundProperty, DataGridTextColumn.ForegroundProperty);
  }

  protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
  {
    if (element is DataGridCell dataGridCell && dataGridCell.Content is FrameworkElement content)
    {
      switch (propertyName)
      {
        case "FontFamily":
          DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, TextElement.FontFamilyProperty, DataGridTextColumn.FontFamilyProperty);
          break;
        case "FontSize":
          DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, TextElement.FontSizeProperty, DataGridTextColumn.FontSizeProperty);
          break;
        case "FontStyle":
          DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, TextElement.FontStyleProperty, DataGridTextColumn.FontStyleProperty);
          break;
        case "FontWeight":
          DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, TextElement.FontWeightProperty, DataGridTextColumn.FontWeightProperty);
          break;
        case "Foreground":
          DataGridHelper.SyncColumnProperty((DependencyObject) this, (DependencyObject) content, TextElement.ForegroundProperty, DataGridTextColumn.ForegroundProperty);
          break;
      }
    }
    base.RefreshCellContent(element, propertyName);
  }

  protected override object PrepareCellForEdit(
    FrameworkElement editingElement,
    RoutedEventArgs editingEventArgs)
  {
    if (!(editingElement is TextBox textBox))
      return (object) null;
    textBox.Focus();
    string text1 = textBox.Text;
    switch (editingEventArgs)
    {
      case TextCompositionEventArgs compositionEventArgs:
        string text2 = compositionEventArgs.Text;
        textBox.Text = text2;
        textBox.Select(text2.Length, 0);
        break;
      case MouseButtonEventArgs _:
        if (DataGridTextColumn.PlaceCaretOnTextBox(textBox, Mouse.GetPosition((IInputElement) textBox)))
          break;
        goto default;
      default:
        textBox.SelectAll();
        break;
    }
    return (object) text1;
  }

  private static bool PlaceCaretOnTextBox(TextBox textBox, Point position)
  {
    int characterIndexFromPoint = textBox.GetCharacterIndexFromPoint(position, false);
    if (characterIndexFromPoint < 0)
      return false;
    textBox.Select(characterIndexFromPoint, 0);
    return true;
  }

  protected override bool CommitCellEdit(FrameworkElement editingElement)
  {
    if (!(editingElement is TextBox element))
      return true;
    DataGridHelper.UpdateSource((FrameworkElement) element, TextBox.TextProperty);
    return !Validation.GetHasError((DependencyObject) element);
  }

  protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
  {
    if (!(editingElement is TextBox element))
      return;
    DataGridHelper.UpdateTarget((FrameworkElement) element, TextBox.TextProperty);
  }

  internal override void OnInput(InputEventArgs e)
  {
    if (!DataGridHelper.HasNonEscapeCharacters(e as TextCompositionEventArgs))
      return;
    this.BeginEdit(e);
  }

  public FontFamily FontFamily
  {
    get => (FontFamily) this.GetValue(DataGridTextColumn.FontFamilyProperty);
    set => this.SetValue(DataGridTextColumn.FontFamilyProperty, (object) value);
  }

  [Localizability(LocalizationCategory.None)]
  [TypeConverter(typeof (FontSizeConverter))]
  public double FontSize
  {
    get => (double) this.GetValue(DataGridTextColumn.FontSizeProperty);
    set => this.SetValue(DataGridTextColumn.FontSizeProperty, (object) value);
  }

  public FontStyle FontStyle
  {
    get => (FontStyle) this.GetValue(DataGridTextColumn.FontStyleProperty);
    set => this.SetValue(DataGridTextColumn.FontStyleProperty, (object) value);
  }

  public FontWeight FontWeight
  {
    get => (FontWeight) this.GetValue(DataGridTextColumn.FontWeightProperty);
    set => this.SetValue(DataGridTextColumn.FontWeightProperty, (object) value);
  }

  public Brush Foreground
  {
    get => (Brush) this.GetValue(DataGridTextColumn.ForegroundProperty);
    set => this.SetValue(DataGridTextColumn.ForegroundProperty, (object) value);
  }
}
