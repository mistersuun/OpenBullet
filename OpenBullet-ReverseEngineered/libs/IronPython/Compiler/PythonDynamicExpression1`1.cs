// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonDynamicExpression1`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class PythonDynamicExpression1<T> : LightDynamicExpression1
{
  private readonly CompilationMode _mode;

  public PythonDynamicExpression1(CallSiteBinder binder, CompilationMode mode, Expression arg0)
    : base(binder, arg0)
  {
    this._mode = mode;
  }

  protected override Expression Rewrite(CallSiteBinder binder, Expression arg0)
  {
    return (Expression) new PythonDynamicExpression1<T>(binder, this._mode, arg0);
  }

  public override Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this.Binder, this.Type, this.Argument0);
  }

  public override Type Type => typeof (T);

  public override void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this.Argument0);
    compiler.Instructions.EmitDynamic<object, T>(this.Binder);
  }
}
