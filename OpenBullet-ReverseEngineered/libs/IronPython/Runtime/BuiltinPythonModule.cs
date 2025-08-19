// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.BuiltinPythonModule
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

public class BuiltinPythonModule
{
  private readonly PythonContext _context;
  private CodeContext _codeContext;

  protected BuiltinPythonModule(PythonContext context)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    this._context = context;
  }

  protected internal virtual void Initialize(
    CodeContext codeContext,
    Dictionary<string, PythonGlobal> optimizedGlobals)
  {
    ContractUtils.RequiresNotNull((object) codeContext, nameof (codeContext));
    ContractUtils.RequiresNotNull((object) optimizedGlobals, "globals");
    this._codeContext = codeContext;
    this.PerformModuleReload();
  }

  protected internal virtual IEnumerable<string> GetGlobalVariableNames()
  {
    return (IEnumerable<string>) ArrayUtils.EmptyStrings;
  }

  protected internal virtual void PerformModuleReload()
  {
  }

  protected PythonContext Context => this._context;

  protected CodeContext Globals => this._codeContext;
}
