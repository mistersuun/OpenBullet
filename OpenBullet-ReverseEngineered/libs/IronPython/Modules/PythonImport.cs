// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonImport
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonImport
{
  public const string __doc__ = "Provides functions for programmatically creating and importing modules and packages.";
  internal const int PythonSource = 1;
  internal const int PythonCompiled = 2;
  internal const int CExtension = 3;
  internal const int PythonResource = 4;
  internal const int PackageDirectory = 5;
  internal const int CBuiltin = 6;
  internal const int PythonFrozen = 7;
  internal const int PythonCodeResource = 8;
  internal const int SearchError = 0;
  internal const int ImporterHook = 9;
  private static readonly object _lockCountKey = new object();
  public const int PY_SOURCE = 1;
  public const int PY_COMPILED = 2;
  public const int C_EXTENSION = 3;
  public const int PY_RESOURCE = 4;
  public const int PKG_DIRECTORY = 5;
  public const int C_BUILTIN = 6;
  public const int PY_FROZEN = 7;
  public const int PY_CODERESOURCE = 8;
  public const int SEARCH_ERROR = 0;
  public const int IMP_HOOK = 9;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (context.HasModuleState(PythonImport._lockCountKey))
      return;
    context.SetModuleState(PythonImport._lockCountKey, (object) 0L);
  }

  public static string get_magic() => "";

  public static IronPython.Runtime.List get_suffixes()
  {
    return IronPython.Runtime.List.FromArrayNoCopy((object) PythonOps.MakeTuple((object) ".py", (object) "U", (object) 1));
  }

  public static PythonTuple find_module(CodeContext context, string name)
  {
    return name != null ? PythonImport.FindBuiltinOrSysPath(context, name) : throw PythonOps.TypeError("find_module() argument 1 must be string, not None");
  }

  public static PythonTuple find_module(CodeContext context, string name, IronPython.Runtime.List path)
  {
    if (name == null)
      throw PythonOps.TypeError("find_module() argument 1 must be string, not None");
    return path == null ? PythonImport.FindBuiltinOrSysPath(context, name) : PythonImport.FindModulePath(context, name, path);
  }

  public static object load_module(
    CodeContext context,
    string name,
    PythonFile file,
    string filename,
    PythonTuple description)
  {
    if (description == null)
      throw PythonOps.TypeError("load_module() argument 4 must be 3-item sequence, not None");
    if (description.__len__() != 3)
      throw PythonOps.TypeError("load_module() argument 4 must be sequence of length 3, not {0}", (object) description.__len__());
    PythonContext languageContext = context.LanguageContext;
    PythonModule moduleByName = languageContext.GetModuleByName(name);
    if (moduleByName != null)
    {
      Importer.ReloadModule(context, moduleByName, file);
      return (object) moduleByName;
    }
    int int32 = context.LanguageContext.ConvertToInt32(description[2]);
    switch (int32)
    {
      case 1:
        return (object) PythonImport.LoadPythonSource(languageContext, name, file, filename);
      case 5:
        return (object) PythonImport.LoadPackageDirectory(languageContext, name, filename);
      case 6:
        return PythonImport.LoadBuiltinModule(context, name);
      default:
        throw PythonOps.TypeError("don't know how to import {0}, (type code {1}", (object) name, (object) int32);
    }
  }

  [Documentation("new_module(name) -> module\nCreates a new module without adding it to sys.modules.")]
  public static PythonModule new_module(CodeContext context, string name)
  {
    return name != null ? new PythonModule()
    {
      __dict__ = {
        [(object) "__name__"] = (object) name,
        [(object) "__doc__"] = (object) null,
        [(object) "__package__"] = (object) null
      }
    } : throw PythonOps.TypeError("new_module() argument 1 must be string, not None");
  }

  public static bool lock_held(CodeContext context) => PythonImport.GetLockCount(context) != 0L;

  public static void acquire_lock(CodeContext context)
  {
    lock (PythonImport._lockCountKey)
      PythonImport.SetLockCount(context, PythonImport.GetLockCount(context) + 1L);
  }

  public static void release_lock(CodeContext context)
  {
    lock (PythonImport._lockCountKey)
    {
      long lockCount = PythonImport.GetLockCount(context);
      if (lockCount == 0L)
        throw PythonOps.RuntimeError("not holding the import lock");
      PythonImport.SetLockCount(context, lockCount - 1L);
    }
  }

  public static object init_builtin(CodeContext context, string name)
  {
    return name != null ? PythonImport.LoadBuiltinModule(context, name) : throw PythonOps.TypeError("init_builtin() argument 1 must be string, not None");
  }

  public static object init_frozen(string name) => (object) null;

  public static object get_frozen_object(string name)
  {
    throw PythonOps.ImportError("No such frozen object named {0}", (object) name);
  }

  public static int is_builtin(CodeContext context, string name)
  {
    if (name == null)
      throw PythonOps.TypeError("is_builtin() argument 1 must be string, not None");
    Type type;
    if (!context.LanguageContext.BuiltinModules.TryGetValue(name, out type))
      return 0;
    return type.Assembly == typeof (PythonContext).Assembly ? -1 : 1;
  }

  public static bool is_frozen(string name) => false;

  public static object load_compiled(string name, string pathname) => (object) null;

  public static object load_compiled(string name, string pathname, PythonFile file)
  {
    return (object) null;
  }

  public static object load_dynamic(string name, string pathname) => (object) null;

  public static object load_dynamic(string name, string pathname, PythonFile file) => (object) null;

  public static object load_package(CodeContext context, string name, string pathname)
  {
    if (name == null)
      throw PythonOps.TypeError("load_package() argument 1 must be string, not None");
    if (pathname == null)
      throw PythonOps.TypeError("load_package() argument 2 must be string, not None");
    return (object) Importer.LoadPackageFromSource(context, name, pathname) ?? (object) PythonImport.CreateEmptyPackage(context, name, pathname);
  }

  private static PythonModule CreateEmptyPackage(CodeContext context, string name, string pathname)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonModule emptyPackage = new PythonModule();
    emptyPackage.__dict__[(object) "__name__"] = (object) name;
    emptyPackage.__dict__[(object) "__path__"] = (object) pathname;
    languageContext.SystemStateModules[(object) name] = (object) emptyPackage;
    return emptyPackage;
  }

  public static object load_source(CodeContext context, string name, string pathname)
  {
    if (name == null)
      throw PythonOps.TypeError("load_source() argument 1 must be string, not None");
    if (pathname == null)
      throw PythonOps.TypeError("load_source() argument 2 must be string, not None");
    PythonContext languageContext = context.LanguageContext;
    if (!languageContext.DomainManager.Platform.FileExists(pathname))
      throw PythonOps.IOError("Couldn't find file: {0}", (object) pathname);
    SourceUnit fileUnit = languageContext.CreateFileUnit(pathname, languageContext.DefaultEncoding, SourceCodeKind.File);
    return (object) languageContext.CompileModule(pathname, name, fileUnit, ModuleOptions.Initialize);
  }

  public static object load_source(
    CodeContext context,
    string name,
    string pathname,
    PythonFile file)
  {
    if (name == null)
      throw PythonOps.TypeError("load_source() argument 1 must be string, not None");
    if (pathname == null)
      throw PythonOps.TypeError("load_source() argument 2 must be string, not None");
    if (file == null)
      throw PythonOps.TypeError("load_source() argument 3 must be file, not None");
    return (object) PythonImport.LoadPythonSource(context.LanguageContext, name, file, pathname);
  }

  public static object reload(CodeContext context, PythonModule scope)
  {
    return Builtin.reload(context, scope);
  }

  private static PythonTuple FindBuiltinOrSysPath(CodeContext context, string name)
  {
    IronPython.Runtime.List path;
    if (!context.LanguageContext.TryGetSystemPath(out path))
      throw PythonOps.ImportError("sys.path must be a list of directory names");
    return PythonImport.FindModuleBuiltinOrPath(context, name, path);
  }

  private static PythonTuple FindModulePath(CodeContext context, string name, IronPython.Runtime.List path)
  {
    if (name == null)
      throw PythonOps.TypeError("find_module() argument 1 must be string, not None");
    PlatformAdaptationLayer platform = context.LanguageContext.DomainManager.Platform;
    foreach (object obj in path)
    {
      if (obj is string path1)
      {
        string str1 = Path.Combine(path1, name);
        if (platform.DirectoryExists(str1) && platform.FileExists(Path.Combine(str1, "__init__.py")))
        {
          object[] objArray = new object[3]
          {
            null,
            (object) str1,
            null
          };
          objArray[2] = (object) PythonTuple.MakeTuple((object) "", (object) "", (object) 5);
          return PythonTuple.MakeTuple(objArray);
        }
        string str2 = str1 + ".py";
        if (platform.FileExists(str2))
        {
          Stream stream = platform.OpenInputFileStream(str2, share: FileShare.ReadWrite);
          return PythonTuple.MakeTuple((object) PythonFile.Create(context, stream, str2, "U"), (object) str2, (object) PythonTuple.MakeTuple((object) ".py", (object) "U", (object) 1));
        }
      }
    }
    throw PythonOps.ImportError("No module named {0}", (object) name);
  }

  private static PythonTuple FindModuleBuiltinOrPath(CodeContext context, string name, IronPython.Runtime.List path)
  {
    switch (name)
    {
      case "sys":
        return PythonImport.BuiltinModuleTuple(name);
      case "clr":
        context.ShowCls = true;
        return PythonImport.BuiltinModuleTuple(name);
      default:
        return context.LanguageContext.BuiltinModules.TryGetValue(name, out Type _) ? PythonImport.BuiltinModuleTuple(name) : PythonImport.FindModulePath(context, name, path);
    }
  }

  private static PythonTuple BuiltinModuleTuple(string name)
  {
    object[] objArray = new object[3]
    {
      null,
      (object) name,
      null
    };
    objArray[2] = (object) PythonTuple.MakeTuple((object) "", (object) "", (object) 6);
    return PythonTuple.MakeTuple(objArray);
  }

  private static PythonModule LoadPythonSource(
    PythonContext context,
    string name,
    PythonFile file,
    string fileName)
  {
    SourceUnit snippet = context.CreateSnippet(file.read(), string.IsNullOrEmpty(fileName) ? (string) null : fileName, SourceCodeKind.File);
    return context.CompileModule(fileName, name, snippet, ModuleOptions.Initialize);
  }

  private static PythonModule LoadPackageDirectory(
    PythonContext context,
    string moduleName,
    string path)
  {
    string str = Path.Combine(path, "__init__.py");
    SourceUnit fileUnit = context.CreateFileUnit(str, context.DefaultEncoding);
    return context.CompileModule(str, moduleName, fileUnit, ModuleOptions.Initialize);
  }

  private static object LoadBuiltinModule(CodeContext context, string name)
  {
    return Importer.ImportBuiltin(context, name);
  }

  private static long GetLockCount(CodeContext context)
  {
    return (long) context.LanguageContext.GetModuleState(PythonImport._lockCountKey);
  }

  private static void SetLockCount(CodeContext context, long lockCount)
  {
    context.LanguageContext.SetModuleState(PythonImport._lockCountKey, (object) lockCount);
  }

  [PythonType]
  public sealed class NullImporter
  {
    public NullImporter(string path_string)
    {
    }

    public object find_module(params object[] args) => (object) null;
  }
}
