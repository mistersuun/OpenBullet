// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Importer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Modules;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

public static class Importer
{
  internal const string ModuleReloadMethod = "PerformModuleReload";

  public static object Import(CodeContext context, string fullName, PythonTuple from, int level)
  {
    return LightExceptions.CheckAndThrow(Importer.ImportLightThrow(context, fullName, from, level));
  }

  [LightThrowing]
  internal static object ImportLightThrow(
    CodeContext context,
    string fullName,
    PythonTuple from,
    int level)
  {
    PythonContext languageContext = context.LanguageContext;
    if (level == -1)
    {
      CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>> oldImportSite = languageContext.OldImportSite;
      return oldImportSite.Target((CallSite) oldImportSite, context, Importer.FindImportFunction(context), fullName, Builtin.globals(context), context.Dict, from);
    }
    CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>> importSite = languageContext.ImportSite;
    return importSite.Target((CallSite) importSite, context, Importer.FindImportFunction(context), fullName, Builtin.globals(context), context.Dict, from, level);
  }

  public static object ImportFrom(CodeContext context, object from, string name)
  {
    switch (from)
    {
      case PythonModule pythonModule:
        if (pythonModule.GetType() == typeof (PythonModule))
        {
          object obj;
          if (pythonModule.__dict__.TryGetValue((object) name, out obj))
            return obj;
        }
        else
        {
          object ret;
          if (PythonOps.TryGetBoundAttr(context, (object) pythonModule, name, out ret))
            return ret;
        }
        object obj1;
        if (pythonModule.__dict__._storage.TryGetPath(out obj1))
        {
          switch (obj1)
          {
            case List path:
              return Importer.ImportNestedModule(context, pythonModule, new string[1]
              {
                name
              }, 0, path);
            case string str:
              return Importer.ImportNestedModule(context, pythonModule, new string[1]
              {
                name
              }, 0, List.FromArrayNoCopy((object) str));
          }
        }
        else
          break;
        break;
      case PythonType owner:
        PythonTypeSlot slot;
        object obj2;
        if (owner.TryResolveSlot(context, name, out slot) && slot.TryGetValue(context, (object) null, owner, out obj2))
          return obj2;
        break;
      case NamespaceTracker self:
        object obj3 = NamespaceTrackerOps.GetCustomMember(context, self, name);
        if (obj3 != OperationFailed.Value)
          return obj3;
        break;
      default:
        object ret1;
        if (PythonOps.TryGetBoundAttr(context, from, name, out ret1))
          return ret1;
        break;
    }
    throw PythonOps.ImportError("Cannot import name {0}", (object) name);
  }

  private static object ImportModuleFrom(
    CodeContext context,
    object from,
    string[] parts,
    int current)
  {
    object obj;
    if (from is PythonModule pythonModule && (pythonModule.__dict__._storage.TryGetPath(out obj) || DynamicHelpers.GetPythonType((object) pythonModule).TryGetMember(context, (object) pythonModule, "__path__", out obj)))
    {
      if (obj is List path)
        return Importer.ImportNestedModule(context, pythonModule, parts, current, path);
      if (obj is string str)
        return Importer.ImportNestedModule(context, pythonModule, parts, current, List.FromArrayNoCopy((object) str));
    }
    object ret;
    if (from is NamespaceTracker namespaceTracker && namespaceTracker.TryGetValue(parts[current], out ret))
      return Importer.MemberTrackerToPython(context, ret);
    throw PythonOps.ImportError("No module named {0}", (object) parts[current]);
  }

