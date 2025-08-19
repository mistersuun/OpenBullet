// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.GeneratorEnumerable`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal class GeneratorEnumerable<T> : IEnumerable<T>, IEnumerable
{
  protected readonly Func<GeneratorNext<T>> _next;

  internal GeneratorEnumerable(Func<GeneratorNext<T>> next) => this._next = next;

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return (IEnumerator<T>) new GeneratorEnumerator<T>(this._next());
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) ((IEnumerable<T>) this).GetEnumerator();
}
