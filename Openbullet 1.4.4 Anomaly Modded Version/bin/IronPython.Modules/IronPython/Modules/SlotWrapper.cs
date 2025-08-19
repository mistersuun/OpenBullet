// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.SlotWrapper
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;

#nullable disable
namespace IronPython.Modules;

[PythonType("wrapper_descriptor")]
internal class SlotWrapper : PythonTypeSlot, ICodeFormattable
{
  private readonly string _name;
  private readonly PythonType _type;

  public SlotWrapper(string slotName, PythonType targetType)
  {
    this._name = slotName;
    this._type = targetType;
  }

  public virtual string __repr__(CodeContext context)
  {
    return $"<slot wrapper {PythonOps.Repr(context, (object) this._name)} of {PythonOps.Repr(context, (object) this._type.Name)} objects>";
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    if (instance == null)
    {
      value = (object) this;
      return true;
    }
    if (!(instance is IProxyObject proxyTarget))
      throw PythonOps.TypeError("descriptor for {0} object doesn't apply to {1} object", (object) PythonOps.Repr(context, (object) this._type.Name), (object) PythonOps.Repr(context, (object) PythonTypeOps.GetName(instance)));
    if (!DynamicHelpers.GetPythonType(proxyTarget.Target).TryGetBoundMember(context, proxyTarget.Target, this._name, out value))
      return false;
    value = (object) new GenericMethodWrapper(this._name, proxyTarget);
    return true;
  }
}
