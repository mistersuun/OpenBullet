// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.Expression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using System;

#nullable disable
namespace IronPython.Compiler.Ast;

public abstract class Expression : Node
{
  internal static Expression[] EmptyArray = new Expression[0];

  internal virtual System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    throw new InvalidOperationException();
  }

  internal virtual System.Linq.Expressions.Expression TransformDelete()
  {
    throw new InvalidOperationException();
  }

  internal virtual ConstantExpression ConstantFold() => (ConstantExpression) null;

  internal virtual string CheckAssign() => "can't assign to " + this.NodeName;

  internal virtual string CheckAugmentedAssign() => this.CheckAssign();

  internal virtual string CheckDelete() => "can't delete " + this.NodeName;

  internal virtual bool IsConstant
  {
    get
    {
      ConstantExpression constantExpression = this.ConstantFold();
      return constantExpression != null && constantExpression.IsConstant;
    }
  }

  internal virtual object GetConstantValue()
  {
    ConstantExpression constantExpression = this.ConstantFold();
    return constantExpression != null && constantExpression.IsConstant ? constantExpression.GetConstantValue() : throw new InvalidOperationException(this.GetType().Name + " is not a constant");
  }

  public override Type Type => typeof (object);
}