  public static object ImportModule(
    CodeContext context,
    object globals,
    string modName,
    bool bottom,
    int level)
  {
    if (modName.IndexOf(Path.DirectorySeparatorChar) != -1)
      throw PythonOps.ImportError("Import by filename is not supported.", (object) modName);
    str = (string) null;
    if (globals is PythonDictionary pythonDictionary)
    {
      object obj;
      if (pythonDictionary._storage.TryGetPackage(out obj))
      {
        switch (obj)
        {
          case string str:
          case null:
            break;
          default:
            throw PythonOps.ValueError("__package__ set to non-string");
        }
      }
      else
      {
        str = (string) null;
        object self;
        if (level > 0 && pythonDictionary._storage.TryGetName(out self) && self is string)
        {
          if (pythonDictionary._storage.TryGetPath(out object _))
            pythonDictionary[(object) "__package__"] = self;
          else
            pythonDictionary[(object) "__package__"] = ((string) self).rpartition(".")[0];
        }
      }
    }
    object ret1 = (object) null;
    int length = modName.IndexOf('.');
    string str1 = length != -1 ? modName.Substring(0, length) : modName;
    string str2 = (string) null;
    string full;
    List path;
    PythonModule parentMod;
    if (level != 0 && Importer.TryGetNameAndPath(context, globals, str1, level, str, out full, out path, out parentMod))
    {
      str2 = full;
      if (!Importer.TryGetExistingOrMetaPathModule(context, full, path, out ret1))
      {
        ret1 = Importer.ImportFromPath(context, str1, full, path);
        if (ret1 == null)
          context.LanguageContext.SystemStateModules[(object) full] = (object) null;
        else if (parentMod != null)
          parentMod.__dict__[(object) str1] = ret1;
      }
      else if (length == -1 && ret1 is NamespaceTracker)
        context.ShowCls = true;
    }
    if (level <= 0 && ret1 == null)
    {
      if (!string.IsNullOrEmpty(str) && !context.LanguageContext.SystemStateModules.TryGetValue((object) str, out object _))
        PythonOps.Warn(new ModuleContext(new PythonModule()
        {
          __dict__ = {
            [(object) "__file__"] = ((object) str),
            [(object) "__name__"] = ((object) str)
          }
        }.__dict__, context.LanguageContext).GlobalContext, PythonExceptions.RuntimeWarning, "Parent module '{0}' not found while handling absolute import", (object) str);
      ret1 = Importer.ImportTopAbsolute(context, str1);
      str2 = str1;
      if (ret1 == null)
        return (object) null;
    }
    string[] parts = modName.Split('.');
    object from = ret1;
    string fullName = (string) null;
    for (int current = 0; current < parts.Length; ++current)
    {
      fullName = current == 0 ? str2 : $"{fullName}.{parts[current]}";
      object ret2;
      if (Importer.TryGetExistingModule(context, fullName, out ret2))
      {
        from = ret2;
        if (current == 0)
          ret1 = from;
      }
      else if (current != 0)
        from = Importer.ImportModuleFrom(context, from, parts, current);
      else
        ret1 = from;
    }
    return !bottom ? ret1 : from;
  }

  private static bool TryGetNameAndPath(
    CodeContext context,
    object globals,
    string name,
    int level,
    string package,
    out string full,
    out List path,
    out PythonModule parentMod)
  {
    full = name;
    path = (List) null;
    parentMod = (PythonModule) null;
    object obj1;
    if (!(globals is PythonDictionary pythonDictionary) || !pythonDictionary._storage.TryGetName(out obj1) || !(obj1 is string str))
      return false;
    string parentModuleName;
    if (package == null)
    {
      if (pythonDictionary._storage.TryGetPath(out obj1) && (path = obj1 as List) != null)
      {
        if (level == -1)
        {
          full = $"{str}.{name}";
          object obj2;
          if (context.LanguageContext.SystemStateModules.TryGetValue((object) str, out obj2))
            parentMod = obj2 as PythonModule;
        }
        else if (string.IsNullOrEmpty(name))
        {
          full = str.rsplit(".", level - 1)[0] as string;
        }
        else
        {
          string key = str.rsplit(".", level - 1)[0] as string;
          full = $"{key}.{name}";
          object obj3;
          if (context.LanguageContext.SystemStateModules.TryGetValue((object) key, out obj3))
            parentMod = obj3 as PythonModule;
        }
        return true;
      }
      int length = str.LastIndexOf('.');
      if (length == -1)
      {
        if (level > 0)
          throw PythonOps.ValueError("Attempted relative import in non-package");
        return false;
      }
      for (int index = level; index > 1 && length != -1; --index)
        length = str.LastIndexOf('.', length - 1);
      parentModuleName = length != -1 ? str.Substring(0, length) : str;
    }
    else
      parentModuleName = Importer.GetParentPackageName(level - 1, package.Split('.'));
    path = Importer.GetParentPathAndModule(context, parentModuleName, out parentMod);
    if (path != null)
    {
      full = !string.IsNullOrEmpty(name) ? $"{parentModuleName}.{name}" : parentModuleName;
      return true;
    }
    if (level > 0)
      throw PythonOps.SystemError("Parent module '{0}' not loaded, cannot perform relative import", (object) parentModuleName);
    return false;
  }

