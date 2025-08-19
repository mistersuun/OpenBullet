// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.UIElementAdorner`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

internal class UIElementAdorner<TElement>(UIElement adornedElement) : Adorner(adornedElement) where TElement : UIElement
{
  private TElement _child;
  private double _offsetLeft;
  private double _offsetTop;

  public TElement Child
  {
    get => this._child;
    set
    {
      if ((object) value == (object) this._child)
        return;
      if ((object) this._child != null)
      {
        this.RemoveLogicalChild((object) this._child);
        this.RemoveVisualChild((Visual) this._child);
      }
      this._child = value;
      if ((object) this._child == null)
        return;
      this.AddLogicalChild((object) this._child);
      this.AddVisualChild((Visual) this._child);
    }
  }

  public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
  {
    return (GeneralTransform) new GeneralTransformGroup()
    {
      Children = {
        base.GetDesiredTransform(transform),
        (GeneralTransform) new TranslateTransform(this._offsetLeft, this._offsetTop)
      }
    };
  }

  public double OffsetLeft
  {
    get => this._offsetLeft;
    set
    {
      this._offsetLeft = value;
      this.UpdateLocation();
    }
  }

  public void SetOffsets(double left, double top)
  {
    this._offsetLeft = left;
    this._offsetTop = top;
    this.UpdateLocation();
  }

  public double OffsetTop
  {
    get => this._offsetTop;
    set
    {
      this._offsetTop = value;
      this.UpdateLocation();
    }
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if ((object) this._child == null)
      return base.MeasureOverride(constraint);
    this._child.Measure(constraint);
    return this._child.DesiredSize;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    if ((object) this._child == null)
      return base.ArrangeOverride(finalSize);
    this._child.Arrange(new Rect(finalSize));
    return finalSize;
  }

  protected override IEnumerator LogicalChildren
  {
    get
    {
      ArrayList arrayList = new ArrayList();
      if ((object) this._child != null)
        arrayList.Add((object) this._child);
      return arrayList.GetEnumerator();
    }
  }

  protected override Visual GetVisualChild(int index) => (Visual) this._child;

  protected override int VisualChildrenCount => (object) this._child != null ? 1 : 0;

  private void UpdateLocation()
  {
    if (!(this.Parent is AdornerLayer parent))
      return;
    parent.Update(this.AdornedElement);
  }
}
