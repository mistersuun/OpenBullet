// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Attributes.NewItemTypesAttribute
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class NewItemTypesAttribute : Attribute
{
  public IList<Type> Types { get; set; }

  public NewItemTypesAttribute(params Type[] types)
  {
    this.Types = (IList<Type>) new List<Type>((IEnumerable<Type>) types);
  }

  public NewItemTypesAttribute()
  {
  }
}
