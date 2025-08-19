// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonRawGlobalValueExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal class PythonRawGlobalValueExpression : Expression
{
  private readonly PythonGlobalVariableExpression _global;

  public PythonRawGlobalValueExpression(PythonGlobalVariableExpression global)
  {
    this._global = global;
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => typeof (object);

  public override bool CanReduce => true;

  public PythonGlobalVariableExpression Global => this._global;

  public override Expression Reduce()
  {
    return (Expression) Expression.Property(this._global.Target, PythonGlobal.RawValueProperty);
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor) => (Expression) this;
}
