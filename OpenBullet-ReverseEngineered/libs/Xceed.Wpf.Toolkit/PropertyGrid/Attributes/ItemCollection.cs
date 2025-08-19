// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

public class ItemCollection : List<Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item>
{
  public void Add(object value)
  {
    this.Add(new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item()
    {
      DisplayName = value.ToString(),
      Value = value
    });
  }

  public void Add(object value, string displayName)
  {
    this.Add(new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item()
    {
      DisplayName = displayName,
      Value = value
    });
  }
}
