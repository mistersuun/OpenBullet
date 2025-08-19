// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.Layer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal class Layer : UIElement
{
  protected readonly TextView textView;
  protected readonly KnownLayer knownLayer;

  public Layer(TextView textView, KnownLayer knownLayer)
  {
    this.textView = textView;
    this.knownLayer = knownLayer;
    this.Focusable = false;
  }

  protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
  {
    return (GeometryHitTestResult) null;
  }

  protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
  {
    return (HitTestResult) null;
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);
    this.textView.RenderBackground(drawingContext, this.knownLayer);
  }
}
