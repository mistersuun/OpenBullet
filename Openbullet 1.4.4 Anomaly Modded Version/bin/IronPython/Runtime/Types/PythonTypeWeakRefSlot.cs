// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeWeakRefSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("getset_descriptor")]
public sealed class PythonTypeWeakRefSlot : PythonTypeSlot, ICodeFormattable
{
  private PythonType _type;

  public PythonTypeWeakRefSlot(PythonType parent) => this._type = parent;

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
    IWeakReferenceable weakref;
    if (context.LanguageContext.TryConvertToWeakReferenceable(instance, out weakref))
    {
      WeakRefTracker weakRef = weakref.GetWeakRef();
      value = weakRef == null || weakRef.HandlerCount == 0 ? (object) null : weakRef.GetHandlerCallback(0);
      return true;
    }
    value = (object) null;
    return false;
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    IWeakReferenceable weakref;
    return context.LanguageContext.TryConvertToWeakReferenceable(instance, out weakref) && weakref.SetWeakRef(new WeakRefTracker(weakref, value, instance));
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    throw PythonOps.TypeError("__weakref__ attribute cannot be deleted");
  }

  public override string ToString() => $"<attribute '__weakref__' of '{this._type.Name}' objects>";

  public void __set__(CodeContext context, object instance, object value)
  {
    this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value);
  }

  public string __repr__(CodeContext context)
  {
    return $"<attribute '__weakref__' of {PythonOps.Repr(context, (object) this._type)} objects";
  }
}
