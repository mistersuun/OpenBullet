// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ReversedEnumerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System.Collections;

#nullable disable
namespace IronPython.Runtime;

[PythonType("reversed")]
public class ReversedEnumerator : IEnumerator
{
  private readonly object _getItemMethod;
  private readonly int _savedIndex;
  private object _current;
  private int _index;

  protected ReversedEnumerator(int length, object getitem)
  {
    this._index = this._savedIndex = length;
    this._getItemMethod = getitem;
  }

  public static object __new__(CodeContext context, PythonType type, [NotNull] IReversible o)
  {
    return (object) o.__reversed__();
  }

  public static object __new__(CodeContext context, PythonType type, object o)
  {
    object ret;
    if (PythonOps.TryGetBoundAttr(context, o, "__reversed__", out ret))
      return PythonCalls.Call(context, ret);
    PythonType pythonType = DynamicHelpers.GetPythonType(o);
    PythonTypeSlot slot;
    object getitem;
    if (!pythonType.TryResolveSlot(context, "__getitem__", out slot) || !slot.TryGetValue(context, o, pythonType, out getitem) || o is PythonDictionary)
      throw PythonOps.TypeError("argument to reversed() must be a sequence");
    int length;
    if (!DynamicHelpers.GetPythonType(o).TryGetLength(context, o, out length))
      throw PythonOps.TypeError("object of type '{0}' has no len()", (object) DynamicHelpers.GetPythonType(o).Name);
    return type.UnderlyingSystemType == typeof (ReversedEnumerator) ? (object) new ReversedEnumerator(length, getitem) : type.CreateInstance(context, (object) length, (object) slot);
  }

  public int __length_hint__() => this._savedIndex;

  public ReversedEnumerator __iter__() => this;

  object IEnumerator.Current => this._current;

  bool IEnumerator.MoveNext()
  {
    if (this._index <= 0)
      return false;
    --this._index;
    this._current = PythonCalls.Call(this._getItemMethod, (object) this._index);
    return true;
  }

  void IEnumerator.Reset() => this._index = this._savedIndex;
}
