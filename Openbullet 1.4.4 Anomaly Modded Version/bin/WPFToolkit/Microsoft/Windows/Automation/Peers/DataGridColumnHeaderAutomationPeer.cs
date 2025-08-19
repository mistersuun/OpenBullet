// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using MS.Internal;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridColumnHeaderAutomationPeer(Microsoft.Windows.Controls.Primitives.DataGridColumnHeader owner) : 
  ButtonBaseAutomationPeer((ButtonBase) owner),
  IInvokeProvider,
  IScrollItemProvider,
  ITransformProvider
{
  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.HeaderItem;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Invoke:
        if (this.OwningHeader.Column != null && this.OwningHeader.Column.CanUserSort)
          return (object) this;
        break;
      case PatternInterface.ScrollItem:
        return (object) this;
      case PatternInterface.Transform:
        if (this.OwningHeader.Column != null && this.OwningHeader.Column.DataGridOwner.CanUserResizeColumns)
          return (object) this;
        break;
    }
    return base.GetPattern(patternInterface);
  }

  protected override bool IsContentElementCore() => false;

  protected override bool IsOffscreenCore()
  {
    if (!this.Owner.IsVisible)
      return true;
    Rect visibleBoundingRect = DataGridAutomationPeer.CalculateVisibleBoundingRect(this.Owner);
    return DoubleUtil.AreClose(visibleBoundingRect, Rect.Empty) || DoubleUtil.AreClose(visibleBoundingRect.Height, 0.0) || DoubleUtil.AreClose(visibleBoundingRect.Width, 0.0);
  }

  void IInvokeProvider.Invoke() => this.OwningHeader.Invoke();

  void IScrollItemProvider.ScrollIntoView()
  {
    if (this.OwningHeader.Column == null)
      return;
    this.OwningHeader.Column.DataGridOwner?.ScrollIntoView((object) null, this.OwningHeader.Column);
  }

  bool ITransformProvider.CanMove => false;

  bool ITransformProvider.CanResize
  {
    get
    {
      return this.OwningHeader.Column != null && this.OwningHeader.Column.DataGridOwner.CanUserResizeColumns;
    }
  }

  bool ITransformProvider.CanRotate => false;

  void ITransformProvider.Move(double x, double y) => throw new InvalidOperationException();

  void ITransformProvider.Resize(double width, double height)
  {
    if (this.OwningHeader.Column == null || !this.OwningHeader.Column.DataGridOwner.CanUserResizeColumns)
      throw new InvalidOperationException();
    this.OwningHeader.Column.Width = new DataGridLength(width);
  }

  void ITransformProvider.Rotate(double degrees) => throw new InvalidOperationException();

  private Microsoft.Windows.Controls.Primitives.DataGridColumnHeader OwningHeader
  {
    get => (Microsoft.Windows.Controls.Primitives.DataGridColumnHeader) this.Owner;
  }
}
