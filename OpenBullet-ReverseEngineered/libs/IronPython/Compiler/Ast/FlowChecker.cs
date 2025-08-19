// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.FlowChecker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class FlowChecker : PythonWalker
{
  private BitArray _bits;
  private Stack<BitArray> _loops;
  private Dictionary<string, PythonVariable> _variables;
  private readonly ScopeStatement _scope;
  private readonly FlowDefiner _fdef;
  private readonly FlowDeleter _fdel;

  private FlowChecker(ScopeStatement scope)
  {
    this._variables = scope.Variables;
    this._bits = new BitArray(this._variables.Count * 2);
    int num = 0;
    foreach (KeyValuePair<string, PythonVariable> variable in this._variables)
      variable.Value.Index = num++;
    this._scope = scope;
    this._fdef = new FlowDefiner(this);
    this._fdel = new FlowDeleter(this);
  }

  [Conditional("DEBUG")]
  public void Dump(BitArray bits)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("FlowChecker ({0})", this._scope is FunctionDefinition ? (object) this._scope.Name : (this._scope is ClassDefinition ? (object) this._scope.Name : (object) ""));
    stringBuilder.Append('{');
    bool flag = false;
    foreach (KeyValuePair<string, PythonVariable> variable in this._variables)
    {
      if (flag)
        stringBuilder.Append(", ");
      else
        flag = true;
      int index = 2 * variable.Value.Index;
      stringBuilder.AppendFormat("{0}:{1}{2}", (object) variable.Key, bits.Get(index) ? (object) "*" : (object) "-", bits.Get(index + 1) ? (object) "-" : (object) "*");
      if (variable.Value.ReadBeforeInitialized)
        stringBuilder.Append("#");
    }
    stringBuilder.Append('}');
  }

  private void SetAssigned(PythonVariable variable, bool value)
  {
    this._bits.Set(variable.Index * 2, value);
  }

  private void SetInitialized(PythonVariable variable, bool value)
  {
    this._bits.Set(variable.Index * 2 + 1, value);
  }

  private bool IsAssigned(PythonVariable variable) => this._bits.Get(variable.Index * 2);

  private bool IsInitialized(PythonVariable variable) => this._bits.Get(variable.Index * 2 + 1);

  public static void Check(ScopeStatement scope)
  {
    if (scope.Variables == null)
      return;
    FlowChecker walker = new FlowChecker(scope);
    scope.Walk((PythonWalker) walker);
  }

  public void Define(string name)
  {
    PythonVariable variable;
    if (!this._variables.TryGetValue(name, out variable))
      return;
    this.SetAssigned(variable, true);
    this.SetInitialized(variable, true);
  }

  public void Delete(string name)
  {
    PythonVariable variable;
    if (!this._variables.TryGetValue(name, out variable))
      return;
    this.SetAssigned(variable, false);
    this.SetInitialized(variable, true);
  }

  private void PushLoop(BitArray ba)
  {
    if (this._loops == null)
      this._loops = new Stack<BitArray>();
    this._loops.Push(ba);
  }

  private BitArray PeekLoop()
  {
    return this._loops == null || this._loops.Count <= 0 ? (BitArray) null : this._loops.Peek();
  }

  private void PopLoop()
  {
    if (this._loops == null)
      return;
    this._loops.Pop();
  }

  public override bool Walk(LambdaExpression node) => false;

  public override bool Walk(ListComprehension node)
  {
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits);
    foreach (Node iterator in (IEnumerable<ComprehensionIterator>) node.Iterators)
      iterator.Walk((PythonWalker) this);
    node.Item.Walk((PythonWalker) this);
    this._bits = bits;
    return false;
  }

  public override bool Walk(SetComprehension node)
  {
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits);
    foreach (Node iterator in (IEnumerable<ComprehensionIterator>) node.Iterators)
      iterator.Walk((PythonWalker) this);
    node.Item.Walk((PythonWalker) this);
    this._bits = bits;
    return false;
  }

  public override bool Walk(DictionaryComprehension node)
  {
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits);
    foreach (Node iterator in (IEnumerable<ComprehensionIterator>) node.Iterators)
      iterator.Walk((PythonWalker) this);
    node.Key.Walk((PythonWalker) this);
    node.Value.Walk((PythonWalker) this);
    this._bits = bits;
    return false;
  }

  public override bool Walk(NameExpression node)
  {
    PythonVariable variable;
    if (this._variables.TryGetValue(node.Name, out variable))
    {
      node.Assigned = this.IsAssigned(variable);
      if (!this.IsInitialized(variable))
        variable.ReadBeforeInitialized = true;
    }
    return true;
  }

  public override void PostWalk(NameExpression node)
  {
  }

  public override bool Walk(AssignmentStatement node)
  {
    node.Right.Walk((PythonWalker) this);
    foreach (Node node1 in (IEnumerable<Expression>) node.Left)
      node1.Walk((PythonWalker) this._fdef);
    return false;
  }

  public override void PostWalk(AssignmentStatement node)
  {
  }

  public override bool Walk(AugmentedAssignStatement node) => true;

  public override void PostWalk(AugmentedAssignStatement node)
  {
    node.Left.Walk((PythonWalker) this._fdef);
  }

  public override bool Walk(BreakStatement node)
  {
    this.PeekLoop()?.And(this._bits);
    return true;
  }

  public override bool Walk(ClassDefinition node)
  {
    if (this._scope == node)
      return true;
    this.Define(node.Name);
    foreach (Node node1 in (IEnumerable<Expression>) node.Bases)
      node1.Walk((PythonWalker) this);
    return false;
  }

  public override bool Walk(ContinueStatement node) => true;

  public override void PostWalk(DelStatement node)
  {
    foreach (Node expression in (IEnumerable<Expression>) node.Expressions)
      expression.Walk((PythonWalker) this._fdel);
  }

  public override bool Walk(ForStatement node)
  {
    node.List.Walk((PythonWalker) this);
    BitArray bitArray = new BitArray(this._bits);
    BitArray ba = new BitArray(this._bits.Length, true);
    this.PushLoop(ba);
    node.Left.Walk((PythonWalker) this._fdef);
    node.Body.Walk((PythonWalker) this);
    this.PopLoop();
    this._bits.And(ba);
    if (node.Else != null)
    {
      BitArray bits = this._bits;
      this._bits = bitArray;
      node.Else.Walk((PythonWalker) this);
      this._bits = bits;
    }
    this._bits.And(bitArray);
    return false;
  }

  public override bool Walk(FromImportStatement node)
  {
    if (node.Names != FromImportStatement.Star)
    {
      for (int index = 0; index < node.Names.Count; ++index)
        this.Define(node.AsNames[index] != null ? node.AsNames[index] : node.Names[index]);
    }
    return true;
  }

  public override bool Walk(FunctionDefinition node)
  {
    if (node == this._scope)
    {
      foreach (Node parameter in (IEnumerable<Parameter>) node.Parameters)
        parameter.Walk((PythonWalker) this._fdef);
      return true;
    }
    this.Define(node.Name);
    foreach (Parameter parameter in (IEnumerable<Parameter>) node.Parameters)
    {
      if (parameter.DefaultValue != null)
        parameter.DefaultValue.Walk((PythonWalker) this);
    }
    return false;
  }

  public override bool Walk(IfStatement node)
  {
    BitArray bitArray = new BitArray(this._bits.Length, true);
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits.Length);
    foreach (IfStatementTest test in (IEnumerable<IfStatementTest>) node.Tests)
    {
      this._bits.SetAll(false);
      this._bits.Or(bits);
      test.Test.Walk((PythonWalker) this);
      test.Body.Walk((PythonWalker) this);
      bitArray.And(this._bits);
    }
    this._bits.SetAll(false);
    this._bits.Or(bits);
    if (node.ElseStatement != null)
      node.ElseStatement.Walk((PythonWalker) this);
    bitArray.And(this._bits);
    this._bits = bits;
    this._bits.SetAll(false);
    this._bits.Or(bitArray);
    return false;
  }

  public override bool Walk(ImportStatement node)
  {
    for (int index = 0; index < node.Names.Count; ++index)
      this.Define(node.AsNames[index] != null ? node.AsNames[index] : node.Names[index].Names[0]);
    return true;
  }

  public override void PostWalk(ReturnStatement node)
  {
  }

  public override bool Walk(WithStatement node)
  {
    node.ContextManager.Walk((PythonWalker) this);
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits);
    if (node.Variable != null)
      node.Variable.Walk((PythonWalker) this._fdef);
    node.Body.Walk((PythonWalker) this);
    this._bits = bits;
    return false;
  }

  public override bool Walk(TryStatement node)
  {
    BitArray bits = this._bits;
    this._bits = new BitArray(this._bits);
    node.Body.Walk((PythonWalker) this);
    if (node.Else != null)
      node.Else.Walk((PythonWalker) this);
    if (node.Handlers != null)
    {
      foreach (TryStatementHandler handler in (IEnumerable<TryStatementHandler>) node.Handlers)
      {
        this._bits.SetAll(false);
        this._bits.Or(bits);
        if (handler.Test != null)
          handler.Test.Walk((PythonWalker) this);
        if (handler.Target != null)
          handler.Target.Walk((PythonWalker) this._fdef);
        handler.Body.Walk((PythonWalker) this);
      }
    }
    this._bits = bits;
    if (node.Finally != null)
      node.Finally.Walk((PythonWalker) this);
    return false;
  }

  public override bool Walk(WhileStatement node)
  {
    node.Test.Walk((PythonWalker) this);
    BitArray bitArray = node.ElseStatement != null ? new BitArray(this._bits) : (BitArray) null;
    BitArray ba = new BitArray(this._bits.Length, true);
    this.PushLoop(ba);
    node.Body.Walk((PythonWalker) this);
    this.PopLoop();
    this._bits.And(ba);
    if (node.ElseStatement != null)
    {
      BitArray bits = this._bits;
      this._bits = bitArray;
      node.ElseStatement.Walk((PythonWalker) this);
      this._bits = bits;
      this._bits.And(bitArray);
    }
    return false;
  }
}
