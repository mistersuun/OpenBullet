// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ClrModule
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

public static class ClrModule
{
  public static readonly bool IsNetCoreApp = false;
  public static readonly bool IsDebug = false;
  private static int _isMono = -1;
  private static int _isMacOS = -1;
  private static PythonType _strongBoxType;

  public static string TargetFramework
  {
    get
    {
      return CustomAttributeExtensions.GetCustomAttribute<TargetFrameworkAttribute>(typeof (ClrModule).Assembly)?.FrameworkName;
    }
  }

  public static bool IsMono
  {
    get
    {
      if (ClrModule._isMono == -1)
        ClrModule._isMono = Type.GetType("Mono.Runtime") != (Type) null ? 1 : 0;
      return ClrModule._isMono == 1;
    }
  }

  public static bool IsMacOS
  {
    get
    {
      if (ClrModule._isMacOS == -1)
        ClrModule._isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 1 : 0;
      return ClrModule._isMacOS == 1;
    }
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (dict.ContainsKey((object) "References"))
      return;
    dict[(object) "References"] = (object) context.ReferencedAssemblies;
  }

  public static ScriptDomainManager GetCurrentRuntime(CodeContext context)
  {
    return context.LanguageContext.DomainManager;
  }

  [Documentation("Adds a reference to a .NET assembly.  Parameters can be an already loaded\r\nAssembly object, a full assembly name, or a partial assembly name. After the\r\nload the assemblies namespaces and top-level types will be available via \r\nimport Namespace.")]
  public static void AddReference(CodeContext context, params object[] references)
  {
    if (references == null)
      throw new TypeErrorException("Expected string or Assembly, got NoneType");
    if (references.Length == 0)
      throw new ValueErrorException("Expected at least one name, got none");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    foreach (object reference in references)
      ClrModule.AddReference(context, reference);
  }

  [Documentation("Adds a reference to a .NET assembly.  One or more assembly names can\r\nbe provided.  The assembly is searched for in the directories specified in \r\nsys.path and dependencies will be loaded from sys.path as well.  The assembly \r\nname should be the filename on disk without a directory specifier and \r\noptionally including the .EXE or .DLL extension. After the load the assemblies \r\nnamespaces and top-level types will be available via import Namespace.")]
  public static void AddReferenceToFile(CodeContext context, params string[] files)
  {
    if (files == null)
      throw new TypeErrorException("Expected string, got NoneType");
    if (files.Length == 0)
      throw new ValueErrorException("Expected at least one name, got none");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    foreach (string file in files)
      ClrModule.AddReferenceToFile(context, file);
  }

  [Documentation("Adds a reference to a .NET assembly.  Parameters are an assembly name. \r\nAfter the load the assemblies namespaces and top-level types will be available via \r\nimport Namespace.")]
  public static void AddReferenceByName(CodeContext context, params string[] names)
  {
    if (names == null)
      throw new TypeErrorException("Expected string, got NoneType");
    if (names.Length == 0)
      throw new ValueErrorException("Expected at least one name, got none");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    foreach (string name in names)
      ClrModule.AddReferenceByName(context, name);
  }

  [Documentation("Adds a reference to a .NET assembly.  Parameters are a partial assembly name. \r\nAfter the load the assemblies namespaces and top-level types will be available via \r\nimport Namespace.")]
  public static void AddReferenceByPartialName(CodeContext context, params string[] names)
  {
    if (names == null)
      throw new TypeErrorException("Expected string, got NoneType");
    if (names.Length == 0)
      throw new ValueErrorException("Expected at least one name, got none");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    foreach (string name in names)
      ClrModule.AddReferenceByPartialName(context, name);
  }

  [Documentation("Adds a reference to a .NET assembly.  Parameters are a full path to an. \r\nassembly on disk. After the load the assemblies namespaces and top-level types \r\nwill be available via import Namespace.")]
  public static Assembly LoadAssemblyFromFileWithPath(CodeContext context, string file)
  {
    if (file == null)
      throw new TypeErrorException("LoadAssemblyFromFileWithPath: arg 1 must be a string.");
    Assembly res;
    if (context.LanguageContext.TryLoadAssemblyFromFileWithPath(file, out res))
      return res;
    if (!Path.IsPathRooted(file))
      throw new ValueErrorException("LoadAssemblyFromFileWithPath: path must be rooted");
    if (!File.Exists(file))
      throw new ValueErrorException("LoadAssemblyFromFileWithPath: file not found");
    throw new ValueErrorException("LoadAssemblyFromFileWithPath: error loading assembly");
  }

