// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ClassMethodDescriptor
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;

#nullable disable
namespace IronPython.Runtime;

[PythonType("method_descriptor")]
[DontMapGetMemberNamesToDir]
public class ClassMethodDescriptor : PythonTypeSlot, ICodeFormattable
{
  internal readonly BuiltinFunction _func;

  internal ClassMethodDescriptor(BuiltinFunction func) => this._func = func;

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    owner = this.CheckGetArgs(context, instance, owner);
    value = (object) new Method((object) this._func, (object) owner, (object) DynamicHelpers.GetPythonType((object) owner));
    return true;
  }

  private PythonType CheckGetArgs(CodeContext context, object instance, PythonType owner)
  {
    if (owner == null)
      owner = instance != null ? DynamicHelpers.GetPythonType(instance) : throw PythonOps.TypeError("__get__(None, None) is invalid");
    else if (!owner.IsSubclassOf(DynamicHelpers.GetPythonTypeFromType(this._func.DeclaringType)))
      throw PythonOps.TypeError("descriptor {0} for type {1} doesn't apply to type {2}", (object) PythonOps.Repr(context, (object) this._func.Name), (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonTypeFromType(this._func.DeclaringType).Name), (object) PythonOps.Repr(context, (object) owner.Name));
    if (instance != null)
      BuiltinMethodDescriptor.CheckSelfWorker(context, instance, this._func);
    return owner;
  }

  public virtual string __repr__(CodeContext context)
  {
    BuiltinFunction func = this._func;
    return func != null ? $"<method {PythonOps.Repr(context, (object) func.Name)} of {PythonOps.Repr(context, (object) DynamicHelpers.GetPythonTypeFromType(func.DeclaringType).Name)} objects>" : $"<classmethod object at {IdDispenser.GetId((object) this)}>";
  }

  public override bool Equals(object obj)
  {
    return obj is ClassMethodDescriptor methodDescriptor && methodDescriptor._func == this._func;
  }

  public override int GetHashCode() => ~this._func.GetHashCode();
}
