// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DictionaryExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class DictionaryExpression : Expression, IInstructionProvider
{
  private readonly SliceExpression[] _items;
  private static System.Linq.Expressions.Expression EmptyDictExpression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeEmptyDict);

  public DictionaryExpression(params SliceExpression[] items) => this._items = items;

  public IList<SliceExpression> Items => (IList<SliceExpression>) this._items;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._items.Length != 0 ? this.ReduceConstant() ?? this.ReduceDictionaryWithItems() : DictionaryExpression.EmptyDictExpression;
  }

  private System.Linq.Expressions.Expression ReduceDictionaryWithItems()
  {
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[this._items.Length * 2];
    Type type1 = (Type) null;
    bool flag = false;
    for (int index = 0; index < this._items.Length; ++index)
    {
      SliceExpression sliceExpression = this._items[index];
      expressionArray[index * 2] = Node.TransformOrConstantNull(sliceExpression.SliceStop, typeof (object));
      System.Linq.Expressions.Expression expression = expressionArray[index * 2 + 1] = Node.TransformOrConstantNull(sliceExpression.SliceStart, typeof (object));
      Type type2 = expression.NodeType != ExpressionType.Convert ? expression.Type : ((System.Linq.Expressions.UnaryExpression) expression).Operand.Type;
      if (type1 == (Type) null)
        type1 = type2;
      else if (type2 == typeof (object))
        flag = true;
      else if (type2 != type1)
        flag = true;
    }
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(flag ? AstMethods.MakeDictFromItems : AstMethods.MakeHomogeneousDictFromItems, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), expressionArray));
  }

  private System.Linq.Expressions.Expression ReduceConstant()
  {
    for (int index = 0; index < this._items.Length; ++index)
    {
      SliceExpression sliceExpression = this._items[index];
      if (!sliceExpression.SliceStop.IsConstant || !sliceExpression.SliceStart.IsConstant)
        return (System.Linq.Expressions.Expression) null;
    }
    CommonDictionaryStorage storage = new CommonDictionaryStorage();
    for (int index = 0; index < this._items.Length; ++index)
    {
      SliceExpression sliceExpression = this._items[index];
      storage.AddNoLock(sliceExpression.SliceStart.GetConstantValue(), sliceExpression.SliceStop.GetConstantValue());
    }
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeConstantDict, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) new ConstantDictionaryStorage(storage), typeof (object)));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._items != null)
    {
      foreach (Node node in this._items)
        node.Walk(walker);
    }
    walker.PostWalk(this);
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    if (this._items.Length == 0)
      compiler.Instructions.Emit((Instruction) DictionaryExpression.EmptyDictInstruction.Instance);
    else
      compiler.Compile(this.Reduce());
  }

  private class EmptyDictInstruction : Instruction
  {
    public static DictionaryExpression.EmptyDictInstruction Instance = new DictionaryExpression.EmptyDictInstruction();

    public override int Run(InterpretedFrame frame)
    {
      frame.Push((object) PythonOps.MakeEmptyDict());
      return 1;
    }

    public override int ProducedStack => 1;
  }
}
