// Decompiled with JetBrains decompiler
// Type: IronPython.Modules._ast
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class _ast
{
  public const string __version__ = "62047";
  public const int PyCF_ONLY_AST = 1024 /*0x0400*/;
  private static char[] MODULE_NAME_SPLITTER = new char[1]
  {
    '.'
  };
  private static bool _containsYield = false;

  internal static PythonAst ConvertToPythonAst(
    CodeContext codeContext,
    _ast.AST source,
    string filename)
  {
    PythonCompilerOptions options = new PythonCompilerOptions(ModuleOptions.ExecOrEvalCode);
    CompilerContext context = new CompilerContext(new SourceUnit((LanguageContext) codeContext.LanguageContext, (TextContentProvider) NullTextContentProvider.Null, filename, SourceCodeKind.AutoDetect), (CompilerOptions) options, ErrorSink.Default);
    bool printExpressions = false;
    Statement body;
    switch (source)
    {
      case _ast.Expression _:
        body = (Statement) new ReturnStatement(_ast.expr.Revert(((_ast.Expression) source).body));
        break;
      case _ast.Module _:
        body = _ast.stmt.RevertStmts(((_ast.Module) source).body);
        break;
      case _ast.Interactive _:
        body = _ast.stmt.RevertStmts(((_ast.Interactive) source).body);
        printExpressions = true;
        break;
      default:
        throw PythonOps.TypeError("unsupported type of AST: {0}", (object) source.GetType());
    }
    return new PythonAst(body, false, ModuleOptions.ExecOrEvalCode, printExpressions, context, new int[0]);
  }

  internal static _ast.AST BuildAst(
    CodeContext context,
    SourceUnit sourceUnit,
    PythonCompilerOptions opts,
    string mode)
  {
    using (Parser parser = Parser.CreateParser(new CompilerContext(sourceUnit, (CompilerOptions) opts, (ErrorSink) _ast.ThrowingErrorSink.Default), (PythonOptions) context.LanguageContext.Options))
      return (_ast.AST) _ast.ConvertToAST(parser.ParseFile(true), mode);
  }

  private static _ast.mod ConvertToAST(PythonAst pythonAst, string kind)
  {
    ContractUtils.RequiresNotNull((object) pythonAst, nameof (pythonAst));
    ContractUtils.RequiresNotNull((object) kind, nameof (kind));
    return _ast.ConvertToAST((SuiteStatement) pythonAst.Body, kind);
  }

  private static _ast.mod ConvertToAST(SuiteStatement suite, string kind)
  {
    ContractUtils.RequiresNotNull((object) suite, nameof (suite));
    ContractUtils.RequiresNotNull((object) kind, nameof (kind));
    switch (kind)
    {
      case "exec":
        return (_ast.mod) new _ast.Module(suite);
      case "eval":
        return (_ast.mod) new _ast.Expression(suite);
      case "single":
        return (_ast.mod) new _ast.Interactive(suite);
      default:
        throw new ArgumentException("kind must be 'exec' or 'eval' or 'single'");
    }
  }

  private class ThrowingErrorSink : ErrorSink
  {
    public static readonly _ast.ThrowingErrorSink Default = new _ast.ThrowingErrorSink();

    private ThrowingErrorSink()
    {
    }

    public override void Add(
      SourceUnit sourceUnit,
      string message,
      SourceSpan span,
      int errorCode,
      Severity severity)
    {
      if (severity != Severity.Warning)
        throw PythonOps.SyntaxError(message, sourceUnit, span, errorCode);
      PythonOps.SyntaxWarning(message, sourceUnit, span, errorCode);
    }
  }

  [PythonType]
  public abstract class AST
  {
    private PythonTuple __fields = new PythonTuple();
    private PythonTuple __attributes = new PythonTuple();
    protected int? _lineno;
    protected int? _col_offset;

    public PythonTuple _fields
    {
      get => this.__fields;
      protected set => this.__fields = value;
    }

    public PythonTuple _attributes
    {
      get => this.__attributes;
      protected set => this.__attributes = value;
    }

    public int lineno
    {
      get
      {
        return this._lineno.HasValue ? this._lineno.Value : throw PythonOps.AttributeErrorForMissingAttribute(PythonTypeOps.GetName((object) this), nameof (lineno));
      }
      set => this._lineno = new int?(value);
    }

    public int col_offset
    {
      get
      {
        return this._col_offset.HasValue ? this._col_offset.Value : throw PythonOps.AttributeErrorForMissingAttribute(PythonTypeOps.GetName((object) this), nameof (col_offset));
      }
      set => this._col_offset = new int?(value);
    }

    public void __setstate__(PythonDictionary state)
    {
      this.restoreProperties((IEnumerable<object>) this.__attributes, (IDictionary) state);
      this.restoreProperties((IEnumerable<object>) this.__fields, (IDictionary) state);
    }

    internal void restoreProperties(IEnumerable<object> names, IDictionary source)
    {
      foreach (object name in names)
      {
        if (name is string)
        {
          try
          {
            string str = (string) name;
            this.GetType().GetProperty(str).SetValue((object) this, source[(object) str], (object[]) null);
          }
          catch (KeyNotFoundException ex)
          {
          }
        }
      }
    }

    internal void storeProperties(IEnumerable<object> names, IDictionary target)
    {
      foreach (object name in names)
      {
        if (name is string)
        {
          string str = (string) name;
          try
          {
            object obj = this.GetType().GetProperty(str).GetValue((object) this, (object[]) null);
            target.Add((object) str, obj);
          }
          catch (TargetInvocationException ex)
          {
          }
        }
      }
    }

    internal PythonDictionary getstate()
    {
      PythonDictionary target = new PythonDictionary(10);
      this.storeProperties((IEnumerable<object>) this.__fields, (IDictionary) target);
      this.storeProperties((IEnumerable<object>) this.__attributes, (IDictionary) target);
      return target;
    }

    public virtual object __reduce__()
    {
      return (object) PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) new PythonTuple(), (object) this.getstate());
    }

    public virtual object __reduce_ex__(int protocol) => this.__reduce__();

    protected void GetSourceLocation(IronPython.Compiler.Ast.Node node)
    {
      this._lineno = new int?(node.Start.Line);
      this._col_offset = new int?(node.Start.Column - 1);
    }

    internal static IronPython.Runtime.List ConvertStatements(Statement stmt)
    {
      return _ast.AST.ConvertStatements(stmt, false);
    }

    internal static IronPython.Runtime.List ConvertStatements(Statement stmt, bool allowNull)
    {
      if (stmt == null)
      {
        if (allowNull)
          return PythonOps.MakeEmptyList(0);
        throw new ArgumentNullException(nameof (stmt));
      }
      if (stmt is SuiteStatement)
      {
        SuiteStatement suiteStatement = (SuiteStatement) stmt;
        IronPython.Runtime.List list = PythonOps.MakeEmptyList(suiteStatement.Statements.Count);
        foreach (Statement statement1 in (IEnumerable<Statement>) suiteStatement.Statements)
        {
          if (statement1 is SuiteStatement)
          {
            foreach (Statement statement2 in (IEnumerable<Statement>) ((SuiteStatement) statement1).Statements)
              list.Add((object) _ast.AST.Convert(statement2));
          }
          else
            list.Add((object) _ast.AST.Convert(statement1));
        }
        return list;
      }
      return PythonOps.MakeListNoCopy((object) _ast.AST.Convert(stmt));
    }

    internal static _ast.stmt Convert(Statement stmt)
    {
      _ast.stmt stmt1;
      switch (stmt)
      {
        case FunctionDefinition _:
          stmt1 = (_ast.stmt) new _ast.FunctionDef((FunctionDefinition) stmt);
          break;
        case ReturnStatement _:
          stmt1 = (_ast.stmt) new _ast.Return((ReturnStatement) stmt);
          break;
        case AssignmentStatement _:
          stmt1 = (_ast.stmt) new _ast.Assign((AssignmentStatement) stmt);
          break;
        case AugmentedAssignStatement _:
          stmt1 = (_ast.stmt) new _ast.AugAssign((AugmentedAssignStatement) stmt);
          break;
        case DelStatement _:
          stmt1 = (_ast.stmt) new _ast.Delete((DelStatement) stmt);
          break;
        case PrintStatement _:
          stmt1 = (_ast.stmt) new _ast.Print((PrintStatement) stmt);
          break;
        case ExpressionStatement _:
          stmt1 = (_ast.stmt) new _ast.Expr((ExpressionStatement) stmt);
          break;
        case ForStatement _:
          stmt1 = (_ast.stmt) new _ast.For((ForStatement) stmt);
          break;
        case WhileStatement _:
          stmt1 = (_ast.stmt) new _ast.While((WhileStatement) stmt);
          break;
        case IfStatement _:
          stmt1 = (_ast.stmt) new _ast.If((IfStatement) stmt);
          break;
        case WithStatement _:
          stmt1 = (_ast.stmt) new _ast.With((WithStatement) stmt);
          break;
        case RaiseStatement _:
          stmt1 = (_ast.stmt) new _ast.Raise((RaiseStatement) stmt);
          break;
        case TryStatement _:
          stmt1 = _ast.AST.Convert((TryStatement) stmt);
          break;
        case AssertStatement _:
          stmt1 = (_ast.stmt) new _ast.Assert((AssertStatement) stmt);
          break;
        case ImportStatement _:
          stmt1 = (_ast.stmt) new _ast.Import((ImportStatement) stmt);
          break;
        case FromImportStatement _:
          stmt1 = (_ast.stmt) new _ast.ImportFrom((FromImportStatement) stmt);
          break;
        case ExecStatement _:
          stmt1 = (_ast.stmt) new _ast.Exec((ExecStatement) stmt);
          break;
        case GlobalStatement _:
          stmt1 = (_ast.stmt) new _ast.Global((GlobalStatement) stmt);
          break;
        case ClassDefinition _:
          stmt1 = (_ast.stmt) new _ast.ClassDef((ClassDefinition) stmt);
          break;
        case BreakStatement _:
          stmt1 = (_ast.stmt) new _ast.Break();
          break;
        case ContinueStatement _:
          stmt1 = (_ast.stmt) new _ast.Continue();
          break;
        case EmptyStatement _:
          stmt1 = (_ast.stmt) new _ast.Pass();
          break;
        default:
          throw new ArgumentTypeException("Unexpected statement type: " + (object) stmt.GetType());
      }
      stmt1.GetSourceLocation((IronPython.Compiler.Ast.Node) stmt);
      return stmt1;
    }

    internal static _ast.stmt Convert(TryStatement stmt)
    {
      if (stmt.Finally == null)
        return (_ast.stmt) new _ast.TryExcept(stmt);
      IronPython.Runtime.List body;
      if (stmt.Handlers != null && stmt.Handlers.Count != 0)
      {
        _ast.stmt stmt1 = (_ast.stmt) new _ast.TryExcept(stmt);
        stmt1.GetSourceLocation((IronPython.Compiler.Ast.Node) stmt);
        body = PythonOps.MakeListNoCopy((object) stmt1);
      }
      else
        body = _ast.AST.ConvertStatements(stmt.Body);
      return (_ast.stmt) new _ast.TryFinally(body, _ast.AST.ConvertStatements(stmt.Finally));
    }

    internal static IronPython.Runtime.List ConvertAliases(
      IList<DottedName> names,
      IList<string> asnames)
    {
      IronPython.Runtime.List list = PythonOps.MakeEmptyList(names.Count);
      if (names == FromImportStatement.Star)
      {
        list.Add((object) new _ast.alias("*"));
      }
      else
      {
        for (int index = 0; index < names.Count; ++index)
          list.Add((object) new _ast.alias(names[index].MakeString(), asnames[index]));
      }
      return list;
    }

    internal static IronPython.Runtime.List ConvertAliases(
      IList<string> names,
      IList<string> asnames)
    {
      IronPython.Runtime.List list = PythonOps.MakeEmptyList(names.Count);
      if (names == FromImportStatement.Star)
      {
        list.Add((object) new _ast.alias("*"));
      }
      else
      {
        for (int index = 0; index < names.Count; ++index)
          list.Add((object) new _ast.alias(names[index], asnames[index]));
      }
      return list;
    }

    internal static _ast.slice TrySliceConvert(IronPython.Compiler.Ast.Expression expr)
    {
      switch (expr)
      {
        case SliceExpression _:
          return (_ast.slice) new _ast.Slice((SliceExpression) expr);
        case ConstantExpression _ when ((ConstantExpression) expr).Value == PythonOps.Ellipsis:
          return (_ast.slice) _ast.Ellipsis.Instance;
        case TupleExpression _ when ((TupleExpression) expr).IsExpandable:
          return (_ast.slice) new _ast.ExtSlice(((_ast.Tuple) _ast.AST.Convert(expr)).elts);
        default:
          return (_ast.slice) null;
      }
    }

    internal static _ast.expr Convert(IronPython.Compiler.Ast.Expression expr)
    {
      return _ast.AST.Convert(expr, (_ast.expr_context) _ast.Load.Instance);
    }

    internal static _ast.expr Convert(IronPython.Compiler.Ast.Expression expr, _ast.expr_context ctx)
    {
      _ast.expr expr1;
      switch (expr)
      {
        case ConstantExpression _:
          expr1 = _ast.AST.Convert((ConstantExpression) expr);
          break;
        case NameExpression _:
          expr1 = (_ast.expr) new _ast.Name((NameExpression) expr, ctx);
          break;
        case UnaryExpression _:
          expr1 = new _ast.UnaryOp((UnaryExpression) expr).TryTrimTrivialUnaryOp();
          break;
        case BinaryExpression _:
          expr1 = _ast.AST.Convert((BinaryExpression) expr);
          break;
        case AndExpression _:
          expr1 = (_ast.expr) new _ast.BoolOp((AndExpression) expr);
          break;
        case OrExpression _:
          expr1 = (_ast.expr) new _ast.BoolOp((OrExpression) expr);
          break;
        case CallExpression _:
          expr1 = (_ast.expr) new _ast.Call((CallExpression) expr);
          break;
        case ParenthesisExpression _:
          return _ast.AST.Convert(((ParenthesisExpression) expr).Expression);
        case LambdaExpression _:
          expr1 = (_ast.expr) new _ast.Lambda((LambdaExpression) expr);
          break;
        case ListExpression _:
          expr1 = (_ast.expr) new _ast.List((ListExpression) expr, ctx);
          break;
        case TupleExpression _:
          expr1 = (_ast.expr) new _ast.Tuple((TupleExpression) expr, ctx);
          break;
        case DictionaryExpression _:
          expr1 = (_ast.expr) new _ast.Dict((DictionaryExpression) expr);
          break;
        case ListComprehension _:
          expr1 = (_ast.expr) new _ast.ListComp((ListComprehension) expr);
          break;
        case GeneratorExpression _:
          expr1 = (_ast.expr) new _ast.GeneratorExp((GeneratorExpression) expr);
          break;
        case MemberExpression _:
          expr1 = (_ast.expr) new _ast.Attribute((MemberExpression) expr, ctx);
          break;
        case YieldExpression _:
          expr1 = (_ast.expr) new _ast.Yield((YieldExpression) expr);
          break;
        case ConditionalExpression _:
          expr1 = (_ast.expr) new _ast.IfExp((ConditionalExpression) expr);
          break;
        case IndexExpression _:
          expr1 = (_ast.expr) new _ast.Subscript((IndexExpression) expr, ctx);
          break;
        case BackQuoteExpression _:
          expr1 = (_ast.expr) new _ast.Repr((BackQuoteExpression) expr);
          break;
        case SetExpression _:
          expr1 = (_ast.expr) new _ast.Set((SetExpression) expr);
          break;
        case DictionaryComprehension _:
          expr1 = (_ast.expr) new _ast.DictComp((DictionaryComprehension) expr);
          break;
        case SetComprehension _:
          expr1 = (_ast.expr) new _ast.SetComp((SetComprehension) expr);
          break;
        default:
          throw new ArgumentTypeException("Unexpected expression type: " + (object) expr.GetType());
      }
      expr1.GetSourceLocation((IronPython.Compiler.Ast.Node) expr);
      return expr1;
    }

    internal static _ast.expr Convert(ConstantExpression expr)
    {
      if (expr.Value == null)
        return (_ast.expr) new _ast.Name("None", (_ast.expr_context) _ast.Load.Instance);
      if (expr.Value is int || expr.Value is double || expr.Value is long || expr.Value is BigInteger || expr.Value is Complex)
        return (_ast.expr) new _ast.Num(expr.Value);
      if (expr.Value is string)
        return (_ast.expr) new _ast.Str((string) expr.Value);
      return expr.Value is Bytes ? (_ast.expr) new _ast.Str(Converter.ConvertToString(expr.Value)) : throw new ArgumentTypeException("Unexpected constant type: " + (object) expr.Value.GetType());
    }

    internal static _ast.expr Convert(BinaryExpression expr)
    {
      _ast.AST op = _ast.AST.Convert(expr.Operator);
      if (BinaryExpression.IsComparison((IronPython.Compiler.Ast.Expression) expr))
        return (_ast.expr) new _ast.Compare(expr);
      return op is _ast.@operator ? (_ast.expr) new _ast.BinOp(expr, (_ast.@operator) op) : throw new ArgumentTypeException("Unexpected operator type: " + (object) op.GetType());
    }

    internal static _ast.AST Convert(IronPython.Compiler.Ast.Node node)
    {
      _ast.AST ast = node is TryStatementHandler ? (_ast.AST) new _ast.ExceptHandler((TryStatementHandler) node) : throw new ArgumentTypeException("Unexpected node type: " + (object) node.GetType());
      ast.GetSourceLocation(node);
      return ast;
    }

    internal static IronPython.Runtime.List Convert(IList<ComprehensionIterator> iterators)
    {
      ComprehensionIterator[] comprehensionIteratorArray = new ComprehensionIterator[iterators.Count];
      iterators.CopyTo(comprehensionIteratorArray, 0);
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      int sourceIndex = 1;
      for (int index = 0; index < comprehensionIteratorArray.Length; ++index)
      {
        if (index == 0 || comprehensionIteratorArray[index] is ComprehensionIf)
        {
          if (index == comprehensionIteratorArray.Length - 1)
            ++index;
          else
            continue;
        }
        ComprehensionIf[] comprehensionIfArray = new ComprehensionIf[index - sourceIndex];
        Array.Copy((Array) comprehensionIteratorArray, sourceIndex, (Array) comprehensionIfArray, 0, comprehensionIfArray.Length);
        list.Add((object) new _ast.comprehension((ComprehensionFor) comprehensionIteratorArray[sourceIndex - 1], comprehensionIfArray));
        sourceIndex = index + 1;
      }
      return list;
    }

    internal static IronPython.Runtime.List Convert(ComprehensionIterator[] iters)
    {
      System.Collections.Generic.List<ComprehensionFor> comprehensionForList = new System.Collections.Generic.List<ComprehensionFor>();
      System.Collections.Generic.List<System.Collections.Generic.List<ComprehensionIf>> comprehensionIfListList = new System.Collections.Generic.List<System.Collections.Generic.List<ComprehensionIf>>();
      System.Collections.Generic.List<ComprehensionIf> comprehensionIfList = (System.Collections.Generic.List<ComprehensionIf>) null;
      for (int index = 0; index < iters.Length; ++index)
      {
        if (iters[index] is ComprehensionFor)
        {
          ComprehensionFor iter = (ComprehensionFor) iters[index];
          comprehensionForList.Add(iter);
          comprehensionIfList = new System.Collections.Generic.List<ComprehensionIf>();
          comprehensionIfListList.Add(comprehensionIfList);
        }
        else
        {
          ComprehensionIf iter = (ComprehensionIf) iters[index];
          comprehensionIfList.Add(iter);
        }
      }
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      for (int index = 0; index < comprehensionForList.Count; ++index)
        list.Add((object) new _ast.comprehension(comprehensionForList[index], comprehensionIfListList[index].ToArray()));
      return list;
    }

    internal static _ast.AST Convert(PythonOperator op)
    {
      switch (op)
      {
        case PythonOperator.Not:
          return (_ast.AST) _ast.Not.Instance;
        case PythonOperator.Pos:
          return (_ast.AST) _ast.UAdd.Instance;
        case PythonOperator.Invert:
          return (_ast.AST) _ast.Invert.Instance;
        case PythonOperator.Negate:
          return (_ast.AST) _ast.USub.Instance;
        case PythonOperator.Add:
          return (_ast.AST) _ast.Add.Instance;
        case PythonOperator.Subtract:
          return (_ast.AST) _ast.Sub.Instance;
        case PythonOperator.Multiply:
          return (_ast.AST) _ast.Mult.Instance;
        case PythonOperator.Divide:
          return (_ast.AST) _ast.Div.Instance;
        case PythonOperator.TrueDivide:
          return (_ast.AST) _ast.TrueDivide.Instance;
        case PythonOperator.Mod:
          return (_ast.AST) _ast.Mod.Instance;
        case PythonOperator.BitwiseAnd:
          return (_ast.AST) _ast.BitAnd.Instance;
        case PythonOperator.BitwiseOr:
          return (_ast.AST) _ast.BitOr.Instance;
        case PythonOperator.Xor:
          return (_ast.AST) _ast.BitXor.Instance;
        case PythonOperator.LeftShift:
          return (_ast.AST) _ast.LShift.Instance;
        case PythonOperator.RightShift:
          return (_ast.AST) _ast.RShift.Instance;
        case PythonOperator.Power:
          return (_ast.AST) _ast.Pow.Instance;
        case PythonOperator.FloorDivide:
          return (_ast.AST) _ast.FloorDiv.Instance;
        case PythonOperator.LessThan:
          return (_ast.AST) _ast.Lt.Instance;
        case PythonOperator.LessThanOrEqual:
          return (_ast.AST) _ast.LtE.Instance;
        case PythonOperator.GreaterThan:
          return (_ast.AST) _ast.Gt.Instance;
        case PythonOperator.GreaterThanOrEqual:
          return (_ast.AST) _ast.GtE.Instance;
        case PythonOperator.Equal:
          return (_ast.AST) _ast.Eq.Instance;
        case PythonOperator.NotEqual:
          return (_ast.AST) _ast.NotEq.Instance;
        case PythonOperator.In:
          return (_ast.AST) _ast.In.Instance;
        case PythonOperator.NotIn:
          return (_ast.AST) _ast.NotIn.Instance;
        case PythonOperator.IsNot:
          return (_ast.AST) _ast.IsNot.Instance;
        case PythonOperator.Is:
          return (_ast.AST) _ast.Is.Instance;
        default:
          throw new ArgumentException("Unexpected PythonOperator: " + (object) op, nameof (op));
      }
    }
  }

  [PythonType]
  public class alias : _ast.AST
  {
    public alias()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (name),
        nameof (asname)
      });
    }

    public alias(string name, [Optional] string asname)
      : this()
    {
      this.name = name;
      this.asname = asname;
    }

    public string name { get; set; }

    public string asname { get; set; }
  }

  [PythonType]
  public class arguments : _ast.AST
  {
    private IronPython.Runtime.List _args;
    private string _vararg;
    private string _kwarg;
    private IronPython.Runtime.List _defaults;

    public arguments()
    {
      this._fields = new PythonTuple((object) new string[4]
      {
        nameof (args),
        nameof (vararg),
        nameof (kwarg),
        nameof (defaults)
      });
    }

    public arguments(IronPython.Runtime.List args, [Optional] string vararg, [Optional] string kwarg, IronPython.Runtime.List defaults)
      : this()
    {
      this._args = args;
      this._vararg = vararg;
      this._kwarg = kwarg;
      this._defaults = defaults;
    }

    internal arguments(IList<Parameter> parameters)
      : this()
    {
      this._args = PythonOps.MakeEmptyList(parameters.Count);
      this._defaults = PythonOps.MakeEmptyList(parameters.Count);
      foreach (Parameter parameter in (IEnumerable<Parameter>) parameters)
      {
        if (parameter.IsList)
          this._vararg = parameter.Name;
        else if (parameter.IsDictionary)
        {
          this._kwarg = parameter.Name;
        }
        else
        {
          this.args.Add((object) new _ast.Name(parameter));
          if (parameter.DefaultValue != null)
            this.defaults.Add((object) _ast.AST.Convert(parameter.DefaultValue));
        }
      }
    }

    internal arguments(Parameter[] parameters)
      : this((IList<Parameter>) parameters)
    {
    }

    internal Parameter[] Revert()
    {
      System.Collections.Generic.List<Parameter> parameterList = new System.Collections.Generic.List<Parameter>();
      int index1 = this.args.Count - 1;
      int index2 = this.defaults.Count - 1;
      while (index2 >= 0)
      {
        parameterList.Add(new Parameter(((_ast.Name) this.args[index1]).id)
        {
          DefaultValue = _ast.expr.Revert(this.defaults[index2])
        });
        --index2;
        --index1;
      }
      while (index1 >= 0)
      {
        _ast.Name name = (_ast.Name) this.args[index1--];
        parameterList.Add(new Parameter(name.id));
      }
      parameterList.Reverse();
      if (this.vararg != null)
        parameterList.Add(new Parameter(this.vararg, ParameterKind.List));
      if (this.kwarg != null)
        parameterList.Add(new Parameter(this.kwarg, ParameterKind.Dictionary));
      return parameterList.ToArray();
    }

    public IronPython.Runtime.List args
    {
      get => this._args;
      set => this._args = value;
    }

    public string vararg
    {
      get => this._vararg;
      set => this._vararg = value;
    }

    public string kwarg
    {
      get => this._kwarg;
      set => this._kwarg = value;
    }

    public IronPython.Runtime.List defaults
    {
      get => this._defaults;
      set => this._defaults = value;
    }
  }

  [PythonType]
  public abstract class boolop : _ast.AST
  {
  }

  [PythonType]
  public abstract class cmpop : _ast.AST
  {
    internal abstract PythonOperator Revert();
  }

  [PythonType]
  public class comprehension : _ast.AST
  {
    private _ast.expr _target;
    private _ast.expr _iter;
    private IronPython.Runtime.List _ifs;

    public comprehension()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (target),
        nameof (iter),
        nameof (ifs)
      });
    }

    public comprehension(_ast.expr target, _ast.expr iter, IronPython.Runtime.List ifs)
      : this()
    {
      this._target = target;
      this._iter = iter;
      this._ifs = ifs;
    }

    internal comprehension(ComprehensionFor listFor, ComprehensionIf[] listIfs)
      : this()
    {
      this._target = _ast.AST.Convert(listFor.Left, (_ast.expr_context) _ast.Store.Instance);
      this._iter = _ast.AST.Convert(listFor.List);
      this._ifs = PythonOps.MakeEmptyList(listIfs.Length);
      foreach (ComprehensionIf listIf in listIfs)
        this._ifs.Add((object) _ast.AST.Convert(listIf.Test));
    }

    internal static ComprehensionIterator[] RevertComprehensions(IronPython.Runtime.List comprehensions)
    {
      System.Collections.Generic.List<ComprehensionIterator> comprehensionIteratorList = new System.Collections.Generic.List<ComprehensionIterator>();
      foreach (_ast.comprehension comprehension in comprehensions)
      {
        ComprehensionFor comprehensionFor = new ComprehensionFor(_ast.expr.Revert(comprehension.target), _ast.expr.Revert(comprehension.iter));
        comprehensionIteratorList.Add((ComprehensionIterator) comprehensionFor);
        foreach (_ast.expr ex in comprehension.ifs)
          comprehensionIteratorList.Add((ComprehensionIterator) new ComprehensionIf(_ast.expr.Revert(ex)));
      }
      return comprehensionIteratorList.ToArray();
    }

    public _ast.expr target
    {
      get => this._target;
      set => this._target = value;
    }

    public _ast.expr iter
    {
      get => this._iter;
      set => this._iter = value;
    }

    public IronPython.Runtime.List ifs
    {
      get => this._ifs;
      set => this._ifs = value;
    }
  }

  [PythonType]
  public class excepthandler : _ast.AST
  {
    public excepthandler()
    {
      this._attributes = new PythonTuple((object) new string[2]
      {
        "lineno",
        "col_offset"
      });
    }
  }

  [PythonType]
  public abstract class expr : _ast.AST
  {
    protected expr()
    {
      this._attributes = new PythonTuple((object) new string[2]
      {
        "lineno",
        "col_offset"
      });
    }

    internal virtual IronPython.Compiler.Ast.Expression Revert()
    {
      throw PythonOps.TypeError("Unexpected expr type: {0}", (object) this.GetType());
    }

    internal static IronPython.Compiler.Ast.Expression Revert(_ast.expr ex) => ex?.Revert();

    internal static IronPython.Compiler.Ast.Expression Revert(object ex)
    {
      return ((_ast.expr) ex)?.Revert();
    }

    internal static IronPython.Compiler.Ast.Expression[] RevertExprs(IronPython.Runtime.List exprs)
    {
      IronPython.Compiler.Ast.Expression[] expressionArray = new IronPython.Compiler.Ast.Expression[exprs.Count];
      for (int index = 0; index < exprs.Count; ++index)
        expressionArray[index] = ((_ast.expr) exprs[index]).Revert();
      return expressionArray;
    }
  }

  [PythonType]
  public abstract class expr_context : _ast.AST
  {
  }

  [PythonType]
  public class keyword : _ast.AST
  {
    private string _arg;
    private _ast.expr _value;

    public keyword()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (arg),
        nameof (value)
      });
    }

    public keyword(string arg, _ast.expr value)
      : this()
    {
      this._arg = arg;
      this._value = value;
    }

    internal keyword(Arg arg)
      : this()
    {
      this._arg = arg.Name;
      this._value = _ast.AST.Convert(arg.Expression);
    }

    public string arg
    {
      get => this._arg;
      set => this._arg = value;
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public abstract class mod : _ast.AST
  {
    internal abstract IronPython.Runtime.List GetStatements();
  }

  [PythonType]
  public abstract class @operator : _ast.AST
  {
    internal abstract PythonOperator Revert();
  }

  [PythonType]
  public abstract class slice : _ast.AST
  {
  }

  [PythonType]
  public abstract class stmt : _ast.AST
  {
    protected stmt()
    {
      this._attributes = new PythonTuple((object) new string[2]
      {
        "lineno",
        "col_offset"
      });
    }

    internal virtual Statement Revert()
    {
      throw PythonOps.TypeError("Unexpected statement type: {0}", (object) this.GetType());
    }

    internal static Statement RevertStmts(IronPython.Runtime.List stmts)
    {
      if (stmts.Count == 1)
        return ((_ast.stmt) stmts[0]).Revert();
      Statement[] statements = new Statement[stmts.Count];
      for (int index = 0; index < stmts.Count; ++index)
        statements[index] = ((_ast.stmt) stmts[index]).Revert();
      return (Statement) new SuiteStatement(statements);
    }
  }

  [PythonType]
  public abstract class unaryop : _ast.AST
  {
    internal abstract PythonOperator Revert();
  }

  [PythonType]
  public class Add : _ast.@operator
  {
    internal static readonly _ast.Add Instance = new _ast.Add();

    internal override PythonOperator Revert() => PythonOperator.Add;
  }

  [PythonType]
  public class And : _ast.boolop
  {
    internal static readonly _ast.And Instance = new _ast.And();
  }

  [PythonType]
  public class Assert : _ast.stmt
  {
    private _ast.expr _test;
    private _ast.expr _msg;

    public Assert()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (test),
        nameof (msg)
      });
    }

    public Assert(_ast.expr test, _ast.expr msg, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._test = test;
      this._msg = msg;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Assert(AssertStatement stmt)
      : this()
    {
      this._test = _ast.AST.Convert(stmt.Test);
      if (stmt.Message == null)
        return;
      this._msg = _ast.AST.Convert(stmt.Message);
    }

    internal override Statement Revert()
    {
      return (Statement) new AssertStatement(_ast.expr.Revert(this.test), _ast.expr.Revert(this.msg));
    }

    public _ast.expr test
    {
      get => this._test;
      set => this._test = value;
    }

    public _ast.expr msg
    {
      get => this._msg;
      set => this._msg = value;
    }
  }

  [PythonType]
  public class Assign : _ast.stmt
  {
    private IronPython.Runtime.List _targets;
    private _ast.expr _value;

    public Assign()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (targets),
        nameof (value)
      });
    }

    public Assign(IronPython.Runtime.List targets, _ast.expr value, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._targets = targets;
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Assign(AssignmentStatement stmt)
      : this()
    {
      this._targets = PythonOps.MakeEmptyList(stmt.Left.Count);
      foreach (IronPython.Compiler.Ast.Expression expr in (IEnumerable<IronPython.Compiler.Ast.Expression>) stmt.Left)
        this._targets.Add((object) _ast.AST.Convert(expr, (_ast.expr_context) _ast.Store.Instance));
      this._value = _ast.AST.Convert(stmt.Right);
    }

    internal override Statement Revert()
    {
      return (Statement) new AssignmentStatement(_ast.expr.RevertExprs(this.targets), _ast.expr.Revert(this.value));
    }

    public IronPython.Runtime.List targets
    {
      get => this._targets;
      set => this._targets = value;
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class Attribute : _ast.expr
  {
    private _ast.expr _value;
    private string _attr;
    private _ast.expr_context _ctx;

    public Attribute()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (value),
        nameof (attr),
        nameof (ctx)
      });
    }

    public Attribute(
      _ast.expr value,
      string attr,
      _ast.expr_context ctx,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._attr = attr;
      this._ctx = ctx;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Attribute(MemberExpression attr, _ast.expr_context ctx)
      : this()
    {
      this._value = _ast.AST.Convert(attr.Target);
      this._attr = attr.Name;
      this._ctx = ctx;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new MemberExpression(_ast.expr.Revert(this.value), this.attr);
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }

    public string attr
    {
      get => this._attr;
      set => this._attr = value;
    }

    public _ast.expr_context ctx
    {
      get => this._ctx;
      set => this._ctx = value;
    }
  }

  [PythonType]
  public class AugAssign : _ast.stmt
  {
    private _ast.expr _target;
    private _ast.@operator _op;
    private _ast.expr _value;

    public AugAssign()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (target),
        nameof (op),
        nameof (value)
      });
    }

    public AugAssign(
      _ast.expr target,
      _ast.@operator op,
      _ast.expr value,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._target = target;
      this._op = op;
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal AugAssign(AugmentedAssignStatement stmt)
      : this()
    {
      this._target = _ast.AST.Convert(stmt.Left, (_ast.expr_context) _ast.Store.Instance);
      this._value = _ast.AST.Convert(stmt.Right);
      this._op = (_ast.@operator) _ast.AST.Convert(stmt.Operator);
    }

    internal override Statement Revert()
    {
      return (Statement) new AugmentedAssignStatement(this.op.Revert(), _ast.expr.Revert(this.target), _ast.expr.Revert(this.value));
    }

    public _ast.expr target
    {
      get => this._target;
      set => this._target = value;
    }

    public _ast.@operator op
    {
      get => this._op;
      set => this._op = value;
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class AugLoad : _ast.expr_context
  {
  }

  [PythonType]
  public class AugStore : _ast.expr_context
  {
  }

  [PythonType]
  public class BinOp : _ast.expr
  {
    private _ast.expr _left;
    private _ast.expr _right;
    private _ast.@operator _op;

    public BinOp()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (left),
        nameof (op),
        nameof (right)
      });
    }

    public BinOp(
      _ast.expr left,
      _ast.@operator op,
      _ast.expr right,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._left = left;
      this._op = op;
      this._right = right;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal BinOp(BinaryExpression expr, _ast.@operator op)
      : this()
    {
      this._left = _ast.AST.Convert(expr.Left);
      this._right = _ast.AST.Convert(expr.Right);
      this._op = op;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new BinaryExpression(this.op.Revert(), _ast.expr.Revert(this.left), _ast.expr.Revert(this.right));
    }

    public _ast.expr left
    {
      get => this._left;
      set => this._left = value;
    }

    public _ast.expr right
    {
      get => this._right;
      set => this._right = value;
    }

    public _ast.@operator op
    {
      get => this._op;
      set => this._op = value;
    }
  }

  [PythonType]
  public class BitAnd : _ast.@operator
  {
    internal static readonly _ast.BitAnd Instance = new _ast.BitAnd();

    internal override PythonOperator Revert() => PythonOperator.BitwiseAnd;
  }

  [PythonType]
  public class BitOr : _ast.@operator
  {
    internal static readonly _ast.BitOr Instance = new _ast.BitOr();

    internal override PythonOperator Revert() => PythonOperator.BitwiseOr;
  }

  [PythonType]
  public class BitXor : _ast.@operator
  {
    internal static readonly _ast.BitXor Instance = new _ast.BitXor();

    internal override PythonOperator Revert() => PythonOperator.Xor;
  }

  [PythonType]
  public class BoolOp : _ast.expr
  {
    private _ast.boolop _op;
    private IronPython.Runtime.List _values;

    public BoolOp()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (op),
        nameof (values)
      });
    }

    public BoolOp(_ast.boolop op, IronPython.Runtime.List values, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._op = op;
      this._values = values;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal BoolOp(AndExpression and)
      : this()
    {
      this._values = PythonOps.MakeListNoCopy((object) _ast.AST.Convert(and.Left), (object) _ast.AST.Convert(and.Right));
      this._op = (_ast.boolop) _ast.And.Instance;
    }

    internal BoolOp(OrExpression or)
      : this()
    {
      this._values = PythonOps.MakeListNoCopy((object) _ast.AST.Convert(or.Left), (object) _ast.AST.Convert(or.Right));
      this._op = (_ast.boolop) _ast.Or.Instance;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      if (this.op == _ast.And.Instance || this.op is _ast.And)
        return (IronPython.Compiler.Ast.Expression) new AndExpression(_ast.expr.Revert(this.values[0]), _ast.expr.Revert(this.values[1]));
      if (this.op == _ast.Or.Instance || this.op is _ast.Or)
        return (IronPython.Compiler.Ast.Expression) new OrExpression(_ast.expr.Revert(this.values[0]), _ast.expr.Revert(this.values[1]));
      throw PythonOps.TypeError("Unexpected boolean operator: {0}", (object) this.op);
    }

    public _ast.boolop op
    {
      get => this._op;
      set => this._op = value;
    }

    public IronPython.Runtime.List values
    {
      get => this._values;
      set => this._values = value;
    }
  }

  [PythonType]
  public class Break : _ast.stmt
  {
    internal static _ast.Break Instance = new _ast.Break();

    internal Break()
      : this(new int?(), new int?())
    {
    }

    public Break([Optional] int? lineno, [Optional] int? col_offset)
    {
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override Statement Revert() => (Statement) new BreakStatement();
  }

  [PythonType]
  public class Call : _ast.expr
  {
    private _ast.expr _func;
    private IronPython.Runtime.List _args;
    private IronPython.Runtime.List _keywords;
    private _ast.expr _starargs;
    private _ast.expr _kwargs;

    public Call()
    {
      this._fields = new PythonTuple((object) new string[5]
      {
        nameof (func),
        nameof (args),
        nameof (keywords),
        nameof (starargs),
        nameof (kwargs)
      });
    }

    public Call(
      _ast.expr func,
      IronPython.Runtime.List args,
      IronPython.Runtime.List keywords,
      [Optional] _ast.expr starargs,
      [Optional] _ast.expr kwargs,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._func = func;
      this._args = args;
      this._keywords = keywords;
      this._starargs = starargs;
      this._kwargs = kwargs;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Call(CallExpression call)
      : this()
    {
      this._args = PythonOps.MakeEmptyList(call.Args.Count);
      this._keywords = new IronPython.Runtime.List();
      this._func = _ast.AST.Convert(call.Target);
      foreach (Arg obj in (IEnumerable<Arg>) call.Args)
      {
        if (obj.Name == null)
          this._args.Add((object) _ast.AST.Convert(obj.Expression));
        else if (obj.Name == "*")
          this._starargs = _ast.AST.Convert(obj.Expression);
        else if (obj.Name == "**")
          this._kwargs = _ast.AST.Convert(obj.Expression);
        else
          this._keywords.Add((object) new _ast.keyword(obj));
      }
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression target = _ast.expr.Revert(this.func);
      System.Collections.Generic.List<Arg> objList = new System.Collections.Generic.List<Arg>();
      foreach (_ast.expr ex in this.args)
        objList.Add(new Arg(_ast.expr.Revert(ex)));
      if (this.starargs != null)
        objList.Add(new Arg("*", _ast.expr.Revert(this.starargs)));
      if (this.kwargs != null)
        objList.Add(new Arg("**", _ast.expr.Revert(this.kwargs)));
      foreach (_ast.keyword keyword in this.keywords)
        objList.Add(new Arg(keyword.arg, _ast.expr.Revert(keyword.value)));
      return (IronPython.Compiler.Ast.Expression) new CallExpression(target, objList.ToArray());
    }

    public _ast.expr func
    {
      get => this._func;
      set => this._func = value;
    }

    public IronPython.Runtime.List args
    {
      get => this._args;
      set => this._args = value;
    }

    public IronPython.Runtime.List keywords
    {
      get => this._keywords;
      set => this._keywords = value;
    }

    public _ast.expr starargs
    {
      get => this._starargs;
      set => this._starargs = value;
    }

    public _ast.expr kwargs
    {
      get => this._kwargs;
      set => this._kwargs = value;
    }
  }

  [PythonType]
  public class ClassDef : _ast.stmt
  {
    private string _name;
    private IronPython.Runtime.List _bases;
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _decorator_list;

    public ClassDef()
    {
      this._fields = new PythonTuple((object) new string[4]
      {
        nameof (name),
        nameof (bases),
        nameof (body),
        nameof (decorator_list)
      });
    }

    public ClassDef(
      string name,
      IronPython.Runtime.List bases,
      IronPython.Runtime.List body,
      IronPython.Runtime.List decorator_list,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._name = name;
      this._bases = bases;
      this._body = body;
      this._decorator_list = decorator_list;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal ClassDef(ClassDefinition def)
      : this()
    {
      this._name = def.Name;
      this._bases = PythonOps.MakeEmptyList(def.Bases.Count);
      foreach (IronPython.Compiler.Ast.Expression expr in (IEnumerable<IronPython.Compiler.Ast.Expression>) def.Bases)
        this._bases.Add((object) _ast.AST.Convert(expr));
      this._body = _ast.AST.ConvertStatements(def.Body);
      if (def.Decorators != null)
      {
        this._decorator_list = PythonOps.MakeEmptyList(def.Decorators.Count);
        foreach (IronPython.Compiler.Ast.Expression decorator in (IEnumerable<IronPython.Compiler.Ast.Expression>) def.Decorators)
          this._decorator_list.Add((object) _ast.AST.Convert(decorator));
      }
      else
        this._decorator_list = PythonOps.MakeEmptyList(0);
    }

    internal override Statement Revert()
    {
      ClassDefinition classDefinition = new ClassDefinition(this.name, _ast.expr.RevertExprs(this.bases), _ast.stmt.RevertStmts(this.body));
      if (this.decorator_list.Count != 0)
        classDefinition.Decorators = (IList<IronPython.Compiler.Ast.Expression>) _ast.expr.RevertExprs(this.decorator_list);
      return (Statement) classDefinition;
    }

    public string name
    {
      get => this._name;
      set => this._name = value;
    }

    public IronPython.Runtime.List bases
    {
      get => this._bases;
      set => this._bases = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List decorator_list
    {
      get => this._decorator_list;
      set => this._decorator_list = value;
    }
  }

  [PythonType]
  public class Compare : _ast.expr
  {
    private _ast.expr _left;
    private IronPython.Runtime.List _ops;
    private IronPython.Runtime.List _comparators;

    public Compare()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (left),
        nameof (ops),
        nameof (comparators)
      });
    }

    public Compare(_ast.expr left, IronPython.Runtime.List ops, IronPython.Runtime.List comparators, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._left = left;
      this._ops = ops;
      this._comparators = comparators;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Compare(BinaryExpression expr)
      : this()
    {
      this._left = _ast.AST.Convert(expr.Left);
      this._ops = PythonOps.MakeList();
      this._comparators = PythonOps.MakeList();
      BinaryExpression right;
      for (; BinaryExpression.IsComparison(expr.Right); expr = right)
      {
        right = (BinaryExpression) expr.Right;
        this._ops.Add((object) _ast.AST.Convert(expr.Operator));
        this._comparators.Add((object) _ast.AST.Convert(right.Left));
      }
      this._ops.Add((object) _ast.AST.Convert(expr.Operator));
      this._comparators.Add((object) _ast.AST.Convert(expr.Right));
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      if (this.ops.Count == 1)
        return (IronPython.Compiler.Ast.Expression) new BinaryExpression(((_ast.cmpop) this.ops[0]).Revert(), _ast.expr.Revert(this.left), _ast.expr.Revert(this.comparators[0]));
      int index1 = this.ops.Count - 1;
      BinaryExpression right = new BinaryExpression(((_ast.cmpop) this.ops[index1]).Revert(), _ast.expr.Revert(this.comparators[index1 - 1]), _ast.expr.Revert(this.comparators[index1]));
      for (int index2 = index1 - 1; index2 > 0; --index2)
        right = new BinaryExpression(((_ast.cmpop) this.ops[index2]).Revert(), _ast.expr.Revert(this.comparators[index2 - 1]), (IronPython.Compiler.Ast.Expression) right);
      return (IronPython.Compiler.Ast.Expression) new BinaryExpression(((_ast.cmpop) this.ops[0]).Revert(), _ast.expr.Revert(this.left), (IronPython.Compiler.Ast.Expression) right);
    }

    public _ast.expr left
    {
      get => this._left;
      set => this._left = value;
    }

    public IronPython.Runtime.List ops
    {
      get => this._ops;
      set => this._ops = value;
    }

    public IronPython.Runtime.List comparators
    {
      get => this._comparators;
      set => this._comparators = value;
    }
  }

  [PythonType]
  public class Continue : _ast.stmt
  {
    internal static _ast.Continue Instance = new _ast.Continue();

    internal Continue()
      : this(new int?(), new int?())
    {
    }

    public Continue([Optional] int? lineno, [Optional] int? col_offset)
    {
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override Statement Revert() => (Statement) new ContinueStatement();
  }

  [PythonType]
  public class Del : _ast.expr_context
  {
    internal static _ast.Del Instance = new _ast.Del();
  }

  [PythonType]
  public class Delete : _ast.stmt
  {
    private IronPython.Runtime.List _targets;

    public Delete()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (targets)
      });
    }

    public Delete(IronPython.Runtime.List targets, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._targets = targets;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Delete(DelStatement stmt)
      : this()
    {
      this._targets = PythonOps.MakeEmptyList(stmt.Expressions.Count);
      foreach (IronPython.Compiler.Ast.Expression expression in (IEnumerable<IronPython.Compiler.Ast.Expression>) stmt.Expressions)
        this._targets.Add((object) _ast.AST.Convert(expression, (_ast.expr_context) _ast.Del.Instance));
    }

    internal override Statement Revert()
    {
      return (Statement) new DelStatement(_ast.expr.RevertExprs(this.targets));
    }

    public IronPython.Runtime.List targets
    {
      get => this._targets;
      set => this._targets = value;
    }
  }

  [PythonType]
  public class Dict : _ast.expr
  {
    private IronPython.Runtime.List _keys;
    private IronPython.Runtime.List _values;

    public Dict()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (keys),
        nameof (values)
      });
    }

    public Dict(IronPython.Runtime.List keys, IronPython.Runtime.List values, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._keys = keys;
      this._values = values;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Dict(DictionaryExpression expr)
      : this()
    {
      this._keys = PythonOps.MakeEmptyList(expr.Items.Count);
      this._values = PythonOps.MakeEmptyList(expr.Items.Count);
      foreach (SliceExpression sliceExpression in (IEnumerable<SliceExpression>) expr.Items)
      {
        this._keys.Add((object) _ast.AST.Convert(sliceExpression.SliceStart));
        this._values.Add((object) _ast.AST.Convert(sliceExpression.SliceStop));
      }
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      SliceExpression[] sliceExpressionArray = new SliceExpression[this.values.Count];
      for (int index = 0; index < this.values.Count; ++index)
        sliceExpressionArray[index] = new SliceExpression(_ast.expr.Revert(this.keys[index]), _ast.expr.Revert(this.values[index]), (IronPython.Compiler.Ast.Expression) null, false);
      return (IronPython.Compiler.Ast.Expression) new DictionaryExpression(sliceExpressionArray);
    }

    public IronPython.Runtime.List keys
    {
      get => this._keys;
      set => this._keys = value;
    }

    public IronPython.Runtime.List values
    {
      get => this._values;
      set => this._values = value;
    }
  }

  [PythonType]
  public class DictComp : _ast.expr
  {
    private _ast.expr _key;
    private _ast.expr _value;
    private IronPython.Runtime.List _generators;

    public DictComp()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (key),
        nameof (value),
        nameof (generators)
      });
    }

    public DictComp(
      _ast.expr key,
      _ast.expr value,
      IronPython.Runtime.List generators,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._key = key;
      this._value = value;
      this._generators = generators;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal DictComp(DictionaryComprehension comp)
      : this()
    {
      this._key = _ast.AST.Convert(comp.Key);
      this._value = _ast.AST.Convert(comp.Value);
      this._generators = _ast.AST.Convert(comp.Iterators);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new DictionaryComprehension(_ast.expr.Revert(this.key), _ast.expr.Revert(this.value), _ast.comprehension.RevertComprehensions(this.generators));
    }

    public _ast.expr key
    {
      get => this._key;
      set => this._key = value;
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }

    public IronPython.Runtime.List generators
    {
      get => this._generators;
      set => this._generators = value;
    }
  }

  [PythonType]
  public class Div : _ast.@operator
  {
    internal static readonly _ast.Div Instance = new _ast.Div();

    internal override PythonOperator Revert() => PythonOperator.Divide;
  }

  [PythonType]
  public class TrueDivide : _ast.@operator
  {
    internal static readonly _ast.TrueDivide Instance = new _ast.TrueDivide();

    internal override PythonOperator Revert() => PythonOperator.TrueDivide;
  }

  [PythonType]
  public class Ellipsis : _ast.slice
  {
    internal static _ast.Ellipsis Instance = new _ast.Ellipsis();
  }

  [PythonType]
  public class Eq : _ast.cmpop
  {
    internal static readonly _ast.Eq Instance = new _ast.Eq();

    internal override PythonOperator Revert() => PythonOperator.Equal;
  }

  [PythonType]
  public class ExceptHandler : _ast.excepthandler
  {
    private _ast.expr _type;
    private _ast.expr _name;
    private IronPython.Runtime.List _body;

    public ExceptHandler()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (type),
        nameof (name),
        nameof (body)
      });
    }

    public ExceptHandler([Optional] _ast.expr type, [Optional] _ast.expr name, IronPython.Runtime.List body, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._type = type;
      this._name = name;
      this._body = body;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal ExceptHandler(TryStatementHandler stmt)
      : this()
    {
      if (stmt.Test != null)
        this._type = _ast.AST.Convert(stmt.Test);
      if (stmt.Target != null)
        this._name = _ast.AST.Convert(stmt.Target, (_ast.expr_context) _ast.Store.Instance);
      this._body = _ast.AST.ConvertStatements(stmt.Body);
    }

    internal TryStatementHandler RevertHandler()
    {
      return new TryStatementHandler(_ast.expr.Revert(this.type), _ast.expr.Revert(this.name), _ast.stmt.RevertStmts(this.body));
    }

    public _ast.expr type
    {
      get => this._type;
      set => this._type = value;
    }

    public _ast.expr name
    {
      get => this._name;
      set => this._name = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }
  }

  [PythonType]
  public class Exec : _ast.stmt
  {
    private _ast.expr _body;
    private _ast.expr _globals;
    private _ast.expr _locals;

    public Exec()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (body),
        nameof (globals),
        nameof (locals)
      });
    }

    public Exec(
      _ast.expr body,
      [Optional] _ast.expr globals,
      [Optional] _ast.expr locals,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._body = body;
      this._globals = globals;
      this._locals = locals;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    public Exec(ExecStatement stmt)
      : this()
    {
      this._body = _ast.AST.Convert(stmt.Code);
      if (stmt.Globals != null)
        this._globals = _ast.AST.Convert(stmt.Globals);
      if (stmt.Locals == null)
        return;
      this._locals = _ast.AST.Convert(stmt.Locals);
    }

    internal override Statement Revert()
    {
      return (Statement) new ExecStatement(_ast.expr.Revert(this.body), _ast.expr.Revert(this.locals), _ast.expr.Revert(this.globals));
    }

    public _ast.expr body
    {
      get => this._body;
      set => this._body = value;
    }

    public _ast.expr globals
    {
      get => this._globals;
      set => this._globals = value;
    }

    public _ast.expr locals
    {
      get => this._locals;
      set => this._locals = value;
    }
  }

  [PythonType]
  public class Expr : _ast.stmt
  {
    private _ast.expr _value;

    public Expr()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (value)
      });
    }

    public Expr(_ast.expr value, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Expr(ExpressionStatement stmt)
      : this()
    {
      this._value = _ast.AST.Convert(stmt.Expression);
    }

    internal override Statement Revert()
    {
      return (Statement) new ExpressionStatement(_ast.expr.Revert(this.value));
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class Expression : _ast.mod
  {
    private _ast.expr _body;

    public Expression()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (body)
      });
    }

    public Expression(_ast.expr body)
      : this()
    {
      this._body = body;
    }

    internal Expression(SuiteStatement suite)
      : this()
    {
      this._body = _ast.AST.Convert(((ExpressionStatement) suite.Statements[0]).Expression);
    }

    internal override IronPython.Runtime.List GetStatements()
    {
      return PythonOps.MakeListNoCopy((object) this._body);
    }

    public _ast.expr body
    {
      get => this._body;
      set => this._body = value;
    }
  }

  [PythonType]
  public class ExtSlice : _ast.slice
  {
    private IronPython.Runtime.List _dims;

    public ExtSlice()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (dims)
      });
    }

    public ExtSlice(IronPython.Runtime.List dims)
      : this()
    {
      this._dims = dims;
    }

    internal IronPython.Compiler.Ast.Expression[] Revert()
    {
      System.Collections.Generic.List<IronPython.Compiler.Ast.Expression> expressionList = new System.Collections.Generic.List<IronPython.Compiler.Ast.Expression>(this.dims.Count);
      foreach (_ast.expr dim in this.dims)
        expressionList.Add(_ast.expr.Revert(dim));
      return expressionList.ToArray();
    }

    public IronPython.Runtime.List dims
    {
      get => this._dims;
      set => this._dims = value;
    }
  }

  [PythonType]
  public class FloorDiv : _ast.@operator
  {
    internal static readonly _ast.FloorDiv Instance = new _ast.FloorDiv();

    internal override PythonOperator Revert() => PythonOperator.FloorDivide;
  }

  [PythonType]
  public class For : _ast.stmt
  {
    private _ast.expr _target;
    private _ast.expr _iter;
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _orelse;

    public For()
    {
      this._fields = new PythonTuple((object) new string[4]
      {
        nameof (target),
        nameof (iter),
        nameof (body),
        nameof (orelse)
      });
    }

    public For(
      _ast.expr target,
      _ast.expr iter,
      IronPython.Runtime.List body,
      [Optional] IronPython.Runtime.List orelse,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._target = target;
      this._iter = iter;
      this._body = body;
      this._orelse = orelse != null ? orelse : new IronPython.Runtime.List();
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal For(ForStatement stmt)
      : this()
    {
      this._target = _ast.AST.Convert(stmt.Left, (_ast.expr_context) _ast.Store.Instance);
      this._iter = _ast.AST.Convert(stmt.List);
      this._body = _ast.AST.ConvertStatements(stmt.Body);
      this._orelse = _ast.AST.ConvertStatements(stmt.Else, true);
    }

    internal override Statement Revert()
    {
      return (Statement) new ForStatement(_ast.expr.Revert(this.target), _ast.expr.Revert(this.iter), _ast.stmt.RevertStmts(this.body), _ast.stmt.RevertStmts(this.orelse));
    }

    public _ast.expr target
    {
      get => this._target;
      set => this._target = value;
    }

    public _ast.expr iter
    {
      get => this._iter;
      set => this._iter = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List orelse
    {
      get => this._orelse;
      set => this._orelse = value;
    }
  }

  [PythonType]
  public class FunctionDef : _ast.stmt
  {
    private string _name;
    private _ast.arguments _args;
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _decorator_list;

    public FunctionDef()
    {
      this._fields = new PythonTuple((object) new string[4]
      {
        nameof (name),
        nameof (args),
        nameof (body),
        nameof (decorator_list)
      });
    }

    public FunctionDef(
      string name,
      _ast.arguments args,
      IronPython.Runtime.List body,
      IronPython.Runtime.List decorator_list,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._name = name;
      this._args = args;
      this._body = body;
      this._decorator_list = decorator_list;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal FunctionDef(FunctionDefinition def)
      : this()
    {
      this._name = def.Name;
      this._args = new _ast.arguments(def.Parameters);
      this._body = _ast.AST.ConvertStatements(def.Body);
      if (def.Decorators != null)
      {
        this._decorator_list = PythonOps.MakeEmptyList(def.Decorators.Count);
        foreach (IronPython.Compiler.Ast.Expression decorator in (IEnumerable<IronPython.Compiler.Ast.Expression>) def.Decorators)
          this._decorator_list.Add((object) _ast.AST.Convert(decorator));
      }
      else
        this._decorator_list = PythonOps.MakeEmptyList(0);
    }

    internal override Statement Revert()
    {
      FunctionDefinition functionDefinition = new FunctionDefinition(this.name, this.args.Revert(), _ast.stmt.RevertStmts(this.body));
      functionDefinition.IsGenerator = _ast._containsYield;
      _ast._containsYield = false;
      if (this.decorator_list.Count != 0)
        functionDefinition.Decorators = (IList<IronPython.Compiler.Ast.Expression>) _ast.expr.RevertExprs(this.decorator_list);
      return (Statement) functionDefinition;
    }

    public string name
    {
      get => this._name;
      set => this._name = value;
    }

    public _ast.arguments args
    {
      get => this._args;
      set => this._args = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List decorator_list
    {
      get => this._decorator_list;
      set => this._decorator_list = value;
    }
  }

  [PythonType]
  public class GeneratorExp : _ast.expr
  {
    private _ast.expr _elt;
    private IronPython.Runtime.List _generators;
    private const string generatorFnName = "<genexpr>";
    private const string generatorFnArgName = "__gen_$_parm__";

    public GeneratorExp()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (elt),
        nameof (generators)
      });
    }

    public GeneratorExp(_ast.expr elt, IronPython.Runtime.List generators, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elt = elt;
      this._generators = generators;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal GeneratorExp(GeneratorExpression expr)
      : this()
    {
      _ast.GeneratorExp.ExtractListComprehensionIterators walker = new _ast.GeneratorExp.ExtractListComprehensionIterators();
      expr.Function.Body.Walk((PythonWalker) walker);
      ComprehensionIterator[] iterators = walker.Iterators;
      iterators[0] = (ComprehensionIterator) new ComprehensionFor(((ComprehensionFor) iterators[0]).Left, expr.Iterable);
      this._elt = _ast.AST.Convert(walker.Yield.Expression);
      this._generators = _ast.AST.Convert(iterators);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      Statement body = (Statement) new ExpressionStatement((IronPython.Compiler.Ast.Expression) new YieldExpression(_ast.expr.Revert(this.elt)));
      int index1 = this.generators.Count - 1;
      IronPython.Compiler.Ast.Expression expression;
      do
      {
        _ast.comprehension generator = (_ast.comprehension) this.generators[index1];
        if (generator.ifs != null && generator.ifs.Count != 0)
        {
          for (int index2 = generator.ifs.Count - 1; index2 >= 0; --index2)
            body = (Statement) new IfStatement(new IfStatementTest[1]
            {
              new IfStatementTest(_ast.expr.Revert(generator.ifs[index2]), body)
            }, (Statement) null);
        }
        expression = _ast.expr.Revert(generator.iter);
        body = (Statement) new ForStatement(_ast.expr.Revert(generator.target), expression, body, (Statement) null);
        --index1;
      }
      while (index1 >= 0);
      ((ForStatement) body).List = (IronPython.Compiler.Ast.Expression) new NameExpression("__gen_$_parm__");
      return (IronPython.Compiler.Ast.Expression) new GeneratorExpression(new FunctionDefinition("<genexpr>", new Parameter[1]
      {
        new Parameter("__gen_$_parm__", ParameterKind.Normal)
      }, body)
      {
        IsGenerator = true
      }, expression);
    }

    public _ast.expr elt
    {
      get => this._elt;
      set => this._elt = value;
    }

    public IronPython.Runtime.List generators
    {
      get => this._generators;
      set => this._generators = value;
    }

    internal class ExtractListComprehensionIterators : PythonWalker
    {
      private readonly System.Collections.Generic.List<ComprehensionIterator> _iterators = new System.Collections.Generic.List<ComprehensionIterator>();
      public YieldExpression Yield;

      public ComprehensionIterator[] Iterators => this._iterators.ToArray();

      public override bool Walk(ForStatement node)
      {
        this._iterators.Add((ComprehensionIterator) new ComprehensionFor(node.Left, node.List));
        node.Body.Walk((PythonWalker) this);
        return false;
      }

      public override bool Walk(IfStatement node)
      {
        this._iterators.Add((ComprehensionIterator) new ComprehensionIf(node.Tests[0].Test));
        node.Tests[0].Body.Walk((PythonWalker) this);
        return false;
      }

      public override bool Walk(YieldExpression node)
      {
        this.Yield = node;
        return false;
      }
    }
  }

  [PythonType]
  public class Global : _ast.stmt
  {
    private IronPython.Runtime.List _names;

    public Global()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (names)
      });
    }

    public Global(IronPython.Runtime.List names, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._names = names;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Global(GlobalStatement stmt)
      : this()
    {
      this._names = new IronPython.Runtime.List((object) stmt.Names);
    }

    internal override Statement Revert()
    {
      string[] names = new string[this.names.Count];
      for (int index = 0; index < this.names.Count; ++index)
        names[index] = (string) this.names[index];
      return (Statement) new GlobalStatement(names);
    }

    public IronPython.Runtime.List names
    {
      get => this._names;
      set => this._names = value;
    }
  }

  [PythonType]
  public class Gt : _ast.cmpop
  {
    internal static readonly _ast.Gt Instance = new _ast.Gt();

    internal override PythonOperator Revert() => PythonOperator.GreaterThan;
  }

  [PythonType]
  public class GtE : _ast.cmpop
  {
    internal static readonly _ast.GtE Instance = new _ast.GtE();

    internal override PythonOperator Revert() => PythonOperator.GreaterThanOrEqual;
  }

  [PythonType]
  public class If : _ast.stmt
  {
    private _ast.expr _test;
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _orelse;

    public If()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (test),
        nameof (body),
        nameof (orelse)
      });
    }

    public If(_ast.expr test, IronPython.Runtime.List body, [Optional] IronPython.Runtime.List orelse, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._test = test;
      this._body = body;
      this._orelse = orelse != null ? orelse : new IronPython.Runtime.List();
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal If(IfStatement stmt)
      : this()
    {
      _ast.If if1 = this;
      _ast.If if2 = (_ast.If) null;
      foreach (IfStatementTest test in (IEnumerable<IfStatementTest>) stmt.Tests)
      {
        if (if2 != null)
        {
          if1 = new _ast.If();
          if2._orelse = PythonOps.MakeListNoCopy((object) if1);
        }
        if1.Initialize(test);
        if2 = if1;
      }
      if1._orelse = _ast.AST.ConvertStatements(stmt.ElseStatement, true);
    }

    internal void Initialize(IfStatementTest ifTest)
    {
      this._test = _ast.AST.Convert(ifTest.Test);
      this._body = _ast.AST.ConvertStatements(ifTest.Body);
      this.GetSourceLocation((IronPython.Compiler.Ast.Node) ifTest);
    }

    internal override Statement Revert()
    {
      System.Collections.Generic.List<IfStatementTest> ifStatementTestList = new System.Collections.Generic.List<IfStatementTest>();
      ifStatementTestList.Add(new IfStatementTest(_ast.expr.Revert(this.test), _ast.stmt.RevertStmts(this.body)));
      _ast.If if1;
      _ast.If if2;
      for (if1 = this; if1.orelse != null && if1.orelse.Count == 1 && if1.orelse[0] is _ast.If; if1 = if2)
      {
        if2 = (_ast.If) if1.orelse[0];
        ifStatementTestList.Add(new IfStatementTest(_ast.expr.Revert(if2.test), _ast.stmt.RevertStmts(if2.body)));
      }
      return (Statement) new IfStatement(ifStatementTestList.ToArray(), _ast.stmt.RevertStmts(if1.orelse));
    }

    public _ast.expr test
    {
      get => this._test;
      set => this._test = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List orelse
    {
      get => this._orelse;
      set => this._orelse = value;
    }
  }

  [PythonType]
  public class IfExp : _ast.expr
  {
    private _ast.expr _test;
    private _ast.expr _body;
    private _ast.expr _orelse;

    public IfExp()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (test),
        nameof (body),
        nameof (orelse)
      });
    }

    public IfExp(_ast.expr test, _ast.expr body, _ast.expr orelse, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._test = test;
      this._body = body;
      this._orelse = orelse;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal IfExp(ConditionalExpression cond)
      : this()
    {
      this._test = _ast.AST.Convert(cond.Test);
      this._body = _ast.AST.Convert(cond.TrueExpression);
      this._orelse = _ast.AST.Convert(cond.FalseExpression);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new ConditionalExpression(_ast.expr.Revert(this.test), _ast.expr.Revert(this.body), _ast.expr.Revert(this.orelse));
    }

    public _ast.expr test
    {
      get => this._test;
      set => this._test = value;
    }

    public _ast.expr body
    {
      get => this._body;
      set => this._body = value;
    }

    public _ast.expr orelse
    {
      get => this._orelse;
      set => this._orelse = value;
    }
  }

  [PythonType]
  public class Import : _ast.stmt
  {
    private IronPython.Runtime.List _names;

    public Import()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (names)
      });
    }

    public Import(IronPython.Runtime.List names, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._names = names;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Import(ImportStatement stmt)
      : this()
    {
      this._names = _ast.AST.ConvertAliases(stmt.Names, stmt.AsNames);
    }

    internal override Statement Revert()
    {
      ModuleName[] names = new ModuleName[this.names.Count];
      string[] asNames = new string[this.names.Count];
      for (int index = 0; index < this.names.Count; ++index)
      {
        _ast.alias name = (_ast.alias) this.names[index];
        names[index] = new ModuleName(name.name.Split(_ast.MODULE_NAME_SPLITTER));
        asNames[index] = name.asname;
      }
      return (Statement) new ImportStatement(names, asNames, false);
    }

    public IronPython.Runtime.List names
    {
      get => this._names;
      set => this._names = value;
    }
  }

  [PythonType]
  public class ImportFrom : _ast.stmt
  {
    private string _module;
    private IronPython.Runtime.List _names;
    private int _level;

    public ImportFrom()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (module),
        nameof (names),
        nameof (level)
      });
    }

    public ImportFrom([Optional] string module, IronPython.Runtime.List names, [Optional] int level, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._module = module;
      this._names = names;
      this._level = level;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    public ImportFrom(FromImportStatement stmt)
      : this()
    {
      this._module = stmt.Root.MakeString();
      this._module = string.IsNullOrEmpty(this._module) ? (string) null : this._module;
      this._names = _ast.AST.ConvertAliases(stmt.Names, stmt.AsNames);
      if (!(stmt.Root is RelativeModuleName))
        return;
      this._level = ((RelativeModuleName) stmt.Root).DotCount;
    }

    internal override Statement Revert()
    {
      ModuleName root = (ModuleName) null;
      bool forceAbsolute = false;
      if (this.module != null)
      {
        if (this.module[0] == '.')
        {
          root = (ModuleName) new RelativeModuleName(this.module.Split(_ast.MODULE_NAME_SPLITTER), this.level);
        }
        else
        {
          root = new ModuleName(this.module.Split(_ast.MODULE_NAME_SPLITTER));
          forceAbsolute = true;
        }
      }
      if (this.names.Count == 1 && ((_ast.alias) this.names[0]).name == "*")
        return (Statement) new FromImportStatement(root, (string[]) FromImportStatement.Star, (string[]) null, false, forceAbsolute);
      string[] names = new string[this.names.Count];
      string[] asNames = new string[this.names.Count];
      for (int index = 0; index < this.names.Count; ++index)
      {
        _ast.alias name = (_ast.alias) this.names[index];
        names[index] = name.name;
        asNames[index] = name.asname;
      }
      return (Statement) new FromImportStatement(root, names, asNames, false, forceAbsolute);
    }

    public string module
    {
      get => this._module;
      set => this._module = value;
    }

    public IronPython.Runtime.List names
    {
      get => this._names;
      set => this._names = value;
    }

    public int level
    {
      get => this._level;
      set => this._level = value;
    }
  }

  [PythonType]
  public class In : _ast.cmpop
  {
    internal static readonly _ast.In Instance = new _ast.In();

    internal override PythonOperator Revert() => PythonOperator.In;
  }

  [PythonType]
  public class Index : _ast.slice
  {
    private _ast.expr _value;

    public Index()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (value)
      });
    }

    public Index(_ast.expr value)
      : this()
    {
      this._value = value;
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class Interactive : _ast.mod
  {
    private IronPython.Runtime.List _body;

    public Interactive()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (body)
      });
    }

    public Interactive(IronPython.Runtime.List body)
      : this()
    {
      this._body = body;
    }

    internal Interactive(SuiteStatement suite)
      : this()
    {
      this._body = _ast.AST.ConvertStatements((Statement) suite);
    }

    internal override IronPython.Runtime.List GetStatements() => this._body;

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }
  }

  [PythonType]
  public class Invert : _ast.unaryop
  {
    internal static readonly _ast.Invert Instance = new _ast.Invert();

    internal override PythonOperator Revert() => PythonOperator.Invert;
  }

  [PythonType]
  public class Is : _ast.cmpop
  {
    internal static readonly _ast.Is Instance = new _ast.Is();

    internal override PythonOperator Revert() => PythonOperator.Is;
  }

  [PythonType]
  public class IsNot : _ast.cmpop
  {
    internal static readonly _ast.IsNot Instance = new _ast.IsNot();

    internal override PythonOperator Revert() => PythonOperator.IsNot;
  }

  [PythonType]
  public class Lambda : _ast.expr
  {
    private _ast.arguments _args;
    private _ast.expr _body;

    public Lambda()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (args),
        nameof (body)
      });
    }

    public Lambda(_ast.arguments args, _ast.expr body, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._args = args;
      this._body = body;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Lambda(LambdaExpression lambda)
      : this()
    {
      _ast.FunctionDef functionDef = (_ast.FunctionDef) _ast.AST.Convert((Statement) lambda.Function);
      this._args = functionDef.args;
      _ast.stmt stmt = (_ast.stmt) functionDef.body[0];
      switch (stmt)
      {
        case _ast.Return _:
          this._body = ((_ast.Return) stmt).value;
          break;
        case _ast.Expr _:
          this._body = ((_ast.Yield) ((_ast.Expr) stmt).value).value;
          break;
        default:
          throw PythonOps.TypeError("Unexpected statement type: {0}, expected Return or Expr", (object) stmt.GetType());
      }
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression expression = _ast.expr.Revert(this.body);
      Statement body = _ast._containsYield ? (Statement) new ExpressionStatement(expression) : (Statement) new ReturnStatement(expression);
      FunctionDefinition function = new FunctionDefinition((string) null, this.args.Revert(), body);
      function.IsGenerator = _ast._containsYield;
      _ast._containsYield = false;
      return (IronPython.Compiler.Ast.Expression) new LambdaExpression(function);
    }

    public _ast.arguments args
    {
      get => this._args;
      set => this._args = value;
    }

    public _ast.expr body
    {
      get => this._body;
      set => this._body = value;
    }
  }

  [PythonType]
  public class List : _ast.expr
  {
    private IronPython.Runtime.List _elts;
    private _ast.expr_context _ctx;

    public List()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (elts),
        nameof (ctx)
      });
    }

    public List(IronPython.Runtime.List elts, _ast.expr_context ctx, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elts = elts;
      this._ctx = ctx;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal List(ListExpression list, _ast.expr_context ctx)
      : this()
    {
      this._elts = PythonOps.MakeEmptyList(list.Items.Count);
      foreach (IronPython.Compiler.Ast.Expression expr in (IEnumerable<IronPython.Compiler.Ast.Expression>) list.Items)
        this._elts.Add((object) _ast.AST.Convert(expr, ctx));
      this._ctx = ctx;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression[] expressionArray = new IronPython.Compiler.Ast.Expression[this.elts.Count];
      int num = 0;
      foreach (_ast.expr elt in this.elts)
        expressionArray[num++] = _ast.expr.Revert(elt);
      return (IronPython.Compiler.Ast.Expression) new ListExpression(expressionArray);
    }

    public IronPython.Runtime.List elts
    {
      get => this._elts;
      set => this._elts = value;
    }

    public _ast.expr_context ctx
    {
      get => this._ctx;
      set => this._ctx = value;
    }
  }

  [PythonType]
  public class ListComp : _ast.expr
  {
    private _ast.expr _elt;
    private IronPython.Runtime.List _generators;

    public ListComp()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (elt),
        nameof (generators)
      });
    }

    public ListComp(_ast.expr elt, IronPython.Runtime.List generators, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elt = elt;
      this._generators = generators;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal ListComp(ListComprehension comp)
      : this()
    {
      this._elt = _ast.AST.Convert(comp.Item);
      this._generators = _ast.AST.Convert(comp.Iterators);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new ListComprehension(_ast.expr.Revert(this.elt), _ast.comprehension.RevertComprehensions(this.generators));
    }

    public _ast.expr elt
    {
      get => this._elt;
      set => this._elt = value;
    }

    public IronPython.Runtime.List generators
    {
      get => this._generators;
      set => this._generators = value;
    }
  }

  [PythonType]
  public class Load : _ast.expr_context
  {
    internal static _ast.Load Instance = new _ast.Load();
  }

  [PythonType]
  public class Lt : _ast.cmpop
  {
    internal static readonly _ast.Lt Instance = new _ast.Lt();

    internal override PythonOperator Revert() => PythonOperator.LessThan;
  }

  [PythonType]
  public class LtE : _ast.cmpop
  {
    internal static readonly _ast.LtE Instance = new _ast.LtE();

    internal override PythonOperator Revert() => PythonOperator.LessThanOrEqual;
  }

  [PythonType]
  public class LShift : _ast.@operator
  {
    internal static readonly _ast.LShift Instance = new _ast.LShift();

    internal override PythonOperator Revert() => PythonOperator.LeftShift;
  }

  [PythonType]
  public class Mod : _ast.@operator
  {
    internal static readonly _ast.Mod Instance = new _ast.Mod();

    internal override PythonOperator Revert() => PythonOperator.Mod;
  }

  [PythonType]
  public class Module : _ast.mod
  {
    private IronPython.Runtime.List _body;

    public Module()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (body)
      });
    }

    public Module(IronPython.Runtime.List body)
      : this()
    {
      this._body = body;
    }

    internal Module(SuiteStatement suite)
      : this()
    {
      this._body = _ast.AST.ConvertStatements((Statement) suite);
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    internal override IronPython.Runtime.List GetStatements() => this._body;
  }

  [PythonType]
  public class Mult : _ast.@operator
  {
    internal static readonly _ast.Mult Instance = new _ast.Mult();

    internal override PythonOperator Revert() => PythonOperator.Multiply;
  }

  [PythonType]
  public class Name : _ast.expr
  {
    private string _id;
    private _ast.expr_context _ctx;

    public Name()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (id),
        nameof (ctx)
      });
    }

    public Name(string id, _ast.expr_context ctx, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._id = id;
      this._ctx = ctx;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    public Name(string id, _ast.expr_context ctx)
      : this(id, ctx, new int?(), new int?())
    {
    }

    internal Name(Parameter para)
      : this(para.Name, (_ast.expr_context) _ast.Param.Instance)
    {
      this.GetSourceLocation((IronPython.Compiler.Ast.Node) para);
    }

    internal Name(NameExpression expr, _ast.expr_context ctx)
      : this(expr.Name, ctx)
    {
      this.GetSourceLocation((IronPython.Compiler.Ast.Node) expr);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new NameExpression(this.id);
    }

    public _ast.expr_context ctx
    {
      get => this._ctx;
      set => this._ctx = value;
    }

    public string id
    {
      get => this._id;
      set => this._id = value;
    }
  }

  [PythonType]
  public class Not : _ast.unaryop
  {
    internal static readonly _ast.Not Instance = new _ast.Not();

    internal override PythonOperator Revert() => PythonOperator.Not;
  }

  [PythonType]
  public class NotEq : _ast.cmpop
  {
    internal static readonly _ast.NotEq Instance = new _ast.NotEq();

    internal override PythonOperator Revert() => PythonOperator.NotEqual;
  }

  [PythonType]
  public class NotIn : _ast.cmpop
  {
    internal static readonly _ast.NotIn Instance = new _ast.NotIn();

    internal override PythonOperator Revert() => PythonOperator.NotIn;
  }

  [PythonType]
  public class Num : _ast.expr
  {
    private object _n;

    public Num()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (n)
      });
    }

    internal Num(object n)
      : this(n, new int?(), new int?())
    {
    }

    public Num(object n, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._n = n;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new ConstantExpression(this.n);
    }

    public object n
    {
      get => this._n;
      set => this._n = value;
    }
  }

  [PythonType]
  public class Or : _ast.boolop
  {
    internal static readonly _ast.Or Instance = new _ast.Or();
  }

  [PythonType]
  public class Param : _ast.expr_context
  {
    internal static _ast.Param Instance = new _ast.Param();
  }

  [PythonType]
  public class Pass : _ast.stmt
  {
    internal static _ast.Pass Instance = new _ast.Pass();

    internal Pass()
      : this(new int?(), new int?())
    {
    }

    public Pass([Optional] int? lineno, [Optional] int? col_offset)
    {
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override Statement Revert() => (Statement) new EmptyStatement();
  }

  [PythonType]
  public class Pow : _ast.@operator
  {
    internal static readonly _ast.Pow Instance = new _ast.Pow();

    internal override PythonOperator Revert() => PythonOperator.Power;
  }

  [PythonType]
  public class Print : _ast.stmt
  {
    private _ast.expr _dest;
    private IronPython.Runtime.List _values;
    private bool _nl;

    public Print()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (dest),
        nameof (values),
        nameof (nl)
      });
    }

    public Print([Optional] _ast.expr dest, IronPython.Runtime.List values, bool nl, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._dest = dest;
      this._values = values;
      this._nl = nl;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Print(PrintStatement stmt)
      : this()
    {
      if (stmt.Destination != null)
        this._dest = _ast.AST.Convert(stmt.Destination);
      this._values = PythonOps.MakeEmptyList(stmt.Expressions.Count);
      foreach (IronPython.Compiler.Ast.Expression expression in (IEnumerable<IronPython.Compiler.Ast.Expression>) stmt.Expressions)
        this._values.Add((object) _ast.AST.Convert(expression));
      this._nl = !stmt.TrailingComma;
    }

    internal override Statement Revert()
    {
      return (Statement) new PrintStatement(_ast.expr.Revert(this.dest), _ast.expr.RevertExprs(this.values), !this.nl);
    }

    public _ast.expr dest
    {
      get => this._dest;
      set => this._dest = value;
    }

    public IronPython.Runtime.List values
    {
      get => this._values;
      set => this._values = value;
    }

    public bool nl
    {
      get => this._nl;
      set => this._nl = value;
    }
  }

  [PythonType]
  public class Raise : _ast.stmt
  {
    private _ast.expr _type;
    private _ast.expr _inst;
    private _ast.expr _tback;

    public Raise()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (type),
        nameof (inst),
        nameof (tback)
      });
    }

    public Raise([Optional] _ast.expr type, [Optional] _ast.expr inst, [Optional] _ast.expr tback, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._type = type;
      this._inst = inst;
      this._tback = tback;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Raise(RaiseStatement stmt)
      : this()
    {
      if (stmt.ExceptType != null)
        this._type = _ast.AST.Convert(stmt.ExceptType);
      if (stmt.Value != null)
        this._inst = _ast.AST.Convert(stmt.Value);
      if (stmt.Traceback == null)
        return;
      this._tback = _ast.AST.Convert(stmt.Traceback);
    }

    internal override Statement Revert()
    {
      return (Statement) new RaiseStatement(_ast.expr.Revert(this.type), _ast.expr.Revert(this.inst), _ast.expr.Revert(this.tback));
    }

    public _ast.expr type
    {
      get => this._type;
      set => this._type = value;
    }

    public _ast.expr inst
    {
      get => this._inst;
      set => this._inst = value;
    }

    public _ast.expr tback
    {
      get => this._tback;
      set => this._tback = value;
    }
  }

  [PythonType]
  public class Repr : _ast.expr
  {
    private _ast.expr _value;

    public Repr()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (value)
      });
    }

    public Repr(_ast.expr value, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Repr(BackQuoteExpression expr)
      : this()
    {
      this._value = _ast.AST.Convert(expr.Expression);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new BackQuoteExpression(_ast.expr.Revert(this.value));
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class Return : _ast.stmt
  {
    private _ast.expr _value;

    public Return()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (value)
      });
    }

    public Return([Optional] _ast.expr value, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    public Return(ReturnStatement statement)
      : this()
    {
      if (statement.Expression == null)
        this._value = (_ast.expr) null;
      else
        this._value = _ast.AST.Convert(statement.Expression);
    }

    internal override Statement Revert()
    {
      return (Statement) new ReturnStatement(_ast.expr.Revert(this.value));
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }

  [PythonType]
  public class RShift : _ast.@operator
  {
    internal static readonly _ast.RShift Instance = new _ast.RShift();

    internal override PythonOperator Revert() => PythonOperator.RightShift;
  }

  [PythonType]
  public class Set : _ast.expr
  {
    private IronPython.Runtime.List _elts;

    public Set()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (elts)
      });
    }

    public Set(IronPython.Runtime.List elts, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elts = elts;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Set(SetExpression setExpression)
      : this()
    {
      this._elts = new IronPython.Runtime.List(setExpression.Items.Count);
      foreach (IronPython.Compiler.Ast.Expression expr in (IEnumerable<IronPython.Compiler.Ast.Expression>) setExpression.Items)
        this._elts.Add((object) _ast.AST.Convert(expr));
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression[] expressionArray = new IronPython.Compiler.Ast.Expression[this.elts.Count];
      int num = 0;
      foreach (_ast.expr elt in this.elts)
        expressionArray[num++] = _ast.expr.Revert(elt);
      return (IronPython.Compiler.Ast.Expression) new SetExpression(expressionArray);
    }

    public IronPython.Runtime.List elts
    {
      get => this._elts;
      set => this._elts = value;
    }
  }

  [PythonType]
  public class SetComp : _ast.expr
  {
    private _ast.expr _elt;
    private IronPython.Runtime.List _generators;

    public SetComp()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (elt),
        nameof (generators)
      });
    }

    public SetComp(_ast.expr elt, IronPython.Runtime.List generators, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elt = elt;
      this._generators = generators;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal SetComp(SetComprehension comp)
      : this()
    {
      this._elt = _ast.AST.Convert(comp.Item);
      this._generators = _ast.AST.Convert(comp.Iterators);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new SetComprehension(_ast.expr.Revert(this.elt), _ast.comprehension.RevertComprehensions(this.generators));
    }

    public _ast.expr elt
    {
      get => this._elt;
      set => this._elt = value;
    }

    public IronPython.Runtime.List generators
    {
      get => this._generators;
      set => this._generators = value;
    }
  }

  [PythonType]
  public class Slice : _ast.slice
  {
    private _ast.expr _lower;
    private _ast.expr _upper;
    private _ast.expr _step;

    public Slice()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (lower),
        nameof (upper),
        nameof (step)
      });
    }

    public Slice([Optional] _ast.expr lower, [Optional] _ast.expr upper, [Optional] _ast.expr step)
      : this()
    {
      this._lower = lower;
      this._upper = upper;
      this._step = step;
    }

    internal Slice(SliceExpression expr)
      : this()
    {
      if (expr.SliceStart != null)
        this._lower = _ast.AST.Convert(expr.SliceStart);
      if (expr.SliceStop != null)
        this._upper = _ast.AST.Convert(expr.SliceStop);
      if (!expr.StepProvided)
        return;
      if (expr.SliceStep != null)
        this._step = _ast.AST.Convert(expr.SliceStep);
      else
        this._step = (_ast.expr) new _ast.Name("None", (_ast.expr_context) _ast.Load.Instance);
    }

    public _ast.expr lower
    {
      get => this._lower;
      set => this._lower = value;
    }

    public _ast.expr upper
    {
      get => this._upper;
      set => this._upper = value;
    }

    public _ast.expr step
    {
      get => this._step;
      set => this._step = value;
    }
  }

  [PythonType]
  public class Store : _ast.expr_context
  {
    internal static _ast.Store Instance = new _ast.Store();
  }

  [PythonType]
  public class Str : _ast.expr
  {
    private string _s;

    public Str()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (s)
      });
    }

    internal Str(string s)
      : this(s, new int?(), new int?())
    {
    }

    public Str(string s, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._s = s;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new ConstantExpression((object) this.s);
    }

    public string s
    {
      get => this._s;
      set => this._s = value;
    }
  }

  [PythonType]
  public class Sub : _ast.@operator
  {
    internal static readonly _ast.Sub Instance = new _ast.Sub();

    internal override PythonOperator Revert() => PythonOperator.Subtract;
  }

  [PythonType]
  public class Subscript : _ast.expr
  {
    private _ast.expr _value;
    private _ast.slice _slice;
    private _ast.expr_context _ctx;

    public Subscript()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (value),
        nameof (slice),
        nameof (ctx)
      });
    }

    public Subscript(
      _ast.expr value,
      _ast.slice slice,
      _ast.expr_context ctx,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._slice = slice;
      this._ctx = ctx;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Subscript(IndexExpression expr, _ast.expr_context ctx)
      : this()
    {
      this._value = _ast.AST.Convert(expr.Target);
      this._ctx = ctx;
      this._slice = _ast.AST.TrySliceConvert(expr.Index);
      if (this._slice != null)
        return;
      this._slice = (_ast.slice) new _ast.Index(_ast.AST.Convert(expr.Index));
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression index = (IronPython.Compiler.Ast.Expression) null;
      if (this.slice is _ast.Index)
        index = _ast.expr.Revert(((_ast.Index) this.slice).value);
      else if (this.slice is _ast.Slice)
      {
        _ast.Slice slice = (_ast.Slice) this.slice;
        IronPython.Compiler.Ast.Expression start = (IronPython.Compiler.Ast.Expression) null;
        if (slice.lower != null)
          start = _ast.expr.Revert(slice.lower);
        IronPython.Compiler.Ast.Expression stop = (IronPython.Compiler.Ast.Expression) null;
        if (slice.upper != null)
          stop = _ast.expr.Revert(slice.upper);
        IronPython.Compiler.Ast.Expression step = (IronPython.Compiler.Ast.Expression) null;
        bool stepProvided = false;
        if (slice.step != null)
        {
          stepProvided = true;
          if (!(slice.step is _ast.Name) || !(((_ast.Name) slice.step).id == "None"))
            step = _ast.expr.Revert(slice.step);
        }
        index = (IronPython.Compiler.Ast.Expression) new SliceExpression(start, stop, step, stepProvided);
      }
      else if (this.slice is _ast.Ellipsis)
        index = (IronPython.Compiler.Ast.Expression) new ConstantExpression((object) PythonOps.Ellipsis);
      else if (this.slice is _ast.ExtSlice)
        index = (IronPython.Compiler.Ast.Expression) new TupleExpression(true, ((_ast.ExtSlice) this.slice).Revert());
      return (IronPython.Compiler.Ast.Expression) new IndexExpression(_ast.expr.Revert(this.value), index);
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }

    public _ast.slice slice
    {
      get => this._slice;
      set => this._slice = value;
    }

    public _ast.expr_context ctx
    {
      get => this._ctx;
      set => this._ctx = value;
    }
  }

  [PythonType]
  public class Suite : _ast.mod
  {
    private IronPython.Runtime.List _body;

    public Suite()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (body)
      });
    }

    public Suite(IronPython.Runtime.List body)
      : this()
    {
      this._body = body;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    internal override IronPython.Runtime.List GetStatements() => this._body;
  }

  [PythonType]
  public class TryExcept : _ast.stmt
  {
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _handlers;
    private IronPython.Runtime.List _orelse;

    public TryExcept()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (body),
        nameof (handlers),
        nameof (orelse)
      });
    }

    public TryExcept(IronPython.Runtime.List body, IronPython.Runtime.List handlers, [Optional] IronPython.Runtime.List orelse, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._body = body;
      this._handlers = handlers;
      this._orelse = orelse != null ? orelse : new IronPython.Runtime.List();
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal TryExcept(TryStatement stmt)
      : this()
    {
      this._body = _ast.AST.ConvertStatements(stmt.Body);
      this._handlers = PythonOps.MakeEmptyList(stmt.Handlers.Count);
      foreach (IronPython.Compiler.Ast.Node handler in (IEnumerable<TryStatementHandler>) stmt.Handlers)
        this._handlers.Add((object) _ast.AST.Convert(handler));
      this._orelse = _ast.AST.ConvertStatements(stmt.Else, true);
    }

    internal override Statement Revert()
    {
      TryStatementHandler[] handlers = new TryStatementHandler[this.handlers.Count];
      for (int index = 0; index < this.handlers.Count; ++index)
        handlers[index] = ((_ast.ExceptHandler) this.handlers[index]).RevertHandler();
      return (Statement) new TryStatement(_ast.stmt.RevertStmts(this.body), handlers, _ast.stmt.RevertStmts(this.orelse), (Statement) null);
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List handlers
    {
      get => this._handlers;
      set => this._handlers = value;
    }

    public IronPython.Runtime.List orelse
    {
      get => this._orelse;
      set => this._orelse = value;
    }
  }

  [PythonType]
  public class TryFinally : _ast.stmt
  {
    public TryFinally()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (body),
        nameof (finalbody)
      });
    }

    public TryFinally(IronPython.Runtime.List body, IronPython.Runtime.List finalbody, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this.body = body;
      this.finalbody = finalbody;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal TryFinally(IronPython.Runtime.List body, IronPython.Runtime.List finalbody)
      : this()
    {
      this.body = body;
      this.finalbody = finalbody;
    }

    internal override Statement Revert()
    {
      if (this.body.Count != 1 || !(this.body[0] is _ast.TryExcept))
        return (Statement) new TryStatement(_ast.stmt.RevertStmts(this.body), (TryStatementHandler[]) null, (Statement) null, _ast.stmt.RevertStmts(this.finalbody));
      _ast.TryExcept tryExcept = (_ast.TryExcept) this.body[0];
      TryStatementHandler[] handlers = new TryStatementHandler[tryExcept.handlers.Count];
      for (int index = 0; index < tryExcept.handlers.Count; ++index)
        handlers[index] = ((_ast.ExceptHandler) tryExcept.handlers[index]).RevertHandler();
      return (Statement) new TryStatement(_ast.stmt.RevertStmts(tryExcept.body), handlers, _ast.stmt.RevertStmts(tryExcept.orelse), _ast.stmt.RevertStmts(this.finalbody));
    }

    public IronPython.Runtime.List body { get; set; }

    public IronPython.Runtime.List finalbody { get; set; }
  }

  [PythonType]
  public class Tuple : _ast.expr
  {
    private IronPython.Runtime.List _elts;
    private _ast.expr_context _ctx;

    public Tuple()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (elts),
        nameof (ctx)
      });
    }

    public Tuple(IronPython.Runtime.List elts, _ast.expr_context ctx, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._elts = elts;
      this._ctx = ctx;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Tuple(TupleExpression list, _ast.expr_context ctx)
      : this()
    {
      this._elts = PythonOps.MakeEmptyList(list.Items.Count);
      foreach (IronPython.Compiler.Ast.Expression expr in (IEnumerable<IronPython.Compiler.Ast.Expression>) list.Items)
        this._elts.Add((object) _ast.AST.Convert(expr, ctx));
      this._ctx = ctx;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      IronPython.Compiler.Ast.Expression[] expressionArray = new IronPython.Compiler.Ast.Expression[this.elts.Count];
      int num = 0;
      foreach (_ast.expr elt in this.elts)
        expressionArray[num++] = _ast.expr.Revert(elt);
      return (IronPython.Compiler.Ast.Expression) new TupleExpression(false, expressionArray);
    }

    public IronPython.Runtime.List elts
    {
      get => this._elts;
      set => this._elts = value;
    }

    public _ast.expr_context ctx
    {
      get => this._ctx;
      set => this._ctx = value;
    }
  }

  [PythonType]
  public class UnaryOp : _ast.expr
  {
    public UnaryOp()
    {
      this._fields = new PythonTuple((object) new string[2]
      {
        nameof (op),
        nameof (operand)
      });
    }

    internal UnaryOp(UnaryExpression expression)
      : this()
    {
      this.op = (_ast.unaryop) _ast.AST.Convert(expression.Op);
      this.operand = _ast.AST.Convert(expression.Expression);
    }

    public UnaryOp(_ast.unaryop op, _ast.expr operand, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this.op = op;
      this.operand = operand;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      return (IronPython.Compiler.Ast.Expression) new UnaryExpression(this.op.Revert(), _ast.expr.Revert(this.operand));
    }

    public _ast.unaryop op { get; set; }

    public _ast.expr operand { get; set; }

    internal _ast.expr TryTrimTrivialUnaryOp()
    {
      if (!(this.operand is _ast.Num operand))
        return (_ast.expr) this;
      if (this.op is _ast.UAdd)
        return (_ast.expr) operand;
      if (!(this.op is _ast.USub))
        return (_ast.expr) this;
      if (operand.n is int)
        operand.n = (object) -(int) operand.n;
      else if (operand.n is double)
        operand.n = (object) -(double) operand.n;
      else if (operand.n is long)
        operand.n = (object) -(long) operand.n;
      else if (operand.n is BigInteger)
      {
        operand.n = (object) -(BigInteger) operand.n;
      }
      else
      {
        if (!(operand.n is Complex))
          return (_ast.expr) this;
        operand.n = (object) -(Complex) operand.n;
      }
      return (_ast.expr) operand;
    }
  }

  [PythonType]
  public class UAdd : _ast.unaryop
  {
    internal static _ast.UAdd Instance = new _ast.UAdd();

    internal override PythonOperator Revert() => PythonOperator.Pos;
  }

  [PythonType]
  public class USub : _ast.unaryop
  {
    internal static _ast.USub Instance = new _ast.USub();

    internal override PythonOperator Revert() => PythonOperator.Negate;
  }

  [PythonType]
  public class While : _ast.stmt
  {
    private _ast.expr _test;
    private IronPython.Runtime.List _body;
    private IronPython.Runtime.List _orelse;

    public While()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (test),
        nameof (body),
        nameof (orelse)
      });
    }

    public While(_ast.expr test, IronPython.Runtime.List body, [Optional] IronPython.Runtime.List orelse, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._test = test;
      this._body = body;
      this._orelse = orelse != null ? orelse : new IronPython.Runtime.List();
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal While(WhileStatement stmt)
      : this()
    {
      this._test = _ast.AST.Convert(stmt.Test);
      this._body = _ast.AST.ConvertStatements(stmt.Body);
      this._orelse = _ast.AST.ConvertStatements(stmt.ElseStatement, true);
    }

    internal override Statement Revert()
    {
      return (Statement) new WhileStatement(_ast.expr.Revert(this.test), _ast.stmt.RevertStmts(this.body), _ast.stmt.RevertStmts(this.orelse));
    }

    public _ast.expr test
    {
      get => this._test;
      set => this._test = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }

    public IronPython.Runtime.List orelse
    {
      get => this._orelse;
      set => this._orelse = value;
    }
  }

  [PythonType]
  public class With : _ast.stmt
  {
    private _ast.expr _context_expr;
    private _ast.expr _optional_vars;
    private IronPython.Runtime.List _body;

    public With()
    {
      this._fields = new PythonTuple((object) new string[3]
      {
        nameof (context_expr),
        nameof (optional_vars),
        nameof (body)
      });
    }

    public With(
      _ast.expr context_expr,
      [Optional] _ast.expr optional_vars,
      IronPython.Runtime.List body,
      [Optional] int? lineno,
      [Optional] int? col_offset)
      : this()
    {
      this._context_expr = context_expr;
      this._optional_vars = optional_vars;
      this._body = body;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal With(WithStatement with)
      : this()
    {
      this._context_expr = _ast.AST.Convert(with.ContextManager);
      if (with.Variable != null)
        this._optional_vars = _ast.AST.Convert(with.Variable);
      this._body = _ast.AST.ConvertStatements(with.Body);
    }

    internal override Statement Revert()
    {
      return (Statement) new WithStatement(_ast.expr.Revert(this.context_expr), _ast.expr.Revert(this.optional_vars), _ast.stmt.RevertStmts(this.body));
    }

    public _ast.expr context_expr
    {
      get => this._context_expr;
      set => this._context_expr = value;
    }

    public _ast.expr optional_vars
    {
      get => this._optional_vars;
      set => this._optional_vars = value;
    }

    public IronPython.Runtime.List body
    {
      get => this._body;
      set => this._body = value;
    }
  }

  [PythonType]
  public class Yield : _ast.expr
  {
    private _ast.expr _value;

    public Yield()
    {
      this._fields = new PythonTuple((object) new string[1]
      {
        nameof (value)
      });
    }

    public Yield([Optional] _ast.expr value, [Optional] int? lineno, [Optional] int? col_offset)
      : this()
    {
      this._value = value;
      this._lineno = lineno;
      this._col_offset = col_offset;
    }

    internal Yield(YieldExpression expr)
      : this()
    {
      this._value = _ast.AST.Convert(expr.Expression);
    }

    internal override IronPython.Compiler.Ast.Expression Revert()
    {
      _ast._containsYield = true;
      return (IronPython.Compiler.Ast.Expression) new YieldExpression(_ast.expr.Revert(this.value));
    }

    public _ast.expr value
    {
      get => this._value;
      set => this._value = value;
    }
  }
}