  [Documentation("Loads an assembly from the specified filename and returns the assembly\r\nobject.  Namespaces or types in the assembly can be accessed directly from \r\nthe assembly object.")]
  public static Assembly LoadAssemblyFromFile(CodeContext context, string file)
  {
    switch (file)
    {
      case null:
        throw new TypeErrorException("Expected string, got NoneType");
      case "":
        throw new ValueErrorException("assembly name must not be empty string");
      default:
        ContractUtils.RequiresNotNull((object) context, nameof (context));
        if (file.IndexOf(Path.DirectorySeparatorChar) != -1)
          throw new ValueErrorException("filenames must not contain full paths, first add the path to sys.path");
        return context.LanguageContext.LoadAssemblyFromFile(file);
    }
  }

  [Documentation("Loads an assembly from the specified partial assembly name and returns the \r\nassembly object.  Namespaces or types in the assembly can be accessed directly \r\nfrom the assembly object.")]
  public static Assembly LoadAssemblyByPartialName(string name)
  {
    return name != null ? Assembly.LoadWithPartialName(name) : throw new TypeErrorException("LoadAssemblyByPartialName: arg 1 must be a string");
  }

  [Documentation("Loads an assembly from the specified assembly name and returns the assembly\r\nobject.  Namespaces or types in the assembly can be accessed directly from \r\nthe assembly object.")]
  public static Assembly LoadAssemblyByName(CodeContext context, string name)
  {
    if (name == null)
      throw new TypeErrorException("LoadAssemblyByName: arg 1 must be a string");
    return context.LanguageContext.DomainManager.Platform.LoadAssembly(name);
  }

