// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType]
public class PythonTypeSlot
{
  internal virtual bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = (object) null;
    return false;
  }

  internal virtual bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    return false;
  }

  internal virtual bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    return false;
  }

  internal virtual bool IsAlwaysVisible => true;

  internal virtual bool CanOptimizeGets => false;

  internal virtual void MakeGetExpression(
    PythonBinder binder,
    Expression codeContext,
    DynamicMetaObject instance,
    DynamicMetaObject owner,
    ConditionalBuilder builder)
  {
    ParameterExpression parameterExpression = Expression.Variable(typeof (object), "slotTmp");
    Expression condition = (Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryGetValue"), codeContext, Utils.Convert((Expression) Utils.WeakConstant((object) this), typeof (PythonTypeSlot)), instance != null ? instance.Expression : Utils.Constant((object) null), owner.Expression, (Expression) parameterExpression);
    builder.AddVariable(parameterExpression);
    if (!this.GetAlwaysSucceeds)
      builder.AddCondition(condition, (Expression) parameterExpression);
    else
      builder.FinishCondition((Expression) Expression.Block(condition, (Expression) parameterExpression));
  }

  internal virtual bool GetAlwaysSucceeds => false;

  internal virtual bool IsSetDescriptor(CodeContext context, PythonType owner) => false;

  public virtual object __get__(CodeContext context, object instance, object typeContext = null)
  {
    PythonType owner = typeContext as PythonType;
    object obj;
    if (this.TryGetValue(context, instance, owner, out obj))
      return obj;
    throw PythonOps.AttributeErrorForMissingAttribute(owner == null ? "?" : owner.Name, nameof (__get__));
  }
}
