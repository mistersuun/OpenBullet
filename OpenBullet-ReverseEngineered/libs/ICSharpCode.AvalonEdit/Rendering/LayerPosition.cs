// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.LayerPosition
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class LayerPosition : IComparable<LayerPosition>
{
  internal static readonly DependencyProperty LayerPositionProperty = DependencyProperty.RegisterAttached(nameof (LayerPosition), typeof (LayerPosition), typeof (LayerPosition));
  internal readonly KnownLayer KnownLayer;
  internal readonly LayerInsertionPosition Position;

  public static void SetLayerPosition(UIElement layer, LayerPosition value)
  {
    layer.SetValue(LayerPosition.LayerPositionProperty, (object) value);
  }

  public static LayerPosition GetLayerPosition(UIElement layer)
  {
    return (LayerPosition) layer.GetValue(LayerPosition.LayerPositionProperty);
  }

  public LayerPosition(KnownLayer knownLayer, LayerInsertionPosition position)
  {
    this.KnownLayer = knownLayer;
    this.Position = position;
  }

  public int CompareTo(LayerPosition other)
  {
    int num = this.KnownLayer.CompareTo((object) other.KnownLayer);
    return num != 0 ? num : this.Position.CompareTo((object) other.Position);
  }
}
