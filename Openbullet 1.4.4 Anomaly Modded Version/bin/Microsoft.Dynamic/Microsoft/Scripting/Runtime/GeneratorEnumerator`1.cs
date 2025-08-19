// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.GeneratorEnumerator`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal class GeneratorEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
{
  private readonly GeneratorNext<T> _next;
  private T _current;
  protected int _state;

  internal GeneratorEnumerator(GeneratorNext<T> next)
  {
    this._next = next;
    this._state = -1;
  }

  T IEnumerator<T>.Current => this._current;

  bool IEnumerator.MoveNext()
  {
    this._next(ref this._state, ref this._current);
    return this._state != 0;
  }

  object IEnumerator.Current => (object) ((IEnumerator<T>) this).Current;

  void IEnumerator.Reset() => throw new NotSupportedException();

  void IDisposable.Dispose() => GC.SuppressFinalize((object) this);
}