  private static string GetParentPackageName(int level, string[] names)
  {
    StringBuilder stringBuilder = new StringBuilder(names[0]);
    if (level < 0)
      level = 1;
    for (int index = 1; index < names.Length - level; ++index)
    {
      stringBuilder.Append('.');
      stringBuilder.Append(names[index]);
    }
    return stringBuilder.ToString();
  }

  public static object ReloadModule(CodeContext context, PythonModule module)
  {
    return Importer.ReloadModule(context, module, (PythonFile) null);
  }

  internal static object ReloadModule(CodeContext context, PythonModule module, PythonFile file)
  {
    PythonContext languageContext = context.LanguageContext;
    string file1 = module.GetFile();
    if (file1 == null)
    {
      Importer.ReloadBuiltinModule(context, module);
      return (object) module;
    }
    string name = module.GetName();
    if (name != null)
    {
      List path1 = (List) null;
      int length = name.LastIndexOf('.');
      if (length != -1)
        path1 = Importer.GetParentPathAndModule(context, name.Substring(0, length), out PythonModule _);
      object ret;
      if (Importer.TryLoadMetaPathModule(context, module.GetName(), path1, out ret) && ret != null)
        return (object) module;
      List path2;
      if (context.LanguageContext.TryGetSystemPath(out path2))
      {
        object obj = Importer.ImportFromPathHook(context, name, name, path2, (Func<CodeContext, string, string, string, object>) null);
        if (obj != null)
          return obj;
      }
    }
    SourceUnit sourceCode;
    if (file != null)
    {
      sourceCode = languageContext.CreateSourceUnit((StreamContentProvider) new Importer.PythonFileStreamContentProvider(file), file1, file.Encoding, SourceCodeKind.File);
    }
    else
    {
      if (!languageContext.DomainManager.Platform.FileExists(file1))
        throw PythonOps.SystemError("module source file not found");
      sourceCode = languageContext.CreateFileUnit(file1, languageContext.DefaultEncoding, SourceCodeKind.File);
    }
    languageContext.GetScriptCode(sourceCode, name, ModuleOptions.None, CompilationMode.Lookup).Run(module.Scope);
    return (object) module;
  }

  private static List GetParentPathAndModule(
    CodeContext context,
    string parentModuleName,
    out PythonModule parentModule)
  {
    List parentPathAndModule = (List) null;
    parentModule = (PythonModule) null;
    object obj1;
    if (context.LanguageContext.SystemStateModules.TryGetValue((object) parentModuleName, out obj1))
    {
      parentModule = obj1 as PythonModule;
      object obj2;
      if (parentModule != null && parentModule.__dict__._storage.TryGetPath(out obj2))
        parentPathAndModule = obj2 as List;
    }
    return parentPathAndModule;
  }

