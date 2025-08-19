// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ButtonSpinner
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_IncreaseButton", Type = typeof (ButtonBase))]
[TemplatePart(Name = "PART_DecreaseButton", Type = typeof (ButtonBase))]
[System.Windows.Markup.ContentProperty("Content")]
public class ButtonSpinner : Spinner
{
  private const string PART_IncreaseButton = "PART_IncreaseButton";
  private const string PART_DecreaseButton = "PART_DecreaseButton";
  public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register(nameof (AllowSpin), typeof (bool), typeof (ButtonSpinner), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(ButtonSpinner.AllowSpinPropertyChanged)));
  public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register(nameof (ButtonSpinnerLocation), typeof (Location), typeof (ButtonSpinner), (PropertyMetadata) new UIPropertyMetadata((object) Location.Right));
  public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (object), typeof (ButtonSpinner), new PropertyMetadata((object) null, new PropertyChangedCallback(ButtonSpinner.OnContentPropertyChanged)));
  private ButtonBase _decreaseButton;
  private ButtonBase _increaseButton;
  public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register(nameof (ShowButtonSpinner), typeof (bool), typeof (ButtonSpinner), (PropertyMetadata) new UIPropertyMetadata((object) true));

  public bool AllowSpin
  {
    get => (bool) this.GetValue(ButtonSpinner.AllowSpinProperty);
    set => this.SetValue(ButtonSpinner.AllowSpinProperty, (object) value);
  }

  private static void AllowSpinPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as ButtonSpinner).OnAllowSpinChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  public Location ButtonSpinnerLocation
  {
    get => (Location) this.GetValue(ButtonSpinner.ButtonSpinnerLocationProperty);
    set => this.SetValue(ButtonSpinner.ButtonSpinnerLocationProperty, (object) value);
  }

  public object Content
  {
    get => this.GetValue(ButtonSpinner.ContentProperty);
    set => this.SetValue(ButtonSpinner.ContentProperty, value);
  }

  private static void OnContentPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as ButtonSpinner).OnContentChanged(e.OldValue, e.NewValue);
  }

  private ButtonBase DecreaseButton
  {
    get => this._decreaseButton;
    set
    {
      if (this._decreaseButton != null)
        this._decreaseButton.Click -= new RoutedEventHandler(this.OnButtonClick);
      this._decreaseButton = value;
      if (this._decreaseButton == null)
        return;
      this._decreaseButton.Click += new RoutedEventHandler(this.OnButtonClick);
    }
  }

  private ButtonBase IncreaseButton
  {
    get => this._increaseButton;
    set
    {
      if (this._increaseButton != null)
        this._increaseButton.Click -= new RoutedEventHandler(this.OnButtonClick);
      this._increaseButton = value;
      if (this._increaseButton == null)
        return;
      this._increaseButton.Click += new RoutedEventHandler(this.OnButtonClick);
    }
  }

  public bool ShowButtonSpinner
  {
    get => (bool) this.GetValue(ButtonSpinner.ShowButtonSpinnerProperty);
    set => this.SetValue(ButtonSpinner.ShowButtonSpinnerProperty, (object) value);
  }

  static ButtonSpinner()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ButtonSpinner), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ButtonSpinner)));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.IncreaseButton = this.GetTemplateChild("PART_IncreaseButton") as ButtonBase;
    this.DecreaseButton = this.GetTemplateChild("PART_DecreaseButton") as ButtonBase;
    this.SetButtonUsage();
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonUp(e);
    Point position;
    if (this.IncreaseButton != null && !this.IncreaseButton.IsEnabled)
    {
      position = e.GetPosition((IInputElement) this.IncreaseButton);
      if (position.X > 0.0 && position.X < this.IncreaseButton.ActualWidth && position.Y > 0.0 && position.Y < this.IncreaseButton.ActualHeight)
        e.Handled = true;
    }
    if (this.DecreaseButton == null || this.DecreaseButton.IsEnabled)
      return;
    position = e.GetPosition((IInputElement) this.DecreaseButton);
    if (position.X <= 0.0 || position.X >= this.DecreaseButton.ActualWidth || position.Y <= 0.0 || position.Y >= this.DecreaseButton.ActualHeight)
      return;
    e.Handled = true;
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Return:
        if ((this.IncreaseButton == null || !this.IncreaseButton.IsFocused) && (this.DecreaseButton == null || !this.DecreaseButton.IsFocused))
          break;
        e.Handled = true;
        break;
      case Key.Up:
        if (!this.AllowSpin)
          break;
        this.OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, SpinDirection.Increase));
        e.Handled = true;
        break;
      case Key.Down:
        if (!this.AllowSpin)
          break;
        this.OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, SpinDirection.Decrease));
        e.Handled = true;
        break;
    }
  }

  protected override void OnMouseWheel(MouseWheelEventArgs e)
  {
    base.OnMouseWheel(e);
    if (e.Handled || !this.AllowSpin || e.Delta == 0)
      return;
    SpinEventArgs e1 = new SpinEventArgs(Spinner.SpinnerSpinEvent, e.Delta < 0 ? SpinDirection.Decrease : SpinDirection.Increase, true);
    this.OnSpin(e1);
    e.Handled = e1.Handled;
  }

  protected override void OnValidSpinDirectionChanged(
    ValidSpinDirections oldValue,
    ValidSpinDirections newValue)
  {
    this.SetButtonUsage();
  }

  private void OnButtonClick(object sender, RoutedEventArgs e)
  {
    if (!this.AllowSpin)
      return;
    SpinDirection direction = sender == this.IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;
    this.OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, direction));
  }

  protected virtual void OnContentChanged(object oldValue, object newValue)
  {
  }

  protected virtual void OnAllowSpinChanged(bool oldValue, bool newValue) => this.SetButtonUsage();

  private void SetButtonUsage()
  {
    if (this.IncreaseButton != null)
      this.IncreaseButton.IsEnabled = this.AllowSpin && (this.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase;
    if (this.DecreaseButton == null)
      return;
    this.DecreaseButton.IsEnabled = this.AllowSpin && (this.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease;
  }
}
