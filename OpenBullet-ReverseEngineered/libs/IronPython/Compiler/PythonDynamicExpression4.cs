// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonDynamicExpression4
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class PythonDynamicExpression4 : LightDynamicExpression4
{
  private readonly CompilationMode _mode;

  public PythonDynamicExpression4(
    CallSiteBinder binder,
    CompilationMode mode,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
    : base(binder, arg0, arg1, arg2, arg3)
  {
    this._mode = mode;
  }

  public override Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this.Binder, this.Type, this.Argument0, this.Argument1, this.Argument2, this.Argument3);
  }

  protected override Expression Rewrite(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
  {
    return (Expression) new PythonDynamicExpression4(binder, this._mode, arg0, arg1, arg2, arg3);
  }

  public override void AddInstructions(LightCompiler compiler)
  {
    if (this.Argument0.Type == typeof (CodeContext))
    {
      compiler.Compile(this.Argument0);
      compiler.Compile(this.Argument1);
      compiler.Compile(this.Argument2);
      compiler.Compile(this.Argument3);
      compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object>(this.Binder);
    }
    else
      base.AddInstructions(compiler);
  }
}
