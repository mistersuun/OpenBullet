// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeDataSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;

#nullable disable
namespace IronPython.Runtime.Types;

public class PythonTypeDataSlot : PythonTypeSlot
{
  public virtual void __set__(CodeContext context, object instance, object value)
  {
    if (!this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value))
      throw PythonOps.AttributeErrorForObjectMissingAttribute(instance, nameof (__set__));
  }

  public virtual void __delete__(CodeContext context, object instance)
  {
    if (!this.TryDeleteValue(context, instance, DynamicHelpers.GetPythonType(instance)))
      throw PythonOps.AttributeErrorForObjectMissingAttribute(instance, nameof (__delete__));
  }

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner) => true;
}
