// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonScriptCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal class PythonScriptCode(PythonAst ast) : RunnableScriptCode(ast)
{
  private CodeContext _defaultContext;
  private LookupCompilationDelegate _target;
  private LookupCompilationDelegate _tracingTarget;

  public override object Run()
  {
    return this.SourceUnit.Kind == SourceCodeKind.Expression ? this.EvalWrapper(this.DefaultContext) : this.RunWorker(this.DefaultContext);
  }

  public override object Run(Scope scope)
  {
    CodeContext contextForScope = RunnableScriptCode.GetContextForScope(scope, this.SourceUnit);
    return this.SourceUnit.Kind == SourceCodeKind.Expression ? this.EvalWrapper(contextForScope) : this.RunWorker(contextForScope);
  }

  private object RunWorker(CodeContext ctx)
  {
    LookupCompilationDelegate target = this.GetTarget(true);
    Exception clrException = PythonOps.SaveCurrentException();
    this.PushFrame(ctx, this._code);
    try
    {
      return target(ctx, this._code);
    }
    finally
    {
      PythonOps.RestoreCurrentException(clrException);
      this.PopFrame();
    }
  }

  private LookupCompilationDelegate GetTarget(bool register)
  {
    LookupCompilationDelegate target;
    if (!((PythonContext) this.Ast.CompilerContext.SourceUnit.LanguageContext).EnableTracing)
    {
      this.EnsureTarget(register);
      target = this._target;
    }
    else
    {
      this.EnsureTracingTarget();
      target = this._tracingTarget;
    }
    return target;
  }

  public override FunctionCode GetFunctionCode(bool register)
  {
    this.GetTarget(register);
    return this._code;
  }

  public override Scope CreateScope() => new Scope();

  private object EvalWrapper(CodeContext ctx)
  {
    try
    {
      return this.RunWorker(ctx);
    }
    catch (Exception ex)
    {
      CodeContext context = ctx;
      FunctionCode code = this.Code;
      PythonOps.UpdateStackTrace(ex, context, code, 0);
      throw;
    }
  }

  private LookupCompilationDelegate CompileBody(LightExpression<LookupCompilationDelegate> lambda)
  {
    PythonConstantExpression constant = PythonScriptCode.ExtractConstant(lambda);
    if (constant != null)
    {
      object value = constant.Value;
      return (LookupCompilationDelegate) ((codeCtx, functionCode) => value);
    }
    PythonContext languageContext = (PythonContext) this.Ast.CompilerContext.SourceUnit.LanguageContext;
    return !this.ShouldInterpret(languageContext) ? lambda.ReduceToLambda().Compile<LookupCompilationDelegate>(languageContext.EmitDebugSymbols(this.Ast.CompilerContext.SourceUnit)) : lambda.Compile(languageContext.Options.CompilationThreshold);
  }

  private bool ShouldInterpret(PythonContext pc)
  {
    return pc.ShouldInterpret((PythonCompilerOptions) this.Ast.CompilerContext.Options, this.Ast.CompilerContext.SourceUnit);
  }

  private static PythonConstantExpression ExtractConstant(
    LightExpression<LookupCompilationDelegate> lambda)
  {
    return !(lambda.Body is BlockExpression body) || body.Expressions.Count != 2 || !(body.Expressions[0] is DebugInfoExpression) || body.Expressions[1].NodeType != ExpressionType.Convert || !(((System.Linq.Expressions.UnaryExpression) body.Expressions[1]).Operand is PythonConstantExpression) ? (PythonConstantExpression) null : (PythonConstantExpression) ((System.Linq.Expressions.UnaryExpression) body.Expressions[1]).Operand;
  }

  private void EnsureTarget(bool register)
  {
    if (this._target != null)
      return;
    this._target = this.CompileBody((LightExpression<LookupCompilationDelegate>) this.Ast.GetLambda());
    this.EnsureFunctionCode((Delegate) this._target, false, register);
  }

  private CodeContext DefaultContext
  {
    get
    {
      if (this._defaultContext == null)
        this._defaultContext = RunnableScriptCode.CreateTopLevelCodeContext(new PythonDictionary(), this.Ast.CompilerContext.SourceUnit.LanguageContext);
      return this._defaultContext;
    }
  }

  private void EnsureTracingTarget()
  {
    if (this._tracingTarget != null)
      return;
    PythonContext languageContext = (PythonContext) this.Ast.CompilerContext.SourceUnit.LanguageContext;
    PythonDebuggingPayload customPayload = new PythonDebuggingPayload((FunctionCode) null);
    DebugLambdaInfo lambdaInfo = new DebugLambdaInfo((IDebugCompilerSupport) null, (string) null, false, (IList<ParameterExpression>) null, (IDictionary<ParameterExpression, string>) null, (object) customPayload);
    Expression<LookupCompilationDelegate> lambda = (Expression<LookupCompilationDelegate>) languageContext.DebugContext.TransformLambda((System.Linq.Expressions.LambdaExpression) this.Ast.GetLambda().Reduce(), lambdaInfo);
    this._tracingTarget = !this.ShouldInterpret(languageContext) ? lambda.Compile<LookupCompilationDelegate>(languageContext.EmitDebugSymbols(this.Ast.CompilerContext.SourceUnit)) : lambda.LightCompile<LookupCompilationDelegate>(languageContext.Options.CompilationThreshold);
    customPayload.Code = this.EnsureFunctionCode((Delegate) this._tracingTarget, true, true);
  }
}
