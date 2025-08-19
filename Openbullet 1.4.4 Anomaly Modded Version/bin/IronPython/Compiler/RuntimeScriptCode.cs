// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.RuntimeScriptCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Threading;

#nullable disable
namespace IronPython.Compiler;

internal class RuntimeScriptCode : RunnableScriptCode
{
  private readonly CodeContext _optimizedContext;
  private Func<FunctionCode, object> _optimizedTarget;
  private ScriptCode _unoptimizedCode;

  public RuntimeScriptCode(PythonAst ast, CodeContext codeContext)
    : base(ast)
  {
    this._optimizedContext = codeContext;
  }

  public override object Run() => this.InvokeTarget(this.CreateScope());

  public override object Run(Scope scope) => this.InvokeTarget(scope);

  public override FunctionCode GetFunctionCode(bool register)
  {
    this.EnsureCompiled();
    return this.EnsureFunctionCode((Delegate) this._optimizedTarget, false, register);
  }

  private object InvokeTarget(Scope scope)
  {
    if (scope == this._optimizedContext.GlobalScope && !this._optimizedContext.LanguageContext.EnableTracing)
    {
      this.EnsureCompiled();
      Exception clrException = PythonOps.SaveCurrentException();
      FunctionCode functionCode = this.EnsureFunctionCode((Delegate) this._optimizedTarget, false, true);
      this.PushFrame(this._optimizedContext, functionCode);
      try
      {
        return this.Ast.CompilerContext.SourceUnit.Kind == SourceCodeKind.Expression ? this.OptimizedEvalWrapper(functionCode) : this._optimizedTarget(functionCode);
      }
      finally
      {
        PythonOps.RestoreCurrentException(clrException);
        this.PopFrame();
      }
    }
    else
    {
      if (this._unoptimizedCode == null)
      {
        ((PythonCompilerOptions) this.Ast.CompilerContext.Options).Optimized = false;
        Interlocked.CompareExchange<ScriptCode>(ref this._unoptimizedCode, this.Ast.MakeLookupCode().ToScriptCode(), (ScriptCode) null);
      }
      return this._unoptimizedCode.Run(scope);
    }
  }

  private object OptimizedEvalWrapper(FunctionCode funcCode)
  {
    try
    {
      return this._optimizedTarget(funcCode);
    }
    catch (Exception ex)
    {
      CodeContext optimizedContext = this._optimizedContext;
      FunctionCode code = this.Code;
      PythonOps.UpdateStackTrace(ex, optimizedContext, code, 0);
      throw;
    }
  }

  public override Scope CreateScope() => this._optimizedContext.GlobalScope;

  private void EnsureCompiled()
  {
    if (this._optimizedTarget != null)
      return;
    Interlocked.CompareExchange<Func<FunctionCode, object>>(ref this._optimizedTarget, this.Compile(), (Func<FunctionCode, object>) null);
  }

  private Func<FunctionCode, object> Compile()
  {
    PythonCompilerOptions options = (PythonCompilerOptions) this.Ast.CompilerContext.Options;
    PythonContext languageContext = (PythonContext) this.SourceUnit.LanguageContext;
    return languageContext.ShouldInterpret(options, this.SourceUnit) ? ((LightExpression<Func<FunctionCode, object>>) this.Ast.GetLambda()).Compile(languageContext.Options.CompilationThreshold) : ((LightExpression<Func<FunctionCode, object>>) this.Ast.GetLambda()).ReduceToLambda().Compile<Func<FunctionCode, object>>(languageContext.EmitDebugSymbols(this.SourceUnit));
  }
}
