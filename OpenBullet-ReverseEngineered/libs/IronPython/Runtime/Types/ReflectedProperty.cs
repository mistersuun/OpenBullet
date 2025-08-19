// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedProperty
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("getset_descriptor")]
public class ReflectedProperty : ReflectedGetterSetter, ICodeFormattable
{
  private readonly PropertyInfo _info;

  public ReflectedProperty(PropertyInfo info, MethodInfo getter, MethodInfo setter, NameType nt)
    : base(new MethodInfo[1]{ getter }, new MethodInfo[1]
    {
      setter
    }, nt)
  {
    this._info = info;
  }

  internal override bool CanOptimizeGets => true;

  public ReflectedProperty(
    PropertyInfo info,
    MethodInfo[] getters,
    MethodInfo[] setters,
    NameType nt)
    : base(getters, setters, nt)
  {
    this._info = info;
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    if (this.Setter.Length == 0)
      return false;
    if (instance == null)
    {
      foreach (MethodInfo info in this.Setter)
      {
        if (info.IsStatic && this.DeclaringType != owner.UnderlyingSystemType)
          return false;
        if (info.IsProtected())
          throw PythonOps.TypeErrorForProtectedMember(owner.UnderlyingSystemType, this._info.Name);
      }
    }
    else if (instance != null)
    {
      foreach (MethodBase methodBase in this.Setter)
      {
        if (methodBase.IsStatic)
          return false;
      }
    }
    return this.CallSetter(context, context.LanguageContext.GetGenericCallSiteStorage(), instance, ArrayUtils.EmptyObjects, value);
  }

  internal override Type DeclaringType => this._info.DeclaringType;

  public override string __name__ => this._info.Name;

  public PropertyInfo Info
  {
    [PythonHidden(new PlatformID[] {})] get => this._info;
  }

  public override PythonType PropertyType
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return DynamicHelpers.GetPythonTypeFromType(this._info.PropertyType);
    }
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = this.CallGetter(context, owner, context.LanguageContext.GetGenericCallSiteStorage0(), instance);
    return true;
  }

  private object CallGetter(
    CodeContext context,
    PythonType owner,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object>>> storage,
    object instance)
  {
    if (ReflectedGetterSetter.NeedToReturnProperty(instance, this.Getter))
      return (object) this;
    if (this.Getter.Length == 0)
      throw new MissingMemberException("unreadable property");
    if (owner == null)
      owner = DynamicHelpers.GetPythonType(instance);
    MethodInfo[] mems = this.Getter;
    Type underlyingSystemType = owner.UnderlyingSystemType;
    if (this.Getter.Length > 1)
    {
      Type declaringType = this.Getter[0].DeclaringType;
      MethodInfo mt1 = this.Getter[0];
      for (int index = 1; index < this.Getter.Length; ++index)
      {
        MethodInfo mt2 = this.Getter[index];
        if (ReflectedProperty.IsApplicableForType(underlyingSystemType, mt2) && (this.Getter[index].DeclaringType.IsSubclassOf(declaringType) || !ReflectedProperty.IsApplicableForType(underlyingSystemType, mt1)))
        {
          mt1 = this.Getter[index];
          declaringType = this.Getter[index].DeclaringType;
        }
      }
      mems = new MethodInfo[1]{ mt1 };
    }
    return PythonTypeOps.GetBuiltinFunction(underlyingSystemType, this.__name__, (MemberInfo[]) mems).Call0(context, storage, instance);
  }

  private static bool IsApplicableForType(Type type, MethodInfo mt)
  {
    return mt.DeclaringType == type || type.IsSubclassOf(mt.DeclaringType);
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    this.__delete__(instance);
    return true;
  }

  internal override void MakeGetExpression(
    PythonBinder binder,
    Expression codeContext,
    DynamicMetaObject instance,
    DynamicMetaObject owner,
    IronPython.Runtime.Binding.ConditionalBuilder builder)
  {
    if (this.Getter.Length != 0 && !this.Getter[0].IsPublic)
      base.MakeGetExpression(binder, codeContext, instance, owner, builder);
    else if (ReflectedGetterSetter.NeedToReturnProperty((object) instance, this.Getter))
      builder.FinishCondition(Microsoft.Scripting.Ast.Utils.Constant((object) this));
    else if (this.Getter[0].ContainsGenericParameters)
      builder.FinishCondition(Microsoft.Scripting.Actions.DefaultBinder.MakeError(binder.MakeContainsGenericParametersError(MemberTracker.FromMemberInfo((MemberInfo) this._info)), typeof (object)).Expression);
    else if (instance != null)
      builder.FinishCondition(Microsoft.Scripting.Ast.Utils.Convert(binder.MakeCallExpression((OverloadResolverFactory) new PythonOverloadResolverFactory(binder, codeContext), this.Getter[0], instance).Expression, typeof (object)));
    else
      builder.FinishCondition(Microsoft.Scripting.Ast.Utils.Convert(binder.MakeCallExpression((OverloadResolverFactory) new PythonOverloadResolverFactory(binder, codeContext), this.Getter[0]).Expression, typeof (object)));
  }

  internal override bool IsAlwaysVisible => this.NameType == NameType.PythonProperty;

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner)
  {
    return this.Setter.Length != 0;
  }

  [PythonHidden(new PlatformID[] {})]
  public object GetValue(CodeContext context, object instance)
  {
    object obj;
    if (this.TryGetValue(context, instance, DynamicHelpers.GetPythonType(instance), out obj))
      return obj;
    throw new InvalidOperationException("cannot get property");
  }

  [PythonHidden(new PlatformID[] {})]
  public void SetValue(CodeContext context, object instance, object value)
  {
    if (!this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value))
      throw new InvalidOperationException("cannot set property");
  }

  public void __set__(CodeContext context, object instance, object value)
  {
    this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value);
  }

  public void __delete__(object instance)
  {
    if (this.Setter.Length != 0)
      throw PythonOps.AttributeErrorForReadonlyAttribute(DynamicHelpers.GetPythonTypeFromType(this.DeclaringType).Name, this.__name__);
    throw PythonOps.AttributeErrorForBuiltinAttributeDeletion(DynamicHelpers.GetPythonTypeFromType(this.DeclaringType).Name, this.__name__);
  }

  public string __doc__ => DocBuilder.DocOneInfo(this.Info);

  public string __repr__(CodeContext context)
  {
    return $"<property# {this.__name__} on {DynamicHelpers.GetPythonTypeFromType(this.DeclaringType).Name}>";
  }
}
