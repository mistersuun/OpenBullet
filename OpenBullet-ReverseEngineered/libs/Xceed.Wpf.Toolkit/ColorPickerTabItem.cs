// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorPickerTabItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class ColorPickerTabItem : TabItem
{
  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (e.Source == this || !this.IsSelected)
      return;
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (e.Source == this || !this.IsSelected)
      base.OnMouseLeftButtonDown(e);
    base.OnMouseLeftButtonUp(e);
  }
}
