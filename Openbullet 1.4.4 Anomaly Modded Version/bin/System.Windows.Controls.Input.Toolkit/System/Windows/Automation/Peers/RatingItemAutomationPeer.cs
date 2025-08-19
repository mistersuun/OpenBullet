// Decompiled with JetBrains decompiler
// Type: System.Windows.Automation.Peers.RatingItemAutomationPeer
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Globalization;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Properties;

#nullable disable
namespace System.Windows.Automation.Peers;

public class RatingItemAutomationPeer(RatingItem owner) : 
  FrameworkElementAutomationPeer((FrameworkElement) owner),
  ISelectionItemProvider
{
  private RatingItem OwnerRatingItem => (RatingItem) this.Owner;

  protected override string GetLocalizedControlTypeCore()
  {
    return Resources.RatingItemAutomationPeer_GetLocalizedControlTypeCore;
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.ListItem;
  }

  protected override string GetClassNameCore() => "RatingItem";

  public override object GetPattern(PatternInterface patternInterface)
  {
    return patternInterface == PatternInterface.SelectionItem ? (object) this : (object) null;
  }

  protected override string GetNameCore()
  {
    int? nullable = this.OwnerRatingItem.ParentRating.GetRatingItems().IndexOf<RatingItem>(this.OwnerRatingItem);
    return nullable.HasValue ? (nullable.Value + 1).ToString((IFormatProvider) CultureInfo.CurrentUICulture) : string.Empty;
  }

  void ISelectionItemProvider.AddToSelection()
  {
    RatingItem ownerRatingItem = this.OwnerRatingItem;
    Rating parentRating = ownerRatingItem.ParentRating;
    if (parentRating == null || parentRating.Value.HasValue)
      throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed);
    ownerRatingItem.SelectValue();
  }

  bool ISelectionItemProvider.IsSelected => this.OwnerRatingItem.Value > 0.0;

  void ISelectionItemProvider.RemoveFromSelection()
  {
    Rating parentRating = this.OwnerRatingItem.ParentRating;
    if (parentRating == null)
      throw new InvalidOperationException(Resources.Automation_OperationCannotBePerformed);
    if (parentRating.IsReadOnly)
      return;
    parentRating.Value = new double?();
  }

  void ISelectionItemProvider.Select() => this.OwnerRatingItem.SelectValue();

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
  {
    get
    {
      Rating parentRating = this.OwnerRatingItem.ParentRating;
      if (parentRating != null)
      {
        AutomationPeer peer = UIElementAutomationPeer.FromElement((UIElement) parentRating);
        if (peer != null)
          return this.ProviderFromPeer(peer);
      }
      return (IRawElementProviderSimple) null;
    }
  }
}
