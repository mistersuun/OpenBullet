// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.RunnableScriptCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace IronPython.Compiler;

internal abstract class RunnableScriptCode : ScriptCode
{
  internal FunctionCode _code;
  private readonly PythonAst _ast;

  public RunnableScriptCode(PythonAst ast)
    : base(ast.SourceUnit)
  {
    this._ast = ast;
  }

  public override object Run() => base.Run();

  public override object Run(Scope scope) => throw new NotImplementedException();

  protected static CodeContext CreateTopLevelCodeContext(
    PythonDictionary dict,
    LanguageContext context)
  {
    return new ModuleContext(dict, (PythonContext) context).GlobalContext;
  }

  protected static CodeContext GetContextForScope(Scope scope, SourceUnit sourceUnit)
  {
    if (!(scope.GetExtension(sourceUnit.LanguageContext.ContextId) is PythonScopeExtension pythonScopeExtension))
      pythonScopeExtension = sourceUnit.LanguageContext.EnsureScopeExtension(scope) as PythonScopeExtension;
    return pythonScopeExtension.ModuleContext.GlobalContext;
  }

  protected FunctionCode EnsureFunctionCode(Delegate dlg)
  {
    return this.EnsureFunctionCode(dlg, false, true);
  }

  protected FunctionCode EnsureFunctionCode(Delegate dlg, bool tracing, bool register)
  {
    if (this._code == null)
      Interlocked.CompareExchange<FunctionCode>(ref this._code, new FunctionCode((PythonContext) this.SourceUnit.LanguageContext, dlg, (ScopeStatement) this._ast, this._ast.GetDocumentation((Statement) this._ast), new bool?(tracing), register), (FunctionCode) null);
    return this._code;
  }

  public PythonAst Ast => this._ast;

  public FunctionCode Code => this._code;

  public abstract FunctionCode GetFunctionCode(bool register);

  protected void PushFrame(CodeContext context, FunctionCode code)
  {
    if (!((PythonContext) this.SourceUnit.LanguageContext).PythonOptions.Frames)
      return;
    PythonOps.PushFrame(context, code);
  }

  protected void PopFrame()
  {
    if (!((PythonContext) this.SourceUnit.LanguageContext).PythonOptions.Frames)
      return;
    List<FunctionStack> functionStack = PythonOps.GetFunctionStack();
    functionStack.RemoveAt(functionStack.Count - 1);
  }
}
