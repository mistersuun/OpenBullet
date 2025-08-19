// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.CalendarButtonAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class CalendarButtonAutomationPeer(CalendarButton owner) : 
  ButtonAutomationPeer((Button) owner),
  IGridItemProvider,
  ISelectionItemProvider
{
  private Microsoft.Windows.Controls.Calendar OwningCalendar => this.OwningCalendarButton.Owner;

  private IRawElementProviderSimple OwningCalendarAutomationPeer
  {
    get
    {
      if (this.OwningCalendar != null)
      {
        AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) this.OwningCalendar);
        if (peerForElement != null)
          return this.ProviderFromPeer(peerForElement);
      }
      return (IRawElementProviderSimple) null;
    }
  }

  private CalendarButton OwningCalendarButton => this.Owner as CalendarButton;

  private DateTime? Date
  {
    get
    {
      return this.OwningCalendarButton != null && this.OwningCalendarButton.DataContext is DateTime ? (DateTime?) this.OwningCalendarButton.DataContext : new DateTime?();
    }
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    object pattern;
    switch (patternInterface)
    {
      case PatternInterface.GridItem:
      case PatternInterface.SelectionItem:
        pattern = this.OwningCalendar == null || this.OwningCalendar.MonthControl == null || this.OwningCalendarButton == null ? base.GetPattern(patternInterface) : (object) this;
        break;
      default:
        pattern = base.GetPattern(patternInterface);
        break;
    }
    return pattern;
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Button;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override string GetLocalizedControlTypeCore()
  {
    return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_CalendarButtonLocalizedControlType);
  }

  protected override string GetHelpTextCore()
  {
    DateTime? date = this.Date;
    return !date.HasValue ? base.GetHelpTextCore() : Microsoft.Windows.Controls.DateTimeHelper.ToLongDateString(date, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this.OwningCalendarButton));
  }

  protected override string GetNameCore()
  {
    DateTime? date = this.Date;
    if (!date.HasValue)
      return base.GetNameCore();
    return this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Decade ? Microsoft.Windows.Controls.DateTimeHelper.ToYearString(date, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this.OwningCalendarButton)) : Microsoft.Windows.Controls.DateTimeHelper.ToYearMonthPatternString(date, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this.OwningCalendarButton));
  }

  int IGridItemProvider.Column => (int) this.OwningCalendarButton.GetValue(Grid.ColumnProperty);

  int IGridItemProvider.ColumnSpan
  {
    get => (int) this.OwningCalendarButton.GetValue(Grid.ColumnSpanProperty);
  }

  IRawElementProviderSimple IGridItemProvider.ContainingGrid => this.OwningCalendarAutomationPeer;

  int IGridItemProvider.Row => (int) this.OwningCalendarButton.GetValue(Grid.RowSpanProperty);

  int IGridItemProvider.RowSpan => 1;

  bool ISelectionItemProvider.IsSelected => this.OwningCalendarButton.IsFocused;

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
  {
    get => this.OwningCalendarAutomationPeer;
  }

  void ISelectionItemProvider.AddToSelection()
  {
  }

  void ISelectionItemProvider.RemoveFromSelection()
  {
  }

  void ISelectionItemProvider.Select()
  {
    if (!this.OwningCalendarButton.IsEnabled)
      throw new ElementNotEnabledException();
    this.OwningCalendarButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
  }
}
