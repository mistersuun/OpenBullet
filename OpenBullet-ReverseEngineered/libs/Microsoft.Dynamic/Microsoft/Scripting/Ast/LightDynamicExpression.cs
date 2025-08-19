// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightDynamicExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public abstract class LightDynamicExpression : Expression, IInstructionProvider
{
  private readonly CallSiteBinder _binder;

  protected LightDynamicExpression(CallSiteBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    this._binder = binder;
  }

  public override bool CanReduce => true;

  public CallSiteBinder Binder => this._binder;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => typeof (object);

  public virtual void AddInstructions(LightCompiler compiler)
  {
    Instruction instruction = DynamicInstructionN.CreateUntypedInstruction(this._binder, this.ArgumentCount);
    if (instruction == null)
    {
      if (!(this._binder is ILightCallSiteBinder binder) || !binder.AcceptsArgumentArray)
      {
        compiler.Compile(this.Reduce());
        return;
      }
      instruction = (Instruction) new DynamicSplatInstruction(this.ArgumentCount, CallSite<Func<CallSite, ArgumentArray, object>>.Create(this._binder));
    }
    for (int index = 0; index < this.ArgumentCount; ++index)
      compiler.Compile(this.GetArgument(index));
    compiler.Instructions.Emit(instruction);
  }

  public abstract override Expression Reduce();

  protected abstract int ArgumentCount { get; }

  protected abstract Expression GetArgument(int index);

  protected CallSiteBinder GetLightBinder()
  {
    return this._binder is ILightExceptionBinder binder ? binder.GetLightExceptionBinder() : this._binder;
  }
}
