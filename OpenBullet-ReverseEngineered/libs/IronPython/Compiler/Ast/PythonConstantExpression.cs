// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonConstantExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class PythonConstantExpression : System.Linq.Expressions.Expression, IInstructionProvider
{
  private readonly CompilationMode _mode;
  private readonly object _value;

  public PythonConstantExpression(CompilationMode mode, object value)
  {
    this._mode = mode;
    this._value = value;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => this._mode.GetConstantType(this._value);

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._mode.GetConstant(this._value);
  }

  public object Value => this._value;

  public CompilationMode Mode => this._mode;

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Instructions.EmitLoad(this._value);
  }
}
