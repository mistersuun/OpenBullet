// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.Node
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public abstract class Node : System.Linq.Expressions.Expression
{
  private ScopeStatement _parent;
  private IndexSpan _span;
  internal static readonly BlockExpression EmptyBlock = System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Empty());
  internal static readonly System.Linq.Expressions.Expression[] EmptyExpression = new System.Linq.Expressions.Expression[0];
  internal static ParameterExpression FunctionStackVariable = System.Linq.Expressions.Expression.Variable(typeof (List<FunctionStack>), "$funcStack");
  internal static readonly LabelTarget GeneratorLabel = System.Linq.Expressions.Expression.Label(typeof (object), "$generatorLabel");
  private static ParameterExpression _lineNumberUpdated = System.Linq.Expressions.Expression.Variable(typeof (bool), "$lineUpdated");
  private static readonly ParameterExpression _lineNoVar = System.Linq.Expressions.Expression.Parameter(typeof (int), "$lineNo");

  public ScopeStatement Parent
  {
    get => this._parent;
    set => this._parent = value;
  }

  public void SetLoc(PythonAst globalParent, int start, int end)
  {
    this._span = new IndexSpan(start, end > start ? end - start : start);
    this._parent = (ScopeStatement) globalParent;
  }

  public void SetLoc(PythonAst globalParent, IndexSpan span)
  {
    this._span = span;
    this._parent = (ScopeStatement) globalParent;
  }

  public IndexSpan IndexSpan
  {
    get => this._span;
    set => this._span = value;
  }

  public SourceLocation Start => this.GlobalParent.IndexToLocation(this.StartIndex);

  public SourceLocation End => this.GlobalParent.IndexToLocation(this.EndIndex);

  public int EndIndex
  {
    get => this._span.End;
    set => this._span = new IndexSpan(this._span.Start, value - this._span.Start);
  }

  public int StartIndex
  {
    get => this._span.Start;
    set => this._span = new IndexSpan(value, 0);
  }

  internal SourceLocation IndexToLocation(int index)
  {
    if (index == -1)
      return SourceLocation.Invalid;
    int[] lineLocations = this.GlobalParent._lineLocations;
    int index1 = Array.BinarySearch<int>(lineLocations, index);
    if (index1 < 0)
    {
      if (index1 == -1)
        return new SourceLocation(index, 1, index + 1);
      index1 = ~index1 - 1;
    }
    return new SourceLocation(index, index1 + 2, index - lineLocations[index1] + 1);
  }

  public SourceSpan Span => new SourceSpan(this.Start, this.End);

  public abstract void Walk(PythonWalker walker);

  public virtual string NodeName => this.GetType().Name;

  internal virtual bool CanThrow => true;

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override string ToString() => this.GetType().Name;

  internal PythonAst GlobalParent
  {
    get
    {
      Node globalParent = this;
      while (!(globalParent is PythonAst))
        globalParent = (Node) globalParent.Parent;
      return (PythonAst) globalParent;
    }
  }

  internal bool EmitDebugSymbols => this.GlobalParent.EmitDebugSymbols;

  internal bool StripDocStrings => this.GlobalParent.PyContext.PythonOptions.StripDocStrings;

  internal bool Optimize => this.GlobalParent.PyContext.PythonOptions.Optimize;

  internal virtual string GetDocumentation(Statement stmt)
  {
    return this.StripDocStrings ? (string) null : stmt.Documentation;
  }

  internal static System.Linq.Expressions.Expression[] ToObjectArray(IList<Expression> expressions)
  {
    System.Linq.Expressions.Expression[] objectArray = new System.Linq.Expressions.Expression[expressions.Count];
    for (int index = 0; index < expressions.Count; ++index)
      objectArray[index] = Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) expressions[index], typeof (object));
    return objectArray;
  }

  internal static System.Linq.Expressions.Expression TransformOrConstantNull(
    Expression expression,
    Type type)
  {
    return expression == null ? (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, type) : Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) expression, type);
  }

  internal System.Linq.Expressions.Expression TransformAndDynamicConvert(
    Expression expression,
    Type type)
  {
    System.Linq.Expressions.Expression expression1 = (System.Linq.Expressions.Expression) expression;
    if (!Node.CanAssign(type, expression.Type))
    {
      System.Linq.Expressions.Expression target = expression.Reduce();
      if (target is LightDynamicExpression)
        target = target.Reduce();
      DynamicExpression dynamicExpression1 = target as DynamicExpression;
      ReducableDynamicExpression dynamicExpression2 = target as ReducableDynamicExpression;
      if (dynamicExpression1 != null && dynamicExpression1.Binder is PythonBinaryOperationBinder || dynamicExpression2 != null && dynamicExpression2.Binder is PythonBinaryOperationBinder)
      {
        PythonBinaryOperationBinder binder;
        IList<System.Linq.Expressions.Expression> arguments;
        if (dynamicExpression1 != null)
        {
          binder = (PythonBinaryOperationBinder) dynamicExpression1.Binder;
          arguments = (IList<System.Linq.Expressions.Expression>) ArrayUtils.ToArray<System.Linq.Expressions.Expression>((ICollection<System.Linq.Expressions.Expression>) dynamicExpression1.Arguments);
        }
        else
        {
          binder = (PythonBinaryOperationBinder) dynamicExpression2.Binder;
          arguments = dynamicExpression2.Args;
        }
        ParameterMappingInfo[] parameterMappingInfoArray = new ParameterMappingInfo[arguments.Count];
        for (int index = 0; index < parameterMappingInfoArray.Length; ++index)
          parameterMappingInfoArray[index] = ParameterMappingInfo.Parameter(index);
        expression1 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) this.GlobalParent.PyContext.BinaryOperationRetType(binder, this.GlobalParent.PyContext.Convert(type, ConversionResultKind.ExplicitCast)), type, (IEnumerable<System.Linq.Expressions.Expression>) arguments);
      }
      else
        expression1 = this.GlobalParent.Convert(type, ConversionResultKind.ExplicitCast, target);
    }
    return expression1;
  }

  internal static bool CanAssign(Type to, Type from)
  {
    return to.IsAssignableFrom(from) && to.IsValueType == from.IsValueType;
  }

  internal static System.Linq.Expressions.Expression ConvertIfNeeded(
    System.Linq.Expressions.Expression expression,
    Type type)
  {
    if (!Node.CanAssign(type, expression.Type))
      expression = Microsoft.Scripting.Ast.Utils.Convert(expression, type);
    return expression;
  }

  internal static System.Linq.Expressions.Expression TransformMaybeSingleLineSuite(
    Statement body,
    SourceLocation prevStart)
  {
    if (body.GlobalParent.IndexToLocation(body.StartIndex).Line != prevStart.Line)
      return (System.Linq.Expressions.Expression) body;
    System.Linq.Expressions.Expression res = body.Reduce();
    System.Linq.Expressions.Expression expression = Node.RemoveDebugInfo(prevStart.Line, res);
    if (expression.Type != typeof (void))
      expression = Microsoft.Scripting.Ast.Utils.Void(expression);
    return expression;
  }

  internal static System.Linq.Expressions.Expression RemoveDebugInfo(int prevStart, System.Linq.Expressions.Expression res)
  {
    if (res is BlockExpression blockExpression && blockExpression.Expressions.Count > 0 && blockExpression.Expressions[0] is DebugInfoExpression expression && expression.StartLine == prevStart)
      res = !(blockExpression.Type == typeof (void)) ? ((System.Linq.Expressions.BinaryExpression) blockExpression.Expressions[1]).Right : blockExpression.Expressions[1];
    return res;
  }

  internal static System.Linq.Expressions.Expression AddFrame(
    System.Linq.Expressions.Expression localContext,
    System.Linq.Expressions.Expression codeObject,
    System.Linq.Expressions.Expression body)
  {
    return (System.Linq.Expressions.Expression) new Node.FramedCodeExpression(localContext, codeObject, body);
  }

  internal static System.Linq.Expressions.Expression RemoveFrame(System.Linq.Expressions.Expression expression)
  {
    return new Node.FramedCodeVisitor().Visit(expression);
  }

  internal static System.Linq.Expressions.Expression MakeAssignment(
    ParameterExpression variable,
    System.Linq.Expressions.Expression right)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) variable, Microsoft.Scripting.Ast.Utils.Convert(right, variable.Type));
  }

  internal System.Linq.Expressions.Expression MakeAssignment(
    ParameterExpression variable,
    System.Linq.Expressions.Expression right,
    SourceSpan span)
  {
    return this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) variable, Microsoft.Scripting.Ast.Utils.Convert(right, variable.Type)), span);
  }

  internal static System.Linq.Expressions.Expression AssignValue(
    System.Linq.Expressions.Expression expression,
    System.Linq.Expressions.Expression value)
  {
    return expression is IPythonVariableExpression variableExpression ? variableExpression.Assign(value) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign(expression, value);
  }

  internal static System.Linq.Expressions.Expression Delete(System.Linq.Expressions.Expression expression)
  {
    return expression is IPythonVariableExpression variableExpression ? variableExpression.Delete() : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign(expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (Uninitialized).GetField("Instance")));
  }

  internal static ParameterExpression LineNumberUpdated => Node._lineNumberUpdated;

  internal static System.Linq.Expressions.Expression UpdateLineNumber(int line)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.LineNumberExpression, Microsoft.Scripting.Ast.Utils.Constant((object) line));
  }

  internal static System.Linq.Expressions.Expression UpdateLineUpdated(bool updated)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.LineNumberUpdated, Microsoft.Scripting.Ast.Utils.Constant((object) updated));
  }

  internal static System.Linq.Expressions.Expression PushLineUpdated(
    bool updated,
    ParameterExpression saveCurrent)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) saveCurrent, (System.Linq.Expressions.Expression) Node.LineNumberUpdated), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.LineNumberUpdated, Microsoft.Scripting.Ast.Utils.Constant((object) updated)));
  }

  internal static System.Linq.Expressions.Expression PopLineUpdated(ParameterExpression saveCurrent)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.LineNumberUpdated, (System.Linq.Expressions.Expression) saveCurrent);
  }

  internal static ParameterExpression LineNumberExpression => Node._lineNoVar;

  private class FramedCodeVisitor : ExpressionVisitor
  {
    public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression node)
    {
      return node is Node.FramedCodeExpression framedCodeExpression ? framedCodeExpression.Body : base.Visit(node);
    }
  }

  private sealed class FramedCodeExpression : System.Linq.Expressions.Expression
  {
    private readonly System.Linq.Expressions.Expression _localContext;
    private readonly System.Linq.Expressions.Expression _codeObject;
    private readonly System.Linq.Expressions.Expression _body;

    public FramedCodeExpression(System.Linq.Expressions.Expression localContext, System.Linq.Expressions.Expression codeObject, System.Linq.Expressions.Expression body)
    {
      this._localContext = localContext;
      this._codeObject = codeObject;
      this._body = body;
    }

    public override ExpressionType NodeType => ExpressionType.Extension;

    public System.Linq.Expressions.Expression Body => this._body;

    public override System.Linq.Expressions.Expression Reduce()
    {
      return (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Try((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) Node.FunctionStackVariable, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.PushFrame, this._localContext, this._codeObject)), this._body).Finally((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) Node.FunctionStackVariable, typeof (List<FunctionStack>).GetMethod("RemoveAt"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) Node.FunctionStackVariable, "Count"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) -1))));
    }

    public override Type Type => this._body.Type;

    public override bool CanReduce => true;

    protected override System.Linq.Expressions.Expression VisitChildren(ExpressionVisitor visitor)
    {
      System.Linq.Expressions.Expression localContext = visitor.Visit(this._localContext);
      System.Linq.Expressions.Expression codeObject = visitor.Visit(this._codeObject);
      System.Linq.Expressions.Expression body = visitor.Visit(this._body);
      return localContext != this._localContext || this._codeObject != codeObject || body != this._body ? (System.Linq.Expressions.Expression) new Node.FramedCodeExpression(localContext, codeObject, body) : (System.Linq.Expressions.Expression) this;
    }
  }
}