  private static void ReloadBuiltinModule(CodeContext context, PythonModule module)
  {
    string name = module.GetName();
    if (!context.LanguageContext.BuiltinModules.TryGetValue(name, out Type _))
      throw PythonOps.ImportError("no module named {0}", (object) module.GetName());
    ((ModuleDictionaryStorage) module.__dict__._storage).Reload();
  }

  private static bool TryGetExistingOrMetaPathModule(
    CodeContext context,
    string fullName,
    List path,
    out object ret)
  {
    return Importer.TryGetExistingModule(context, fullName, out ret) || Importer.TryLoadMetaPathModule(context, fullName, path, out ret);
  }

  private static bool TryLoadMetaPathModule(
    CodeContext context,
    string fullName,
    List path,
    out object ret)
  {
    if (context.LanguageContext.GetSystemStateValue("meta_path") is List systemStateValue)
    {
      foreach (object importer in (IEnumerable) systemStateValue)
      {
        if (Importer.FindAndLoadModuleFromImporter(context, importer, fullName, path, out ret))
          return true;
      }
    }
    ret = (object) null;
    return false;
  }

  private static bool FindAndLoadModuleFromImporter(
    CodeContext context,
    object importer,
    string fullName,
    List path,
    out object ret)
  {
    object boundAttr1 = PythonOps.GetBoundAttr(context, importer, "find_module");
    PythonContext languageContext = context.LanguageContext;
    object o = languageContext.Call(context, boundAttr1, (object) fullName, (object) path);
    if (o != null)
    {
      object boundAttr2 = PythonOps.GetBoundAttr(context, o, "load_module");
      ret = languageContext.Call(context, boundAttr2, (object) fullName);
      return ret != null;
    }
    ret = (object) null;
    return false;
  }

  internal static bool TryGetExistingModule(CodeContext context, string fullName, out object ret)
  {
    return context.LanguageContext.SystemStateModules.TryGetValue((object) fullName, out ret);
  }

  private static object ImportTopAbsolute(CodeContext context, string name)
  {
    object ret;
    if (Importer.TryGetExistingModule(context, name, out ret))
    {
      if (Importer.IsReflected(ret))
        ret = Importer.ImportReflected(context, name) ?? ret;
      if (ret is NamespaceTracker || ret == context.LanguageContext.ClrModule)
        context.ShowCls = true;
      return ret;
    }
    if (Importer.TryLoadMetaPathModule(context, name, (List) null, out ret))
      return ret;
    object obj1 = Importer.ImportBuiltin(context, name);
    if (obj1 != null)
      return obj1;
    List path;
    if (context.LanguageContext.TryGetSystemPath(out path))
    {
      object obj2 = Importer.ImportFromPath(context, name, name, path);
      if (obj2 != null)
        return obj2;
    }
    return Importer.ImportReflected(context, name) ?? (object) null;
  }

  private static string[] SubArray(string[] t, int len)
  {
    string[] destinationArray = new string[len];
    Array.Copy((Array) t, (Array) destinationArray, len);
    return destinationArray;
  }

  private static bool TryGetNestedModule(
    CodeContext context,
    PythonModule scope,
    string[] parts,
    int current,
    out object nested)
  {
    string part = parts[current];
    if (scope.__dict__.TryGetValue((object) part, out nested))
    {
      if (nested is PythonModule pythonModule)
      {
        string str = ".".join((object) Importer.SubArray(parts, current));
        if (pythonModule.GetName() == str)
          return true;
      }
      if (nested is PythonType pythonType && pythonType.IsSystemType)
        return true;
    }
    return false;
  }

