// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DatePickerAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DatePickerAutomationPeer(DatePicker owner) : 
  FrameworkElementAutomationPeer((FrameworkElement) owner),
  IExpandCollapseProvider,
  IValueProvider
{
  private DatePicker OwningDatePicker => this.Owner as DatePicker;

  public override object GetPattern(PatternInterface patternInterface)
  {
    return patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Value ? (object) this : base.GetPattern(patternInterface);
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Custom;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override string GetLocalizedControlTypeCore()
  {
    return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DatePickerAutomationPeer_LocalizedControlType);
  }

  ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
  {
    get
    {
      return this.OwningDatePicker.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
    }
  }

  void IExpandCollapseProvider.Collapse() => this.OwningDatePicker.IsDropDownOpen = false;

  void IExpandCollapseProvider.Expand() => this.OwningDatePicker.IsDropDownOpen = true;

  bool IValueProvider.IsReadOnly => false;

  string IValueProvider.Value => this.OwningDatePicker.ToString();

  void IValueProvider.SetValue(string value) => this.OwningDatePicker.Text = value;

  [MethodImpl(MethodImplOptions.NoInlining)]
  internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
  {
    if (!(oldValue != newValue))
      return;
    this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, (object) oldValue, (object) newValue);
  }
}
