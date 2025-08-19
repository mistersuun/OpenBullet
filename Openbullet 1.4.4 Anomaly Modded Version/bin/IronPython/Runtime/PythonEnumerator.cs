// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;

#nullable disable
namespace IronPython.Runtime;

[PythonType("enumerator")]
public class PythonEnumerator : IEnumerator
{
  private readonly object _baseObject;
  private object _nextMethod;
  private object _current;

  public static bool TryCastIEnumer(object baseObject, out IEnumerator enumerator)
  {
    switch (baseObject)
    {
      case IEnumerator _:
        enumerator = (IEnumerator) baseObject;
        return true;
      case IEnumerable _:
        enumerator = ((IEnumerable) baseObject).GetEnumerator();
        return true;
      default:
        enumerator = (IEnumerator) null;
        return false;
    }
  }

  public static bool TryCreate(object baseObject, out IEnumerator enumerator)
  {
    if (PythonEnumerator.TryCastIEnumer(baseObject, out enumerator))
      return true;
    object ret;
    if (PythonOps.TryGetBoundAttr(baseObject, "__iter__", out ret))
    {
      object obj = PythonCalls.Call(ret);
      if (PythonEnumerator.TryCastIEnumer(obj, out enumerator))
        return true;
      enumerator = (IEnumerator) new PythonEnumerator(obj);
      return true;
    }
    enumerator = (IEnumerator) null;
    return false;
  }

  public static IEnumerator Create(object baseObject)
  {
    IEnumerator enumerator;
    if (!PythonEnumerator.TryCreate(baseObject, out enumerator))
      throw PythonOps.TypeError("cannot convert {0} to IEnumerator", (object) PythonTypeOps.GetName(baseObject));
    return enumerator;
  }

  internal PythonEnumerator(object iter) => this._baseObject = iter;

  public void Reset() => throw new NotImplementedException();

  public object Current => this._current;

  public bool MoveNext()
  {
    if (this._nextMethod == null && (!PythonOps.TryGetBoundAttr(this._baseObject, "next", out this._nextMethod) || this._nextMethod == null))
      throw PythonOps.TypeError("instance has no next() method");
    try
    {
      this._current = DefaultContext.Default.LanguageContext.CallLightEh(DefaultContext.Default, this._nextMethod);
      Exception lightException = LightExceptions.GetLightException(this._current);
      if (lightException == null)
        return true;
      if (lightException is StopIterationException)
        return false;
      throw lightException;
    }
    catch (StopIterationException ex)
    {
      return false;
    }
  }

  public object __iter__() => (object) this;
}
