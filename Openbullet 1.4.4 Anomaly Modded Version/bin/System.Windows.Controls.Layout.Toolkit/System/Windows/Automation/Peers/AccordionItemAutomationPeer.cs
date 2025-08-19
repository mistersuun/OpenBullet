// Decompiled with JetBrains decompiler
// Type: System.Windows.Automation.Peers.AccordionItemAutomationPeer
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Properties;

#nullable disable
namespace System.Windows.Automation.Peers;

public class AccordionItemAutomationPeer(
  object item,
  ItemsControlAutomationPeer itemsControlAutomationPeer) : 
  ItemAutomationPeer(item, itemsControlAutomationPeer),
  IExpandCollapseProvider,
  ISelectionItemProvider
{
  private AccordionItem OwnerAccordionItem => this.Item as AccordionItem;

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.ListItem;
  }

  protected override string GetClassNameCore() => "AccordionItem";

  public override object GetPattern(PatternInterface patternInterface)
  {
    return patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.SelectionItem ? (object) this : (object) null;
  }

  ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
  {
    get
    {
      return !this.OwnerAccordionItem.IsSelected ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;
    }
  }

  void IExpandCollapseProvider.Collapse()
  {
    if (!this.IsEnabled())
      throw new ElementNotEnabledException();
    AccordionItem ownerAccordionItem = this.OwnerAccordionItem;
    if (ownerAccordionItem.IsLocked)
      throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed);
    ownerAccordionItem.IsSelected = false;
  }

  void IExpandCollapseProvider.Expand()
  {
    if (!this.IsEnabled())
      throw new ElementNotEnabledException();
    AccordionItem ownerAccordionItem = this.OwnerAccordionItem;
    if (ownerAccordionItem.IsLocked)
      throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed);
    ownerAccordionItem.IsSelected = true;
  }

  void ISelectionItemProvider.AddToSelection()
  {
    AccordionItem ownerAccordionItem = this.OwnerAccordionItem;
    (ownerAccordionItem.ParentAccordion ?? throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed)).SelectedItems.Add((object) ownerAccordionItem);
  }

  bool ISelectionItemProvider.IsSelected => this.OwnerAccordionItem.IsSelected;

  void ISelectionItemProvider.RemoveFromSelection()
  {
    AccordionItem ownerAccordionItem = this.OwnerAccordionItem;
    (ownerAccordionItem.ParentAccordion ?? throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed)).SelectedItems.Remove((object) ownerAccordionItem);
  }

  void ISelectionItemProvider.Select() => this.OwnerAccordionItem.IsSelected = true;

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
  {
    get
    {
      Accordion parentAccordion = this.OwnerAccordionItem.ParentAccordion;
      if (parentAccordion != null)
      {
        AutomationPeer peer = UIElementAutomationPeer.FromElement((UIElement) parentAccordion);
        if (peer != null)
          return this.ProviderFromPeer(peer);
      }
      return (IRawElementProviderSimple) null;
    }
  }
}
