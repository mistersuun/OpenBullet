// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedExtensionProperty
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("member_descriptor")]
public class ReflectedExtensionProperty : ReflectedGetterSetter
{
  private readonly MethodInfo _deleter;
  private readonly ExtensionPropertyInfo _extInfo;

  public ReflectedExtensionProperty(ExtensionPropertyInfo info, NameType nt)
    : base(new MethodInfo[1]{ info.Getter }, new MethodInfo[1]
    {
      info.Setter
    }, nt)
  {
    this._extInfo = info;
    this._deleter = info.Deleter;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    if (this.Getter.Length == 0 || instance == null && this.Getter[0].IsDefined(typeof (WrapperDescriptorAttribute), false))
    {
      value = (object) null;
      return false;
    }
    value = this.CallGetter(context, (SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>>) null, instance, ArrayUtils.EmptyObjects);
    return true;
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    return this.Setter.Length != 0 && instance != null && this.CallSetter(context, (SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>>) null, instance, ArrayUtils.EmptyObjects, value);
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    if (this._deleter == (MethodInfo) null || instance == null)
      return base.TryDeleteValue(context, instance, owner);
    this.CallTarget(context, (SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>>) null, new MethodInfo[1]
    {
      this._deleter
    }, instance);
    return true;
  }

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner)
  {
    return this.Setter.Length != 0;
  }

  internal override bool GetAlwaysSucceeds
  {
    get
    {
      return this.Getter.Length != 0 && !this.Getter[0].IsDefined(typeof (WrapperDescriptorAttribute), false);
    }
  }

  internal override bool CanOptimizeGets => true;

  public void __set__(CodeContext context, object instance, object value)
  {
    if (!this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value))
      throw PythonOps.TypeError("readonly attribute");
  }

  public void __delete__(CodeContext context, object instance)
  {
    if (!this.TryDeleteValue(context, instance, DynamicHelpers.GetPythonType(instance)))
      throw PythonOps.AttributeErrorForObjectMissingAttribute(instance, nameof (__delete__));
  }

  internal override Type DeclaringType => this._extInfo.DeclaringType;

  internal ExtensionPropertyInfo ExtInfo => this._extInfo;

  public override string __name__ => this._extInfo.Name;

  public string __doc__ => DocBuilder.DocOneInfo(this.ExtInfo);
}
