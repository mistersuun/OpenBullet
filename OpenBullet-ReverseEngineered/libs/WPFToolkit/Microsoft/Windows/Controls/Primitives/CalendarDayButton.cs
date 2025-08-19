// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.CalendarDayButton
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Automation.Peers;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
[TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
[TemplateVisualState(Name = "CalendarButtonUnfocused", GroupName = "CalendarButtonFocusStates")]
[TemplateVisualState(Name = "CalendarButtonFocused", GroupName = "CalendarButtonFocusStates")]
[TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
[TemplateVisualState(Name = "Today", GroupName = "DayStates")]
[TemplateVisualState(Name = "NormalDay", GroupName = "BlackoutDayStates")]
[TemplateVisualState(Name = "BlackoutDay", GroupName = "BlackoutDayStates")]
[TemplateVisualState(Name = "RegularDay", GroupName = "DayStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
public sealed class CalendarDayButton : Button
{
  private const int DEFAULTCONTENT = 1;
  internal const string StateToday = "Today";
  internal const string StateRegularDay = "RegularDay";
  internal const string GroupDay = "DayStates";
  internal const string StateBlackoutDay = "BlackoutDay";
  internal const string StateNormalDay = "NormalDay";
  internal const string GroupBlackout = "BlackoutDayStates";
  private bool _shouldCoerceContent;
  private object _coercedContent;
  internal static readonly DependencyPropertyKey IsTodayPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsToday), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsTodayProperty = CalendarDayButton.IsTodayPropertyKey.DependencyProperty;
  internal static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsSelected), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsSelectedProperty = CalendarDayButton.IsSelectedPropertyKey.DependencyProperty;
  internal static readonly DependencyPropertyKey IsInactivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsInactive), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsInactiveProperty = CalendarDayButton.IsInactivePropertyKey.DependencyProperty;
  internal static readonly DependencyPropertyKey IsBlackedOutPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsBlackedOut), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsBlackedOutProperty = CalendarDayButton.IsBlackedOutPropertyKey.DependencyProperty;
  internal static readonly DependencyPropertyKey IsHighlightedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsHighlighted), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsHighlightedProperty = CalendarDayButton.IsHighlightedPropertyKey.DependencyProperty;

  static CalendarDayButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CalendarDayButton)));
    ContentControl.ContentProperty.OverrideMetadata(typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(CalendarDayButton.OnCoerceContent)));
  }

  public CalendarDayButton()
  {
    this.Loaded += (RoutedEventHandler) delegate
    {
      this.ChangeVisualState(false);
    };
  }

  public bool IsToday => (bool) this.GetValue(CalendarDayButton.IsTodayProperty);

  public bool IsSelected => (bool) this.GetValue(CalendarDayButton.IsSelectedProperty);

  public bool IsInactive => (bool) this.GetValue(CalendarDayButton.IsInactiveProperty);

  public bool IsBlackedOut => (bool) this.GetValue(CalendarDayButton.IsBlackedOutProperty);

  public bool IsHighlighted => (bool) this.GetValue(CalendarDayButton.IsHighlightedProperty);

  internal Microsoft.Windows.Controls.Calendar Owner { get; set; }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.ChangeVisualState(false);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new CalendarDayButtonAutomationPeer(this);
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    this.ChangeVisualState(true);
    base.OnGotKeyboardFocus(e);
  }

  protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    this.ChangeVisualState(true);
    base.OnLostKeyboardFocus(e);
  }

  internal new void ChangeVisualState(bool useTransitions)
  {
    if (this.IsEnabled)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Normal");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Disabled");
    if (this.IsSelected || this.IsHighlighted)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Selected", "Unselected");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unselected");
    if (!this.IsInactive)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Active", "Inactive");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Inactive");
    if (this.IsToday && this.Owner != null && this.Owner.IsTodayHighlighted)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Today", "RegularDay");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "RegularDay");
    if (this.IsBlackedOut)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "BlackoutDay", "NormalDay");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "NormalDay");
    if (this.IsKeyboardFocused)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "CalendarButtonFocused", "CalendarButtonUnfocused");
    else
      VisualStateManager.GoToState((Control) this, "CalendarButtonUnfocused", useTransitions);
  }

  internal void SetContentInternal(string value)
  {
    if (BindingOperations.GetBindingExpressionBase((DependencyObject) this, ContentControl.ContentProperty) != null)
    {
      this.Content = (object) value;
    }
    else
    {
      this._shouldCoerceContent = true;
      this._coercedContent = (object) value;
      this.CoerceValue(ContentControl.ContentProperty);
    }
  }

  private new static void OnVisualStatePropertyChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is CalendarDayButton calendarDayButton))
      return;
    calendarDayButton.ChangeVisualState(true);
  }

  private static object OnCoerceContent(DependencyObject sender, object baseValue)
  {
    CalendarDayButton calendarDayButton = (CalendarDayButton) sender;
    if (!calendarDayButton._shouldCoerceContent)
      return baseValue;
    calendarDayButton._shouldCoerceContent = false;
    return calendarDayButton._coercedContent;
  }
}
