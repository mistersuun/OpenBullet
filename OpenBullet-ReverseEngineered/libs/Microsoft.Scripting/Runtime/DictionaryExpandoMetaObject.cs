// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DictionaryExpandoMetaObject
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal sealed class DictionaryExpandoMetaObject : DynamicMetaObject
{
  private readonly Func<object, string, object> _getMember;
  private readonly Action<object, string, object> _setMember;
  private readonly Func<object, string, bool> _deleteMember;
  private readonly IEnumerable _keys;

  public DictionaryExpandoMetaObject(
    Expression parameter,
    object storage,
    IEnumerable keys,
    Func<object, string, object> getMember,
    Action<object, string, object> setMember,
    Func<object, string, bool> deleteMember)
    : base(parameter, BindingRestrictions.Empty, storage)
  {
    this._getMember = getMember;
    this._setMember = setMember;
    this._deleteMember = deleteMember;
    this._keys = keys;
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    return this.DynamicTryGetMember(binder.Name, binder.FallbackGetMember((DynamicMetaObject) this).Expression, (Func<Expression, Expression>) (tmp => tmp));
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject[] args)
  {
    return this.DynamicTryGetMember(binder.Name, binder.FallbackInvokeMember((DynamicMetaObject) this, args).Expression, (Func<Expression, Expression>) (tmp => binder.FallbackInvoke(new DynamicMetaObject(tmp, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression));
  }

  private DynamicMetaObject DynamicTryGetMember(
    string name,
    Expression fallback,
    Func<Expression, Expression> resultOp)
  {
    ParameterExpression left = Expression.Parameter(typeof (object));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, (Expression) Expression.Invoke((Expression) Expression.Constant((object) this._getMember), this.Expression, (Expression) Expression.Constant((object) name))), (Expression) Expression.Constant(StringDictionaryExpando._getFailed)), ExpressionUtils.Convert(resultOp((Expression) left), typeof (object)), ExpressionUtils.Convert(fallback, typeof (object)))), this.GetRestrictions());
  }

  private BindingRestrictions GetRestrictions()
  {
    return BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType());
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
  {
    return new DynamicMetaObject((Expression) Expression.Block((Expression) Expression.Invoke((Expression) Expression.Constant((object) this._setMember), this.Expression, (Expression) Expression.Constant((object) binder.Name), (Expression) Expression.Convert(value.Expression, typeof (object))), value.Expression), this.GetRestrictions());
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
  {
    return new DynamicMetaObject((Expression) Expression.Condition((Expression) Expression.Invoke((Expression) Expression.Constant((object) this._deleteMember), this.Expression, (Expression) Expression.Constant((object) binder.Name)), (Expression) Expression.Default(binder.ReturnType), binder.FallbackDeleteMember((DynamicMetaObject) this).Expression), this.GetRestrictions());
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object key in this._keys)
    {
      if (key is string dynamicMemberName)
        yield return dynamicMemberName;
    }
  }
}
