// Decompiled with JetBrains decompiler
// Type: AngleSharp.Common.AttachedProperty`2
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace AngleSharp.Common;

internal sealed class AttachedProperty<TObj, TProp>
  where TObj : class
  where TProp : class
{
  private readonly ConditionalWeakTable<TObj, TProp> _properties = new ConditionalWeakTable<TObj, TProp>();

  public TProp Get(TObj item)
  {
    TProp prop;
    this._properties.TryGetValue(item, out prop);
    return prop;
  }

  public void Set(TObj item, TProp value) => this._properties.Add(item, value);
}
