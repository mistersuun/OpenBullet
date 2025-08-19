// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.RichTextBoxFormatBar
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class RichTextBoxFormatBar : Control, IRichTextBoxFormatBar
{
  private ComboBox _cmbFontFamilies;
  private ComboBox _cmbFontSizes;
  private ColorPicker _cmbFontBackgroundColor;
  private ColorPicker _cmbFontColor;
  private ToggleButton _btnNumbers;
  private ToggleButton _btnBullets;
  private ToggleButton _btnBold;
  private ToggleButton _btnItalic;
  private ToggleButton _btnUnderline;
  private ToggleButton _btnAlignLeft;
  private ToggleButton _btnAlignCenter;
  private ToggleButton _btnAlignRight;
  private Thumb _dragWidget;
  private bool _waitingForMouseOver;
  public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof (Target), typeof (System.Windows.Controls.RichTextBox), typeof (RichTextBoxFormatBar), new PropertyMetadata((object) null, new PropertyChangedCallback(RichTextBoxFormatBar.OnRichTextBoxPropertyChanged)));

  public static double[] FontSizes
  {
    get
    {
      return new double[54]
      {
        3.0,
        4.0,
        5.0,
        6.0,
        6.5,
        7.0,
        7.5,
        8.0,
        8.5,
        9.0,
        9.5,
        10.0,
        10.5,
        11.0,
        11.5,
        12.0,
        12.5,
        13.0,
        13.5,
        14.0,
        15.0,
        16.0,
        17.0,
        18.0,
        19.0,
        20.0,
        22.0,
        24.0,
        26.0,
        28.0,
        30.0,
        32.0,
        34.0,
        36.0,
        38.0,
        40.0,
        44.0,
        48.0,
        52.0,
        56.0,
        60.0,
        64.0,
        68.0,
        72.0,
        76.0,
        80.0,
        88.0,
        96.0,
        104.0,
        112.0,
        120.0,
        128.0,
        136.0,
        144.0
      };
    }
  }

  static RichTextBoxFormatBar()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (RichTextBoxFormatBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (RichTextBoxFormatBar)));
  }

  private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count == 0)
      return;
    FontFamily addedItem = (FontFamily) e.AddedItems[0];
    this.ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, (object) addedItem);
    this._waitingForMouseOver = true;
  }

  private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count == 0)
      return;
    this.ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, e.AddedItems[0]);
    this._waitingForMouseOver = true;
  }

  private void FontColor_SelectedColorChanged(
    object sender,
    RoutedPropertyChangedEventArgs<Color?> e)
  {
    Color? newValue = e.NewValue;
    this.ApplyPropertyValueToSelectedText(TextElement.ForegroundProperty, newValue.HasValue ? (object) new SolidColorBrush(newValue.Value) : (object) (SolidColorBrush) null);
    this._waitingForMouseOver = true;
  }

  private void FontBackgroundColor_SelectedColorChanged(
    object sender,
    RoutedPropertyChangedEventArgs<Color?> e)
  {
    Color? newValue = e.NewValue;
    this.ApplyPropertyValueToSelectedText(TextElement.BackgroundProperty, newValue.HasValue ? (object) new SolidColorBrush(newValue.Value) : (object) (SolidColorBrush) null);
    this._waitingForMouseOver = true;
  }

  private void Bullets_Clicked(object sender, RoutedEventArgs e)
  {
    if (!this.BothSelectionListsAreChecked() || this._btnNumbers == null)
      return;
    this._btnNumbers.IsChecked = new bool?(false);
  }

  private void Numbers_Clicked(object sender, RoutedEventArgs e)
  {
    if (!this.BothSelectionListsAreChecked() || this._btnBullets == null)
      return;
    this._btnBullets.IsChecked = new bool?(false);
  }

  private void DragWidget_DragDelta(object sender, DragDeltaEventArgs e) => this.ProcessMove(e);

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    this._waitingForMouseOver = false;
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._dragWidget != null)
      this._dragWidget.DragDelta -= new DragDeltaEventHandler(this.DragWidget_DragDelta);
    if (this._cmbFontFamilies != null)
      this._cmbFontFamilies.SelectionChanged -= new SelectionChangedEventHandler(this.FontFamily_SelectionChanged);
    if (this._cmbFontSizes != null)
      this._cmbFontSizes.SelectionChanged -= new SelectionChangedEventHandler(this.FontSize_SelectionChanged);
    if (this._btnBullets != null)
      this._btnBullets.Click -= new RoutedEventHandler(this.Bullets_Clicked);
    if (this._btnNumbers != null)
      this._btnNumbers.Click -= new RoutedEventHandler(this.Numbers_Clicked);
    if (this._cmbFontBackgroundColor != null)
      this._cmbFontBackgroundColor.SelectedColorChanged -= new RoutedPropertyChangedEventHandler<Color?>(this.FontBackgroundColor_SelectedColorChanged);
    if (this._cmbFontColor != null)
      this._cmbFontColor.SelectedColorChanged -= new RoutedPropertyChangedEventHandler<Color?>(this.FontColor_SelectedColorChanged);
    this.GetTemplateComponent<ComboBox>(ref this._cmbFontFamilies, "_cmbFontFamilies");
    this.GetTemplateComponent<ComboBox>(ref this._cmbFontSizes, "_cmbFontSizes");
    this.GetTemplateComponent<ColorPicker>(ref this._cmbFontBackgroundColor, "_cmbFontBackgroundColor");
    this.GetTemplateComponent<ColorPicker>(ref this._cmbFontColor, "_cmbFontColor");
    this.GetTemplateComponent<ToggleButton>(ref this._btnNumbers, "_btnNumbers");
    this.GetTemplateComponent<ToggleButton>(ref this._btnBullets, "_btnBullets");
    this.GetTemplateComponent<ToggleButton>(ref this._btnBold, "_btnBold");
    this.GetTemplateComponent<ToggleButton>(ref this._btnItalic, "_btnItalic");
    this.GetTemplateComponent<ToggleButton>(ref this._btnUnderline, "_btnUnderline");
    this.GetTemplateComponent<ToggleButton>(ref this._btnAlignLeft, "_btnAlignLeft");
    this.GetTemplateComponent<ToggleButton>(ref this._btnAlignCenter, "_btnAlignCenter");
    this.GetTemplateComponent<ToggleButton>(ref this._btnAlignRight, "_btnAlignRight");
    this.GetTemplateComponent<Thumb>(ref this._dragWidget, "_dragWidget");
    if (this._dragWidget != null)
      this._dragWidget.DragDelta += new DragDeltaEventHandler(this.DragWidget_DragDelta);
    if (this._cmbFontFamilies != null)
    {
      this._cmbFontFamilies.ItemsSource = (IEnumerable) FontUtilities.Families.OrderBy<FontFamily, string>((Func<FontFamily, string>) (fontFamily => fontFamily.Source));
      this._cmbFontFamilies.SelectionChanged += new SelectionChangedEventHandler(this.FontFamily_SelectionChanged);
    }
    if (this._cmbFontSizes != null)
    {
      this._cmbFontSizes.ItemsSource = (IEnumerable) RichTextBoxFormatBar.FontSizes;
      this._cmbFontSizes.SelectionChanged += new SelectionChangedEventHandler(this.FontSize_SelectionChanged);
    }
    if (this._btnBullets != null)
      this._btnBullets.Click += new RoutedEventHandler(this.Bullets_Clicked);
    if (this._btnNumbers != null)
      this._btnNumbers.Click += new RoutedEventHandler(this.Numbers_Clicked);
    if (this._cmbFontBackgroundColor != null)
      this._cmbFontBackgroundColor.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(this.FontBackgroundColor_SelectedColorChanged);
    if (this._cmbFontColor != null)
      this._cmbFontColor.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(this.FontColor_SelectedColorChanged);
    this.Update();
  }

  private void GetTemplateComponent<T>(ref T partMember, string partName) where T : class
  {
    partMember = this.Template != null ? this.Template.FindName(partName, (FrameworkElement) this) as T : default (T);
  }

  private void UpdateToggleButtonState()
  {
    this.UpdateItemCheckedState(this._btnBold, TextElement.FontWeightProperty, (object) FontWeights.Bold);
    this.UpdateItemCheckedState(this._btnItalic, TextElement.FontStyleProperty, (object) FontStyles.Italic);
    this.UpdateItemCheckedState(this._btnUnderline, Inline.TextDecorationsProperty, (object) TextDecorations.Underline);
    this.UpdateItemCheckedState(this._btnAlignLeft, Block.TextAlignmentProperty, (object) TextAlignment.Left);
    this.UpdateItemCheckedState(this._btnAlignCenter, Block.TextAlignmentProperty, (object) TextAlignment.Center);
    this.UpdateItemCheckedState(this._btnAlignRight, Block.TextAlignmentProperty, (object) TextAlignment.Right);
  }

  private void UpdateItemCheckedState(
    ToggleButton button,
    DependencyProperty formattingProperty,
    object expectedValue)
  {
    object obj = DependencyProperty.UnsetValue;
    if (this.Target != null && this.Target.Selection != null)
      obj = this.Target.Selection.GetPropertyValue(formattingProperty);
    if (obj == DependencyProperty.UnsetValue || button == null)
      return;
    button.IsChecked = new bool?(obj != null && obj != null && obj.Equals(expectedValue));
  }

  private void UpdateSelectedFontFamily()
  {
    object obj = DependencyProperty.UnsetValue;
    if (this.Target != null && this.Target.Selection != null)
      obj = this.Target.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
    if (obj == DependencyProperty.UnsetValue)
      return;
    FontFamily fontFamily = (FontFamily) obj;
    if (fontFamily == null || this._cmbFontFamilies == null)
      return;
    this._cmbFontFamilies.SelectedItem = (object) fontFamily;
  }

  private void UpdateSelectedFontSize()
  {
    object obj = DependencyProperty.UnsetValue;
    if (this.Target != null && this.Target.Selection != null)
      obj = this.Target.Selection.GetPropertyValue(TextElement.FontSizeProperty);
    if (obj == DependencyProperty.UnsetValue || this._cmbFontSizes == null)
      return;
    this._cmbFontSizes.SelectedValue = obj;
  }

  private void UpdateFontColor()
  {
    object obj = DependencyProperty.UnsetValue;
    if (this.Target != null && this.Target.Selection != null)
      obj = this.Target.Selection.GetPropertyValue(TextElement.ForegroundProperty);
    if (obj == DependencyProperty.UnsetValue)
      return;
    Color? nullable = obj == null ? new Color?() : new Color?(((SolidColorBrush) obj).Color);
    if (this._cmbFontColor == null)
      return;
    this._cmbFontColor.SelectedColor = nullable;
  }

  private void UpdateFontBackgroundColor()
  {
    object obj = DependencyProperty.UnsetValue;
    if (this.Target != null && this.Target.Selection != null)
      obj = this.Target.Selection.GetPropertyValue(TextElement.BackgroundProperty);
    if (obj == DependencyProperty.UnsetValue)
      return;
    Color? nullable = obj == null ? new Color?() : new Color?(((SolidColorBrush) obj).Color);
    if (this._cmbFontBackgroundColor == null)
      return;
    this._cmbFontBackgroundColor.SelectedColor = nullable;
  }

  private void UpdateSelectionListType()
  {
    if (this._btnNumbers == null || this._btnBullets == null)
      return;
    this._btnBullets.IsChecked = new bool?(false);
    this._btnNumbers.IsChecked = new bool?(false);
    Paragraph paragraph1 = this.Target == null || this.Target.Selection == null ? (Paragraph) null : this.Target.Selection.Start.Paragraph;
    Paragraph paragraph2 = this.Target == null || this.Target.Selection == null ? (Paragraph) null : this.Target.Selection.End.Paragraph;
    if (paragraph1 == null || paragraph2 == null || !(paragraph1.Parent is System.Windows.Documents.ListItem) || !(paragraph2.Parent is System.Windows.Documents.ListItem) || ((System.Windows.Documents.ListItem) paragraph1.Parent).List != ((System.Windows.Documents.ListItem) paragraph2.Parent).List)
      return;
    switch (((System.Windows.Documents.ListItem) paragraph1.Parent).List.MarkerStyle)
    {
      case TextMarkerStyle.Disc:
        this._btnBullets.IsChecked = new bool?(true);
        break;
      case TextMarkerStyle.Decimal:
        this._btnNumbers.IsChecked = new bool?(true);
        break;
    }
  }

  private bool BothSelectionListsAreChecked()
  {
    if (this._btnBullets != null)
    {
      bool? isChecked = this._btnBullets.IsChecked;
      bool flag1 = true;
      if (isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue && this._btnNumbers != null)
      {
        isChecked = this._btnNumbers.IsChecked;
        bool flag2 = true;
        return isChecked.GetValueOrDefault() == flag2 & isChecked.HasValue;
      }
    }
    return false;
  }

  private void ApplyPropertyValueToSelectedText(DependencyProperty formattingProperty, object value)
  {
    if (this.Target == null || this.Target.Selection == null)
      return;
    if (value is SolidColorBrush solidColorBrush && solidColorBrush.Color.Equals(Colors.Transparent))
      this.Target.Selection.ApplyPropertyValue(formattingProperty, (object) null);
    else
      this.Target.Selection.ApplyPropertyValue(formattingProperty, value);
  }

  private void ProcessMove(DragDeltaEventArgs e)
  {
    UIElementAdorner<Control> uiElementAdorner = AdornerLayer.GetAdornerLayer((Visual) this.Target).GetAdorners((UIElement) this.Target).OfType<UIElementAdorner<Control>>().First<UIElementAdorner<Control>>();
    uiElementAdorner.SetOffsets(uiElementAdorner.OffsetLeft + e.HorizontalChange, uiElementAdorner.OffsetTop + e.VerticalChange);
  }

  public System.Windows.Controls.RichTextBox Target
  {
    get => (System.Windows.Controls.RichTextBox) this.GetValue(RichTextBoxFormatBar.TargetProperty);
    set => this.SetValue(RichTextBoxFormatBar.TargetProperty, (object) value);
  }

  private static void OnRichTextBoxPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
  }

  public bool PreventDisplayFadeOut
  {
    get
    {
      return this._cmbFontFamilies != null && this._cmbFontFamilies.IsDropDownOpen || this._cmbFontSizes != null && this._cmbFontSizes.IsDropDownOpen || this._cmbFontBackgroundColor != null && this._cmbFontBackgroundColor.IsOpen || this._cmbFontColor != null && this._cmbFontColor.IsOpen || this._waitingForMouseOver;
    }
  }

  public void Update()
  {
    this.UpdateToggleButtonState();
    this.UpdateSelectedFontFamily();
    this.UpdateSelectedFontSize();
    this.UpdateFontColor();
    this.UpdateFontBackgroundColor();
    this.UpdateSelectionListType();
  }
}
