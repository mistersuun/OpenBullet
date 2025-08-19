// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonDynamicExpressionN
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class PythonDynamicExpressionN : LightTypedDynamicExpressionN
{
  private readonly CompilationMode _mode;

  public PythonDynamicExpressionN(
    CallSiteBinder binder,
    CompilationMode mode,
    IList<Expression> args)
    : base(binder, typeof (object), args)
  {
    this._mode = mode;
  }

  protected override Expression Rewrite(CallSiteBinder binder, IList<Expression> args)
  {
    return (Expression) new PythonDynamicExpressionN(binder, this._mode, args);
  }

  public override Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this.Binder, this.Type, ArrayUtils.ToArray<Expression>((ICollection<Expression>) this.Arguments));
  }

  public override void AddInstructions(LightCompiler compiler)
  {
    if (this.ArgumentCount > 15)
      compiler.Compile(this.Reduce());
    else if (this.GetArgument(0).Type == typeof (CodeContext))
    {
      for (int index = 0; index < this.ArgumentCount; ++index)
        compiler.Compile(this.GetArgument(index));
      switch (this.ArgumentCount)
      {
        case 1:
          compiler.Instructions.EmitDynamic<CodeContext, object>(this.Binder);
          break;
        case 2:
          compiler.Instructions.EmitDynamic<CodeContext, object, object>(this.Binder);
          break;
        case 3:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object>(this.Binder);
          break;
        case 4:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object>(this.Binder);
          break;
        case 5:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object>(this.Binder);
          break;
        case 6:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object>(this.Binder);
          break;
        case 7:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 8:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 9:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 10:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 11:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 12:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 13:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 14:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
        case 15:
          compiler.Instructions.EmitDynamic<CodeContext, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(this.Binder);
          break;
      }
    }
    else
      base.AddInstructions(compiler);
  }
}
