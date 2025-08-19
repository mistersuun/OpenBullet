// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SliceExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class SliceExpression : Expression
{
  private readonly Expression _sliceStart;
  private readonly Expression _sliceStop;
  private readonly Expression _sliceStep;
  private readonly bool _stepProvided;

  public SliceExpression(Expression start, Expression stop, Expression step, bool stepProvided)
  {
    this._sliceStart = start;
    this._sliceStop = stop;
    this._sliceStep = step;
    this._stepProvided = stepProvided;
  }

  public Expression SliceStart => this._sliceStart;

  public Expression SliceStop => this._sliceStop;

  public Expression SliceStep => this._sliceStep;

  public bool StepProvided => this._stepProvided;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeSlice, Node.TransformOrConstantNull(this._sliceStart, typeof (object)), Node.TransformOrConstantNull(this._sliceStop, typeof (object)), Node.TransformOrConstantNull(this._sliceStep, typeof (object)));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._sliceStart != null)
        this._sliceStart.Walk(walker);
      if (this._sliceStop != null)
        this._sliceStop.Walk(walker);
      if (this._sliceStep != null)
        this._sliceStep.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
