// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaExpandable`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Binding;

public sealed class MetaExpandable<T>(Expression parameter, IPythonExpandable value) : 
  DynamicMetaObject(parameter, BindingRestrictions.Empty, (object) value)
  where T : IPythonExpandable
{
  private static readonly object _getFailed = new object();

  public T Value => (T) base.Value;

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    return this.DynamicTryGetMember(binder.Name, binder.FallbackGetMember((DynamicMetaObject) this).Expression, (Func<Expression, Expression>) (res => res));
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject[] args)
  {
    return this.DynamicTryGetMember(binder.Name, binder.FallbackInvokeMember((DynamicMetaObject) this, args).Expression, (Func<Expression, Expression>) (res => binder.FallbackInvoke(new DynamicMetaObject(res, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression));
  }

  private DynamicMetaObject DynamicTryGetMember(
    string name,
    Expression fallback,
    Func<Expression, Expression> transform)
  {
    ParameterExpression left = Expression.Parameter(typeof (object));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, (Expression) Expression.Invoke((Expression) Expression.Constant((object) new Func<T, string, object>(MetaExpandable<T>.TryGetMember)), MetaExpandable<T>.Convert(this.Expression, typeof (T)), (Expression) Expression.Constant((object) name))), (Expression) Expression.Constant(MetaExpandable<T>._getFailed)), Utils.Convert(transform((Expression) left), typeof (object)), Utils.Convert(fallback, typeof (object)))), this.GetRestrictions());
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
  {
    return new DynamicMetaObject((Expression) Expression.Block((Expression) Expression.Condition(MetaExpandable<T>.Convert((Expression) Expression.Invoke((Expression) Expression.Constant((object) new Func<T, string, object, bool>(MetaExpandable<T>.TrySetMember)), MetaExpandable<T>.Convert(this.Expression, typeof (T)), (Expression) Expression.Constant((object) binder.Name), MetaExpandable<T>.Convert(value.Expression, typeof (object))), typeof (bool)), (Expression) Expression.Empty(), binder.FallbackSetMember((DynamicMetaObject) this, value).Expression, typeof (void)), value.Expression), this.GetRestrictions());
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
  {
    return new DynamicMetaObject((Expression) Expression.Condition(MetaExpandable<T>.Convert((Expression) Expression.Invoke((Expression) Expression.Constant((object) new Func<T, string, bool>(MetaExpandable<T>.TryDeleteMember)), MetaExpandable<T>.Convert(this.Expression, typeof (T)), (Expression) Expression.Constant((object) binder.Name)), typeof (bool)), (Expression) Expression.Empty(), binder.FallbackDeleteMember((DynamicMetaObject) this).Expression, typeof (void)), this.GetRestrictions());
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    IDictionary<string, object> customAttributes = this.Value.CustomAttributes;
    if (customAttributes == null)
      return base.GetDynamicMemberNames();
    IList<object> memberNames = (IList<object>) DynamicHelpers.GetPythonType((object) this.Value).GetMemberNames(this.Value.Context);
    List<string> dynamicMemberNames = new List<string>(customAttributes.Keys.Count + memberNames.Count);
    dynamicMemberNames.AddRange((IEnumerable<string>) customAttributes.Keys);
    for (int index = 0; index < memberNames.Count; ++index)
    {
      if (!(memberNames[index] is string str))
      {
        if (memberNames[index] is Extensible<string> extensible)
          str = extensible.Value;
        else
          continue;
      }
      dynamicMemberNames.Add(str);
    }
    return (IEnumerable<string>) dynamicMemberNames;
  }

  private BindingRestrictions GetRestrictions()
  {
    return BindingRestrictions.GetTypeRestriction(this.Expression, typeof (T));
  }

  private static Expression Convert(Expression expression, Type type)
  {
    return !(expression.Type != type) ? expression : (Expression) Expression.Convert(expression, type);
  }

  private static object TryGetMember(T target, string name)
  {
    IDictionary<string, object> customAttributes = target.CustomAttributes;
    object obj;
    return customAttributes != null && customAttributes.TryGetValue(name, out obj) ? obj : MetaExpandable<T>._getFailed;
  }

  private static bool TrySetMember(T target, string name, object value)
  {
    MemberInfo memberInfo = (MemberInfo) typeof (T).GetProperty(name);
    if ((object) memberInfo == null)
      memberInfo = (MemberInfo) typeof (T).GetField(name);
    MemberInfo m = memberInfo;
    if (m != (MemberInfo) null && !PythonHiddenAttribute.IsHidden(m))
      return false;
    target.EnsureCustomAttributes()[name] = value;
    return true;
  }

  private static bool TryDeleteMember(T target, string name)
  {
    IDictionary<string, object> customAttributes = target.CustomAttributes;
    return customAttributes != null && customAttributes.Remove(name);
  }
}
