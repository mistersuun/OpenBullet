// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorSide
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows.Markup;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
{
  private AnchorSide _side;

  public AnchorSide Side
  {
    get => this._side;
    private set
    {
      if (this._side == value)
        return;
      this.RaisePropertyChanging(nameof (Side));
      this._side = value;
      this.RaisePropertyChanged(nameof (Side));
    }
  }

  protected override bool GetVisibility() => this.Children.Count > 0;

  protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
  {
    base.OnParentChanged(oldValue, newValue);
    this.UpdateSide();
  }

  private void UpdateSide()
  {
    if (this.Root.LeftSide == this)
      this.Side = AnchorSide.Left;
    else if (this.Root.TopSide == this)
      this.Side = AnchorSide.Top;
    else if (this.Root.RightSide == this)
    {
      this.Side = AnchorSide.Right;
    }
    else
    {
      if (this.Root.BottomSide != this)
        return;
      this.Side = AnchorSide.Bottom;
    }
  }
}
