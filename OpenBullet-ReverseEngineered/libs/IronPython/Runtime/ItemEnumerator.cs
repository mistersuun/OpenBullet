// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ItemEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("iterator")]
public class ItemEnumerator : IEnumerator
{
  private readonly object _getItemMethod;
  private readonly CallSite<Func<CallSite, CodeContext, object, int, object>> _site;
  private object _current;
  private int _index;

  internal ItemEnumerator(
    object getItemMethod,
    CallSite<Func<CallSite, CodeContext, object, int, object>> site)
  {
    this._getItemMethod = getItemMethod;
    this._site = site;
  }

  object IEnumerator.Current => this._current;

  bool IEnumerator.MoveNext()
  {
    if (this._index < 0)
      return false;
    try
    {
      this._current = this._site.Target((CallSite) this._site, DefaultContext.Default, this._getItemMethod, this._index);
      ++this._index;
      return true;
    }
    catch (IndexOutOfRangeException ex)
    {
      this._current = (object) null;
      this._index = -1;
      return false;
    }
    catch (StopIterationException ex)
    {
      this._current = (object) null;
      this._index = -1;
      return false;
    }
  }

  void IEnumerator.Reset()
  {
    this._index = 0;
    this._current = (object) null;
  }
}
