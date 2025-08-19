// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.CalendarAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class CalendarAutomationPeer(Microsoft.Windows.Controls.Calendar owner) : 
  FrameworkElementAutomationPeer((FrameworkElement) owner),
  IMultipleViewProvider,
  ISelectionProvider,
  ITableProvider,
  IGridProvider
{
  private Microsoft.Windows.Controls.Calendar OwningCalendar => this.Owner as Microsoft.Windows.Controls.Calendar;

  private Grid OwningGrid
  {
    get
    {
      if (this.OwningCalendar == null || this.OwningCalendar.MonthControl == null)
        return (Grid) null;
      return this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Month ? this.OwningCalendar.MonthControl.MonthView : this.OwningCalendar.MonthControl.YearView;
    }
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Selection:
      case PatternInterface.Grid:
      case PatternInterface.MultipleView:
      case PatternInterface.Table:
        if (this.OwningGrid != null)
          return (object) this;
        break;
    }
    return base.GetPattern(patternInterface);
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Calendar;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
  {
    int count1 = this.OwningCalendar.SelectedDates.Count;
    int count2 = e.AddedItems.Count;
    if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && count1 == 1 && count2 == 1)
    {
      CalendarDayButton dayButtonFromDay = this.OwningCalendar.FindDayButtonFromDay((DateTime) e.AddedItems[0]);
      if (dayButtonFromDay == null)
        return;
      UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
    }
    else
    {
      if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
      {
        foreach (DateTime addedItem in (IEnumerable) e.AddedItems)
        {
          CalendarDayButton dayButtonFromDay = this.OwningCalendar.FindDayButtonFromDay(addedItem);
          if (dayButtonFromDay != null)
            UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
        }
      }
      if (!AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
        return;
      foreach (DateTime removedItem in (IEnumerable) e.RemovedItems)
      {
        CalendarDayButton dayButtonFromDay = this.OwningCalendar.FindDayButtonFromDay(removedItem);
        if (dayButtonFromDay != null)
          UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
      }
    }
  }

  int IGridProvider.ColumnCount
  {
    get => this.OwningGrid != null ? this.OwningGrid.ColumnDefinitions.Count : 0;
  }

  int IGridProvider.RowCount
  {
    get
    {
      if (this.OwningGrid == null)
        return 0;
      return this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Month ? Math.Max(0, this.OwningGrid.RowDefinitions.Count - 1) : this.OwningGrid.RowDefinitions.Count;
    }
  }

  IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
  {
    if (this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Month)
      ++row;
    if (this.OwningGrid != null && row >= 0 && row < this.OwningGrid.RowDefinitions.Count && column >= 0 && column < this.OwningGrid.ColumnDefinitions.Count)
    {
      foreach (UIElement child in this.OwningGrid.Children)
      {
        int num1 = (int) child.GetValue(Grid.RowProperty);
        int num2 = (int) child.GetValue(Grid.ColumnProperty);
        if (num1 == row && num2 == column)
        {
          AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(child);
          if (peerForElement != null)
            return this.ProviderFromPeer(peerForElement);
        }
      }
    }
    return (IRawElementProviderSimple) null;
  }

  int IMultipleViewProvider.CurrentView => (int) this.OwningCalendar.DisplayMode;

  int[] IMultipleViewProvider.GetSupportedViews()
  {
    return new int[3]{ 0, 1, 2 };
  }

  string IMultipleViewProvider.GetViewName(int viewId)
  {
    switch (viewId)
    {
      case 0:
        return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_MonthMode);
      case 1:
        return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_YearMode);
      case 2:
        return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.CalendarAutomationPeer_DecadeMode);
      default:
        return string.Empty;
    }
  }

  void IMultipleViewProvider.SetCurrentView(int viewId)
  {
    this.OwningCalendar.DisplayMode = (Microsoft.Windows.Controls.CalendarMode) viewId;
  }

  bool ISelectionProvider.CanSelectMultiple
  {
    get
    {
      return this.OwningCalendar.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.SingleRange || this.OwningCalendar.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.MultipleRange;
    }
  }

  bool ISelectionProvider.IsSelectionRequired => false;

  IRawElementProviderSimple[] ISelectionProvider.GetSelection()
  {
    List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
    if (this.OwningGrid != null)
    {
      if (this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Month && this.OwningCalendar.SelectedDates != null && this.OwningCalendar.SelectedDates.Count != 0)
      {
        foreach (UIElement child in this.OwningGrid.Children)
        {
          if ((int) child.GetValue(Grid.RowProperty) != 0 && child is CalendarDayButton element && element.IsSelected)
          {
            AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
            if (peerForElement != null)
              elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
          }
        }
      }
      else
      {
        foreach (UIElement child in this.OwningGrid.Children)
        {
          if (child is CalendarButton element && element.IsFocused)
          {
            AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
            if (peerForElement != null)
            {
              elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
              break;
            }
            break;
          }
        }
      }
      if (elementProviderSimpleList.Count > 0)
        return elementProviderSimpleList.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  RowOrColumnMajor ITableProvider.RowOrColumnMajor => RowOrColumnMajor.RowMajor;

  IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
  {
    if (this.OwningCalendar.DisplayMode == Microsoft.Windows.Controls.CalendarMode.Month)
    {
      List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
      foreach (UIElement child in this.OwningGrid.Children)
      {
        if ((int) child.GetValue(Grid.RowProperty) == 0)
        {
          AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(child);
          if (peerForElement != null)
            elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
        }
      }
      if (elementProviderSimpleList.Count > 0)
        return elementProviderSimpleList.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  IRawElementProviderSimple[] ITableProvider.GetRowHeaders() => (IRawElementProviderSimple[]) null;
}
