// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.FieldTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class FieldTracker : MemberTracker
{
  private readonly FieldInfo _field;

  public FieldTracker(FieldInfo field)
  {
    ContractUtils.RequiresNotNull((object) field, nameof (field));
    this._field = field;
  }

  public override Type DeclaringType => this._field.DeclaringType;

  public override TrackerTypes MemberType => TrackerTypes.Field;

  public override string Name => this._field.Name;

  public bool IsPublic => this._field.IsPublic;

  public bool IsInitOnly => this._field.IsInitOnly;

  public bool IsLiteral => this._field.IsLiteral;

  public Type FieldType => this._field.FieldType;

  public bool IsStatic => this._field.IsStatic;

  public FieldInfo Field => this._field;

  public override string ToString() => this._field.ToString();

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type)
  {
    if (this.Field.IsLiteral)
      return new DynamicMetaObject((Expression) Microsoft.Scripting.Ast.Utils.Constant(this.Field.GetValue((object) null), typeof (object)), BindingRestrictions.Empty);
    if (!this.IsStatic)
      return binder.ReturnMemberTracker(type, (MemberTracker) this);
    if (this.Field.DeclaringType.ContainsGenericParameters())
      return (DynamicMetaObject) null;
    if (this.IsPublic && this.DeclaringType.IsPublic())
      return new DynamicMetaObject((Expression) Expression.Convert((Expression) Expression.Field((Expression) null, this.Field), typeof (object)), BindingRestrictions.Empty);
    return new DynamicMetaObject((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Constant((object) this.Field), typeof (FieldInfo)), typeof (FieldInfo).GetMethod(nameof (GetValue)), Microsoft.Scripting.Ast.Utils.Constant((object) null)), BindingRestrictions.Empty);
  }

  public override ErrorInfo GetError(ActionBinder binder, Type instanceType)
  {
    return binder.MakeContainsGenericParametersError((MemberTracker) this);
  }

  protected internal override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject instance)
  {
    return this.IsPublic && this.DeclaringType.IsVisible() ? new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, this.Field.DeclaringType), this.Field), typeof (object)), BindingRestrictions.Empty) : DefaultBinder.MakeError(((DefaultBinder) binder).MakeNonPublicMemberGetError(resolverFactory, (MemberTracker) this, type, instance), BindingRestrictions.Empty, typeof (object));
  }

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return this.IsStatic ? (MemberTracker) this : (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance);
  }
}
