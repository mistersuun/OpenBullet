// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DictionaryComprehension
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler.Ast;

public sealed class DictionaryComprehension : Comprehension
{
  private readonly ComprehensionIterator[] _iterators;
  private readonly Expression _key;
  private readonly Expression _value;
  private readonly ComprehensionScope _scope;

  public DictionaryComprehension(
    Expression key,
    Expression value,
    ComprehensionIterator[] iterators)
  {
    this._key = key;
    this._value = value;
    this._iterators = iterators;
    this._scope = new ComprehensionScope((Expression) this);
  }

  public Expression Key => this._key;

  public Expression Value => this._value;

  public override IList<ComprehensionIterator> Iterators
  {
    get => (IList<ComprehensionIterator>) this._iterators;
  }

  protected override ParameterExpression MakeParameter()
  {
    return System.Linq.Expressions.Expression.Parameter(typeof (PythonDictionary), "dict_comprehension_dict");
  }

  protected override MethodInfo Factory() => AstMethods.MakeEmptyDict;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._scope.AddVariables(base.Reduce());
  }

  protected override System.Linq.Expressions.Expression Body(ParameterExpression res)
  {
    PythonAst globalParent = this.GlobalParent;
    MethodCallExpression methodCallExpression = System.Linq.Expressions.Expression.Call(AstMethods.DictAddForComprehension, (System.Linq.Expressions.Expression) res, Utils.Convert((System.Linq.Expressions.Expression) this._key, typeof (object)), Utils.Convert((System.Linq.Expressions.Expression) this._value, typeof (object)));
    SourceSpan span = this._key.Span;
    SourceLocation start = span.Start;
    span = this._value.Span;
    SourceLocation end = span.End;
    SourceSpan location = new SourceSpan(start, end);
    return globalParent.AddDebugInfo((System.Linq.Expressions.Expression) methodCallExpression, location);
  }

  public override string NodeName => "dict comprehension";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._key != null)
        this._key.Walk(walker);
      if (this._value != null)
        this._value.Walk(walker);
      if (this._iterators != null)
      {
        foreach (Node iterator in this._iterators)
          iterator.Walk(walker);
      }
    }
    walker.PostWalk(this);
  }

  internal ComprehensionScope Scope => this._scope;
}
