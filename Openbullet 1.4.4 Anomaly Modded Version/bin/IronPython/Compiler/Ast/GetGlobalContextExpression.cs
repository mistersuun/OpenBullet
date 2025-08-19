// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.GetGlobalContextExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class GetGlobalContextExpression : System.Linq.Expressions.Expression, IInstructionProvider
{
  private readonly System.Linq.Expressions.Expression _parentContext;

  public GetGlobalContextExpression(System.Linq.Expressions.Expression parentContext)
  {
    this._parentContext = parentContext;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => typeof (CodeContext);

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.GetGlobalContext, this._parentContext);
  }

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this._parentContext);
    compiler.Instructions.Emit((Instruction) GetGlobalContextExpression.GetGlobalContextInstruction.Instance);
  }

  private class GetGlobalContextInstruction : Instruction
  {
    public static readonly GetGlobalContextExpression.GetGlobalContextInstruction Instance = new GetGlobalContextExpression.GetGlobalContextInstruction();

    public override int ConsumedStack => 1;

    public override int ProducedStack => 1;

    public override int Run(InterpretedFrame frame)
    {
      frame.Push((object) PythonOps.GetGlobalContext((CodeContext) frame.Pop()));
      return 1;
    }
  }
}
