// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ComprehensionScope
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class ComprehensionScope : ScopeStatement
{
  private readonly Expression _comprehension;
  private static readonly ParameterExpression _compContext = System.Linq.Expressions.Expression.Parameter(typeof (CodeContext), "$compContext");

  public ComprehensionScope(Expression comprehension) => this._comprehension = comprehension;

  internal override bool ExposesLocalVariable(PythonVariable variable)
  {
    if (this.NeedsLocalsDictionary)
      return true;
    return variable.Scope != this && this._comprehension.Parent.ExposesLocalVariable(variable);
  }

  internal override System.Linq.Expressions.Expression GetParentClosureTuple()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) null, typeof (PythonOps).GetMethod("GetClosureTupleFromContext"), this._comprehension.Parent.LocalContext);
  }

  internal override PythonVariable BindReference(PythonNameBinder binder, PythonReference reference)
  {
    PythonVariable variable;
    if (!this.TryGetVariable(reference.Name, out variable))
      return this._comprehension.Parent.BindReference(binder, reference);
    if (variable.Kind == VariableKind.Global)
      this.AddReferencedGlobal(reference.Name);
    return variable;
  }

  internal override System.Linq.Expressions.Expression GetVariableExpression(PythonVariable variable)
  {
    if (variable.IsGlobal)
      return this.GlobalParent.ModuleVariables[variable];
    System.Linq.Expressions.Expression expression;
    return this._variableMapping.TryGetValue(variable, out expression) ? expression : this._comprehension.Parent.GetVariableExpression(variable);
  }

  internal override LightLambdaExpression GetLambda() => throw new NotImplementedException();

  public override void Walk(PythonWalker walker) => this._comprehension.Walk(walker);

  internal override System.Linq.Expressions.Expression LocalContext
  {
    get
    {
      return this.NeedsLocalContext ? (System.Linq.Expressions.Expression) ComprehensionScope._compContext : this._comprehension.Parent.LocalContext;
    }
  }

  internal System.Linq.Expressions.Expression AddVariables(System.Linq.Expressions.Expression expression)
  {
    ReadOnlyCollectionBuilder<ParameterExpression> collectionBuilder = new ReadOnlyCollectionBuilder<ParameterExpression>();
    ParameterExpression parameterExpression = (ParameterExpression) null;
    if (this.NeedsLocalContext)
    {
      parameterExpression = ComprehensionScope._compContext;
      collectionBuilder.Add(ComprehensionScope._compContext);
    }
    List<System.Linq.Expressions.Expression> init = new List<System.Linq.Expressions.Expression>();
    this.CreateVariables(collectionBuilder, init);
    if (parameterExpression != null)
    {
      MethodCallExpression localContext = this.CreateLocalContext(this._comprehension.Parent.LocalContext);
      init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) ComprehensionScope._compContext, (System.Linq.Expressions.Expression) localContext));
    }
    init.Add(expression);
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) collectionBuilder, (IEnumerable<System.Linq.Expressions.Expression>) init);
  }
}
