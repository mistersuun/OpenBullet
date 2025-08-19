// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ClassDefinition
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ClassDefinition : ScopeStatement
{
  private int _headerIndex;
  private readonly string _name;
  private Statement _body;
  private readonly Expression[] _bases;
  private IList<Expression> _decorators;
  private PythonVariable _variable;
  private PythonVariable _modVariable;
  private PythonVariable _docVariable;
  private PythonVariable _modNameVariable;
  private LightLambdaExpression _dlrBody;
  private static int _classId;
  private static ParameterExpression _parentContextParam = System.Linq.Expressions.Expression.Parameter(typeof (CodeContext), "$parentContext");
  private static System.Linq.Expressions.Expression _tupleExpression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.GetClosureTupleFromContext, (System.Linq.Expressions.Expression) ClassDefinition._parentContextParam);
  private static System.Linq.Expressions.Expression NullLambda = (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Default(typeof (Func<CodeContext, CodeContext>));

  public ClassDefinition(string name, Expression[] bases, Statement body)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) bases, nameof (bases));
    this._name = name;
    this._bases = bases;
    this._body = body;
  }

  public SourceLocation Header => this.GlobalParent.IndexToLocation(this._headerIndex);

  public int HeaderIndex
  {
    get => this._headerIndex;
    set => this._headerIndex = value;
  }

  public override string Name => this._name;

  public IList<Expression> Bases => (IList<Expression>) this._bases;

  public Statement Body => this._body;

  public IList<Expression> Decorators
  {
    get => this._decorators;
    internal set => this._decorators = value;
  }

  internal PythonVariable PythonVariable
  {
    get => this._variable;
    set => this._variable = value;
  }

  internal PythonVariable ModVariable
  {
    get => this._modVariable;
    set => this._modVariable = value;
  }

  internal PythonVariable DocVariable
  {
    get => this._docVariable;
    set => this._docVariable = value;
  }

  internal PythonVariable ModuleNameVariable
  {
    get => this._modNameVariable;
    set => this._modNameVariable = value;
  }

  internal override bool HasLateBoundVariableSets
  {
    get => base.HasLateBoundVariableSets || this.NeedsLocalsDictionary;
    set => base.HasLateBoundVariableSets = value;
  }

  internal override bool ExposesLocalVariable(PythonVariable variable) => true;

  internal override PythonVariable BindReference(PythonNameBinder binder, PythonReference reference)
  {
    PythonVariable variable;
    if (this.TryGetVariable(reference.Name, out variable))
    {
      if (variable.Kind == VariableKind.Global)
        this.AddReferencedGlobal(reference.Name);
      else if (variable.Kind == VariableKind.Local)
        return (PythonVariable) null;
      return variable;
    }
    for (ScopeStatement parent = this.Parent; parent != null; parent = parent.Parent)
    {
      if (parent.TryBindOuter((ScopeStatement) this, reference, out variable))
        return variable;
    }
    return (PythonVariable) null;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    FunctionCode codeObj = this.GetOrMakeFunctionCode();
    System.Linq.Expressions.Expression expression1 = this.GlobalParent.Constant((object) codeObj);
    this.FuncCodeExpr = expression1;
    System.Linq.Expressions.Expression expression2;
    if (this.EmitDebugSymbols)
    {
      expression2 = (System.Linq.Expressions.Expression) this.GetLambda();
    }
    else
    {
      expression2 = ClassDefinition.NullLambda;
      ThreadPool.QueueUserWorkItem((WaitCallback) (x => codeObj.UpdateDelegate(this.PyContext, true)));
    }
    System.Linq.Expressions.Expression expression3 = this.AddDecorators((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeClass, expression1, expression2, this.Parent.LocalContext, Microsoft.Scripting.Ast.Utils.Constant((object) this._name), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), Node.ToObjectArray((IList<Expression>) this._bases)), Microsoft.Scripting.Ast.Utils.Constant((object) this.FindSelfNames())), this._decorators);
    return this.GlobalParent.AddDebugInfoAndVoid(Node.AssignValue(this.Parent.GetVariableExpression(this._variable), expression3), new SourceSpan(this.GlobalParent.IndexToLocation(this.StartIndex), this.GlobalParent.IndexToLocation(this.HeaderIndex)));
  }

  private LightExpression<Func<CodeContext, CodeContext>> MakeClassBody()
  {
    List<System.Linq.Expressions.Expression> init = new List<System.Linq.Expressions.Expression>();
    ReadOnlyCollectionBuilder<ParameterExpression> collectionBuilder = new ReadOnlyCollectionBuilder<ParameterExpression>();
    collectionBuilder.Add(ScopeStatement.LocalCodeContextVariable);
    collectionBuilder.Add(PythonAst._globalContext);
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalContext, (System.Linq.Expressions.Expression) new GetGlobalContextExpression((System.Linq.Expressions.Expression) ClassDefinition._parentContextParam)));
    this.GlobalParent.PrepareScope(collectionBuilder, init);
    this.CreateVariables(collectionBuilder, init);
    MethodCallExpression localContext = this.CreateLocalContext((System.Linq.Expressions.Expression) ClassDefinition._parentContextParam);
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) ScopeStatement.LocalCodeContextVariable, (System.Linq.Expressions.Expression) localContext));
    List<System.Linq.Expressions.Expression> list = new List<System.Linq.Expressions.Expression>();
    System.Linq.Expressions.Expression body = (System.Linq.Expressions.Expression) this._body;
    System.Linq.Expressions.Expression expression1 = Node.AssignValue(this.GetVariableExpression(this._modVariable), this.GetVariableExpression(this._modNameVariable));
    string documentation = this.GetDocumentation(this._body);
    if (documentation != null)
      list.Add(Node.AssignValue(this.GetVariableExpression(this._docVariable), Microsoft.Scripting.Ast.Utils.Constant((object) documentation)));
    if (this._body.CanThrow && this.GlobalParent.PyContext.PythonOptions.Frames)
    {
      body = Node.AddFrame(this.LocalContext, this.FuncCodeExpr, body);
      collectionBuilder.Add(Node.FunctionStackVariable);
    }
    System.Linq.Expressions.Expression expression2 = this.WrapScopeStatements((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) init), list.Count == 0 ? (System.Linq.Expressions.Expression) Node.EmptyBlock : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) new ReadOnlyCollection<System.Linq.Expressions.Expression>((IList<System.Linq.Expressions.Expression>) list)), expression1, body, this.LocalContext), this._body.CanThrow);
    return Microsoft.Scripting.Ast.Utils.LightLambda<Func<CodeContext, CodeContext>>(typeof (CodeContext), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) collectionBuilder, expression2), $"{this.Name}${(object) Interlocked.Increment(ref ClassDefinition._classId)}", (IList<ParameterExpression>) new ParameterExpression[1]
    {
      ClassDefinition._parentContextParam
    });
  }

  internal override LightLambdaExpression GetLambda()
  {
    if (this._dlrBody == null)
      this._dlrBody = (LightLambdaExpression) this.MakeClassBody();
    return this._dlrBody;
  }

  internal override System.Linq.Expressions.Expression GetParentClosureTuple()
  {
    return ClassDefinition._tupleExpression;
  }

  internal override string ScopeDocumentation => this.GetDocumentation(this._body);

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._decorators != null)
      {
        foreach (Node decorator in (IEnumerable<Expression>) this._decorators)
          decorator.Walk(walker);
      }
      if (this._bases != null)
      {
        foreach (Node node in this._bases)
          node.Walk(walker);
      }
      if (this._body != null)
        this._body.Walk(walker);
    }
    walker.PostWalk(this);
  }

  private string FindSelfNames()
  {
    if (!(this.Body is SuiteStatement body))
      return "";
    foreach (Statement statement in (IEnumerable<Statement>) body.Statements)
    {
      if (statement is FunctionDefinition function && function.Name == "__init__")
        return string.Join(",", ClassDefinition.SelfNameFinder.FindNames(function));
    }
    return "";
  }

  internal override void RewriteBody(ExpressionVisitor visitor)
  {
    this._dlrBody = (LightLambdaExpression) null;
    this._body = (Statement) new RewrittenBodyStatement(this.Body, visitor.Visit((System.Linq.Expressions.Expression) this.Body));
  }

  private class SelfNameFinder : PythonWalker
  {
    private readonly FunctionDefinition _function;
    private readonly IronPython.Compiler.Ast.Parameter _self;
    private Dictionary<string, bool> _names = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.Ordinal);

    public SelfNameFinder(FunctionDefinition function, IronPython.Compiler.Ast.Parameter self)
    {
      this._function = function;
      this._self = self;
    }

    public static string[] FindNames(FunctionDefinition function)
    {
      IList<IronPython.Compiler.Ast.Parameter> parameters = function.Parameters;
      if (parameters.Count <= 0)
        return ArrayUtils.EmptyStrings;
      ClassDefinition.SelfNameFinder walker = new ClassDefinition.SelfNameFinder(function, parameters[0]);
      function.Body.Walk((PythonWalker) walker);
      return ArrayUtils.ToArray<string>((ICollection<string>) walker._names.Keys);
    }

    private bool IsSelfReference(Expression expr)
    {
      PythonVariable variable;
      return expr is NameExpression nameExpression && this._function.TryGetVariable(nameExpression.Name, out variable) && variable == this._self.PythonVariable;
    }

    public override bool Walk(ClassDefinition node) => false;

    public override bool Walk(FunctionDefinition node) => false;

    public override bool Walk(AssignmentStatement node)
    {
      foreach (Expression expression in (IEnumerable<Expression>) node.Left)
      {
        if (expression is MemberExpression memberExpression && this.IsSelfReference(memberExpression.Target))
          this._names[memberExpression.Name] = true;
      }
      return true;
    }
  }
}
