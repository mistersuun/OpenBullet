// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.WeakObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal class WeakObject
{
  private readonly WeakReference weakReference;
  private readonly int hashCode;

  public WeakObject(object obj)
  {
    this.weakReference = new WeakReference(obj, true);
    this.hashCode = obj == null ? 0 : ReferenceEqualityComparer<object>.Instance.GetHashCode(obj);
  }

  public object Target => this.weakReference.Target;

  public override int GetHashCode() => this.hashCode;

  public override bool Equals(object obj)
  {
    object target = this.weakReference.Target;
    return target != null && target.Equals(obj);
  }
}
