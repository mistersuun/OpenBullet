// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ConstantExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using System;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ConstantExpression : Expression, IInstructionProvider
{
  private readonly object _value;
  private static readonly System.Linq.Expressions.Expression EllipsisExpr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) null, typeof (PythonOps).GetProperty("Ellipsis"));
  private static readonly System.Linq.Expressions.Expression TrueExpr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (ScriptingRuntimeHelpers).GetField("True"));
  private static readonly System.Linq.Expressions.Expression FalseExpr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (ScriptingRuntimeHelpers).GetField("False"));

  public ConstantExpression(object value) => this._value = value;

  internal static ConstantExpression MakeUnicode(string value)
  {
    return new ConstantExpression((object) new ConstantExpression.UnicodeWrapper(value));
  }

  public object Value
  {
    get
    {
      return this._value is ConstantExpression.UnicodeWrapper unicodeWrapper ? unicodeWrapper.Value : this._value;
    }
  }

  internal bool IsUnicodeString => this._value is ConstantExpression.UnicodeWrapper;

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (this._value == Ellipsis.Value)
      return ConstantExpression.EllipsisExpr;
    return this._value is bool ? ((bool) this._value ? ConstantExpression.TrueExpr : ConstantExpression.FalseExpr) : (this._value is ConstantExpression.UnicodeWrapper unicodeWrapper ? this.GlobalParent.Constant(unicodeWrapper.Value) : this.GlobalParent.Constant(this._value));
  }

  internal override ConstantExpression ConstantFold() => this;

  public override Type Type => this.GlobalParent.CompilationMode.GetConstantType(this.Value);

  internal override string CheckAssign()
  {
    return this._value == null ? "cannot assign to None" : base.CheckAssign();
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }

  public override string NodeName => this._value != null ? "literal" : "None";

  internal override bool CanThrow => false;

  internal override object GetConstantValue() => this.Value;

  internal override bool IsConstant => true;

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    if (this._value is bool)
      compiler.Instructions.EmitLoad((bool) this._value);
    else if (this._value is ConstantExpression.UnicodeWrapper)
      compiler.Instructions.EmitLoad(((ConstantExpression.UnicodeWrapper) this._value).Value);
    else
      compiler.Instructions.EmitLoad(this._value);
  }

  private class UnicodeWrapper
  {
    public readonly object Value;

    public UnicodeWrapper(string value) => this.Value = (object) value;
  }
}
