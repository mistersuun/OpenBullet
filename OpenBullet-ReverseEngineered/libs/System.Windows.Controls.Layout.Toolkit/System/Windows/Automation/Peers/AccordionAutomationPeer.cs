// Decompiled with JetBrains decompiler
// Type: System.Windows.Automation.Peers.AccordionAutomationPeer
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#nullable disable
namespace System.Windows.Automation.Peers;

public class AccordionAutomationPeer(Accordion owner) : 
  ItemsControlAutomationPeer((ItemsControl) owner),
  ISelectionProvider
{
  private Accordion OwnerAccordion => (Accordion) this.Owner;

  protected override string GetClassNameCore() => "Accordion";

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.List;
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    return patternInterface == PatternInterface.Selection ? (object) this : base.GetPattern(patternInterface);
  }

  protected override List<AutomationPeer> GetChildrenCore()
  {
    Accordion ownerAccordion = this.OwnerAccordion;
    ItemCollection items = ownerAccordion.Items;
    if (items.Count <= 0)
      return (List<AutomationPeer>) null;
    List<AutomationPeer> childrenCore = new List<AutomationPeer>(items.Count);
    for (int index = 0; index < items.Count; ++index)
    {
      if (ownerAccordion.ItemContainerGenerator.ContainerFromIndex(index) is AccordionItem element)
        childrenCore.Add(UIElementAutomationPeer.FromElement((UIElement) element) ?? UIElementAutomationPeer.CreatePeerForElement((UIElement) element));
    }
    return childrenCore;
  }

  public bool CanSelectMultiple
  {
    get
    {
      return this.OwnerAccordion.SelectionMode == AccordionSelectionMode.OneOrMore || this.OwnerAccordion.SelectionMode == AccordionSelectionMode.ZeroOrMore;
    }
  }

  public IRawElementProviderSimple[] GetSelection()
  {
    Accordion ownerAccordion = this.OwnerAccordion;
    List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>(ownerAccordion.SelectedIndices.Count);
    foreach (int selectedIndex in (IEnumerable<int>) ownerAccordion.SelectedIndices)
    {
      if (ownerAccordion.ItemContainerGenerator.ContainerFromIndex(selectedIndex) is AccordionItem element)
      {
        AutomationPeer peer = UIElementAutomationPeer.FromElement((UIElement) element);
        if (peer != null)
          elementProviderSimpleList.Add(this.ProviderFromPeer(peer));
      }
    }
    return elementProviderSimpleList.ToArray();
  }

  public bool IsSelectionRequired
  {
    get
    {
      return this.OwnerAccordion.SelectionMode == AccordionSelectionMode.One || this.OwnerAccordion.SelectionMode == AccordionSelectionMode.OneOrMore;
    }
  }

  protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
  {
    return (ItemAutomationPeer) new AccordionItemAutomationPeer(item, (ItemsControlAutomationPeer) this);
  }
}
