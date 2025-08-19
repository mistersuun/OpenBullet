// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CompiledLoader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace IronPython.Runtime;

public class CompiledLoader
{
  private Dictionary<string, OnDiskScriptCode> _codes = new Dictionary<string, OnDiskScriptCode>();

  internal void AddScriptCode(ScriptCode code)
  {
    if (!(code is OnDiskScriptCode onDiskScriptCode))
      return;
    if (onDiskScriptCode.ModuleName == "__main__")
    {
      this._codes["__main__"] = onDiskScriptCode;
    }
    else
    {
      string key = code.SourceUnit.Path.Replace(Path.DirectorySeparatorChar, '.');
      if (key.EndsWith("__init__.py"))
        key = key.Substring(0, key.Length - ".__init__.py".Length);
      this._codes[key] = onDiskScriptCode;
    }
  }

  public ModuleLoader find_module(CodeContext context, string fullname, List path)
  {
    OnDiskScriptCode sc;
    if (!this._codes.TryGetValue(fullname, out sc))
      return (ModuleLoader) null;
    int length = fullname.LastIndexOf('.');
    string name = fullname;
    string parentName = (string) null;
    if (length != -1)
    {
      parentName = fullname.Substring(0, length);
      name = fullname.Substring(length + 1);
    }
    return new ModuleLoader(sc, parentName, name);
  }
}
