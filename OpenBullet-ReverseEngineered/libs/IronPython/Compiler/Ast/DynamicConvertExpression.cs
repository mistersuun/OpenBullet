// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DynamicConvertExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using Microsoft.Scripting.Interpreter;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class DynamicConvertExpression : System.Linq.Expressions.Expression, IInstructionProvider
{
  private readonly PythonConversionBinder _binder;
  private readonly CompilationMode _mode;
  private readonly System.Linq.Expressions.Expression _target;

  public DynamicConvertExpression(
    PythonConversionBinder binder,
    CompilationMode mode,
    System.Linq.Expressions.Expression target)
  {
    this._binder = binder;
    this._mode = mode;
    this._target = target;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => this._binder.Type;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this._binder, this._binder.Type, this._target);
  }

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this._target);
    if (Type.GetTypeCode(this._binder.Type) == TypeCode.Boolean)
      compiler.Instructions.Emit((Instruction) DynamicConvertExpression.BooleanConversionInstruction.Instance);
    else
      compiler.Instructions.Emit((Instruction) new DynamicConvertExpression.TypedConversionInstruction(this._binder.Type));
  }

  private abstract class ConversionInstruction : Instruction
  {
    public override int ConsumedStack => 1;

    public override int ProducedStack => 1;
  }

  private class BooleanConversionInstruction : DynamicConvertExpression.ConversionInstruction
  {
    public static DynamicConvertExpression.BooleanConversionInstruction Instance = new DynamicConvertExpression.BooleanConversionInstruction();

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(Converter.ConvertToBoolean(frame.Pop()));
      return 1;
    }
  }

  private class TypedConversionInstruction : DynamicConvertExpression.ConversionInstruction
  {
    private readonly Type _type;

    public TypedConversionInstruction(Type type) => this._type = type;

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(Converter.Convert(frame.Pop(), this._type));
      return 1;
    }
  }
}
