// Decompiled with JetBrains decompiler
// Type: System.Windows.Automation.Peers.AutoCompleteBoxAutomationPeer
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#nullable disable
namespace System.Windows.Automation.Peers;

public sealed class AutoCompleteBoxAutomationPeer(AutoCompleteBox owner) : 
  FrameworkElementAutomationPeer((FrameworkElement) owner),
  IValueProvider,
  IExpandCollapseProvider,
  ISelectionProvider
{
  private const string AutoCompleteBoxClassNameCore = "AutoCompleteBox";

  private AutoCompleteBox OwnerAutoCompleteBox => (AutoCompleteBox) this.Owner;

  bool ISelectionProvider.CanSelectMultiple => false;

  bool ISelectionProvider.IsSelectionRequired => false;

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.ComboBox;
  }

  protected override string GetClassNameCore() => "AutoCompleteBox";

  public override object GetPattern(PatternInterface patternInterface)
  {
    object pattern = (object) null;
    AutoCompleteBox ownerAutoCompleteBox = this.OwnerAutoCompleteBox;
    switch (patternInterface)
    {
      case PatternInterface.Value:
        pattern = (object) this;
        break;
      case PatternInterface.ExpandCollapse:
        pattern = (object) this;
        break;
      default:
        if (ownerAutoCompleteBox.SelectionAdapter != null)
        {
          AutomationPeer automationPeer = ownerAutoCompleteBox.SelectionAdapter.CreateAutomationPeer();
          if (automationPeer != null)
          {
            pattern = automationPeer.GetPattern(patternInterface);
            break;
          }
          break;
        }
        break;
    }
    if (pattern == null)
      pattern = base.GetPattern(patternInterface);
    return pattern;
  }

  void IExpandCollapseProvider.Expand()
  {
    if (!this.IsEnabled())
      throw new ElementNotEnabledException();
    this.OwnerAutoCompleteBox.IsDropDownOpen = true;
  }

  void IExpandCollapseProvider.Collapse()
  {
    if (!this.IsEnabled())
      throw new ElementNotEnabledException();
    this.OwnerAutoCompleteBox.IsDropDownOpen = false;
  }

  ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
  {
    get
    {
      return !this.OwnerAutoCompleteBox.IsDropDownOpen ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;
    }
  }

  internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
  {
    this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, (object) (ExpandCollapseState) (oldValue ? 1 : 0), (object) (ExpandCollapseState) (newValue ? 1 : 0));
  }

  void IValueProvider.SetValue(string value) => this.OwnerAutoCompleteBox.Text = value;

  bool IValueProvider.IsReadOnly => !this.OwnerAutoCompleteBox.IsEnabled;

  string IValueProvider.Value => this.OwnerAutoCompleteBox.Text ?? string.Empty;

  protected override List<AutomationPeer> GetChildrenCore()
  {
    List<AutomationPeer> childrenCore = new List<AutomationPeer>();
    AutoCompleteBox ownerAutoCompleteBox = this.OwnerAutoCompleteBox;
    TextBox textBox = ownerAutoCompleteBox.TextBox;
    if (textBox != null)
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) textBox);
      if (peerForElement != null)
        childrenCore.Insert(0, peerForElement);
    }
    if (ownerAutoCompleteBox.SelectionAdapter != null)
    {
      AutomationPeer automationPeer1 = ownerAutoCompleteBox.SelectionAdapter.CreateAutomationPeer();
      if (automationPeer1 != null)
      {
        List<AutomationPeer> children = automationPeer1.GetChildren();
        if (children != null)
        {
          foreach (AutomationPeer automationPeer2 in children)
            childrenCore.Add(automationPeer2);
        }
      }
    }
    return childrenCore;
  }

  IRawElementProviderSimple[] ISelectionProvider.GetSelection()
  {
    if (this.OwnerAutoCompleteBox.SelectionAdapter != null)
    {
      object selectedItem = this.OwnerAutoCompleteBox.SelectionAdapter.SelectedItem;
      if (selectedItem != null && selectedItem is UIElement element)
      {
        AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(element);
        if (peerForElement != null)
          return new IRawElementProviderSimple[1]
          {
            this.ProviderFromPeer(peerForElement)
          };
      }
    }
    return new IRawElementProviderSimple[0];
  }
}
