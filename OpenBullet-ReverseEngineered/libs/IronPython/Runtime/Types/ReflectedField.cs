// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedField
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("field#")]
public sealed class ReflectedField : PythonTypeSlot, ICodeFormattable
{
  private readonly NameType _nameType;
  internal readonly FieldInfo _info;
  internal const string UpdateValueTypeFieldWarning = "Setting field {0} on value type {1} may result in updating a copy.  Use {1}.{0}.SetValue(instance, value) if this is safe.  For more information help({1}.{0}.SetValue).";

  public ReflectedField(FieldInfo info, NameType nameType)
  {
    this._nameType = nameType;
    this._info = info;
  }

  public ReflectedField(FieldInfo info)
    : this(info, NameType.PythonField)
  {
  }

  public FieldInfo Info
  {
    [PythonHidden(new PlatformID[] {})] get => this._info;
  }

  public object GetValue(CodeContext context, object instance)
  {
    object obj;
    if (this.TryGetValue(context, instance, DynamicHelpers.GetPythonType(instance), out obj))
      return obj;
    throw new InvalidOperationException("cannot get field");
  }

  public void SetValue(CodeContext context, object instance, object value)
  {
    if (!this.TrySetValueWorker(context, instance, DynamicHelpers.GetPythonType(instance), value, true))
      throw new InvalidOperationException("cannot set field");
  }

  public void __set__(CodeContext context, object instance, object value)
  {
    if (instance == null && this._info.IsStatic)
    {
      this.DoSet(context, (object) null, value, false);
    }
    else
    {
      if (this._info.IsStatic)
        throw PythonOps.AttributeErrorForReadonlyAttribute(this._info.DeclaringType.Name, this._info.Name);
      this.DoSet(context, instance, value, false);
    }
  }

  [SpecialName]
  public void __delete__(object instance)
  {
    throw PythonOps.AttributeErrorForBuiltinAttributeDeletion(this._info.DeclaringType.Name, this._info.Name);
  }

  public string __doc__ => DocBuilder.DocOneInfo(this._info);

  public PythonType FieldType
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return DynamicHelpers.GetPythonTypeFromType(this._info.FieldType);
    }
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = instance != null ? this._info.GetValue(context.LanguageContext.Binder.Convert(instance, this._info.DeclaringType)) : (!this._info.IsStatic ? (object) this : this._info.GetValue((object) null));
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override bool CanOptimizeGets => !this._info.IsLiteral;

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    return this.TrySetValueWorker(context, instance, owner, value, false);
  }

  private bool TrySetValueWorker(
    CodeContext context,
    object instance,
    PythonType owner,
    object value,
    bool suppressWarning)
  {
    if (!this.ShouldSetOrDelete(owner))
      return false;
    this.DoSet(context, instance, value, suppressWarning);
    return true;
  }

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner)
  {
    return (this._info.Attributes & FieldAttributes.InitOnly) == FieldAttributes.PrivateScope && !this._info.IsLiteral;
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    if (this.ShouldSetOrDelete(owner))
      throw PythonOps.AttributeErrorForBuiltinAttributeDeletion(this._info.DeclaringType.Name, this._info.Name);
    return false;
  }

  internal override bool IsAlwaysVisible => this._nameType == NameType.PythonField;

  internal override void MakeGetExpression(
    PythonBinder binder,
    Expression codeContext,
    DynamicMetaObject instance,
    DynamicMetaObject owner,
    IronPython.Runtime.Binding.ConditionalBuilder builder)
  {
    if (!this._info.IsPublic || this._info.DeclaringType.ContainsGenericParameters())
      base.MakeGetExpression(binder, codeContext, instance, owner, builder);
    else if (instance == null)
    {
      if (this._info.IsStatic)
        builder.FinishCondition(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Field((Expression) null, this._info), typeof (object)));
      else
        builder.FinishCondition((Expression) Expression.Constant((object) this));
    }
    else
      builder.FinishCondition(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Field(binder.ConvertExpression(instance.Expression, this._info.DeclaringType, ConversionResultKind.ExplicitCast, (OverloadResolverFactory) new PythonOverloadResolverFactory(binder, codeContext)), this._info), typeof (object)));
  }

  private void DoSet(CodeContext context, object instance, object val, bool suppressWarning)
  {
    if (this._info.IsInitOnly || this._info.IsLiteral)
      throw PythonOps.AttributeErrorForReadonlyAttribute(this._info.DeclaringType.Name, this._info.Name);
    if (!suppressWarning && instance != null && instance.GetType().IsValueType())
      PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "Setting field {0} on value type {1} may result in updating a copy.  Use {1}.{0}.SetValue(instance, value) if this is safe.  For more information help({1}.{0}.SetValue).", (object) this._info.Name, (object) this._info.DeclaringType.Name);
    this._info.SetValue(instance, context.LanguageContext.Binder.Convert(val, this._info.FieldType));
  }

  private bool ShouldSetOrDelete(PythonType type)
  {
    PythonType pythonType = type;
    return pythonType != null && this._info.DeclaringType == pythonType.UnderlyingSystemType || !this._info.IsStatic || this._info.IsLiteral || this._info.IsInitOnly;
  }

  public string __repr__(CodeContext context)
  {
    return $"<field# {this._info.Name} on {this._info.DeclaringType.Name}>";
  }
}
