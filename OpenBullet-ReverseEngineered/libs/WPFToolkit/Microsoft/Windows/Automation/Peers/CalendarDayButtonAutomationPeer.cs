// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.CalendarDayButtonAutomationPeer
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

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class CalendarDayButtonAutomationPeer(CalendarDayButton owner) : 
  ButtonAutomationPeer((Button) owner),
  ISelectionItemProvider,
  ITableItemProvider,
  IGridItemProvider
{
  private Microsoft.Windows.Controls.Calendar OwningCalendar => this.OwningCalendarDayButton.Owner;

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

  private CalendarDayButton OwningCalendarDayButton => this.Owner as CalendarDayButton;

  private DateTime? Date
  {
    get
    {
      return this.OwningCalendarDayButton != null && this.OwningCalendarDayButton.DataContext is DateTime ? (DateTime?) this.OwningCalendarDayButton.DataContext : new DateTime?();
    }
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    object pattern;
    switch (patternInterface)
    {
      case PatternInterface.GridItem:
      case PatternInterface.SelectionItem:
      case PatternInterface.TableItem:
        pattern = this.OwningCalendar == null || this.OwningCalendarDayButton == null ? base.GetPattern(patternInterface) : (object) this;
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

  protected override string GetHelpTextCore()
  {
    if (!this.Date.HasValue)
      return base.GetHelpTextCore();
    string longDateString = Microsoft.Windows.Controls.DateTimeHelper.ToLongDateString(this.Date, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this.OwningCalendarDayButton));
    if (!this.OwningCalendarDayButton.IsBlackedOut)
      return longDateString;
    return string.Format((IFormatProvider) Microsoft.Windows.Controls.DateTimeHelper.GetCurrentDateFormat(), Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_BlackoutDayHelpText), (object) longDateString);
  }

  protected override string GetLocalizedControlTypeCore()
  {
    return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_DayButtonLocalizedControlType);
  }

  protected override string GetNameCore()
  {
    return !this.Date.HasValue ? base.GetNameCore() : Microsoft.Windows.Controls.DateTimeHelper.ToLongDateString(this.Date, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this.OwningCalendarDayButton));
  }

  int IGridItemProvider.Column => (int) this.OwningCalendarDayButton.GetValue(Grid.ColumnProperty);

  int IGridItemProvider.ColumnSpan
  {
    get => (int) this.OwningCalendarDayButton.GetValue(Grid.ColumnSpanProperty);
  }

  IRawElementProviderSimple IGridItemProvider.ContainingGrid => this.OwningCalendarAutomationPeer;

  int IGridItemProvider.Row => (int) this.OwningCalendarDayButton.GetValue(Grid.RowProperty) - 1;

  int IGridItemProvider.RowSpan
  {
    get => (int) this.OwningCalendarDayButton.GetValue(Grid.RowSpanProperty);
  }

  bool ISelectionItemProvider.IsSelected => this.OwningCalendarDayButton.IsSelected;

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
  {
    get => this.OwningCalendarAutomationPeer;
  }

  void ISelectionItemProvider.AddToSelection()
  {
    if (((ISelectionItemProvider) this).IsSelected || !this.EnsureSelection() || !(this.OwningCalendarDayButton.DataContext is DateTime))
      return;
    if (this.OwningCalendar.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate)
      this.OwningCalendar.SelectedDate = new DateTime?((DateTime) this.OwningCalendarDayButton.DataContext);
    else
      this.OwningCalendar.SelectedDates.Add((DateTime) this.OwningCalendarDayButton.DataContext);
  }

  void ISelectionItemProvider.RemoveFromSelection()
  {
    if (!((ISelectionItemProvider) this).IsSelected || !(this.OwningCalendarDayButton.DataContext is DateTime))
      return;
    this.OwningCalendar.SelectedDates.Remove((DateTime) this.OwningCalendarDayButton.DataContext);
  }

  void ISelectionItemProvider.Select()
  {
    if (!this.EnsureSelection())
      return;
    this.OwningCalendar.SelectedDates.Clear();
    if (!(this.OwningCalendarDayButton.DataContext is DateTime))
      return;
    this.OwningCalendar.SelectedDates.Add((DateTime) this.OwningCalendarDayButton.DataContext);
  }

  IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
  {
    if (this.OwningCalendar != null && this.OwningCalendarAutomationPeer != null)
    {
      IRawElementProviderSimple[] columnHeaders = ((ITableProvider) UIElementAutomationPeer.CreatePeerForElement((UIElement) this.OwningCalendar)).GetColumnHeaders();
      if (columnHeaders != null)
      {
        int column = ((IGridItemProvider) this).Column;
        return new IRawElementProviderSimple[1]
        {
          columnHeaders[column]
        };
      }
    }
    return (IRawElementProviderSimple[]) null;
  }

  IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
  {
    return (IRawElementProviderSimple[]) null;
  }

  private bool EnsureSelection()
  {
    if (!this.OwningCalendarDayButton.IsEnabled)
      throw new ElementNotEnabledException();
    return !this.OwningCalendarDayButton.IsBlackedOut && this.OwningCalendar.SelectionMode != Microsoft.Windows.Controls.CalendarSelectionMode.None;
  }
}
