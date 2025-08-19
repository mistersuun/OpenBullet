// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Enumerate
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace IronPython.Runtime;

[PythonType("enumerate")]
[Documentation("enumerate(iterable) -> iterator for index, value of iterable")]
[DontMapIDisposableToContextManager]
[DontMapIEnumerableToContains]
public class Enumerate : IEnumerator, IEnumerator<object>, IDisposable
{
  private readonly IEnumerator _iter;
  private object _index;

  public Enumerate(object iter)
  {
    this._iter = PythonOps.GetEnumerator(iter);
    this._index = ScriptingRuntimeHelpers.Int32ToObject(-1);
  }

  public Enumerate(CodeContext context, object iter, object start)
  {
    object index;
    if (!Converter.TryConvertToIndex(start, out index))
      throw PythonOps.TypeErrorForUnIndexableObject(start);
    this._iter = PythonOps.GetEnumerator(iter);
    this._index = context.LanguageContext.Operation(PythonOperationKind.Subtract, index, ScriptingRuntimeHelpers.Int32ToObject(1));
  }

  public object __iter__() => (object) this;

  void IEnumerator.Reset() => throw new NotImplementedException();

  object IEnumerator.Current => (object) PythonTuple.MakeTuple(this._index, this._iter.Current);

  object IEnumerator<object>.Current => ((IEnumerator) this).Current;

  bool IEnumerator.MoveNext()
  {
    if (this._index is int)
    {
      int index = (int) this._index;
      this._index = index == int.MaxValue ? (object) (new BigInteger(int.MaxValue) + (BigInteger) 1) : ScriptingRuntimeHelpers.Int32ToObject(index + 1);
    }
    else
      this._index = (object) ((BigInteger) this._index + (BigInteger) 1);
    return this._iter.MoveNext();
  }

  void IDisposable.Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  [PythonHidden(new PlatformID[] {})]
  protected virtual void Dispose(bool notFinalizing)
  {
  }
}
