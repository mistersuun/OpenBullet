// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.GetParentContextFromFunctionExpression
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

internal class GetParentContextFromFunctionExpression : System.Linq.Expressions.Expression, IInstructionProvider
{
  private static System.Linq.Expressions.Expression _parentContext = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.GetParentContextFromFunction, (System.Linq.Expressions.Expression) FunctionDefinition._functionParam);

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => typeof (CodeContext);

  public override System.Linq.Expressions.Expression Reduce()
  {
    return GetParentContextFromFunctionExpression._parentContext;
  }

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile((System.Linq.Expressions.Expression) FunctionDefinition._functionParam);
    compiler.Instructions.Emit((Instruction) GetParentContextFromFunctionExpression.GetParentContextFromFunctionInstruction.Instance);
  }

  private class GetParentContextFromFunctionInstruction : Instruction
  {
    public static readonly GetParentContextFromFunctionExpression.GetParentContextFromFunctionInstruction Instance = new GetParentContextFromFunctionExpression.GetParentContextFromFunctionInstruction();

    public override int ProducedStack => 1;

    public override int ConsumedStack => 1;

    public override int Run(InterpretedFrame frame)
    {
      frame.Push((object) PythonOps.GetParentContextFromFunction((PythonFunction) frame.Pop()));
      return 1;
    }
  }
}
