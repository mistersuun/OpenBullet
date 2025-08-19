// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.RatingItem
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

[TemplateVisualState(Name = "Empty", GroupName = "FillStates")]
[TemplateVisualState(Name = "ReadOnly", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Partial", GroupName = "FillStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Filled", GroupName = "FillStates")]
public class RatingItem : ButtonBase, IUpdateVisualState
{
  private const string StateFilled = "Filled";
  private const string StateEmpty = "Empty";
  private const string GroupFill = "FillStates";
  private const string StatePartial = "Partial";
  private InteractionHelper _interactionHelper;
  private bool _settingDisplayValue;
  public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(nameof (DisplayValue), typeof (double), typeof (RatingItem), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(RatingItem.OnDisplayValueChanged)));
  private bool _settingIsReadOnly;
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (RatingItem), new PropertyMetadata((object) false, new PropertyChangedCallback(RatingItem.OnIsReadOnlyChanged)));
  internal static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (double), typeof (RatingItem), new PropertyMetadata((object) 0.0));

  public double DisplayValue
  {
    get => (double) this.GetValue(RatingItem.DisplayValueProperty);
    internal set
    {
      this._settingDisplayValue = true;
      try
      {
        this.SetValue(RatingItem.DisplayValueProperty, (object) value);
      }
      finally
      {
        this._settingDisplayValue = false;
      }
    }
  }

  private static void OnDisplayValueChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((RatingItem) d).OnDisplayValueChanged((double) e.OldValue, (double) e.NewValue);
  }

  private void OnDisplayValueChanged(double oldValue, double newValue)
  {
    if (!this._settingDisplayValue)
    {
      this._settingDisplayValue = true;
      this.DisplayValue = oldValue;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, System.Windows.Controls.Properties.Resources.InvalidAttemptToChangeReadOnlyProperty, (object) "DisplayValue"));
    }
    if (newValue <= 0.0)
      VisualStates.GoToState((Control) this, true, "Empty");
    else if (newValue >= 1.0)
      VisualStates.GoToState((Control) this, true, "Filled");
    else
      VisualStates.GoToState((Control) this, true, "Partial");
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(RatingItem.IsReadOnlyProperty);
    internal set
    {
      this._settingIsReadOnly = true;
      try
      {
        this.SetValue(RatingItem.IsReadOnlyProperty, (object) value);
      }
      finally
      {
        this._settingIsReadOnly = false;
      }
    }
  }

  private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((RatingItem) d).OnIsReadOnlyChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
  {
    if (!this._settingIsReadOnly)
    {
      this._settingIsReadOnly = true;
      this.IsReadOnly = oldValue;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, System.Windows.Controls.Properties.Resources.InvalidAttemptToChangeReadOnlyProperty, (object) "IsReadOnly"));
    }
    this._interactionHelper.OnIsReadOnlyChanged(newValue);
  }

  internal Rating ParentRating { get; set; }

  internal double Value
  {
    get => (double) this.GetValue(RatingItem.ValueProperty);
    set => this.SetValue(RatingItem.ValueProperty, (object) value);
  }

  internal void SelectValue()
  {
    if (this.IsReadOnly)
      return;
    this.Value = 1.0;
    this.OnClick();
  }

  static RatingItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (RatingItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (RatingItem)));
  }

  public RatingItem() => this._interactionHelper = new InteractionHelper((Control) this);

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (this._interactionHelper.AllowMouseLeftButtonDown(e))
      this._interactionHelper.OnMouseLeftButtonDownBase();
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this._interactionHelper.AllowMouseLeftButtonUp(e))
      this._interactionHelper.OnMouseLeftButtonUpBase();
    base.OnMouseLeftButtonUp(e);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    if (this._interactionHelper.AllowMouseEnter(e))
      this._interactionHelper.UpdateVisualStateBase(true);
    base.OnMouseEnter(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (this._interactionHelper.AllowMouseLeave(e))
      this._interactionHelper.UpdateVisualStateBase(true);
    base.OnMouseLeave(e);
  }

  protected override void OnClick() => base.OnClick();

  void IUpdateVisualState.UpdateVisualState(bool useTransitions)
  {
    this._interactionHelper.UpdateVisualStateBase(useTransitions);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new RatingItemAutomationPeer(this);
  }
}
