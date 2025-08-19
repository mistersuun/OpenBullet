// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonDynamicExpression2`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Interpreter;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class PythonDynamicExpression2<T> : PythonDynamicExpression2
{
  private readonly CompilationMode _mode;

  public PythonDynamicExpression2(
    CallSiteBinder binder,
    CompilationMode mode,
    Expression arg0,
    Expression arg1)
    : base(binder, mode, arg0, arg1)
  {
    this._mode = mode;
  }

  public override Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this.Binder, this.Type, this.Argument0, this.Argument1);
  }

  protected override Expression Rewrite(CallSiteBinder binder, Expression arg0, Expression arg1)
  {
    return (Expression) new PythonDynamicExpression2<T>(binder, this._mode, arg0, arg1);
  }

  public override Type Type => typeof (T);

  public override void AddInstructions(LightCompiler compiler)
  {
    if (this.Argument0.Type == typeof (CodeContext))
    {
      compiler.Compile(this.Argument0);
      compiler.Compile(this.Argument1);
      compiler.Instructions.EmitDynamic<CodeContext, object, T>(this.Binder);
    }
    else if (this.Argument1.Type == typeof (CodeContext))
    {
      compiler.Compile(this.Argument0);
      compiler.Compile(this.Argument1);
      compiler.Instructions.EmitDynamic<object, CodeContext, T>(this.Binder);
    }
    else
    {
      compiler.Compile(this.Argument0);
      compiler.Compile(this.Argument1);
      compiler.Instructions.EmitDynamic<object, object, T>(this.Binder);
    }
  }
}
