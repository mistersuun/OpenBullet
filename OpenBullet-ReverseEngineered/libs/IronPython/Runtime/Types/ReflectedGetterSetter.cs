// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedGetterSetter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

public abstract class ReflectedGetterSetter : PythonTypeSlot
{
  private MethodInfo[] _getter;
  private MethodInfo[] _setter;
  private readonly NameType _nameType;
  private BuiltinFunction _getfunc;
  private BuiltinFunction _setfunc;

  protected ReflectedGetterSetter(MethodInfo[] getter, MethodInfo[] setter, NameType nt)
  {
    this._getter = ReflectedGetterSetter.RemoveNullEntries(getter);
    this._setter = ReflectedGetterSetter.RemoveNullEntries(setter);
    this._nameType = nt;
  }

  protected ReflectedGetterSetter(ReflectedGetterSetter from)
  {
    this._getter = from._getter;
    this._setter = from._setter;
    this._nameType = from._nameType;
  }

  internal void AddGetter(MethodInfo mi)
  {
    lock (this)
    {
      this._getter = ArrayUtils.Append<MethodInfo>(this._getter, mi);
      this.MakeGetFunc();
    }
  }

  private void MakeGetFunc()
  {
    this._getfunc = PythonTypeOps.GetBuiltinFunction(this.DeclaringType, this.__name__, (MemberInfo[]) this._getter);
  }

  internal void AddSetter(MethodInfo mi)
  {
    lock (this)
    {
      this._setter = ArrayUtils.Append<MethodInfo>(this._setter, mi);
      this.MakeSetFunc();
    }
  }

  private void MakeSetFunc()
  {
    this._setfunc = PythonTypeOps.GetBuiltinFunction(this.DeclaringType, this.__name__, (MemberInfo[]) this._setter);
  }

  internal abstract Type DeclaringType { get; }

  public abstract string __name__ { get; }

  public PythonType __objclass__ => DynamicHelpers.GetPythonTypeFromType(this.DeclaringType);

  internal MethodInfo[] Getter => this._getter;

  internal MethodInfo[] Setter => this._setter;

  public virtual PythonType PropertyType
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return this.Getter != null && this.Getter.Length != 0 ? DynamicHelpers.GetPythonTypeFromType(this.Getter[0].ReturnType) : DynamicHelpers.GetPythonTypeFromType(typeof (object));
    }
  }

  internal NameType NameType => this._nameType;

  internal object CallGetter(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    object instance,
    object[] args)
  {
    if (ReflectedGetterSetter.NeedToReturnProperty(instance, this.Getter))
      return (object) this;
    if (this.Getter.Length == 0)
      throw new MissingMemberException("unreadable property");
    if (this._getfunc == null)
    {
      lock (this)
      {
        if (this._getfunc == null)
          this.MakeGetFunc();
      }
    }
    return this._getfunc.Call(context, storage, instance, args);
  }

  internal object CallTarget(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    MethodInfo[] targets,
    object instance,
    params object[] args)
  {
    return PythonTypeOps.GetBuiltinFunction(this.DeclaringType, this.__name__, (MemberInfo[]) targets).Call(context, storage, instance, args);
  }

  internal static bool NeedToReturnProperty(object instance, MethodInfo[] mis)
  {
    if (instance == null)
    {
      if (mis.Length == 0)
        return true;
      foreach (MethodInfo mi in mis)
      {
        if (!mi.IsStatic || mi.IsDefined(typeof (PropertyMethodAttribute), true) && !mi.IsDefined(typeof (StaticExtensionMethodAttribute), true) && !mi.IsDefined(typeof (WrapperDescriptorAttribute), true))
          return true;
      }
    }
    return false;
  }

  internal bool CallSetter(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    object instance,
    object[] args,
    object value)
  {
    if (ReflectedGetterSetter.NeedToReturnProperty(instance, this.Setter))
      return false;
    if (this._setfunc == null)
    {
      lock (this)
      {
        if (this._setfunc == null)
          this.MakeSetFunc();
      }
    }
    if (args.Length != 0)
      this._setfunc.Call(context, storage, instance, ArrayUtils.Append<object>(args, value));
    else
      this._setfunc.Call(context, storage, instance, new object[1]
      {
        value
      });
    return true;
  }

  internal override bool IsAlwaysVisible => this._nameType == NameType.PythonProperty;

  private static MethodInfo[] RemoveNullEntries(MethodInfo[] mis)
  {
    List<MethodInfo> methodInfoList = (List<MethodInfo>) null;
    for (int index1 = 0; index1 < mis.Length; ++index1)
    {
      if (mis[index1] == (MethodInfo) null)
      {
        if (methodInfoList == null)
        {
          methodInfoList = new List<MethodInfo>();
          for (int index2 = 0; index2 < index1; ++index2)
            methodInfoList.Add(mis[index2]);
        }
      }
      else
        methodInfoList?.Add(mis[index1]);
    }
    return methodInfoList != null ? methodInfoList.ToArray() : mis;
  }
}
