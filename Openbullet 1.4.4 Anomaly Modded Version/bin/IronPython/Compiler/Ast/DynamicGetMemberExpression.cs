// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DynamicGetMemberExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using Microsoft.Scripting.Interpreter;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class DynamicGetMemberExpression : System.Linq.Expressions.Expression, IInstructionProvider
{
  private readonly PythonGetMemberBinder _binder;
  private readonly CompilationMode _mode;
  private readonly System.Linq.Expressions.Expression _target;
  private readonly System.Linq.Expressions.Expression _codeContext;

  public DynamicGetMemberExpression(
    PythonGetMemberBinder binder,
    CompilationMode mode,
    System.Linq.Expressions.Expression target,
    System.Linq.Expressions.Expression codeContext)
  {
    this._binder = binder;
    this._mode = mode;
    this._target = target;
    this._codeContext = codeContext;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => typeof (object);

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._mode.ReduceDynamic((DynamicMetaObjectBinder) this._binder, typeof (object), this._target, this._codeContext);
  }

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this._target);
    compiler.Compile(this._codeContext);
    compiler.Instructions.Emit((Instruction) new DynamicGetMemberExpression.GetMemberInstruction(this._binder));
  }

  private class GetMemberInstruction : Instruction
  {
    private CallSite<Func<CallSite, object, CodeContext, object>> _site;
    private readonly PythonGetMemberBinder _binder;

    public GetMemberInstruction(PythonGetMemberBinder binder) => this._binder = binder;

    public override int ConsumedStack => 2;

    public override int ProducedStack => 1;

    public override int Run(InterpretedFrame frame)
    {
      if (this._site == null)
        this._site = CallSite<Func<CallSite, object, CodeContext, object>>.Create((CallSiteBinder) this._binder);
      CodeContext codeContext = (CodeContext) frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, frame.Pop(), codeContext));
      return 1;
    }
  }
}