  private static object ImportNestedModule(
    CodeContext context,
    PythonModule module,
    string[] parts,
    int current,
    List path)
  {
    string part = parts[current];
    string fullName = Importer.CreateFullName(module.GetName(), part);
    object obj1;
    if (Importer.TryGetExistingOrMetaPathModule(context, fullName, path, out obj1))
    {
      module.__dict__[(object) part] = obj1;
      return obj1;
    }
    if (Importer.TryGetNestedModule(context, module, parts, current, out obj1))
      return obj1;
    Importer.ImportFromPath(context, part, fullName, path);
    object obj2;
    if (context.LanguageContext.SystemStateModules.TryGetValue((object) fullName, out obj2))
    {
      module.__dict__[(object) part] = obj2;
      return obj2;
    }
    throw PythonOps.ImportError("cannot import {0} from {1}", (object) part, (object) module.GetName());
  }

  private static object FindImportFunction(CodeContext context)
  {
    object importFunction;
    if ((context.GetBuiltinsDict() ?? context.LanguageContext.BuiltinModuleDict)._storage.TryGetImport(out importFunction))
      return importFunction;
    throw PythonOps.ImportError("cannot find __import__");
  }

  internal static object ImportBuiltin(CodeContext context, string name)
  {
    PythonContext languageContext = context.LanguageContext;
    switch (name)
    {
      case "sys":
        return (object) languageContext.SystemState;
      case "clr":
        context.ShowCls = true;
        languageContext.SystemStateModules[(object) "clr"] = (object) languageContext.ClrModule;
        return (object) languageContext.ClrModule;
      default:
        return (object) languageContext.GetBuiltinModule(name);
    }
  }

  private static object ImportReflected(CodeContext context, string name)
  {
    PythonContext languageContext = context.LanguageContext;
    object ret;
    if (!PythonOps.ScopeTryGetMember(context, languageContext.DomainManager.Globals, name, out ret) && (MemberTracker) (ret = (object) languageContext.TopNamespace.TryGetPackageAny(name)) == null)
      ret = (object) Importer.TryImportSourceFile(languageContext, name);
    object python = Importer.MemberTrackerToPython(context, ret);
    if (python != null)
      context.LanguageContext.SystemStateModules[(object) name] = python;
    return python;
  }

  internal static object MemberTrackerToPython(CodeContext context, object ret)
  {
    if (ret is MemberTracker tracker)
    {
      context.ShowCls = true;
      object obj = (object) tracker;
      switch (tracker.MemberType)
      {
        case TrackerTypes.Event:
          obj = (object) PythonTypeOps.GetReflectedEvent((EventTracker) tracker);
          break;
        case TrackerTypes.Field:
          obj = (object) PythonTypeOps.GetReflectedField(((FieldTracker) tracker).Field);
          break;
        case TrackerTypes.Method:
          MethodTracker methodTracker = tracker as MethodTracker;
          obj = (object) PythonTypeOps.GetBuiltinFunction(methodTracker.DeclaringType, methodTracker.Name, new MemberInfo[1]
          {
            (MemberInfo) methodTracker.Method
          });
          break;
        case TrackerTypes.Type:
          obj = (object) DynamicHelpers.GetPythonTypeFromType(((TypeTracker) tracker).Type);
          break;
      }
      ret = obj;
    }
    return ret;
  }

  internal static PythonModule TryImportSourceFile(PythonContext context, string name)
  {
    SourceUnit sourceFile = Importer.TryFindSourceFile(context, name);
    PlatformAdaptationLayer platform = context.DomainManager.Platform;
    if (sourceFile == null || Importer.GetFullPathAndValidateCase((LanguageContext) context, platform.CombinePaths(platform.GetDirectoryName(sourceFile.Path), name + platform.GetExtension(sourceFile.Path)), false) == null)
      return (PythonModule) null;
    PythonModule pythonModule = Importer.ExecuteSourceUnit(context, sourceFile);
    if (sourceFile.LanguageContext != context)
      context.SystemStateModules[(object) name] = (object) pythonModule;
    PythonOps.ScopeSetMember(context.SharedContext, sourceFile.LanguageContext.DomainManager.Globals, name, (object) pythonModule);
    return pythonModule;
  }

