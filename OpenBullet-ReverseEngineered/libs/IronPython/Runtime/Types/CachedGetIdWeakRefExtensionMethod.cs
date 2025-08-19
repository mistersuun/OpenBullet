// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CachedGetIdWeakRefExtensionMethod
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime.Types;

internal sealed class CachedGetIdWeakRefExtensionMethod : CachedGetKey
{
  private readonly WeakReference _extMethodSet;

  public CachedGetIdWeakRefExtensionMethod(string name, WeakReference weakReference)
    : base(name)
  {
    this._extMethodSet = weakReference;
  }

  public override bool Equals(CachedGetKey other)
  {
    return other is CachedGetIdWeakRefExtensionMethod refExtensionMethod && refExtensionMethod._extMethodSet.Target == this._extMethodSet.Target && refExtensionMethod.Name == this.Name;
  }
}
