// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class ColorItem
{
  public System.Windows.Media.Color? Color { get; set; }

  public string Name { get; set; }

  public ColorItem(System.Windows.Media.Color? color, string name)
  {
    this.Color = color;
    this.Name = name;
  }

  public override bool Equals(object obj)
  {
    return obj is ColorItem colorItem && colorItem.Color.Equals((object) this.Color) && colorItem.Name.Equals(this.Name);
  }

  public override int GetHashCode() => this.Color.GetHashCode() ^ this.Name.GetHashCode();
}