  internal static PythonModule ExecuteSourceUnit(PythonContext context, SourceUnit sourceUnit)
  {
    ScriptCode scriptCode = sourceUnit.Compile();
    Scope scope = scriptCode.CreateScope();
    PythonModule module = ((PythonScopeExtension) context.EnsureScopeExtension(scope)).Module;
    scriptCode.Run(scope);
    return module;
  }

  internal static SourceUnit TryFindSourceFile(PythonContext context, string name)
  {
    List path1;
    if (!context.TryGetSystemPath(out path1))
      return (SourceUnit) null;
    foreach (object obj in path1)
    {
      if (obj is string path1_1)
      {
        string path2 = (string) null;
        LanguageContext languageContext = (LanguageContext) null;
        foreach (string fileExtension in context.DomainManager.Configuration.GetFileExtensions())
        {
          string path3;
          try
          {
            path3 = context.DomainManager.Platform.CombinePaths(path1_1, name + fileExtension);
          }
          catch (ArgumentException ex)
          {
            continue;
          }
          if (context.DomainManager.Platform.FileExists(path3))
          {
            path2 = path2 == null ? path3 : throw PythonOps.ImportError($"Found multiple modules of the same name '{name}': '{path2}' and '{path3}'");
            languageContext = context.DomainManager.GetLanguageByExtension(fileExtension);
          }
        }
        if (path2 != null)
          return languageContext.CreateFileUnit(path2);
      }
    }
    return (SourceUnit) null;
  }

  private static bool IsReflected(object module)
  {
    switch (module)
    {
      case MemberTracker _:
      case PythonType _:
      case ReflectedEvent _:
      case ReflectedField _:
        return true;
      default:
        return module is BuiltinFunction;
    }
  }

  private static string CreateFullName(string baseName, string name)
  {
    return baseName == null || baseName.Length == 0 || baseName == "__main__" ? name : $"{baseName}.{name}";
  }

  private static object ImportFromPath(
    CodeContext context,
    string name,
    string fullName,
    List path)
  {
    return Importer.ImportFromPathHook(context, name, fullName, path, new Func<CodeContext, string, string, string, object>(Importer.LoadFromDisk));
  }

  private static object ImportFromPathHook(
    CodeContext context,
    string name,
    string fullName,
    List path,
    Func<CodeContext, string, string, string, object> defaultLoader)
  {
    if (!(context.LanguageContext.GetSystemStateValue("path_importer_cache") is IDictionary<object, object> systemStateValue))
      return (object) null;
    foreach (object obj1 in (IEnumerable) path)
    {
      if (obj1 is string result || Converter.TryConvertToString(obj1, out result) && result != null)
      {
        object importerForPath;
        if (!systemStateValue.TryGetValue((object) result, out importerForPath))
          systemStateValue[(object) result] = importerForPath = Importer.FindImporterForPath(context, result);
        if (importerForPath != null)
        {
          object ret;
          if (Importer.FindAndLoadModuleFromImporter(context, importerForPath, fullName, (List) null, out ret))
            return ret;
        }
        else if (defaultLoader != null)
        {
          object obj2 = defaultLoader(context, name, fullName, result);
          if (obj2 != null)
            return obj2;
        }
      }
    }
    return (object) null;
  }

  internal static bool TryImportMainFromZip(CodeContext context, string path, out object importer)
  {
    if (!(context.LanguageContext.GetSystemStateValue("path_importer_cache") is IDictionary<object, object> systemStateValue1))
    {
      importer = (object) null;
      return false;
    }
    systemStateValue1[(object) path] = importer = Importer.FindImporterForPath(context, path);
    if (importer == null)
      return false;
    if (context.LanguageContext.GetSystemStateValue(nameof (path)) is List systemStateValue2)
      systemStateValue2.Insert(0, (object) path);
    return Importer.FindAndLoadModuleFromImporter(context, importer, "__main__", (List) null, out object _);
  }

