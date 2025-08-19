// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.MethodTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class MethodTracker : MemberTracker
{
  private readonly MethodInfo _method;

  internal MethodTracker(MethodInfo method)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    this._method = method;
    this.IsStatic = method.IsStatic;
  }

  internal MethodTracker(MethodInfo method, bool isStatic)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    this._method = method;
    this.IsStatic = isStatic;
  }

  public override Type DeclaringType => this._method.DeclaringType;

  public override TrackerTypes MemberType => TrackerTypes.Method;

  public override string Name => this._method.Name;

  public MethodInfo Method => this._method;

  public bool IsPublic => this._method.IsPublic;

  public bool IsStatic { get; }

  public override string ToString() => this._method.ToString();

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return this.IsStatic ? (MemberTracker) this : (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance);
  }

  protected internal override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject instance)
  {
    return binder.ReturnMemberTracker(type, this.BindToInstance(instance));
  }

  internal override DynamicMetaObject Call(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    params DynamicMetaObject[] arguments)
  {
    if (this.Method.IsPublic && this.Method.DeclaringType.IsVisible())
      return binder.MakeCallExpression(resolverFactory, this.Method, arguments);
    if (this.Method.IsStatic)
      return new DynamicMetaObject((Expression) Expression.Convert((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Constant((object) this.Method), typeof (MethodInfo).GetMethod("Invoke", new Type[2]
      {
        typeof (object),
        typeof (object[])
      }), Microsoft.Scripting.Ast.Utils.Constant((object) null), (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) ArrayUtils.ConvertAll<DynamicMetaObject, Expression>(arguments, (Func<DynamicMetaObject, Expression>) (x => x.Expression)))), this.Method.ReturnType), BindingRestrictions.Empty);
    if (arguments.Length == 0)
      throw Microsoft.Scripting.Error.NoInstanceForCall();
    return new DynamicMetaObject((Expression) Expression.Convert((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Constant((object) this.Method), typeof (MethodInfo).GetMethod("Invoke", new Type[2]
    {
      typeof (object),
      typeof (object[])
    }), arguments[0].Expression, (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) ArrayUtils.ConvertAll<DynamicMetaObject, Expression>(ArrayUtils.RemoveFirst<DynamicMetaObject>(arguments), (Func<DynamicMetaObject, Expression>) (x => x.Expression)))), this.Method.ReturnType), BindingRestrictions.Empty);
  }
}
