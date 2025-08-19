// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SentinelIterator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("SentinelIterator")]
public sealed class SentinelIterator : IEnumerator, IEnumerator<object>, IDisposable
{
  private readonly object _target;
  private readonly object _sentinel;
  private readonly CodeContext _context;
  private readonly CallSite<Func<CallSite, CodeContext, object, object>> _site;
  private object _current;
  private bool _sinkState;

  public SentinelIterator(CodeContext context, object target, object sentinel)
  {
    this._target = target;
    this._sentinel = sentinel;
    this._context = context;
    this._site = CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) this._context.LanguageContext.InvokeOne);
  }

  public object __iter__() => (object) this;

  public object next()
  {
    if (((IEnumerator) this).MoveNext())
      return ((IEnumerator) this).Current;
    throw PythonOps.StopIteration();
  }

  object IEnumerator.Current => this._current;

  object IEnumerator<object>.Current => this._current;

  bool IEnumerator.MoveNext()
  {
    if (this._sinkState)
      return false;
    this._current = this._site.Target((CallSite) this._site, this._context, this._target);
    int num = this._sentinel == this._current ? 1 : (PythonOps.EqualRetBool(this._context, this._sentinel, this._current) ? 1 : 0);
    if (num != 0)
      this._sinkState = true;
    return num == 0;
  }

  void IEnumerator.Reset() => throw new NotImplementedException();

  void IDisposable.Dispose()
  {
  }
}
