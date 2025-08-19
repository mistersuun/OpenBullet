// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.OnDiskScriptCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler;

internal class OnDiskScriptCode : RunnableScriptCode
{
  private readonly LookupCompilationDelegate _target;
  private CodeContext _optimizedContext;
  private readonly string _moduleName;

  public OnDiskScriptCode(LookupCompilationDelegate code, SourceUnit sourceUnit, string moduleName)
    : base(OnDiskScriptCode.MakeAstFromSourceUnit(sourceUnit))
  {
    this._target = code;
    this._moduleName = moduleName;
  }

  private static PythonAst MakeAstFromSourceUnit(SourceUnit sourceUnit)
  {
    return new PythonAst(new CompilerContext(sourceUnit, (CompilerOptions) new PythonCompilerOptions(), ErrorSink.Null));
  }

  public override object Run()
  {
    CodeContext context = this.CreateContext();
    try
    {
      FunctionCode code = this.EnsureFunctionCode((Delegate) this._target, false, true);
      this.PushFrame(context, code);
      return this._target(context, code);
    }
    finally
    {
      this.PopFrame();
    }
  }

  public override object Run(Scope scope)
  {
    if (scope == this.CreateScope())
      return this.Run();
    throw new NotSupportedException();
  }

  public string ModuleName => this._moduleName;

  public override FunctionCode GetFunctionCode(bool register)
  {
    return this.EnsureFunctionCode((Delegate) this._target, false, register);
  }

  public override Scope CreateScope() => this.CreateContext().GlobalScope;

  internal CodeContext CreateContext()
  {
    if (this._optimizedContext == null)
    {
      CachedOptimizedCodeAttribute customAttribute = ((CachedOptimizedCodeAttribute[]) RuntimeReflectionExtensions.GetMethodInfo((Delegate) this._target).GetCustomAttributes(typeof (CachedOptimizedCodeAttribute), false))[0];
      Dictionary<string, PythonGlobal> globals = new Dictionary<string, PythonGlobal>((IEqualityComparer<string>) StringComparer.Ordinal);
      PythonGlobal[] data = new PythonGlobal[customAttribute.Names.Length];
      PythonDictionary pythonDictionary = new PythonDictionary((DictionaryStorage) new GlobalDictionaryStorage(globals, data));
      CodeContext globalContext = new ModuleContext(pythonDictionary, (PythonContext) this.SourceUnit.LanguageContext).GlobalContext;
      for (int index = 0; index < customAttribute.Names.Length; ++index)
      {
        string name = customAttribute.Names[index];
        data[index] = globals[name] = new PythonGlobal(globalContext, name);
      }
      this._optimizedContext = RunnableScriptCode.CreateTopLevelCodeContext(pythonDictionary, this.SourceUnit.LanguageContext);
    }
    return this._optimizedContext;
  }
}
