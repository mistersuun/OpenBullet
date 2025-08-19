// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.staticmethod
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;

#nullable disable
namespace IronPython.Runtime;

[PythonType]
public class staticmethod : PythonTypeSlot
{
  internal object _func;

  public staticmethod(object func) => this._func = func;

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = this.__get__(instance, PythonOps.ToPythonType(owner));
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  public object __func__ => this._func;

  public object __get__(object instance) => this.__get__(instance, (object) null);

  public object __get__(object instance, object owner) => this._func;
}
