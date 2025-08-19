// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.FastSetBase
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class FastSetBase
{
  internal Delegate _func;
  internal int _version;
  internal int _hitCount;

  public FastSetBase(int version) => this._version = version;

  public bool ShouldUseNonOptimizedSite => this._hitCount < 100;
}
