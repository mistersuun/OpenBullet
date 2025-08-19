// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorPicker
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_AvailableColors", Type = typeof (ListBox))]
[TemplatePart(Name = "PART_StandardColors", Type = typeof (ListBox))]
[TemplatePart(Name = "PART_RecentColors", Type = typeof (ListBox))]
[TemplatePart(Name = "PART_ColorPickerToggleButton", Type = typeof (ToggleButton))]
[TemplatePart(Name = "PART_ColorPickerPalettePopup", Type = typeof (Popup))]
public class ColorPicker : Control
{
  private const string PART_AvailableColors = "PART_AvailableColors";
  private const string PART_StandardColors = "PART_StandardColors";
  private const string PART_RecentColors = "PART_RecentColors";
  private const string PART_ColorPickerToggleButton = "PART_ColorPickerToggleButton";
  private const string PART_ColorPickerPalettePopup = "PART_ColorPickerPalettePopup";
  private ListBox _availableColors;
  private ListBox _standardColors;
  private ListBox _recentColors;
  private ToggleButton _toggleButton;
  private Popup _popup;
  private Color? _initialColor;
  private bool _selectionChanged;
  public static readonly DependencyProperty AdvancedButtonHeaderProperty = DependencyProperty.Register(nameof (AdvancedButtonHeader), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) "Advanced"));
  public static readonly DependencyProperty AvailableColorsProperty = DependencyProperty.Register(nameof (AvailableColors), typeof (ObservableCollection<ColorItem>), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) ColorPicker.CreateAvailableColors()));
  public static readonly DependencyProperty AvailableColorsSortingModeProperty = DependencyProperty.Register(nameof (AvailableColorsSortingMode), typeof (ColorSortingMode), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) ColorSortingMode.Alphabetical, new PropertyChangedCallback(ColorPicker.OnAvailableColorsSortingModeChanged)));
  public static readonly DependencyProperty AvailableColorsHeaderProperty = DependencyProperty.Register(nameof (AvailableColorsHeader), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) "Available Colors"));
  public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof (ButtonStyle), typeof (Style), typeof (ColorPicker));
  public static readonly DependencyProperty DisplayColorAndNameProperty = DependencyProperty.Register(nameof (DisplayColorAndName), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty DisplayColorTooltipProperty = DependencyProperty.Register(nameof (DisplayColorTooltip), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ColorModeProperty = DependencyProperty.Register(nameof (ColorMode), typeof (ColorMode), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) ColorMode.ColorPalette));
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ColorPicker.OnIsOpenChanged)));
  public static readonly DependencyProperty MaxDropDownWidthProperty = DependencyProperty.Register(nameof (MaxDropDownWidth), typeof (double), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) 214.0));
  public static readonly DependencyProperty RecentColorsProperty = DependencyProperty.Register(nameof (RecentColors), typeof (ObservableCollection<ColorItem>), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty RecentColorsHeaderProperty = DependencyProperty.Register(nameof (RecentColorsHeader), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) "Recent Colors"));
  public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof (SelectedColor), typeof (Color?), typeof (ColorPicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ColorPicker.OnSelectedColorPropertyChanged)));
  public static readonly DependencyProperty SelectedColorTextProperty = DependencyProperty.Register(nameof (SelectedColorText), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) ""));
  public static readonly DependencyProperty ShowTabHeadersProperty = DependencyProperty.Register(nameof (ShowTabHeaders), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowAvailableColorsProperty = DependencyProperty.Register(nameof (ShowAvailableColors), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowRecentColorsProperty = DependencyProperty.Register(nameof (ShowRecentColors), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ShowStandardColorsProperty = DependencyProperty.Register(nameof (ShowStandardColors), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowDropDownButtonProperty = DependencyProperty.Register(nameof (ShowDropDownButton), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty StandardButtonHeaderProperty = DependencyProperty.Register(nameof (StandardButtonHeader), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) "Standard"));
  public static readonly DependencyProperty StandardColorsProperty = DependencyProperty.Register(nameof (StandardColors), typeof (ObservableCollection<ColorItem>), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) ColorPicker.CreateStandardColors()));
  public static readonly DependencyProperty StandardColorsHeaderProperty = DependencyProperty.Register(nameof (StandardColorsHeader), typeof (string), typeof (ColorPicker), (PropertyMetadata) new UIPropertyMetadata((object) "Standard Colors"));
  public static readonly DependencyProperty UsingAlphaChannelProperty = DependencyProperty.Register(nameof (UsingAlphaChannel), typeof (bool), typeof (ColorPicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ColorPicker.OnUsingAlphaChannelPropertyChanged)));
  public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<Color?>), typeof (ColorPicker));
  public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent(nameof (OpenedEvent), RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ColorPicker));
  public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(nameof (ClosedEvent), RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ColorPicker));

  public string AdvancedButtonHeader
  {
    get => (string) this.GetValue(ColorPicker.AdvancedButtonHeaderProperty);
    set => this.SetValue(ColorPicker.AdvancedButtonHeaderProperty, (object) value);
  }

  public ObservableCollection<ColorItem> AvailableColors
  {
    get => (ObservableCollection<ColorItem>) this.GetValue(ColorPicker.AvailableColorsProperty);
    set => this.SetValue(ColorPicker.AvailableColorsProperty, (object) value);
  }

  public ColorSortingMode AvailableColorsSortingMode
  {
    get => (ColorSortingMode) this.GetValue(ColorPicker.AvailableColorsSortingModeProperty);
    set => this.SetValue(ColorPicker.AvailableColorsSortingModeProperty, (object) value);
  }

  private static void OnAvailableColorsSortingModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((ColorPicker) d)?.OnAvailableColorsSortingModeChanged((ColorSortingMode) e.OldValue, (ColorSortingMode) e.NewValue);
  }

  private void OnAvailableColorsSortingModeChanged(
    ColorSortingMode oldValue,
    ColorSortingMode newValue)
  {
    ListCollectionView defaultView = (ListCollectionView) CollectionViewSource.GetDefaultView((object) this.AvailableColors);
    if (defaultView == null)
      return;
    defaultView.CustomSort = this.AvailableColorsSortingMode == ColorSortingMode.HueSaturationBrightness ? (IComparer) new ColorSorter() : (IComparer) null;
  }

  public string AvailableColorsHeader
  {
    get => (string) this.GetValue(ColorPicker.AvailableColorsHeaderProperty);
    set => this.SetValue(ColorPicker.AvailableColorsHeaderProperty, (object) value);
  }

  public Style ButtonStyle
  {
    get => (Style) this.GetValue(ColorPicker.ButtonStyleProperty);
    set => this.SetValue(ColorPicker.ButtonStyleProperty, (object) value);
  }

  public bool DisplayColorAndName
  {
    get => (bool) this.GetValue(ColorPicker.DisplayColorAndNameProperty);
    set => this.SetValue(ColorPicker.DisplayColorAndNameProperty, (object) value);
  }

  public bool DisplayColorTooltip
  {
    get => (bool) this.GetValue(ColorPicker.DisplayColorTooltipProperty);
    set => this.SetValue(ColorPicker.DisplayColorTooltipProperty, (object) value);
  }

  public ColorMode ColorMode
  {
    get => (ColorMode) this.GetValue(ColorPicker.ColorModeProperty);
    set => this.SetValue(ColorPicker.ColorModeProperty, (object) value);
  }

  public bool IsOpen
  {
    get => (bool) this.GetValue(ColorPicker.IsOpenProperty);
    set => this.SetValue(ColorPicker.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((ColorPicker) d)?.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  private void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    if (newValue)
      this._initialColor = this.SelectedColor;
    this.RaiseEvent(new RoutedEventArgs(newValue ? ColorPicker.OpenedEvent : ColorPicker.ClosedEvent, (object) this));
  }

  public double MaxDropDownWidth
  {
    get => (double) this.GetValue(ColorPicker.MaxDropDownWidthProperty);
    set => this.SetValue(ColorPicker.MaxDropDownWidthProperty, (object) value);
  }

  private static void OnMaxDropDownWidthChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorPicker colorPicker))
      return;
    colorPicker.OnMaxDropDownWidthChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnMaxDropDownWidthChanged(double oldValue, double newValue)
  {
  }

  public ObservableCollection<ColorItem> RecentColors
  {
    get => (ObservableCollection<ColorItem>) this.GetValue(ColorPicker.RecentColorsProperty);
    set => this.SetValue(ColorPicker.RecentColorsProperty, (object) value);
  }

  public string RecentColorsHeader
  {
    get => (string) this.GetValue(ColorPicker.RecentColorsHeaderProperty);
    set => this.SetValue(ColorPicker.RecentColorsHeaderProperty, (object) value);
  }

  public Color? SelectedColor
  {
    get => (Color?) this.GetValue(ColorPicker.SelectedColorProperty);
    set => this.SetValue(ColorPicker.SelectedColorProperty, (object) value);
  }

  private static void OnSelectedColorPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((ColorPicker) d)?.OnSelectedColorChanged((Color?) e.OldValue, (Color?) e.NewValue);
  }

  private void OnSelectedColorChanged(Color? oldValue, Color? newValue)
  {
    this.SelectedColorText = this.GetFormatedColorString(newValue);
    RoutedPropertyChangedEventArgs<Color?> e = new RoutedPropertyChangedEventArgs<Color?>(oldValue, newValue);
    e.RoutedEvent = ColorPicker.SelectedColorChangedEvent;
    this.RaiseEvent((RoutedEventArgs) e);
  }

  public string SelectedColorText
  {
    get => (string) this.GetValue(ColorPicker.SelectedColorTextProperty);
    protected set => this.SetValue(ColorPicker.SelectedColorTextProperty, (object) value);
  }

  public bool ShowTabHeaders
  {
    get => (bool) this.GetValue(ColorPicker.ShowTabHeadersProperty);
    set => this.SetValue(ColorPicker.ShowTabHeadersProperty, (object) value);
  }

  public bool ShowAvailableColors
  {
    get => (bool) this.GetValue(ColorPicker.ShowAvailableColorsProperty);
    set => this.SetValue(ColorPicker.ShowAvailableColorsProperty, (object) value);
  }

  public bool ShowRecentColors
  {
    get => (bool) this.GetValue(ColorPicker.ShowRecentColorsProperty);
    set => this.SetValue(ColorPicker.ShowRecentColorsProperty, (object) value);
  }

  public bool ShowStandardColors
  {
    get => (bool) this.GetValue(ColorPicker.ShowStandardColorsProperty);
    set => this.SetValue(ColorPicker.ShowStandardColorsProperty, (object) value);
  }

  public bool ShowDropDownButton
  {
    get => (bool) this.GetValue(ColorPicker.ShowDropDownButtonProperty);
    set => this.SetValue(ColorPicker.ShowDropDownButtonProperty, (object) value);
  }

  public string StandardButtonHeader
  {
    get => (string) this.GetValue(ColorPicker.StandardButtonHeaderProperty);
    set => this.SetValue(ColorPicker.StandardButtonHeaderProperty, (object) value);
  }

  public ObservableCollection<ColorItem> StandardColors
  {
    get => (ObservableCollection<ColorItem>) this.GetValue(ColorPicker.StandardColorsProperty);
    set => this.SetValue(ColorPicker.StandardColorsProperty, (object) value);
  }

  public string StandardColorsHeader
  {
    get => (string) this.GetValue(ColorPicker.StandardColorsHeaderProperty);
    set => this.SetValue(ColorPicker.StandardColorsHeaderProperty, (object) value);
  }

  public bool UsingAlphaChannel
  {
    get => (bool) this.GetValue(ColorPicker.UsingAlphaChannelProperty);
    set => this.SetValue(ColorPicker.UsingAlphaChannelProperty, (object) value);
  }

  private static void OnUsingAlphaChannelPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((ColorPicker) d)?.OnUsingAlphaChannelChanged();
  }

  private void OnUsingAlphaChannelChanged()
  {
    this.SelectedColorText = this.GetFormatedColorString(this.SelectedColor);
  }

  static ColorPicker()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorPicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ColorPicker)));
  }

  public ColorPicker()
  {
    this.SetCurrentValue(ColorPicker.RecentColorsProperty, (object) new ObservableCollection<ColorItem>());
    Keyboard.AddKeyDownHandler((DependencyObject) this, new KeyEventHandler(this.OnKeyDown));
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._availableColors != null)
      this._availableColors.SelectionChanged -= new SelectionChangedEventHandler(this.Color_SelectionChanged);
    this._availableColors = this.GetTemplateChild("PART_AvailableColors") as ListBox;
    if (this._availableColors != null)
      this._availableColors.SelectionChanged += new SelectionChangedEventHandler(this.Color_SelectionChanged);
    if (this._standardColors != null)
      this._standardColors.SelectionChanged -= new SelectionChangedEventHandler(this.Color_SelectionChanged);
    this._standardColors = this.GetTemplateChild("PART_StandardColors") as ListBox;
    if (this._standardColors != null)
      this._standardColors.SelectionChanged += new SelectionChangedEventHandler(this.Color_SelectionChanged);
    if (this._recentColors != null)
      this._recentColors.SelectionChanged -= new SelectionChangedEventHandler(this.Color_SelectionChanged);
    this._recentColors = this.GetTemplateChild("PART_RecentColors") as ListBox;
    if (this._recentColors != null)
      this._recentColors.SelectionChanged += new SelectionChangedEventHandler(this.Color_SelectionChanged);
    if (this._popup != null)
      this._popup.Opened -= new EventHandler(this.Popup_Opened);
    this._popup = this.GetTemplateChild("PART_ColorPickerPalettePopup") as Popup;
    if (this._popup != null)
      this._popup.Opened += new EventHandler(this.Popup_Opened);
    this._toggleButton = this.Template.FindName("PART_ColorPickerToggleButton", (FrameworkElement) this) as ToggleButton;
  }

  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    base.OnMouseUp(e);
    if (!this._selectionChanged)
      return;
    this.CloseColorPicker(true);
    this._selectionChanged = false;
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsOpen = true;
      e.Handled = true;
    }
    else if (KeyboardUtilities.IsKeyModifyingPopupState(e))
    {
      this.CloseColorPicker(true);
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Escape)
        return;
      this.SelectedColor = this._initialColor;
      this.CloseColorPicker(true);
      e.Handled = true;
    }
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.CloseColorPicker(true);
  }

  private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    ListBox listBox = (ListBox) sender;
    if (e.AddedItems.Count <= 0)
      return;
    ColorItem addedItem = (ColorItem) e.AddedItems[0];
    this.SelectedColor = addedItem.Color;
    if (!string.IsNullOrEmpty(addedItem.Name))
      this.SelectedColorText = addedItem.Name;
    this.UpdateRecentColors(addedItem);
    this._selectionChanged = true;
    listBox.SelectedIndex = -1;
  }

  private void Popup_Opened(object sender, EventArgs e)
  {
    if (this._availableColors != null && this.ShowAvailableColors)
      this.FocusOnListBoxItem(this._availableColors);
    else if (this._standardColors != null && this.ShowStandardColors)
    {
      this.FocusOnListBoxItem(this._standardColors);
    }
    else
    {
      if (this._recentColors == null || !this.ShowRecentColors)
        return;
      this.FocusOnListBoxItem(this._recentColors);
    }
  }

  private void FocusOnListBoxItem(ListBox listBox)
  {
    ListBoxItem listBoxItem = (ListBoxItem) listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
    if (listBoxItem == null && listBox.Items.Count > 0)
      listBoxItem = (ListBoxItem) listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[0]);
    listBoxItem?.Focus();
  }

  public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
  {
    add => this.AddHandler(ColorPicker.SelectedColorChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(ColorPicker.SelectedColorChangedEvent, (Delegate) value);
  }

  public event RoutedEventHandler Opened
  {
    add => this.AddHandler(ColorPicker.OpenedEvent, (Delegate) value);
    remove => this.RemoveHandler(ColorPicker.OpenedEvent, (Delegate) value);
  }

  public event RoutedEventHandler Closed
  {
    add => this.AddHandler(ColorPicker.ClosedEvent, (Delegate) value);
    remove => this.RemoveHandler(ColorPicker.ClosedEvent, (Delegate) value);
  }

  private void CloseColorPicker(bool isFocusOnColorPicker)
  {
    if (this.IsOpen)
      this.IsOpen = false;
    this.ReleaseMouseCapture();
    if (isFocusOnColorPicker && this._toggleButton != null)
      this._toggleButton.Focus();
    this.UpdateRecentColors(new ColorItem(this.SelectedColor, this.SelectedColorText));
  }

  private void UpdateRecentColors(ColorItem colorItem)
  {
    if (!this.RecentColors.Contains(colorItem))
      this.RecentColors.Add(colorItem);
    if (this.RecentColors.Count <= 10)
      return;
    this.RecentColors.RemoveAt(0);
  }

  private string GetFormatedColorString(Color? colorToFormat)
  {
    return !colorToFormat.HasValue || !colorToFormat.HasValue ? string.Empty : ColorUtilities.FormatColorString(colorToFormat.Value.GetColorName(), this.UsingAlphaChannel);
  }

  private static ObservableCollection<ColorItem> CreateStandardColors()
  {
    ObservableCollection<ColorItem> standardColors = new ObservableCollection<ColorItem>();
    standardColors.Add(new ColorItem(new Color?(Colors.Transparent), "Transparent"));
    standardColors.Add(new ColorItem(new Color?(Colors.White), "White"));
    standardColors.Add(new ColorItem(new Color?(Colors.Gray), "Gray"));
    standardColors.Add(new ColorItem(new Color?(Colors.Black), "Black"));
    standardColors.Add(new ColorItem(new Color?(Colors.Red), "Red"));
    standardColors.Add(new ColorItem(new Color?(Colors.Green), "Green"));
    standardColors.Add(new ColorItem(new Color?(Colors.Blue), "Blue"));
    standardColors.Add(new ColorItem(new Color?(Colors.Yellow), "Yellow"));
    standardColors.Add(new ColorItem(new Color?(Colors.Orange), "Orange"));
    standardColors.Add(new ColorItem(new Color?(Colors.Purple), "Purple"));
    return standardColors;
  }

  private static ObservableCollection<ColorItem> CreateAvailableColors()
  {
    ObservableCollection<ColorItem> availableColors = new ObservableCollection<ColorItem>();
    foreach (KeyValuePair<string, Color> knownColor in ColorUtilities.KnownColors)
    {
      if (!string.Equals(knownColor.Key, "Transparent"))
      {
        ColorItem colorItem = new ColorItem(new Color?(knownColor.Value), knownColor.Key);
        if (!availableColors.Contains(colorItem))
          availableColors.Add(colorItem);
      }
    }
    return availableColors;
  }
}