  public static object Use(CodeContext context, string name)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    if (name == null)
      throw new TypeErrorException("Use: arg 1 must be a string");
    return (object) (Importer.TryImportSourceFile(context.LanguageContext, name) ?? throw new ValueErrorException($"couldn't find module {name} to use"));
  }

  [Documentation("Converts an array of bytes to a string.")]
  public static string GetString(byte[] bytes) => ClrModule.GetString(bytes, bytes.Length);

  [Documentation("Converts maxCount of an array of bytes to a string")]
  public static string GetString(byte[] bytes, int maxCount)
  {
    int count = Math.Min(bytes.Length, maxCount);
    return Encoding.GetEncoding("iso-8859-1").GetString(bytes, 0, count);
  }

  [Documentation("Converts a string to an array of bytes")]
  public static byte[] GetBytes(string s) => ClrModule.GetBytes(s, s.Length);

  [Documentation("Converts maxCount of a string to an array of bytes")]
  public static byte[] GetBytes(string s, int maxCount)
  {
    int charCount = Math.Min(s.Length, maxCount);
    byte[] bytes = new byte[charCount];
    Encoding.GetEncoding("iso-8859-1").GetBytes(s, 0, charCount, bytes, 0);
    return bytes;
  }

  public static object Use(CodeContext context, string path, string language)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    if (path == null)
      throw new TypeErrorException("Use: arg 1 must be a string");
    if (language == null)
      throw new TypeErrorException("Use: arg 2 must be a string");
    ScriptDomainManager domainManager = context.LanguageContext.DomainManager;
    if (!domainManager.Platform.FileExists(path))
      throw new ValueErrorException($"couldn't load module at path '{path}' in language '{language}'");
    SourceUnit fileUnit = domainManager.GetLanguageByName(language).CreateFileUnit(path);
    return (object) Importer.ExecuteSourceUnit(context.LanguageContext, fileUnit);
  }

  public static Action<Action> SetCommandDispatcher(CodeContext context, Action<Action> dispatcher)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    return context.LanguageContext.GetSetCommandDispatcher(dispatcher);
  }

  public static void ImportExtensions(CodeContext context, PythonType type)
  {
    if (type == null)
      throw PythonOps.TypeError("type must not be None");
    if (!type.IsSystemType)
      throw PythonOps.ValueError("type must be .NET type");
    lock (context.ModuleContext)
      context.ModuleContext.ExtensionMethods = ExtensionMethodSet.AddType(context.LanguageContext, context.ModuleContext.ExtensionMethods, type);
  }

  public static void ImportExtensions(CodeContext context, [NotNull] NamespaceTracker @namespace)
  {
    lock (context.ModuleContext)
      context.ModuleContext.ExtensionMethods = ExtensionMethodSet.AddNamespace(context.LanguageContext, context.ModuleContext.ExtensionMethods, @namespace);
  }

  public static ComTypeLibInfo LoadTypeLibrary(CodeContext context, object rcw)
  {
    return ComTypeLibDesc.CreateFromObject(rcw);
  }

  public static ComTypeLibInfo LoadTypeLibrary(CodeContext context, Guid typeLibGuid)
  {
    return ComTypeLibDesc.CreateFromGuid(typeLibGuid);
  }

  public static void AddReferenceToTypeLibrary(CodeContext context, object rcw)
  {
    ComTypeLibInfo fromObject = ComTypeLibDesc.CreateFromObject(rcw);
    ClrModule.PublishTypeLibDesc(context, fromObject.TypeLibDesc);
  }

  public static void AddReferenceToTypeLibrary(CodeContext context, Guid typeLibGuid)
  {
    ComTypeLibInfo fromGuid = ComTypeLibDesc.CreateFromGuid(typeLibGuid);
    ClrModule.PublishTypeLibDesc(context, fromGuid.TypeLibDesc);
  }

  private static void PublishTypeLibDesc(CodeContext context, ComTypeLibDesc typeLibDesc)
  {
    PythonOps.ScopeSetMember(context, context.LanguageContext.DomainManager.Globals, typeLibDesc.Name, (object) typeLibDesc);
  }

  private static void AddReference(CodeContext context, object reference)
  {
    Assembly assembly = reference as Assembly;
    if (assembly != (Assembly) null)
    {
      ClrModule.AddReference(context, assembly);
    }
    else
    {
      if (!(reference is string name))
        throw new TypeErrorException($"Invalid assembly type. Expected string or Assembly, got {reference}.");
      ClrModule.AddReference(context, name);
    }
  }

  private static void AddReference(CodeContext context, Assembly assembly)
  {
    context.LanguageContext.DomainManager.LoadAssembly(assembly);
  }

  private static void AddReference(CodeContext context, string name)
  {
    if (name == null)
      throw new TypeErrorException("Expected string, got NoneType");
    Assembly assembly = (Assembly) null;
    try
    {
      assembly = ClrModule.LoadAssemblyByName(context, name);
    }
    catch
    {
    }
    if (assembly == (Assembly) null)
      assembly = ClrModule.LoadAssemblyByPartialName(name);
    if (assembly == (Assembly) null)
      throw new IOException($"Could not add reference to assembly {name}");
    ClrModule.AddReference(context, assembly);
  }

  private static void AddReferenceToFile(CodeContext context, string file)
  {
    Assembly assembly = file != null ? ClrModule.LoadAssemblyFromFile(context, file) : throw new TypeErrorException("Expected string, got NoneType");
    if (assembly == (Assembly) null)
      throw new IOException($"Could not add reference to assembly {file}");
    ClrModule.AddReference(context, assembly);
  }

  private static void AddReferenceByPartialName(CodeContext context, string name)
  {
    if (name == null)
      throw new TypeErrorException("Expected string, got NoneType");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    Assembly assembly = ClrModule.LoadAssemblyByPartialName(name);
    if (assembly == (Assembly) null)
      throw new IOException($"Could not add reference to assembly {name}");
    ClrModule.AddReference(context, assembly);
  }

  private static void AddReferenceByName(CodeContext context, string name)
  {
    Assembly assembly = name != null ? ClrModule.LoadAssemblyByName(context, name) : throw new TypeErrorException("Expected string, got NoneType");
    if (assembly == (Assembly) null)
      throw new IOException($"Could not add reference to assembly {name}");
    ClrModule.AddReference(context, assembly);
  }

  [Documentation("Adds a reference to a .NET assembly.  One or more assembly names can\r\nbe provided which are fully qualified names to the file on disk.  The \r\ndirectory is added to sys.path and AddReferenceToFile is then called. After the \r\nload the assemblies namespaces and top-level types will be available via \r\nimport Namespace.")]
  public static void AddReferenceToFileAndPath(CodeContext context, params string[] files)
  {
    if (files == null)
      throw new TypeErrorException("Expected string, got NoneType");
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    foreach (string file in files)
      ClrModule.AddReferenceToFileAndPath(context, file);
  }

  private static void AddReferenceToFileAndPath(CodeContext context, string file)
  {
    string str = file != null ? Path.GetDirectoryName(Path.GetFullPath(file)) : throw PythonOps.TypeError("Expected string, got NoneType");
    PythonContext languageContext = context.LanguageContext;
    List path;
    if (!languageContext.TryGetSystemPath(out path))
      throw PythonOps.TypeError("cannot update path, it is not a list");
    path.append((object) str);
    Assembly assembly = languageContext.LoadAssemblyFromFile(file);
    if (assembly == (Assembly) null)
      throw PythonOps.IOError("file does not exist: {0}", (object) file);
    ClrModule.AddReference(context, assembly);
  }

  public static Type GetClrType(Type type) => type;

  public static PythonType GetPythonType(Type t) => DynamicHelpers.GetPythonTypeFromType(t);

  [Obsolete("Call clr.GetPythonType instead")]
  public static PythonType GetDynamicType(Type t) => DynamicHelpers.GetPythonTypeFromType(t);

  public static PythonType Reference => ClrModule.StrongBox;

  public static PythonType StrongBox
  {
    get
    {
      if (ClrModule._strongBoxType == null)
        ClrModule._strongBoxType = DynamicHelpers.GetPythonTypeFromType(typeof (System.Runtime.CompilerServices.StrongBox<>));
      return ClrModule._strongBoxType;
    }
  }

  public static object accepts(params object[] types) => (object) new ClrModule.ArgChecker(types);

  public static object returns(object type) => (object) new ClrModule.ReturnChecker(type);

  public static object Self() => (object) null;

  public static List Dir(object o)
  {
    List list = new List((object) PythonOps.GetAttrNames(DefaultContext.Default, o));
    list.sort(DefaultContext.Default);
    return list;
  }

  public static List DirClr(object o)
  {
    List list = new List((object) PythonOps.GetAttrNames(DefaultContext.DefaultCLS, o));
    list.sort(DefaultContext.DefaultCLS);
    return list;
  }

  public static object Convert(CodeContext context, object o, Type toType)
  {
    return Converter.Convert(o, toType);
  }

  public static void CompileModules(
    CodeContext context,
    string assemblyName,
    [ParamDictionary] IDictionary<string, object> kwArgs,
    params string[] filenames)
  {
    ContractUtils.RequiresNotNull((object) assemblyName, nameof (assemblyName));
    ContractUtils.RequiresNotNullItems<string>((IList<string>) filenames, nameof (filenames));
    PythonContext languageContext = context.LanguageContext;
    for (int index = 0; index < filenames.Length; ++index)
      filenames[index] = Path.GetFullPath(filenames[index]);
    Dictionary<string, string> dictionary = ClrModule.BuildPackageMap(filenames);
    List<SavableScriptCode> savableScriptCodeList = new List<SavableScriptCode>();
    foreach (string filename in filenames)
    {
      if (!languageContext.DomainManager.Platform.FileExists(filename))
        throw PythonOps.IOError("Couldn't find file for compilation: " + filename);
      string directoryName = Path.GetDirectoryName(filename);
      string str1 = "";
      string moduleName;
      if (Path.GetFileName(filename) == "__init__.py")
      {
        directoryName = Path.GetDirectoryName(directoryName);
        moduleName = !string.IsNullOrEmpty(directoryName) ? Path.GetFileNameWithoutExtension(Path.GetDirectoryName(filename)) : Path.GetDirectoryName(filename);
        str1 = Path.DirectorySeparatorChar.ToString() + "__init__.py";
      }
      else
        moduleName = Path.GetFileNameWithoutExtension(filename);
      string str2;
      if (dictionary.TryGetValue(directoryName, out str2))
        moduleName = $"{str2}.{moduleName}";
      string path = moduleName.Replace('.', Path.DirectorySeparatorChar) + str1;
      SourceUnit sourceUnit = languageContext.CreateSourceUnit((StreamContentProvider) new ClrModule.FileStreamContentProvider(context.LanguageContext.DomainManager.Platform, filename), path, languageContext.DefaultEncoding, SourceCodeKind.File);
      ScriptCode scriptCode = context.LanguageContext.GetScriptCode(sourceUnit, moduleName, ModuleOptions.Initialize, CompilationMode.ToDisk);
      savableScriptCodeList.Add((SavableScriptCode) scriptCode);
    }
    object obj;
    if (kwArgs != null && kwArgs.TryGetValue("mainModule", out obj) && obj is string path1)
    {
      if (!languageContext.DomainManager.Platform.FileExists(path1))
        throw PythonOps.IOError("Couldn't find main file for compilation: {0}", (object) path1);
      SourceUnit fileUnit = languageContext.CreateFileUnit(path1, languageContext.DefaultEncoding, SourceCodeKind.File);
      savableScriptCodeList.Add((SavableScriptCode) context.LanguageContext.GetScriptCode(fileUnit, "__main__", ModuleOptions.Initialize, CompilationMode.ToDisk));
    }
    SavableScriptCode.SaveToAssembly(assemblyName, kwArgs, savableScriptCodeList.ToArray());
  }

  public static void CompileSubclassTypes(string assemblyName, params object[] newTypes)
  {
    if (assemblyName == null)
      throw PythonOps.TypeError("CompileTypes expected str for assemblyName, got NoneType");
    List<PythonTuple> types = new List<PythonTuple>();
    foreach (object newType in newTypes)
    {
      if (newType is PythonType)
        types.Add(PythonTuple.MakeTuple(newType));
      else
        types.Add(PythonTuple.Make(newType));
    }
    NewTypeMaker.SaveNewTypes(assemblyName, (IList<PythonTuple>) types);
  }

  public static PythonTuple GetSubclassedTypes()
  {
    List<object> objectList = new List<object>();
    foreach (NewTypeInfo key in NewTypeMaker._newTypes.Keys)
    {
      Type type1 = key.BaseType;
      for (Type type2 = type1; type2 != (Type) null; type2 = type2.BaseType)
      {
        if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof (Extensible<>))
        {
          type1 = type2.GetGenericArguments()[0];
          break;
        }
      }
      PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(type1);
      if (key.InterfaceTypes.Count == 0)
        objectList.Add((object) pythonTypeFromType);
      else if (key.InterfaceTypes.Count > 0)
      {
        PythonType[] pythonTypeArray = new PythonType[key.InterfaceTypes.Count + 1];
        pythonTypeArray[0] = pythonTypeFromType;
        for (int index = 0; index < key.InterfaceTypes.Count; ++index)
          pythonTypeArray[index + 1] = DynamicHelpers.GetPythonTypeFromType(key.InterfaceTypes[index]);
        objectList.Add((object) PythonTuple.MakeTuple((object[]) pythonTypeArray));
      }
    }
    return PythonTuple.MakeTuple(objectList.ToArray());
  }

  private static Dictionary<string, string> BuildPackageMap(string[] filenames)
  {
    List<string> modules = new List<string>();
    foreach (string filename in filenames)
    {
      if (filename.EndsWith("__init__.py"))
        modules.Add(filename);
    }
    ClrModule.SortModules(modules);
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string path in modules)
    {
      string directoryName = Path.GetDirectoryName(path);
      string empty = string.Empty;
      string str = Path.GetFileName(Path.GetDirectoryName(path));
      if (dictionary.TryGetValue(Path.GetDirectoryName(directoryName), out empty))
        str = $"{empty}.{str}";
      dictionary[Path.GetDirectoryName(path)] = str;
    }
    return dictionary;
  }

  private static void SortModules(List<string> modules)
  {
    modules.Sort((Comparison<string>) ((x, y) => x.Length - y.Length));
  }

  public static PythonTuple GetProfilerData(CodeContext context, bool includeUnused = false)
  {
    return new PythonTuple((object) Profiler.GetProfiler(context.LanguageContext).GetProfile(includeUnused));
  }

  public static void ClearProfilerData(CodeContext context)
  {
    Profiler.GetProfiler(context.LanguageContext).Reset();
  }

  public static void EnableProfiler(CodeContext context, bool enable)
  {
    (context.LanguageContext.Options as PythonOptions).EnableProfiler = enable;
  }

  public static PythonTuple Serialize(object self)
  {
    if (self == null)
      return PythonTuple.MakeTuple(null, (object) string.Empty);
    string str1;
    string str2;
    switch (CompilerHelpers.GetType(self).GetTypeCode())
    {
      case TypeCode.DBNull:
      case TypeCode.Char:
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Decimal:
        str1 = self.ToString();
        str2 = CompilerHelpers.GetType(self).FullName;
        break;
      default:
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        MemoryStream serializationStream = memoryStream;
        object graph = self;
        binaryFormatter.Serialize((Stream) serializationStream, graph);
        str1 = ((IList<byte>) memoryStream.ToArray()).MakeString();
        str2 = (string) null;
        break;
    }
    return PythonTuple.MakeTuple((object) str2, (object) str1);
  }

  public static object Deserialize(string serializationFormat, [NotNull] string data)
  {
    switch (serializationFormat)
    {
      case "System.Byte":
        return (object) byte.Parse(data);
      case "System.Char":
        return (object) char.Parse(data);
      case "System.DBNull":
        return (object) DBNull.Value;
      case "System.Decimal":
        return (object) Decimal.Parse(data);
      case "System.Int16":
        return (object) short.Parse(data);
      case "System.Int64":
        return (object) long.Parse(data);
      case "System.SByte":
        return (object) sbyte.Parse(data);
      case "System.Single":
        return (object) float.Parse(data);
      case "System.UInt16":
        return (object) ushort.Parse(data);
      case "System.UInt32":
        return (object) uint.Parse(data);
      case "System.UInt64":
        return (object) ulong.Parse(data);
      case null:
        return string.IsNullOrEmpty(data) ? (object) null : new BinaryFormatter().Deserialize((Stream) new MemoryStream(data.MakeByteArray()));
      default:
        throw PythonOps.ValueError("unknown serialization format: {0}", (object) serializationFormat);
    }
  }

  public sealed class ReferencesList : List<Assembly>, ICodeFormattable
  {
    public new void Add(Assembly other) => base.Add(other);

    [SpecialName]
    public ClrModule.ReferencesList Add(object other)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(other);
      while (enumerator.MoveNext())
      {
        Assembly current = enumerator.Current as Assembly;
        if (current == (Assembly) null)
          throw PythonOps.TypeError("non-assembly added to references list");
        base.Add(current);
      }
      return this;
    }

    public string __repr__(CodeContext context)
    {
      StringBuilder stringBuilder = new StringBuilder("(");
      string str = "";
      foreach (Assembly assembly in (List<Assembly>) this)
      {
        stringBuilder.Append(str);
        stringBuilder.Append('<');
        stringBuilder.Append(assembly.FullName);
        stringBuilder.Append('>');
        str = "," + Environment.NewLine;
      }
      stringBuilder.AppendLine(")");
      return stringBuilder.ToString();
    }
  }

  public class ArgChecker
  {
    private object[] expected;

    public ArgChecker(object[] prms) => this.expected = prms;

    [SpecialName]
    public object Call(CodeContext context, object func)
    {
      return (object) new ClrModule.RuntimeArgChecker(func, this.expected);
    }
  }

  public class RuntimeArgChecker : PythonTypeSlot
  {
    private object[] _expected;
    private object _func;
    private object _inst;

    public RuntimeArgChecker(object function, object[] expectedArgs)
    {
      this._expected = expectedArgs;
      this._func = function;
    }

    public RuntimeArgChecker(object instance, object function, object[] expectedArgs)
      : this(function, expectedArgs)
    {
      this._inst = instance;
    }

    private void ValidateArgs(object[] args)
    {
      int num = 0;
      if (this._inst != null)
        num = 1;
      for (int index = num; index < args.Length + num; ++index)
      {
        PythonType pythonType = DynamicHelpers.GetPythonType(args[index - num]);
        if (!(this._expected[index] is PythonType typeObject))
          typeObject = ((OldClass) this._expected[index]).TypeObject;
        if (pythonType != this._expected[index] && !pythonType.IsSubclassOf(typeObject))
          throw PythonOps.AssertionError("argument {0} has bad value (got {1}, expected {2})", (object) index, (object) pythonType, this._expected[index]);
      }
    }

    [SpecialName]
    public object Call(CodeContext context, params object[] args)
    {
      this.ValidateArgs(args);
      return this._inst != null ? PythonOps.CallWithContext(context, this._func, ArrayUtils.Insert<object>(this._inst, args)) : PythonOps.CallWithContext(context, this._func, args);
    }

    internal override bool TryGetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      out object value)
    {
      value = (object) new ClrModule.RuntimeArgChecker(instance, this._func, this._expected);
      return true;
    }

    internal override bool GetAlwaysSucceeds => true;

    [SpecialName]
    public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
    {
      this.ValidateArgs(args);
      return this._inst != null ? PythonCalls.CallWithKeywordArgs(context, this._func, ArrayUtils.Insert<object>(this._inst, args), dict) : PythonCalls.CallWithKeywordArgs(context, this._func, args, dict);
    }
  }

  public class ReturnChecker
  {
    public object retType;

    public ReturnChecker(object returnType) => this.retType = returnType;

    [SpecialName]
    public object Call(CodeContext context, object func)
    {
      return (object) new ClrModule.RuntimeReturnChecker(func, this.retType);
    }
  }

  public class RuntimeReturnChecker : PythonTypeSlot
  {
    private object _retType;
    private object _func;
    private object _inst;

    public RuntimeReturnChecker(object function, object expectedReturn)
    {
      this._retType = expectedReturn;
      this._func = function;
    }

    public RuntimeReturnChecker(object instance, object function, object expectedReturn)
      : this(function, expectedReturn)
    {
      this._inst = instance;
    }

    private void ValidateReturn(object ret)
    {
      if (ret == null && this._retType == null)
        return;
      PythonType pythonType = DynamicHelpers.GetPythonType(ret);
      if (pythonType == this._retType)
        return;
      if (!(this._retType is PythonType other))
        other = ((OldClass) this._retType).TypeObject;
      if (!pythonType.IsSubclassOf(other))
        throw PythonOps.AssertionError("bad return value returned (expected {0}, got {1})", this._retType, (object) pythonType);
    }

    [SpecialName]
    public object Call(CodeContext context, params object[] args)
    {
      object ret = this._inst == null ? PythonOps.CallWithContext(context, this._func, args) : PythonOps.CallWithContext(context, this._func, ArrayUtils.Insert<object>(this._inst, args));
      this.ValidateReturn(ret);
      return ret;
    }

    public object GetAttribute(object instance, object owner)
    {
      return (object) new ClrModule.RuntimeReturnChecker(instance, this._func, this._retType);
    }

    internal override bool TryGetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      out object value)
    {
      value = this.GetAttribute(instance, (object) owner);
      return true;
    }

    internal override bool GetAlwaysSucceeds => true;

    [SpecialName]
    public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
    {
      if (this._inst == null)
        return PythonCalls.CallWithKeywordArgs(context, this._func, args, dict);
      object ret = PythonCalls.CallWithKeywordArgs(context, this._func, ArrayUtils.Insert<object>(this._inst, args), dict);
      this.ValidateReturn(ret);
      return ret;
    }
  }

  [Serializable]
  internal sealed class FileStreamContentProvider : StreamContentProvider
  {
    private readonly string _path;
    private readonly ClrModule.FileStreamContentProvider.PALHolder _pal;

    internal string Path => this._path;

    internal FileStreamContentProvider(PlatformAdaptationLayer manager, string path)
    {
      ContractUtils.RequiresNotNull((object) path, nameof (path));
      this._path = path;
      this._pal = new ClrModule.FileStreamContentProvider.PALHolder(manager);
    }

    public override Stream GetStream() => this._pal.GetStream(this.Path);

    [Serializable]
    private class PALHolder : MarshalByRefObject
    {
      [NonSerialized]
      private readonly PlatformAdaptationLayer _pal;

      internal PALHolder(PlatformAdaptationLayer pal) => this._pal = pal;

      internal Stream GetStream(string path) => this._pal.OpenInputFileStream(path);
    }
  }
}
