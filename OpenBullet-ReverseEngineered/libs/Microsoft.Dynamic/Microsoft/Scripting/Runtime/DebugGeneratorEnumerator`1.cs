// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DebugGeneratorEnumerator`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal sealed class DebugGeneratorEnumerator<T> : GeneratorEnumerator<T>, IDebuggableGenerator
{
  private readonly int[] _yieldMarkers;

  internal DebugGeneratorEnumerator(GeneratorNext<T> next, int[] yieldMarkers)
    : base(next)
  {
    this._yieldMarkers = yieldMarkers;
  }

  int IDebuggableGenerator.YieldMarkerLocation
  {
    get
    {
      if (this._state < this._yieldMarkers.Length)
        return this._yieldMarkers[this._state];
      throw new InvalidOperationException("unknown yield marker");
    }
    set
    {
      for (int index = 0; index < this._yieldMarkers.Length; ++index)
      {
        if (this._yieldMarkers[index] == value)
        {
          this._state = index;
          return;
        }
      }
      throw new InvalidOperationException("unknown yield marker");
    }
  }
}
