// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.CallExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class CallExpression : Expression, IInstructionProvider
{
  private readonly Expression _target;
  private readonly Arg[] _args;
  private static MethodCallExpression _GetUnicode = System.Linq.Expressions.Expression.Call(AstMethods.GetUnicodeFunction);

  public CallExpression(Expression target, Arg[] args)
  {
    this._target = target;
    this._args = args;
  }

  public Expression Target => this._target;

  public IList<Arg> Args => (IList<Arg>) this._args;

  public bool NeedsLocalsDictionary()
  {
    if (!(this._target is NameExpression target))
      return false;
    if (this._args.Length == 0)
      return target.Name == "locals" || target.Name == "vars" || target.Name == "dir";
    if (this._args.Length == 1 && (target.Name == "dir" || target.Name == "vars"))
    {
      if (this._args[0].Name == "*" || this._args[0].Name == "**")
        return true;
    }
    else if (this._args.Length == 2 && (target.Name == "dir" || target.Name == "vars"))
    {
      if (this._args[0].Name == "*" && this._args[1].Name == "**")
        return true;
    }
    else if (target.Name == "eval" || target.Name == "execfile")
      return true;
    return false;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.UnicodeCall() ?? this.NormalCall((System.Linq.Expressions.Expression) this._target);
  }

  private System.Linq.Expressions.Expression NormalCall(System.Linq.Expressions.Expression target)
  {
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[this._args.Length + 2];
    Argument[] objArray = new Argument[this._args.Length];
    expressionArray[0] = this.Parent.LocalContext;
    expressionArray[1] = target;
    for (int index = 0; index < this._args.Length; ++index)
    {
      objArray[index] = this._args[index].GetArgumentInfo();
      expressionArray[index + 2] = (System.Linq.Expressions.Expression) this._args[index].Expression;
    }
    return this.Parent.Invoke(new CallSignature(objArray), expressionArray);
  }

  private System.Linq.Expressions.Expression UnicodeCall()
  {
    if (!(this._target is NameExpression) || !(((NameExpression) this._target).Name == "unicode"))
      return (System.Linq.Expressions.Expression) null;
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object));
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression, (System.Linq.Expressions.Expression) this._target), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.IsUnicode, (System.Linq.Expressions.Expression) parameterExpression), this.NormalCall((System.Linq.Expressions.Expression) CallExpression._GetUnicode), this.NormalCall((System.Linq.Expressions.Expression) parameterExpression)));
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    if (this._target is NameExpression && ((NameExpression) this._target).Name == "unicode")
    {
      compiler.Compile(this.Reduce());
    }
    else
    {
      for (int index = 0; index < this._args.Length; ++index)
      {
        if (!this._args[index].GetArgumentInfo().IsSimple)
        {
          compiler.Compile(this.Reduce());
          return;
        }
      }
      switch (this._args.Length)
      {
        case 0:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke0Instruction(this.Parent.PyContext));
          break;
        case 1:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke1Instruction(this.Parent.PyContext));
          break;
        case 2:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[1].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke2Instruction(this.Parent.PyContext));
          break;
        case 3:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[1].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[2].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke3Instruction(this.Parent.PyContext));
          break;
        case 4:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[1].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[2].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[3].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke4Instruction(this.Parent.PyContext));
          break;
        case 5:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[1].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[2].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[3].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[4].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke5Instruction(this.Parent.PyContext));
          break;
        case 6:
          compiler.Compile(this.Parent.LocalContext);
          compiler.Compile((System.Linq.Expressions.Expression) this._target);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[0].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[1].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[2].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[3].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[4].Expression);
          compiler.Compile((System.Linq.Expressions.Expression) this._args[5].Expression);
          compiler.Instructions.Emit((Instruction) new CallExpression.Invoke6Instruction(this.Parent.PyContext));
          break;
        default:
          compiler.Compile(this.Reduce());
          break;
      }
    }
  }

  public override string NodeName => "function call";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._target != null)
        this._target.Walk(walker);
      if (this._args != null)
      {
        foreach (Node node in this._args)
          node.Walk(walker);
      }
    }
    walker.PostWalk(this);
  }

  private abstract class InvokeInstruction : Instruction
  {
    public override int ProducedStack => 1;

    public override string InstructionName => "Python Invoke" + (object) (this.ConsumedStack - 1);
  }

  private class Invoke0Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object>> _site;

    public Invoke0Instruction(PythonContext context) => this._site = context.CallSite0;

    public override int ConsumedStack => 2;

    public override int Run(InterpretedFrame frame)
    {
      object obj = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj));
      return 1;
    }
  }

  private class Invoke1Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object>> _site;

    public Invoke1Instruction(PythonContext context) => this._site = context.CallSite1;

    public override int ConsumedStack => 3;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj2, obj1));
      return 1;
    }
  }

  private class Invoke2Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object, object>> _site;

    public Invoke2Instruction(PythonContext context) => this._site = context.CallSite2;

    public override int ConsumedStack => 4;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      object obj3 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj3, obj2, obj1));
      return 1;
    }
  }

  private class Invoke3Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object, object, object>> _site;

    public Invoke3Instruction(PythonContext context) => this._site = context.CallSite3;

    public override int ConsumedStack => 5;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      object obj3 = frame.Pop();
      object obj4 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj4, obj3, obj2, obj1));
      return 1;
    }
  }

  private class Invoke4Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>> _site;

    public Invoke4Instruction(PythonContext context) => this._site = context.CallSite4;

    public override int ConsumedStack => 6;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      object obj3 = frame.Pop();
      object obj4 = frame.Pop();
      object obj5 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj5, obj4, obj3, obj2, obj1));
      return 1;
    }
  }

  private class Invoke5Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>> _site;

    public Invoke5Instruction(PythonContext context) => this._site = context.CallSite5;

    public override int ConsumedStack => 7;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      object obj3 = frame.Pop();
      object obj4 = frame.Pop();
      object obj5 = frame.Pop();
      object obj6 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj6, obj5, obj4, obj3, obj2, obj1));
      return 1;
    }
  }

  private class Invoke6Instruction : CallExpression.InvokeInstruction
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>> _site;

    public Invoke6Instruction(PythonContext context) => this._site = context.CallSite6;

    public override int ConsumedStack => 8;

    public override int Run(InterpretedFrame frame)
    {
      object obj1 = frame.Pop();
      object obj2 = frame.Pop();
      object obj3 = frame.Pop();
      object obj4 = frame.Pop();
      object obj5 = frame.Pop();
      object obj6 = frame.Pop();
      object obj7 = frame.Pop();
      frame.Push(this._site.Target((CallSite) this._site, (CodeContext) frame.Pop(), obj7, obj6, obj5, obj4, obj3, obj2, obj1));
      return 1;
    }
  }
}
