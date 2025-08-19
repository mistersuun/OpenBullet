﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.CalendarButton
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
[TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
[TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
[TemplateVisualState(Name = "CalendarButtonUnfocused", GroupName = "CalendarButtonFocusStates")]
[TemplateVisualState(Name = "CalendarButtonFocused", GroupName = "CalendarButtonFocusStates")]
public sealed class CalendarButton : Button
{
  private bool _shouldCoerceContent;
  private object _coercedContent;
  internal static readonly DependencyPropertyKey HasSelectedDaysPropertyKey = DependencyProperty.RegisterReadOnly(nameof (HasSelectedDays), typeof (bool), typeof (CalendarButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty HasSelectedDaysProperty = CalendarButton.HasSelectedDaysPropertyKey.DependencyProperty;
  internal static readonly DependencyPropertyKey IsInactivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsInactive), typeof (bool), typeof (CalendarButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarButton.OnVisualStatePropertyChanged)));
  public static readonly DependencyProperty IsInactiveProperty = CalendarButton.IsInactivePropertyKey.DependencyProperty;

  static CalendarButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CalendarButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CalendarButton)));
    ContentControl.ContentProperty.OverrideMetadata(typeof (CalendarButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(CalendarButton.OnCoerceContent)));
  }

  public CalendarButton()
  {
    this.Loaded += (RoutedEventHandler) delegate
    {
      this.ChangeVisualState(false);
    };
  }

  public bool HasSelectedDays
  {
    get => (bool) this.GetValue(CalendarButton.HasSelectedDaysProperty);
    internal set => this.SetValue(CalendarButton.HasSelectedDaysPropertyKey, (object) value);
  }

  public bool IsInactive
  {
    get => (bool) this.GetValue(CalendarButton.IsInactiveProperty);
    internal set => this.SetValue(CalendarButton.IsInactivePropertyKey, (object) value);
  }

  internal Microsoft.Windows.Controls.Calendar Owner { get; set; }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.ChangeVisualState(false);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.CalendarButtonAutomationPeer(this);
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

  private new void ChangeVisualState(bool useTransitions)
  {
    if (this.HasSelectedDays)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Selected", "Unselected");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unselected");
    if (this.IsInactive)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Inactive");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Active", "Inactive");
    if (this.IsKeyboardFocused)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "CalendarButtonFocused", "CalendarButtonUnfocused");
    else
      VisualStateManager.GoToState((Control) this, "CalendarButtonUnfocused", useTransitions);
  }

  private new static void OnVisualStatePropertyChanged(
    DependencyObject dObject,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(dObject is CalendarButton calendarButton) || object.Equals(e.OldValue, e.NewValue))
      return;
    calendarButton.ChangeVisualState(true);
  }

  private static object OnCoerceContent(DependencyObject sender, object baseValue)
  {
    CalendarButton calendarButton = (CalendarButton) sender;
    if (!calendarButton._shouldCoerceContent)
      return baseValue;
    calendarButton._shouldCoerceContent = false;
    return calendarButton._coercedContent;
  }
}