  private static object LoadFromDisk(
    CodeContext context,
    string name,
    string fullName,
    string str)
  {
    string path1 = context.LanguageContext.DomainManager.Platform.CombinePaths(str, name);
    PythonModule pythonModule = Importer.LoadPackageFromSource(context, fullName, path1);
    if (pythonModule != null)
      return (object) pythonModule;
    string path2 = path1 + ".py";
    return (object) Importer.LoadModuleFromSource(context, fullName, path2) ?? (object) null;
  }

  private static object FindImporterForPath(CodeContext context, string dirname)
  {
    foreach (object func in (IEnumerable) (context.LanguageContext.GetSystemStateValue("path_hooks") as List))
    {
      try
      {
        object importerForPath = PythonCalls.Call(context, func, (object) dirname);
        if (importerForPath != null)
          return importerForPath;
      }
      catch (ImportException ex)
      {
      }
    }
    return !context.LanguageContext.DomainManager.Platform.DirectoryExists(dirname) ? (object) new PythonImport.NullImporter(dirname) : (object) null;
  }

  private static PythonModule LoadModuleFromSource(CodeContext context, string name, string path)
  {
    PythonContext languageContext = context.LanguageContext;
    string pathAndValidateCase = Importer.GetFullPathAndValidateCase((LanguageContext) languageContext, path, false);
    if (pathAndValidateCase == null || !languageContext.DomainManager.Platform.FileExists(pathAndValidateCase))
      return (PythonModule) null;
    SourceUnit fileUnit = languageContext.CreateFileUnit(pathAndValidateCase, languageContext.DefaultEncoding, SourceCodeKind.File);
    return Importer.LoadFromSourceUnit(context, fileUnit, name, fileUnit.Path);
  }

  private static string GetFullPathAndValidateCase(
    LanguageContext context,
    string path,
    bool isDir)
  {
    PlatformAdaptationLayer platform = context.DomainManager.Platform;
    string directoryName = platform.GetDirectoryName(path);
    if (!platform.DirectoryExists(directoryName))
      return (string) null;
    try
    {
      string fileName = platform.GetFileName(path);
      string[] fileSystemEntries = platform.GetFileSystemEntries(directoryName, fileName, !isDir, isDir);
      return fileSystemEntries.Length != 1 || platform.GetFileName(fileSystemEntries[0]) != fileName ? (string) null : platform.GetFullPath(fileSystemEntries[0]);
    }
    catch (IOException ex)
    {
      return (string) null;
    }
  }

  internal static PythonModule LoadPackageFromSource(CodeContext context, string name, string path)
  {
    path = Importer.GetFullPathAndValidateCase((LanguageContext) context.LanguageContext, path, true);
    if (path == null)
      return (PythonModule) null;
    if (context.LanguageContext.DomainManager.Platform.DirectoryExists(path) && !context.LanguageContext.DomainManager.Platform.FileExists(context.LanguageContext.DomainManager.Platform.CombinePaths(path, "__init__.py")))
      PythonOps.Warn(context, PythonExceptions.ImportWarning, "Not importing directory '{0}': missing __init__.py", (object) path);
    return Importer.LoadModuleFromSource(context, name, context.LanguageContext.DomainManager.Platform.CombinePaths(path, "__init__.py"));
  }

  private static PythonModule LoadFromSourceUnit(
    CodeContext context,
    SourceUnit sourceCode,
    string name,
    string path)
  {
    return context.LanguageContext.CompileModule(path, name, sourceCode, ModuleOptions.Optimized | ModuleOptions.Initialize);
  }

  private class PythonFileStreamContentProvider : StreamContentProvider
  {
    private readonly PythonFile _file;

    public PythonFileStreamContentProvider(PythonFile file) => this._file = file;

    public override Stream GetStream() => this._file._stream;
  }
}
