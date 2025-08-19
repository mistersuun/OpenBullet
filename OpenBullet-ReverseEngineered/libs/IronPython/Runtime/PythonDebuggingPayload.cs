// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonDebuggingPayload
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using Microsoft.Scripting;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class PythonDebuggingPayload
{
  public FunctionCode Code;
  private Dictionary<int, bool> _handlerLocations;
  private Dictionary<int, Dictionary<int, bool>> _loopAndFinallyLocations;

  public PythonDebuggingPayload(FunctionCode code) => this.Code = code;

  public Dictionary<int, bool> HandlerLocations
  {
    get
    {
      if (this._handlerLocations == null)
        this.GatherLocations();
      return this._handlerLocations;
    }
  }

  public Dictionary<int, Dictionary<int, bool>> LoopAndFinallyLocations
  {
    get
    {
      if (this._loopAndFinallyLocations == null)
        this.GatherLocations();
      return this._loopAndFinallyLocations;
    }
  }

  private void GatherLocations()
  {
    PythonDebuggingPayload.TracingWalker walker = new PythonDebuggingPayload.TracingWalker();
    this.Code.PythonCode.Walk((PythonWalker) walker);
    this._loopAndFinallyLocations = walker.LoopAndFinallyLocations;
    this._handlerLocations = walker.HandlerLocations;
  }

  private class TracingWalker : PythonWalker
  {
    private bool _inLoop;
    private bool _inFinally;
    private int _loopId;
    public Dictionary<int, bool> HandlerLocations = new Dictionary<int, bool>();
    public Dictionary<int, Dictionary<int, bool>> LoopAndFinallyLocations = new Dictionary<int, Dictionary<int, bool>>();
    private Dictionary<int, bool> _loopIds = new Dictionary<int, bool>();

    public override bool Walk(ForStatement node)
    {
      this.UpdateLoops((Statement) node);
      this.WalkLoopBody(node.Body, false);
      if (node.Else != null)
        node.Else.Walk((PythonWalker) this);
      return false;
    }

    private void WalkLoopBody(Statement body, bool isFinally)
    {
      bool inLoop = this._inLoop;
      bool inFinally = this._inFinally;
      int key = ++this._loopId;
      this._inFinally = false;
      this._inLoop = true;
      this._loopIds.Add(key, isFinally);
      body.Walk((PythonWalker) this);
      this._inLoop = inLoop;
      this._inFinally = inFinally;
      this.LoopOrFinallyIds.Remove(key);
    }

    public override bool Walk(WhileStatement node)
    {
      this.UpdateLoops((Statement) node);
      this.WalkLoopBody(node.Body, false);
      if (node.ElseStatement != null)
        node.ElseStatement.Walk((PythonWalker) this);
      return false;
    }

    public override bool Walk(TryStatement node)
    {
      this.UpdateLoops((Statement) node);
      node.Body.Walk((PythonWalker) this);
      if (node.Handlers != null)
      {
        foreach (TryStatementHandler handler in (IEnumerable<TryStatementHandler>) node.Handlers)
        {
          this.HandlerLocations[handler.Span.Start.Line] = false;
          handler.Body.Walk((PythonWalker) this);
        }
      }
      if (node.Finally != null)
        this.WalkLoopBody(node.Finally, true);
      return false;
    }

    public override bool Walk(AssertStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(AssignmentStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(AugmentedAssignStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(BreakStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(ClassDefinition node)
    {
      this.UpdateLoops((Statement) node);
      return false;
    }

    public override bool Walk(ContinueStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(ExecStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override void PostWalk(EmptyStatement node)
    {
      this.UpdateLoops((Statement) node);
      base.PostWalk(node);
    }

    public override bool Walk(DelStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(EmptyStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(GlobalStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(FromImportStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(ExpressionStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(FunctionDefinition node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(IfStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(ImportStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(RaiseStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    public override bool Walk(WithStatement node)
    {
      this.UpdateLoops((Statement) node);
      return base.Walk(node);
    }

    private void UpdateLoops(Statement stmt)
    {
      if (!this._inFinally && !this._inLoop)
        return;
      Dictionary<int, Dictionary<int, bool>> finallyLocations1 = this.LoopAndFinallyLocations;
      SourceSpan span = stmt.Span;
      int line1 = span.Start.Line;
      if (finallyLocations1.ContainsKey(line1))
        return;
      Dictionary<int, Dictionary<int, bool>> finallyLocations2 = this.LoopAndFinallyLocations;
      span = stmt.Span;
      int line2 = span.Start.Line;
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>((IDictionary<int, bool>) this.LoopOrFinallyIds);
      finallyLocations2.Add(line2, dictionary);
    }

    public Dictionary<int, bool> LoopOrFinallyIds
    {
      get
      {
        if (this._loopIds == null)
          this._loopIds = new Dictionary<int, bool>();
        return this._loopIds;
      }
    }
  }
}
