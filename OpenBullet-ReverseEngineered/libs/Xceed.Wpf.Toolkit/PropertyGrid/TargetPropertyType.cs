// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.TargetPropertyType
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public sealed class TargetPropertyType
{
  private Type _type;
  private bool _sealed;

  public Type Type
  {
    get => this._type;
    set
    {
      if (this._sealed)
        throw new InvalidOperationException($"{typeof (TargetPropertyType)}.Type property cannot be modified once the instance is used");
      this._type = value;
    }
  }

  internal void Seal()
  {
    if (this._type == (Type) null)
      throw new InvalidOperationException($"{typeof (TargetPropertyType)}.Type property must be initialized");
    this._sealed = true;
  }
}
