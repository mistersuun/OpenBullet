// Decompiled with JetBrains decompiler
// Type: System.Windows.Automation.Peers.RatingAutomationPeer
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Properties;

#nullable disable
namespace System.Windows.Automation.Peers;

public sealed class RatingAutomationPeer(Rating owner) : 
  ItemsControlAutomationPeer((ItemsControl) owner),
  IValueProvider,
  ISelectionProvider
{
  protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
  {
    ItemAutomationPeer itemAutomationPeer = (ItemAutomationPeer) null;
    if (item is UIElement element)
      itemAutomationPeer = UIElementAutomationPeer.CreatePeerForElement(element) as ItemAutomationPeer;
    return itemAutomationPeer;
  }

  private Rating OwnerRating => (Rating) this.Owner;

  protected override string GetNameCore()
  {
    string nameCore = base.GetNameCore();
    return string.IsNullOrEmpty(nameCore) ? "Rating" : nameCore;
  }

  protected override string GetLocalizedControlTypeCore()
  {
    return Resources.RatingAutomationPeer_GetLocalizedControlTypeCore;
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Slider;
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    return patternInterface == PatternInterface.Value || patternInterface == PatternInterface.Selection ? (object) this : base.GetPattern(patternInterface);
  }

  protected override List<AutomationPeer> GetChildrenCore()
  {
    Rating ownerRating = this.OwnerRating;
    ItemCollection items = ownerRating.Items;
    if (items.Count <= 0)
      return (List<AutomationPeer>) null;
    List<AutomationPeer> childrenCore = new List<AutomationPeer>(items.Count);
    for (int index = 0; index < items.Count; ++index)
    {
      if (ownerRating.ItemContainerGenerator.ContainerFromIndex(index) is RatingItem element)
        childrenCore.Add(UIElementAutomationPeer.FromElement((UIElement) element) ?? UIElementAutomationPeer.CreatePeerForElement((UIElement) element));
    }
    return childrenCore;
  }

  public bool CanSelectMultiple => false;

  public IRawElementProviderSimple[] GetSelection()
  {
    RatingItem element = this.OwnerRating.GetRatingItems().LastOrDefault<RatingItem>((Func<RatingItem, bool>) (ratingItem => ratingItem.Value > 0.0));
    if (element == null)
      return new IRawElementProviderSimple[0];
    return new IRawElementProviderSimple[1]
    {
      this.ProviderFromPeer(UIElementAutomationPeer.FromElement((UIElement) element))
    };
  }

  public bool IsSelectionRequired => false;

  public bool IsReadOnly => this.OwnerRating.IsReadOnly;

  public void SetValue(string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      this.OwnerRating.Value = new double?();
    }
    else
    {
      double result;
      if (!double.TryParse(value, out result))
        throw new InvalidOperationException(Resources.RatingAutomationPeer_SetValue);
      this.OwnerRating.Value = result >= 0.0 && result <= 1.0 ? new double?(result) : throw new InvalidOperationException(Resources.RatingAutomationPeer_SetValue);
    }
  }

  public string Value
  {
    get => this.OwnerRating.Value.HasValue ? this.OwnerRating.Value.ToString() : (string) null;
  }
}
