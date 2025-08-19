// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonEnumerable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Collections;

#nullable disable
namespace IronPython.Runtime;

[PythonType("enumerable")]
public class PythonEnumerable : IEnumerable
{
  private object _iterator;

  public static bool TryCreate(object baseEnumerator, out IEnumerable enumerator)
  {
    object ret;
    if (PythonOps.TryGetBoundAttr(baseEnumerator, "__iter__", out ret))
    {
      object iterator = PythonCalls.Call(ret);
      enumerator = !(iterator is IEnumerable) ? (IEnumerable) new PythonEnumerable(iterator) : (IEnumerable) iterator;
      return true;
    }
    enumerator = (IEnumerable) null;
    return false;
  }

  public static IEnumerable Create(object baseObject)
  {
    IEnumerable enumerator;
    if (!PythonEnumerable.TryCreate(baseObject, out enumerator))
      throw PythonOps.TypeError("cannot convert {0} to IEnumerable", (object) PythonTypeOps.GetName(baseObject));
    return enumerator;
  }

  private PythonEnumerable(object iterator) => this._iterator = iterator;

  IEnumerator IEnumerable.GetEnumerator()
  {
    return this._iterator is IEnumerator iterator ? iterator : (IEnumerator) new PythonEnumerator(this._iterator);
  }
}
