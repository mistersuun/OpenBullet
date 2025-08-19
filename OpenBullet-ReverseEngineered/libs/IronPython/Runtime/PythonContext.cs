// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonContext
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Debugging;
using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

public sealed class PythonContext : LanguageContext
{
  internal const string IronPythonDisplayName = "IronPython 2.7.9";
  internal const string IronPythonNames = "IronPython;Python;py";
  internal const string IronPythonFileExtensions = ".py";
  private static readonly Guid PythonLanguageGuid = new Guid("03ed4b80-d10b-442f-ad9a-47dae85b2051");
  private static readonly Guid LanguageVendor_Microsoft = new Guid(-1723120188, (short) -6423, (short) 4562, (byte) 144 /*0x90*/, (byte) 63 /*0x3F*/, (byte) 0, (byte) 192 /*0xC0*/, (byte) 79, (byte) 163, (byte) 2, (byte) 161);
  private readonly IDictionary<object, object> _modulesDict = (IDictionary<object, object>) new PythonDictionary();
  private readonly Dictionary<string, ModuleGlobalCache> _builtinCache = new Dictionary<string, ModuleGlobalCache>((IEqualityComparer<string>) StringComparer.Ordinal);
  private readonly Dictionary<Type, string> _builtinModuleNames = new Dictionary<Type, string>();
  private readonly PythonOptions _options;
  private readonly PythonModule _systemState;
  private readonly Dictionary<string, Type> _builtinModulesDict;
  private readonly PythonOverloadResolverFactory _sharedOverloadResolverFactory;
  private readonly PythonBinder _binder;
  private readonly SysModuleDictionaryStorage _sysDict = new SysModuleDictionaryStorage();
  private readonly PythonContext.AssemblyResolveHolder _resolveHolder;
  private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();
  private Encoding _defaultEncoding = PythonAsciiEncoding.Instance;
  private PythonService _pythonService;
  private string _initialExecutable;
  private string _initialPrefix = PythonContext.GetInitialPrefix();
  private string _initialVersionString;
  private PythonModule _clrModule;
  private PythonDictionary _builtinDict;
  private PythonModule _builtins;
  private PythonFileManager _fileManager;
  private Dictionary<string, object> _errorHandlers;
  private List<object> _searchFunctions;
  private Dictionary<object, object> _moduleState;
  internal BuiltinFunction NewObject;
  internal BuiltinFunction PythonReconstructor;
  private Dictionary<Type, object> _genericSiteStorage;
  private CallSite<Func<CallSite, CodeContext, object, object>>[] _newUnarySites;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object>>[] _newTernarySites;
  private CallSite<Func<CallSite, object, object, int>> _compareSite;
  private Dictionary<PythonContext.AttrKey, CallSite<Func<CallSite, object, object, object>>> _setAttrSites;
  private Dictionary<PythonContext.AttrKey, CallSite<Action<CallSite, object>>> _deleteAttrSites;
  private CallSite<Func<CallSite, CodeContext, object, string, PythonTuple, PythonDictionary, object>> _metaClassSite;
  private CallSite<Func<CallSite, CodeContext, object, string, object>> _writeSite;
  private CallSite<Func<CallSite, object, object, object>> _getIndexSite;
  private CallSite<Func<CallSite, object, object, object>> _equalSite;
  private CallSite<Action<CallSite, object, object>> _delIndexSite;
  private CallSite<Func<CallSite, CodeContext, object, object>> _finalizerSite;
  private CallSite<Func<CallSite, CodeContext, PythonFunction, object>> _functionCallSite;
  private CallSite<Func<CallSite, object, object, bool>> _greaterThanSite;
  private CallSite<Func<CallSite, object, object, bool>> _lessThanSite;
  private CallSite<Func<CallSite, object, object, bool>> _greaterThanEqualSite;
  private CallSite<Func<CallSite, object, object, bool>> _lessThanEqualSite;
  private CallSite<Func<CallSite, object, object, bool>> _containsSite;
  private CallSite<Func<CallSite, CodeContext, object, object[], object>> _callSplatSite;
  private CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>> _callDictSite;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object>> _callDictSiteLooselyTyped;
  private CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>> _importSite;
  private CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>> _oldImportSite;
  private CallSite<Func<CallSite, object, bool>> _isCallableSite;
  private CallSite<Func<CallSite, object, IList<string>>> _getSignaturesSite;
  private CallSite<Func<CallSite, object, object, object>> _addSite;
  private CallSite<Func<CallSite, object, object, object>> _divModSite;
  private CallSite<Func<CallSite, object, object, object>> _rdivModSite;
  private CallSite<Func<CallSite, object, object, object, object>> _setIndexSite;
  private CallSite<Func<CallSite, object, object, object, object>> _delSliceSite;
  private CallSite<Func<CallSite, object, object, object, object, object>> _setSliceSite;
  private CallSite<Func<CallSite, object, string>> _docSite;
  private CallSite<Func<CallSite, object, int>> _intSite;
  private CallSite<Func<CallSite, object, string>> _tryStringSite;
  private CallSite<Func<CallSite, object, object>> _tryIntSite;
  private CallSite<Func<CallSite, object, IEnumerable>> _tryIEnumerableSite;
  private Dictionary<Type, CallSite<Func<CallSite, object, object>>> _implicitConvertSites;
  private Dictionary<PythonOperationKind, CallSite<Func<CallSite, object, object, object>>> _binarySites;
  private Dictionary<Type, PythonContext.DefaultPythonComparer> _defaultComparer;
  private CallSite<Func<CallSite, CodeContext, object, object, object, int>> _sharedFunctionCompareSite;
  private CallSite<Func<CallSite, CodeContext, PythonFunction, object, object, int>> _sharedPythonFunctionCompareSite;
  private CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object, int>> _sharedBuiltinFunctionCompareSite;
  private CallSite<Func<CallSite, CodeContext, object, int, object>> _getItemCallSite;
  private CallSite<Func<CallSite, CodeContext, object, object, object>> _propGetSite;
  private CallSite<Func<CallSite, CodeContext, object, object, object>> _propDelSite;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object>> _propSetSite;
  private CompiledLoader _compiledLoader;
  internal bool _importWarningThrows;
  private bool _importedEncodings;
  private Action<Action> _commandDispatcher;
  private IronPython.Runtime.ClrModule.ReferencesList _referencesList;
  private FloatFormat _floatFormat;
  private FloatFormat _doubleFormat;
  private CultureInfo _collateCulture;
  private CultureInfo _ctypeCulture;
  private CultureInfo _timeCulture;
  private CultureInfo _monetaryCulture;
  private CultureInfo _numericCulture;
  private CodeContext _defaultContext;
  private CodeContext _defaultClsContext;
  private readonly TopNamespaceTracker _topNamespace;
  private readonly IEqualityComparer<object> _equalityComparer;
  private readonly IEqualityComparer _equalityComparerNonGeneric;
  private Dictionary<Type, CallSite<Func<CallSite, object, object, bool>>> _equalSites;
  private Dictionary<Type, PythonSiteCache> _systemSiteCache;
  internal static object _syntaxErrorNoCaret = new object();
  private PythonInvokeBinder _invokeNoArgs;
  private PythonInvokeBinder _invokeOneArg;
  private Dictionary<CallSignature, PythonInvokeBinder> _invokeBinders;
  private Dictionary<string, PythonGetMemberBinder> _getMemberBinders;
  private Dictionary<string, PythonGetMemberBinder> _tryGetMemberBinders;
  private Dictionary<string, PythonSetMemberBinder> _setMemberBinders;
  private Dictionary<string, PythonDeleteMemberBinder> _deleteMemberBinders;
  private Dictionary<string, CompatibilityGetMember> _compatGetMember;
  private Dictionary<string, CompatibilityGetMember> _compatGetMemberNoThrow;
  private Dictionary<PythonOperationKind, PythonOperationBinder> _operationBinders;
  private Dictionary<ExpressionType, PythonUnaryOperationBinder> _unaryBinders;
  private PythonBinaryOperationBinder[] _binaryBinders;
  private Dictionary<PythonContext.OperationRetTypeKey<ExpressionType>, BinaryRetTypeBinder> _binaryRetTypeBinders;
  private Dictionary<PythonContext.OperationRetTypeKey<PythonOperationKind>, BinaryRetTypeBinder> _operationRetTypeBinders;
  private Dictionary<Type, PythonConversionBinder>[] _conversionBinders;
  private Dictionary<Type, DynamicMetaObjectBinder>[] _convertRetObjectBinders;
  private Dictionary<CallSignature, CreateFallback> _createBinders;
  private Dictionary<CallSignature, CompatibilityInvokeBinder> _compatInvokeBinders;
  private PythonGetSliceBinder _getSlice;
  private PythonSetSliceBinder _setSlice;
  private PythonDeleteSliceBinder _deleteSlice;
  private PythonGetIndexBinder[] _getIndexBinders;
  private PythonSetIndexBinder[] _setIndexBinders;
  private PythonDeleteIndexBinder[] _deleteIndexBinders;
  private DynamicMetaObjectBinder _invokeTwoConvertToInt;
  private static CultureInfo _CCulture;
  private DynamicDelegateCreator _delegateCreator;
  private DebugContext _debugContext;
  private Microsoft.Scripting.Debugging.TracePipeline _tracePipeline;
  private readonly Microsoft.Scripting.Utils.ThreadLocal<PythonTracebackListener> _tracebackListeners = new Microsoft.Scripting.Utils.ThreadLocal<PythonTracebackListener>();
  private int _tracebackListenersCount;
  internal FunctionCode.CodeList _allCodes;
  internal readonly object _codeCleanupLock = new object();
  internal readonly object _codeUpdateLock = new object();
  internal int _codeCount;
  internal int _nextCodeCleanup = 200;
  private int _recursionLimit;
  internal readonly List<FunctionStack> _mainThreadFunctionStack;
  private CallSite<Func<CallSite, CodeContext, object, object>> _callSite0LightEh;
  private List<WeakReference> _weakExtensionMethodSets;
  private CommonDictionaryStorage _systemPythonTypesWeakRefs = new CommonDictionaryStorage();
  private CallSite<Func<CallSite, CodeContext, object, object>> _callSite0;
  private CallSite<Func<CallSite, CodeContext, object, object, object>> _callSite1;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object>> _callSite2;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object, object>> _callSite3;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>> _callSite4;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>> _callSite5;
  private CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>> _callSite6;
  private Thread _mainThread;
  internal readonly HashDelegate InitialHasher;
  internal readonly HashDelegate IntHasher;
  internal readonly HashDelegate DoubleHasher;
  internal readonly HashDelegate StringHasher;
  internal readonly HashDelegate FallbackHasher;

  public PythonContext(ScriptDomainManager manager, IDictionary<string, object> options)
    : base(manager)
  {
    this._options = new PythonOptions(options);
    this._builtinModulesDict = this.CreateBuiltinTable();
    this._defaultContext = new ModuleContext(new PythonDictionary(), this).GlobalContext;
    this._systemState = new PythonModule(new PythonDictionary((DictionaryStorage) this._sysDict));
    this._systemState.__dict__[(object) "__name__"] = (object) "sys";
    this._systemState.__dict__[(object) "__package__"] = (object) null;
    PythonBinder binder = new PythonBinder(this, this._defaultContext);
    this._sharedOverloadResolverFactory = new PythonOverloadResolverFactory(binder, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this._defaultContext));
    this._binder = binder;
    CodeContext defaultClsContext = DefaultContext.CreateDefaultCLSContext(this);
    this._defaultClsContext = defaultClsContext;
    if (DefaultContext._default == null)
      DefaultContext.InitializeDefaults(this._defaultContext, defaultClsContext);
    this.InitializeBuiltins();
    this.InitializeSystemState();
    List list1;
    if (this._options.Arguments.Count != 0)
      list1 = new List((ICollection) this._options.Arguments);
    else
      list1 = new List((ICollection) new object[1]
      {
        (object) string.Empty
      });
    this.SetSystemStateValue("argv", (object) list1);
    if (this._options.WarningFilters.Count > 0)
      this._systemState.__dict__[(object) "warnoptions"] = (object) new List((ICollection) this._options.WarningFilters);
    if (this._options.Frames)
      this._systemState.__dict__[(object) "_getframe"] = (object) BuiltinFunction.MakeFunction("_getframe", ArrayUtils.ConvertAll<MemberInfo, MethodBase>(typeof (SysModule).GetMember("_getframeImpl"), (Func<MemberInfo, MethodBase>) (x => (MethodBase) x)), typeof (SysModule));
    if (this._options.Tracing)
      this.EnsureDebugContext();
    List list2 = new List((ICollection) this._options.SearchPaths);
    this._resolveHolder = new PythonContext.AssemblyResolveHolder(this);
    try
    {
      Assembly entryAssembly = Assembly.GetEntryAssembly();
      if (entryAssembly != (Assembly) null)
      {
        string directoryName = Path.GetDirectoryName(entryAssembly.Location);
        string path1 = Path.Combine(directoryName, "Lib");
        if (Directory.Exists(path1))
          list2.append((object) path1);
        string path2 = Path.Combine(directoryName, "DLLs");
        if (Directory.Exists(path2))
          list2.append((object) path2);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
          string[] strArray = new string[2]{ "lib", "DLLs" };
          string str1 = $"{2}.{7}.{9}";
          foreach (string str2 in strArray)
          {
            string path3 = $"/Library/Frameworks/IronPython.framework/Versions/{str1}/{str2}";
            if (Directory.Exists(path3))
              list2.append((object) path3);
          }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
          string[] strArray = new string[2]
          {
            "/usr/lib/ironpython2.7",
            "/usr/share/ironpython2.7/DLLs"
          };
          foreach (string path4 in strArray)
          {
            if (Directory.Exists(path4))
              list2.append((object) path4);
          }
        }
      }
    }
    catch (SecurityException ex)
    {
    }
    this._systemState.__dict__[(object) "path"] = (object) list2;
    this.RecursionLimit = this._options.RecursionLimit;
    object obj;
    if (options == null || !options.TryGetValue("NoAssemblyResolveHook", out obj) || !System.Convert.ToBoolean(obj))
    {
      try
      {
        this.HookAssemblyResolve();
      }
      catch (SecurityException ex)
      {
      }
    }
    this._equalityComparer = (IEqualityComparer<object>) new PythonContext.PythonEqualityComparer(this);
    this._equalityComparerNonGeneric = (IEqualityComparer) this._equalityComparer;
    this.InitialHasher = new HashDelegate(this.InitialHasherImpl);
    this.IntHasher = new HashDelegate(this.IntHasherImpl);
    this.DoubleHasher = new HashDelegate(this.DoubleHasherImpl);
    this.StringHasher = new HashDelegate(this.StringHasherImpl);
    this.FallbackHasher = new HashDelegate(this.FallbackHasherImpl);
    this._topNamespace = new TopNamespaceTracker(manager);
    foreach (Assembly loadedAssembly in (IEnumerable<Assembly>) manager.GetLoadedAssemblyList())
      this._topNamespace.LoadAssembly(loadedAssembly);
    manager.AssemblyLoaded += new EventHandler<AssemblyLoadedEventArgs>(this.ManagerAssemblyLoaded);
    this._mainThreadFunctionStack = PythonOps.GetFunctionStack();
  }

  private void ManagerAssemblyLoaded(object sender, AssemblyLoadedEventArgs e)
  {
    this._topNamespace.LoadAssembly(e.Assembly);
  }

  public int RecursionLimit
  {
    get => this._recursionLimit;
    set
    {
      if (value < 0)
        throw PythonOps.ValueError("recursion limit must be positive");
      lock (this._codeUpdateLock)
      {
        this._recursionLimit = value;
        if (this._recursionLimit == int.MaxValue == (value == int.MaxValue))
          return;
        FunctionCode.UpdateAllCode(this);
      }
    }
  }

  internal bool EnableTracing => this.PythonOptions.Tracing || this._tracebackListenersCount > 0;

  internal TopNamespaceTracker TopNamespace => this._topNamespace;

  public Thread MainThread
  {
    get => this._mainThread;
    set => this._mainThread = value;
  }

  public IEqualityComparer<object> EqualityComparer => this._equalityComparer;

  public IEqualityComparer EqualityComparerNonGeneric => this._equalityComparerNonGeneric;

  private int InitialHasherImpl(object o, ref HashDelegate dlg)
  {
    if (o == null)
      return 505032256;
    switch (o.GetType().GetTypeCode())
    {
      case TypeCode.Int32:
        dlg = this.IntHasher;
        return this.IntHasher(o, ref dlg);
      case TypeCode.Double:
        dlg = this.DoubleHasher;
        return this.DoubleHasher(o, ref dlg);
      case TypeCode.String:
        dlg = this.StringHasher;
        return this.StringHasher(o, ref dlg);
      default:
        dlg = !(o is IPythonObject) ? new HashDelegate(new PythonContext.OptimizedBuiltinHasher(this, o.GetType()).Hasher) : new HashDelegate(new PythonContext.OptimizedUserHasher(this, ((IPythonObject) o).PythonType).Hasher);
        return dlg(o, ref dlg);
    }
  }

  private int IntHasherImpl(object o, ref HashDelegate dlg)
  {
    if (o != null && o.GetType() == typeof (int))
      return o.GetHashCode();
    dlg = this.FallbackHasher;
    return this.FallbackHasher(o, ref dlg);
  }

  private int DoubleHasherImpl(object o, ref HashDelegate dlg)
  {
    if (o != null && o.GetType() == typeof (double))
      return DoubleOps.__hash__((double) o);
    dlg = this.FallbackHasher;
    return this.FallbackHasher(o, ref dlg);
  }

  private int StringHasherImpl(object o, ref HashDelegate dlg)
  {
    if (o != null && o.GetType() == typeof (string))
      return o.GetHashCode();
    dlg = this.FallbackHasher;
    return this.FallbackHasher(o, ref dlg);
  }

  private int FallbackHasherImpl(object o, ref HashDelegate dlg)
  {
    return PythonOps.Hash(this.SharedContext, o);
  }

  public override LanguageOptions Options => (LanguageOptions) this.PythonOptions;

  public bool HasModuleState(object key)
  {
    this.EnsureModuleState();
    lock (this._moduleState)
      return this._moduleState.ContainsKey(key);
  }

  private void EnsureModuleState()
  {
    if (this._moduleState != null)
      return;
    Interlocked.CompareExchange<Dictionary<object, object>>(ref this._moduleState, new Dictionary<object, object>(), (Dictionary<object, object>) null);
  }

  public object GetModuleState(object key)
  {
    this.EnsureModuleState();
    lock (this._moduleState)
      return this._moduleState[key];
  }

  public void SetModuleState(object key, object value)
  {
    this.EnsureModuleState();
    lock (this._moduleState)
      this._moduleState[key] = value;
  }

  public object GetSetModuleState(object key, object value)
  {
    this.EnsureModuleState();
    lock (this._moduleState)
    {
      object setModuleState;
      this._moduleState.TryGetValue(key, out setModuleState);
      this._moduleState[key] = value;
      return setModuleState;
    }
  }

  public T GetOrCreateModuleState<T>(object key, Func<T> value) where T : class
  {
    this.EnsureModuleState();
    lock (this._moduleState)
    {
      object moduleState;
      if (!this._moduleState.TryGetValue(key, out moduleState))
        this._moduleState[key] = (object) (__Boxed<T>) (moduleState = (object) value());
      return moduleState as T;
    }
  }

  public PythonType EnsureModuleException(
    object key,
    PythonDictionary dict,
    string name,
    string module)
  {
    return (PythonType) (dict[(object) name] = (object) this.GetOrCreateModuleState<PythonType>(key, (Func<PythonType>) (() => PythonExceptions.CreateSubType(this, PythonExceptions.Exception, name, module, (string) null, PythonType.DefaultMakeException))));
  }

  public PythonType EnsureModuleException(
    object key,
    PythonType baseType,
    PythonDictionary dict,
    string name,
    string module)
  {
    return (PythonType) (dict[(object) name] = (object) this.GetOrCreateModuleState<PythonType>(key, (Func<PythonType>) (() => PythonExceptions.CreateSubType(this, baseType, name, module, (string) null, PythonType.DefaultMakeException))));
  }

  public PythonType EnsureModuleException(
    object key,
    PythonType baseType,
    Type underlyingType,
    PythonDictionary dict,
    string name,
    string module,
    Func<string, Exception> exceptionMaker)
  {
    return (PythonType) (dict[(object) name] = (object) this.GetOrCreateModuleState<PythonType>(key, (Func<PythonType>) (() => PythonExceptions.CreateSubType(this, baseType, underlyingType, name, module, (string) null, exceptionMaker))));
  }

  public PythonType EnsureModuleException(
    object key,
    PythonType baseType,
    Type underlyingType,
    PythonDictionary dict,
    string name,
    string module,
    string documentation,
    Func<string, Exception> exceptionMaker)
  {
    return (PythonType) (dict[(object) name] = (object) this.GetOrCreateModuleState<PythonType>(key, (Func<PythonType>) (() => PythonExceptions.CreateSubType(this, baseType, underlyingType, name, module, documentation, exceptionMaker))));
  }

  public PythonType EnsureModuleException(
    object key,
    PythonType[] baseTypes,
    Type underlyingType,
    PythonDictionary dict,
    string name,
    string module)
  {
    return (PythonType) (dict[(object) name] = (object) this.GetOrCreateModuleState<PythonType>(key, (Func<PythonType>) (() => PythonExceptions.CreateSubType(this, baseTypes, underlyingType, name, module, (string) null, PythonType.DefaultMakeException))));
  }

  internal PythonOptions PythonOptions => this._options;

  public override Guid VendorGuid => PythonContext.LanguageVendor_Microsoft;

  public override Guid LanguageGuid => PythonContext.PythonLanguageGuid;

  public PythonModule SystemState => this._systemState;

  public PythonModule ClrModule
  {
    get
    {
      if (this._clrModule == null)
        Interlocked.CompareExchange<PythonModule>(ref this._clrModule, this.CreateBuiltinModule("clr"), (PythonModule) null);
      return this._clrModule;
    }
  }

  internal bool TryGetSystemPath(out List path)
  {
    object obj;
    path = !this.SystemState.__dict__.TryGetValue((object) nameof (path), out obj) ? (List) null : obj as List;
    return path != null;
  }

  internal object SystemStandardOut => this.GetSystemStateValue("stdout");

  internal object SystemStandardIn => this.GetSystemStateValue("stdin");

  internal object SystemStandardError => this.GetSystemStateValue("stderr");

  internal IDictionary<object, object> SystemStateModules => this._modulesDict;

  internal void UpdateExceptionInfo(object type, object value, object traceback)
  {
    this._sysDict.UpdateExceptionInfo(type, value, traceback);
  }

  internal void UpdateExceptionInfo(
    Exception clrException,
    object type,
    object value,
    List<DynamicStackFrame> traceback)
  {
    this._sysDict.UpdateExceptionInfo(clrException, type, value, traceback);
  }

  internal void ExceptionHandled() => this._sysDict.ExceptionHandled();

  internal PythonModule GetModuleByName(string name)
  {
    object obj;
    return this.SystemStateModules.TryGetValue((object) name, out obj) && obj is PythonModule pythonModule ? pythonModule : (PythonModule) null;
  }

  internal PythonModule GetModuleByPath(string path)
  {
    foreach (object obj in (IEnumerable<object>) this.SystemStateModules.Values)
    {
      if (obj is PythonModule moduleByPath && this.DomainManager.Platform.PathComparer.Compare(moduleByPath.GetFile(), path) == 0)
        return moduleByPath;
    }
    return (PythonModule) null;
  }

  public override Version LanguageVersion => PythonContext.GetPythonVersion();

  internal static Version GetPythonVersion()
  {
    return new AssemblyName(typeof (PythonContext).Assembly.FullName).Version;
  }

  internal FloatFormat FloatFormat
  {
    get => this._floatFormat;
    set => this._floatFormat = value;
  }

  internal FloatFormat DoubleFormat
  {
    get => this._doubleFormat;
    set => this._doubleFormat = value;
  }

  private void InitializeSystemState()
  {
    this.SetSystemStateValue("argv", (object) List.FromArrayNoCopy((object) string.Empty));
    this.SetSystemStateValue("modules", (object) this._modulesDict);
    this.InitializeSysFlags();
    this._modulesDict[(object) "sys"] = (object) this._systemState;
    this.SetSystemStateValue("path", (object) new List(3));
    this.SetStandardIO();
    SysModule.PerformModuleReload(this, this._systemState.__dict__);
  }

  internal bool EmitDebugSymbols(SourceUnit sourceUnit)
  {
    if (!sourceUnit.EmitDebugSymbols)
      return false;
    return this.PythonOptions.NoDebug == null || !this.PythonOptions.NoDebug.IsMatch(sourceUnit.Path);
  }

  private void InitializeSysFlags()
  {
    SysModule.SysFlags sysFlags = new SysModule.SysFlags();
    this.SetSystemStateValue("flags", (object) sysFlags);
    sysFlags.debug = this._options.Debug ? 1 : 0;
    sysFlags.py3k_warning = this._options.WarnPython30 ? 1 : 0;
    this.SetSystemStateValue("py3kwarning", (object) this._options.WarnPython30);
    switch (this._options.DivisionOptions)
    {
      case PythonDivisionOptions.New:
        sysFlags.division_new = 1;
        break;
      case PythonDivisionOptions.Warn:
        sysFlags.division_warning = 1;
        break;
      case PythonDivisionOptions.WarnAll:
        sysFlags.division_warning = 2;
        break;
    }
    sysFlags.inspect = sysFlags.interactive = this._options.Inspect ? 1 : 0;
    if (this._options.StripDocStrings)
      sysFlags.optimize = 2;
    else if (this._options.Optimize)
      sysFlags.optimize = 1;
    sysFlags.dont_write_bytecode = 1;
    this.SetSystemStateValue("dont_write_bytecode", (object) true);
    sysFlags.no_user_site = this._options.NoUserSite ? 1 : 0;
    sysFlags.no_site = this._options.NoSite ? 1 : 0;
    sysFlags.ignore_environment = this._options.IgnoreEnvironment ? 1 : 0;
    switch (this._options.IndentationInconsistencySeverity)
    {
      case Severity.Warning:
        sysFlags.tabcheck = 1;
        break;
      case Severity.Error:
        sysFlags.tabcheck = 2;
        break;
    }
    sysFlags.verbose = this._options.Verbose ? 1 : 0;
    sysFlags.unicode = 1;
    sysFlags.bytes_warning = this._options.BytesWarning ? 1 : 0;
  }

  internal bool ShouldInterpret(PythonCompilerOptions options, SourceUnit source)
  {
    bool flag = !this._options.NoAdaptiveCompilation && !this.EmitDebugSymbols(source);
    return options.Interpreted | flag;
  }

  private static PythonAst ParseAndBindAst(CompilerContext context)
  {
    ScriptCodeParseResult properties = ScriptCodeParseResult.Complete;
    bool flag = false;
    int num = 0;
    PythonAst andBindAst;
    using (Parser parser = Parser.CreateParser(context, PythonContext.GetPythonOptions((CodeContext) null)))
    {
      switch (context.SourceUnit.Kind)
      {
        case SourceCodeKind.Expression:
          andBindAst = parser.ParseTopExpression();
          break;
        case SourceCodeKind.Statements:
          andBindAst = parser.ParseFile(false, false);
          break;
        case SourceCodeKind.SingleStatement:
          andBindAst = parser.ParseSingleStatement();
          break;
        case SourceCodeKind.File:
          andBindAst = parser.ParseFile(true, false);
          break;
        case SourceCodeKind.InteractiveCode:
          andBindAst = parser.ParseInteractiveCode(out properties);
          flag = true;
          break;
        default:
          andBindAst = parser.ParseFile(true, true);
          break;
      }
      num = parser.ErrorCode;
    }
    if (!flag && num != 0)
      properties = ScriptCodeParseResult.Invalid;
    context.SourceUnit.CodeProperties = new ScriptCodeParseResult?(properties);
    if (num != 0 || properties == ScriptCodeParseResult.Empty)
      return (PythonAst) null;
    andBindAst.Bind();
    return andBindAst;
  }

  internal static ScriptCode CompilePythonCode(
    SourceUnit sourceUnit,
    CompilerOptions options,
    ErrorSink errorSink)
  {
    PythonCompilerOptions pythonCompilerOptions = (PythonCompilerOptions) options;
    if (sourceUnit.Kind == SourceCodeKind.File)
      pythonCompilerOptions.Module |= ModuleOptions.Initialize;
    return PythonContext.ParseAndBindAst(new CompilerContext(sourceUnit, options, errorSink))?.ToScriptCode();
  }

  public override ScriptCode CompileSourceCode(
    SourceUnit sourceUnit,
    CompilerOptions options,
    ErrorSink errorSink)
  {
    ScriptCode scriptCode = PythonContext.CompilePythonCode(sourceUnit, options, errorSink);
    if (scriptCode != null)
    {
      PythonScopeExtension extension = (PythonScopeExtension) scriptCode.CreateScope().GetExtension(this.ContextId);
      if (extension != null)
        this.InitializeModule(sourceUnit.Path, extension.ModuleContext, scriptCode, ModuleOptions.None);
    }
    return scriptCode;
  }

  public override ScriptCode LoadCompiledCode(Delegate method, string path, string customData)
  {
    if (Path.DirectorySeparatorChar != '\\')
      path = path.Replace('\\', Path.DirectorySeparatorChar);
    if (Path.DirectorySeparatorChar != '/')
      path = path.Replace('/', Path.DirectorySeparatorChar);
    SourceUnit sourceUnit = new SourceUnit((LanguageContext) this, (TextContentProvider) NullTextContentProvider.Null, path, SourceCodeKind.File);
    return (ScriptCode) new OnDiskScriptCode((LookupCompilationDelegate) method, sourceUnit, customData);
  }

  public override SourceCodeReader GetSourceReader(
    Stream stream,
    Encoding defaultEncoding,
    string path)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) defaultEncoding, nameof (defaultEncoding));
    ContractUtils.Requires(stream.CanSeek && stream.CanRead, nameof (stream), "The stream must support seeking and reading");
    Encoding sourceEncoding = PythonAsciiEncoding.SourceEncoding;
    long position = stream.Position;
    StreamReader reader = new StreamReader(stream, PythonAsciiEncoding.SourceEncoding);
    byte[] buffer = new byte[3];
    int num = stream.Read(buffer, 0, 3);
    int totalRead = 0;
    bool flag1 = false;
    if (num == 3 && buffer[0] == (byte) 239 && buffer[1] == (byte) 187 && buffer[2] == (byte) 191)
    {
      flag1 = true;
      totalRead = 3;
    }
    else
      stream.Seek(0L, SeekOrigin.Begin);
    string line1;
    try
    {
      line1 = PythonContext.ReadOneLine(reader, ref totalRead);
    }
    catch (BadSourceException ex)
    {
      throw PythonContext.ReportEncodingError(stream, path);
    }
    bool flag2 = false;
    string encName = (string) null;
    if (line1 != null)
    {
      if (!(flag2 = Tokenizer.TryGetEncoding(defaultEncoding, line1, ref sourceEncoding, out encName)))
      {
        string line2;
        try
        {
          line2 = PythonContext.ReadOneLine(reader, ref totalRead);
        }
        catch (BadSourceException ex)
        {
          throw PythonContext.ReportEncodingError(stream, path);
        }
        if (line2 != null)
          flag2 = Tokenizer.TryGetEncoding(defaultEncoding, line2, ref sourceEncoding, out encName);
      }
    }
    if (flag2 & flag1 && encName != "utf-8")
      throw new IOException("file has both Unicode marker and PEP-263 file encoding.  You can only use \"utf-8\" as the encoding name when a BOM is present.");
    if (sourceEncoding == null)
      throw new IOException("unknown encoding type");
    if (!flag2 || stream.Position != stream.Length)
      stream.Seek(position, SeekOrigin.Begin);
    return new SourceCodeReader((TextReader) new StreamReader(stream, sourceEncoding), sourceEncoding);
  }

  internal static Exception ReportEncodingError(Stream stream, string path)
  {
    stream.Seek(0L, SeekOrigin.Begin);
    byte[] buffer = new byte[1024 /*0x0400*/];
    int line = 1;
    int column = 1;
    int index1 = 0;
    int num;
    while ((num = stream.Read(buffer, 0, buffer.Length)) != -1)
    {
      for (int index2 = 0; index2 < num; ++index2)
      {
        if (buffer[index2] > (byte) 127 /*0x7F*/)
          return (Exception) PythonOps.BadSourceError(buffer[index2], new SourceSpan(new SourceLocation(index1, line, column), new SourceLocation(index1, line, column)), path);
        if (buffer[index2] == (byte) 10)
        {
          ++line;
          column = 1;
        }
        else
          ++column;
        ++index1;
      }
    }
    return (Exception) new InvalidOperationException();
  }

  private static string ReadOneLine(StreamReader reader, ref int totalRead)
  {
    Stream baseStream = reader.BaseStream;
    byte[] numArray = new byte[256 /*0x0100*/];
    StringBuilder stringBuilder = (StringBuilder) null;
    for (int count = baseStream.Read(numArray, 0, numArray.Length); count > 0; count = baseStream.Read(numArray, 0, numArray.Length))
    {
      totalRead += count;
      bool flag = false;
      for (int index = 0; index < count; ++index)
      {
        if (numArray[index] == (byte) 13)
        {
          if (index + 1 < count)
          {
            if (numArray[index + 1] == (byte) 10)
            {
              totalRead -= count - (index + 2);
              baseStream.Seek((long) (index + 2), SeekOrigin.Begin);
              reader.DiscardBufferedData();
              flag = true;
            }
          }
          else
          {
            totalRead -= count - (index + 1);
            baseStream.Seek((long) (index + 1), SeekOrigin.Begin);
            reader.DiscardBufferedData();
            flag = true;
          }
        }
        else if (numArray[index] == (byte) 10)
        {
          totalRead -= count - (index + 1);
          baseStream.Seek((long) (index + 1), SeekOrigin.Begin);
          reader.DiscardBufferedData();
          flag = true;
        }
        if (flag)
        {
          if (stringBuilder == null)
            return ((IList<byte>) numArray).MakeString().Substring(0, index);
          stringBuilder.Append(((IList<byte>) numArray).MakeString(), 0, index);
          return stringBuilder.ToString();
        }
      }
      if (stringBuilder == null)
        stringBuilder = new StringBuilder();
      stringBuilder.Append(((IList<byte>) numArray).MakeString(), 0, count);
    }
    return stringBuilder?.ToString();
  }

  public override SourceUnit GenerateSourceCode(
    CodeObject codeDom,
    string path,
    SourceCodeKind kind)
  {
    return new PythonCodeDomCodeGen().GenerateCode((CodeMemberMethod) codeDom, (LanguageContext) this, path, kind);
  }

  public override Scope GetScope(string path) => this.GetModuleByPath(path)?.Scope;

  public PythonModule InitializeModule(
    string fileName,
    ModuleContext moduleContext,
    ScriptCode scriptCode,
    ModuleOptions options)
  {
    if ((options & ModuleOptions.NoBuiltins) == ModuleOptions.None)
      moduleContext.InitializeBuiltins((options & ModuleOptions.ModuleBuiltins) != 0);
    if (fileName != null && Path.GetFileName(fileName) == "__init__.py")
    {
      string fullPath = this.DomainManager.Platform.GetFullPath(Path.GetDirectoryName(fileName));
      moduleContext.Globals[(object) "__path__"] = (object) PythonOps.MakeList((object) fullPath);
    }
    moduleContext.ShowCls = (options & ModuleOptions.ShowClsMethods) != 0;
    moduleContext.Features = options;
    if ((options & ModuleOptions.Initialize) != ModuleOptions.None)
    {
      scriptCode.Run(moduleContext.GlobalScope);
      if (!moduleContext.Globals.ContainsKey((object) "__package__"))
        moduleContext.Globals[(object) "__package__"] = (object) null;
    }
    return moduleContext.Module;
  }

  public override ScopeExtension CreateScopeExtension(Scope scope)
  {
    PythonScopeExtension scopeExtension = new PythonScopeExtension(this, scope);
    scopeExtension.ModuleContext.InitializeBuiltins(false);
    return (ScopeExtension) scopeExtension;
  }

  public PythonModule CompileModule(
    string fileName,
    string moduleName,
    SourceUnit sourceCode,
    ModuleOptions options)
  {
    return this.CompileModule(fileName, moduleName, sourceCode, options, out ScriptCode _);
  }

  public PythonModule CompileModule(
    string fileName,
    string moduleName,
    SourceUnit sourceCode,
    ModuleOptions options,
    out ScriptCode scriptCode)
  {
    ContractUtils.RequiresNotNull((object) fileName, nameof (fileName));
    ContractUtils.RequiresNotNull((object) moduleName, nameof (moduleName));
    ContractUtils.RequiresNotNull((object) sourceCode, nameof (sourceCode));
    scriptCode = this.GetScriptCode(sourceCode, moduleName, options);
    Scope scope = scriptCode.CreateScope();
    return this.InitializeModule(fileName, ((PythonScopeExtension) scope.GetExtension(this.ContextId)).ModuleContext, scriptCode, options);
  }

  internal ScriptCode GetScriptCode(
    SourceUnit sourceCode,
    string moduleName,
    ModuleOptions options)
  {
    return this.GetScriptCode(sourceCode, moduleName, options, (CompilationMode) null);
  }

  internal ScriptCode GetScriptCode(
    SourceUnit sourceCode,
    string moduleName,
    ModuleOptions options,
    CompilationMode mode)
  {
    PythonCompilerOptions pythonCompilerOptions = this.GetPythonCompilerOptions();
    pythonCompilerOptions.SkipFirstLine = (options & ModuleOptions.SkipFirstLine) != 0;
    pythonCompilerOptions.ModuleName = moduleName;
    pythonCompilerOptions.Module = options;
    pythonCompilerOptions.CompilationMode = mode;
    return PythonContext.CompilePythonCode(sourceCode, (CompilerOptions) pythonCompilerOptions, (ErrorSink) ThrowingErrorSink.Default);
  }

  internal PythonModule GetBuiltinModule(string name)
  {
    lock (this)
    {
      PythonModule builtinModule = this.CreateBuiltinModule(name);
      if (builtinModule == null)
        return (PythonModule) null;
      this.PublishModule(name, builtinModule);
      return builtinModule;
    }
  }

  internal PythonModule CreateBuiltinModule(string name)
  {
    Type type;
    if (!this.BuiltinModules.TryGetValue(name, out type))
      return (PythonModule) null;
    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    return this.CreateBuiltinModule(name, type);
  }

  internal PythonModule CreateBuiltinModule(string moduleName, Type type)
  {
    PythonDictionary pythonDictionary;
    if (type.IsSubclassOf(typeof (BuiltinPythonModule)))
    {
      BuiltinPythonModule instance = (BuiltinPythonModule) Activator.CreateInstance(type, (object) this);
      Dictionary<string, PythonGlobal> dictionary = new Dictionary<string, PythonGlobal>();
      pythonDictionary = new PythonDictionary((DictionaryStorage) new InstancedModuleDictionaryStorage(instance, dictionary));
      IEnumerable<string> globalVariableNames = instance.GetGlobalVariableNames();
      CodeContext globalContext = new ModuleContext(pythonDictionary, this).GlobalContext;
      foreach (string str in globalVariableNames)
        dictionary[str] = new PythonGlobal(globalContext, str);
      instance.Initialize(globalContext, dictionary);
    }
    else
    {
      pythonDictionary = new PythonDictionary((DictionaryStorage) new ModuleDictionaryStorage(type));
      if (type == typeof (Builtin))
        Builtin.PerformModuleReload(this, pythonDictionary);
      else if (type != typeof (SysModule))
      {
        MethodInfo method = type.GetMethod("PerformModuleReload");
        if (method != (MethodInfo) null)
          method.Invoke((object) null, new object[2]
          {
            (object) this,
            (object) pythonDictionary
          });
      }
    }
    return new PythonModule(pythonDictionary)
    {
      __dict__ = {
        [(object) "__name__"] = (object) moduleName,
        [(object) "__package__"] = (object) null
      }
    };
  }

  public void PublishModule(string name, PythonModule module)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) module, nameof (module));
    this.SystemStateModules[(object) name] = (object) module;
  }

  internal PythonModule GetReloadableModule(PythonModule module)
  {
    object key;
    if (!module.__dict__._storage.TryGetName(out key) || !(key is string))
      throw PythonOps.SystemError("nameless module");
    if (!this.SystemStateModules.ContainsKey(key))
      throw PythonOps.ImportError("module {0} not in sys.modules", key);
    return module;
  }

  public object GetWarningsModule()
  {
    object warningsModule = (object) null;
    try
    {
      if (!this._importWarningThrows)
        warningsModule = Importer.ImportModule(this.SharedContext, (object) new PythonDictionary(), "warnings", false, -1);
    }
    catch
    {
      this._importWarningThrows = true;
    }
    return warningsModule;
  }

  public void EnsureEncodings()
  {
    if (this._importedEncodings)
      return;
    try
    {
      Importer.ImportModule(this.SharedContext, (object) new PythonDictionary(), "encodings", false, -1);
    }
    catch (ImportException ex)
    {
    }
    this._importedEncodings = true;
  }

  internal ModuleGlobalCache GetModuleGlobalCache(string name)
  {
    ModuleGlobalCache cache;
    if (!this.TryGetModuleGlobalCache(name, out cache))
      cache = ModuleGlobalCache.NoCache;
    return cache;
  }

  internal Assembly LoadAssemblyFromFile(string file)
  {
    List path1;
    if (this.TryGetSystemPath(out path1))
    {
      IEnumerator enumerator = PythonOps.GetEnumerator((object) path1);
      while (enumerator.MoveNext())
      {
        string res1;
        if (this.TryConvertToString(enumerator.Current, out res1))
        {
          string path2 = Path.Combine(res1, file);
          Assembly res2;
          if (this.TryLoadAssemblyFromFileWithPath(path2, out res2) || this.TryLoadAssemblyFromFileWithPath(path2 + ".exe", out res2) || this.TryLoadAssemblyFromFileWithPath(path2 + ".dll", out res2))
            return res2;
        }
      }
    }
    return (Assembly) null;
  }

  internal bool TryLoadAssemblyFromFileWithPath(string path, out Assembly res)
  {
    if (File.Exists(path) && Path.IsPathRooted(path))
    {
      res = Assembly.LoadFile(path);
      if (res != (Assembly) null)
      {
        this._loadedAssemblies.Add(res);
        return true;
      }
    }
    res = (Assembly) null;
    return false;
  }

  internal Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
  {
    if (args.RequestingAssembly != (Assembly) null && !this._loadedAssemblies.Contains(args.RequestingAssembly))
      return (Assembly) null;
    AssemblyName assemblyName = new AssemblyName(args.Name);
    try
    {
      return this.LoadAssemblyFromFile(assemblyName.Name);
    }
    catch
    {
      return (Assembly) null;
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private void HookAssemblyResolve()
  {
    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this._resolveHolder.AssemblyResolveEvent);
  }

  private void UnhookAssemblyResolve()
  {
    try
    {
      AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this._resolveHolder.AssemblyResolveEvent);
    }
    catch (SecurityException ex)
    {
    }
  }

  public override ICollection<string> GetSearchPaths()
  {
    List<string> searchPaths = new List<string>();
    List path;
    if (this.TryGetSystemPath(out path))
    {
      IEnumerator enumerator = PythonOps.GetEnumerator((object) path);
      while (enumerator.MoveNext())
      {
        string res;
        if (this.TryConvertToString(enumerator.Current, out res))
          searchPaths.Add(res);
      }
    }
    return (ICollection<string>) searchPaths;
  }

  public override void SetSearchPaths(ICollection<string> paths)
  {
    this.SetSystemStateValue("path", (object) new List((object) paths));
  }

  public override void Shutdown()
  {
    this.UnhookAssemblyResolve();
    object obj;
    object func;
    if (this.SystemStateModules.TryGetValue((object) "threading", out obj) && obj is PythonModule)
    {
      if (((PythonModule) obj).__dict__.TryGetValue((object) "_shutdown", out func))
      {
        try
        {
          PythonCalls.Call(this.SharedContext, func);
        }
        catch (Exception ex)
        {
          PythonOps.PrintWithDest(this.SharedContext, this.SystemStandardError, (object) $"Exception {this.FormatException(ex)} ignored");
        }
      }
    }
    try
    {
      if (!this._systemState.__dict__.TryGetValue((object) "exitfunc", out func))
        return;
      PythonCalls.Call(this.SharedContext, func);
    }
    finally
    {
      if (this.PythonOptions.PerfStats)
        PerfTrack.DumpStats();
    }
  }

  public override string FormatException(Exception exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    if (exception is SyntaxErrorException e)
      return PythonContext.FormatPythonSyntaxError(e);
    object python = PythonExceptions.ToPython(exception);
    string str = this.FormatStackTraces(exception) + PythonContext.FormatPythonException(python);
    if (this.Options.ShowClrExceptions)
      str += PythonContext.FormatCLSException(exception);
    return str;
  }

  internal static string FormatPythonSyntaxError(SyntaxErrorException e)
  {
    string str1 = PythonContext.GetSourceLine(e);
    int num1 = 0;
    if (str1 != null)
    {
      int length = str1.Length;
      string str2 = str1.TrimStart();
      num1 = length - str2.Length;
      str1 = str2.TrimEnd();
    }
    string symbolDocumentName = e.GetSymbolDocumentName();
    bool flag = e.GetData(PythonContext._syntaxErrorNoCaret) == null;
    int num2 = str1 == null ? 0 : (flag ? 1 : (symbolDocumentName != "<stdin>" ? 1 : 0));
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine($"  File \"{symbolDocumentName}\", line {(e.Line > 0 ? e.Line.ToString() : " ? ")}");
    if (num2 != 0)
      stringBuilder.AppendLine("    " + str1);
    if (flag)
      stringBuilder.AppendLine($"    {new string(' ', Math.Max(e.Column - 1 - num1, 0))}^");
    stringBuilder.Append($"{PythonContext.GetPythonExceptionClassName(PythonExceptions.ToPython((Exception) e))}: {e.Message}");
    return stringBuilder.ToString();
  }

  internal static string GetSourceLine(SyntaxErrorException e)
  {
    if (e.SourceCode == null)
      return (string) null;
    try
    {
      using (StringReader stringReader = new StringReader(e.SourceCode))
      {
        char[] buffer = new char[80 /*0x50*/];
        int num1 = 1;
        StringBuilder stringBuilder = new StringBuilder();
        int num2;
        while ((num2 = stringReader.Read(buffer, 0, buffer.Length)) > 0 && num1 <= e.Line)
        {
          for (int index = 0; index < num2; ++index)
          {
            if (num1 == e.Line)
              stringBuilder.Append(buffer[index]);
            if (buffer[index] == '\n')
              ++num1;
            if (num1 > e.Line)
              break;
          }
        }
        return stringBuilder.ToString();
      }
    }
    catch (IOException ex)
    {
      return (string) null;
    }
  }

  private static string FormatCLSException(Exception e)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("CLR Exception: ");
    for (; e != null; e = e.InnerException)
    {
      stringBuilder.Append("    ");
      stringBuilder.AppendLine(e.GetType().Name);
      if (!string.IsNullOrEmpty(e.Message))
      {
        stringBuilder.AppendLine(": ");
        stringBuilder.AppendLine(e.Message);
      }
      else
        stringBuilder.AppendLine();
    }
    return stringBuilder.ToString();
  }

  internal static string FormatPythonException(object pythonException)
  {
    string str1 = "";
    if (pythonException != null)
    {
      if (pythonException is string str3)
      {
        str1 += str3;
      }
      else
      {
        str1 += PythonContext.GetPythonExceptionClassName(pythonException);
        string str2 = PythonOps.ToString(pythonException);
        if (!string.IsNullOrEmpty(str2))
          str1 = $"{str1}: {str2}";
      }
    }
    return str1;
  }

  private static string GetPythonExceptionClassName(object pythonException)
  {
    string exceptionClassName = "";
    object ret;
    if (PythonOps.TryGetBoundAttr(pythonException, "__class__", out ret) && PythonOps.TryGetBoundAttr(ret, "__name__", out ret))
    {
      exceptionClassName = ret.ToString();
      if (PythonOps.TryGetBoundAttr(pythonException, "__module__", out ret))
      {
        string str = ret.ToString();
        if (str != "exceptions")
          exceptionClassName = $"{str}.{exceptionClassName}";
      }
    }
    return exceptionClassName;
  }

  public override IList<DynamicStackFrame> GetStackFrames(Exception exception)
  {
    return (IList<DynamicStackFrame>) PythonOps.GetDynamicStackFrames(exception);
  }

  private string FormatStackTraces(Exception e)
  {
    bool printedHeader = false;
    return this.FormatStackTraces(e, ref printedHeader);
  }

  private string FormatStackTraces(Exception e, ref bool printedHeader)
  {
    string str = "";
    if (this.Options.ExceptionDetail)
    {
      if (!printedHeader)
      {
        str = e.Message + Environment.NewLine;
        printedHeader = true;
      }
      IList<StackTrace> exceptionStackTraces = ExceptionHelpers.GetExceptionStackTraces(e);
      if (exceptionStackTraces != null)
      {
        foreach (StackTrace stackTrace in (IEnumerable<StackTrace>) exceptionStackTraces)
        {
          foreach (StackFrame frame in stackTrace.GetFrames())
            str = str + frame.ToString() + Environment.NewLine;
        }
      }
      if (e.StackTrace != null)
        str = str + e.StackTrace.ToString() + Environment.NewLine;
      if (e.InnerException != null)
        str += this.FormatStackTraces(e.InnerException, ref printedHeader);
    }
    else
      str = this.FormatStackTraceNoDetail(e, ref printedHeader);
    return str;
  }

  internal string FormatStackTraceNoDetail(Exception e, ref bool printedHeader)
  {
    string str = string.Empty;
    if (e.InnerException != null)
      str += this.FormatStackTraceNoDetail(e.InnerException, ref printedHeader);
    if (!printedHeader)
    {
      str = $"{str}Traceback (most recent call last):{Environment.NewLine}";
      printedHeader = true;
    }
    TraceBack traceBack = e.GetTraceBack();
    if (traceBack != null)
      return str + traceBack.Extract();
    DynamicStackFrame[] dynamicStackFrames = PythonExceptions.GetDynamicStackFrames(e);
    for (int index = dynamicStackFrames.Length - 1; index >= 0; --index)
    {
      DynamicStackFrame frame = dynamicStackFrames[index];
      MethodBase method = frame.GetMethod();
      if (!CallSiteHelpers.IsInternalFrame(method) && (!(method.DeclaringType != (Type) null) || !method.DeclaringType.FullName.StartsWith("IronPython.")))
        str = str + PythonContext.FrameToString(frame) + Environment.NewLine;
    }
    return str;
  }

  private static string FrameToString(DynamicStackFrame frame)
  {
    string methodName = frame.GetMethodName();
    int fileLineNumber = frame.GetFileLineNumber();
    string str = frame.GetFileName();
    if (str == "-c")
      str = "<string>";
    return $"  File \"{str}\", line {(fileLineNumber == 0 ? (object) "unknown" : (object) fileLineNumber.ToString())}, in {methodName}";
  }

  public override TService GetService<TService>(params object[] args)
  {
    if (typeof (TService) == typeof (TokenizerService))
      return (TService) new Tokenizer(ErrorSink.Null, this.GetPythonCompilerOptions(), true);
    if (typeof (TService) == typeof (PythonService))
      return (TService) this.GetPythonService((ScriptEngine) args[0]);
    return typeof (TService) == typeof (DocumentationProvider) ? (TService) new PythonDocumentationProvider(this) : base.GetService<TService>(args);
  }

  internal PythonService GetPythonService(ScriptEngine engine)
  {
    if (this._pythonService == null)
      Interlocked.CompareExchange<PythonService>(ref this._pythonService, new PythonService(this, engine), (PythonService) null);
    return this._pythonService;
  }

  internal static PythonOptions GetPythonOptions(CodeContext context)
  {
    return DefaultContext.DefaultPythonContext._options;
  }

  internal void InsertIntoPath(int index, string directory)
  {
    List path;
    if (!this.TryGetSystemPath(out path))
      return;
    path.insert(index, (object) directory);
  }

  internal void AddToPath(string directory)
  {
    List path;
    if (!this.TryGetSystemPath(out path))
      return;
    path.append((object) directory);
  }

  internal void AddToPath(string directory, int index)
  {
    List path;
    if (!this.TryGetSystemPath(out path))
      return;
    path.insert(index, (object) directory);
  }

  internal PythonCompilerOptions GetPythonCompilerOptions()
  {
    ModuleOptions features = ModuleOptions.None;
    if (this.PythonOptions.DivisionOptions == PythonDivisionOptions.New)
      features |= ModuleOptions.TrueDivision;
    return new PythonCompilerOptions(features);
  }

  public override CompilerOptions GetCompilerOptions()
  {
    return (CompilerOptions) this.GetPythonCompilerOptions();
  }

  public override CompilerOptions GetCompilerOptions(Scope scope)
  {
    PythonCompilerOptions pythonCompilerOptions = this.GetPythonCompilerOptions();
    PythonScopeExtension extension = (PythonScopeExtension) scope.GetExtension(this.ContextId);
    if (extension != null)
      pythonCompilerOptions.Module |= extension.ModuleContext.Features;
    return (CompilerOptions) pythonCompilerOptions;
  }

  public override void GetExceptionMessage(
    Exception exception,
    out string message,
    out string typeName)
  {
    object python = PythonExceptions.ToPython(exception);
    message = PythonContext.FormatPythonException(PythonExceptions.ToPython(exception));
    typeName = PythonContext.GetPythonExceptionClassName(python);
  }

  public Encoding DefaultEncoding
  {
    get => this._defaultEncoding;
    set => this._defaultEncoding = value;
  }

  public string GetDefaultEncodingName()
  {
    return this.DefaultEncoding.WebName.ToLower().Replace('-', '_');
  }

  internal Dictionary<string, Type> BuiltinModules => this._builtinModulesDict;

  internal Dictionary<Type, string> BuiltinModuleNames => this._builtinModuleNames;

  private void InitializeBuiltins()
  {
    PythonDictionary dict = new PythonDictionary((DictionaryStorage) new BuiltinsDictionaryStorage(new EventHandler<ModuleChangeEventArgs>(this.BuiltinsChanged)));
    Builtin.PerformModuleReload(this, dict);
    this._builtinDict = dict;
    this._builtins = new PythonModule(dict);
    this._modulesDict[(object) "__builtin__"] = (object) this._builtins;
  }

  private Dictionary<string, Type> CreateBuiltinTable()
  {
    Dictionary<string, Type> builtinTable = new Dictionary<string, Type>();
    this.LoadBuiltins(builtinTable, typeof (PythonContext).Assembly, false);
    Assembly assem = (Assembly) null;
    try
    {
      assem = this.DomainManager.Platform.LoadAssembly(PythonContext.GetIronPythonAssembly("IronPython.Modules"));
    }
    catch (FileNotFoundException ex)
    {
    }
    if (assem != (Assembly) null)
    {
      this.LoadBuiltins(builtinTable, assem, false);
      if (Environment.OSVersion.Platform == PlatformID.Unix)
      {
        builtinTable["posix"] = builtinTable["nt"];
        builtinTable.Remove("nt");
      }
    }
    return builtinTable;
  }

  internal void LoadBuiltins(Dictionary<string, Type> builtinTable, Assembly assem, bool updateSys)
  {
    object[] customAttributes = assem.GetCustomAttributes(typeof (PythonModuleAttribute), false);
    if (!((IEnumerable<object>) customAttributes).Any<object>())
      return;
    foreach (PythonModuleAttribute pythonModuleAttribute in customAttributes)
    {
      if (pythonModuleAttribute.IsPlatformValid)
      {
        builtinTable[pythonModuleAttribute.Name] = pythonModuleAttribute.Type;
        this.BuiltinModuleNames[pythonModuleAttribute.Type] = pythonModuleAttribute.Name;
      }
    }
    if (!updateSys)
      return;
    SysModule.PublishBuiltinModuleNames(this, this._systemState.__dict__);
  }

  public static string GetIronPythonAssembly(string baseName)
  {
    ContractUtils.RequiresNotNull((object) baseName, nameof (baseName));
    string fullName = typeof (PythonContext).Assembly.FullName;
    int startIndex = fullName.IndexOf(',');
    return startIndex <= 0 ? baseName : baseName + fullName.Substring(startIndex);
  }

  public PythonModule BuiltinModuleInstance => this._builtins;

  public PythonDictionary BuiltinModuleDict => this._builtinDict;

  private void BuiltinsChanged(object sender, ModuleChangeEventArgs e)
  {
    lock (this._builtinCache)
    {
      ModuleGlobalCache moduleGlobalCache;
      if (this._builtinCache.TryGetValue(e.Name, out moduleGlobalCache))
      {
        switch (e.ChangeType)
        {
          case ModuleChangeType.Set:
            moduleGlobalCache.Value = e.Value;
            break;
          case ModuleChangeType.Delete:
            moduleGlobalCache.Value = (object) Uninitialized.Instance;
            break;
        }
      }
      else
      {
        object obj = e.ChangeType == ModuleChangeType.Set ? e.Value : (object) Uninitialized.Instance;
        this._builtinCache[e.Name] = new ModuleGlobalCache(obj);
      }
    }
  }

  internal bool TryGetModuleGlobalCache(string name, out ModuleGlobalCache cache)
  {
    lock (this._builtinCache)
    {
      if (!this._builtinCache.TryGetValue(name, out cache))
      {
        object obj;
        if (this.BuiltinModuleInstance.__dict__.TryGetValue((object) name, out obj))
          this._builtinCache[name] = cache = new ModuleGlobalCache(obj);
      }
    }
    return cache != null;
  }

  internal void SetHostVariables(string prefix, string executable, string versionString)
  {
    this._initialVersionString = !string.IsNullOrEmpty(versionString) ? versionString : PythonContext.GetVersionString();
    this._initialExecutable = executable ?? "";
    this._initialPrefix = prefix;
    this.AddToPath(Path.Combine(prefix, "Lib"), 0);
    this.SetHostVariables(this.SystemState.__dict__);
  }

  internal string InitialPrefix => this._initialPrefix;

  internal void SetHostVariables(PythonDictionary dict)
  {
    dict[(object) "executable"] = (object) this._initialExecutable;
    dict[(object) "prefix"] = (object) this._initialPrefix;
    dict[(object) "exec_prefix"] = (object) this._initialPrefix;
    this.SetVersionVariables(dict);
  }

  private void SetVersionVariables(PythonDictionary dict)
  {
    Implementation implementation = new Implementation();
    dict[(object) "implementation"] = (object) implementation;
    dict[(object) "version_info"] = (object) implementation.version;
    dict[(object) "hexversion"] = (object) implementation.hexversion;
    dict[(object) "version"] = (object) implementation.version.GetVersionString(this._initialVersionString ?? PythonContext.GetVersionString());
  }

  internal static string GetVersionString()
  {
    string str1 = "";
    string str2 = Type.GetType("Mono.Runtime") == (Type) null ? ".NET" : "Mono";
    string str3 = (IntPtr.Size * 8).ToString();
    return string.Format("{0}{3} ({1}) on {4} {2} ({5}-bit)", (object) "IronPython 2.7.9", (object) PythonContext.GetPythonVersion().ToString(), (object) Environment.Version, (object) str1, (object) str2, (object) str3);
  }

  private static string GetInitialPrefix()
  {
    try
    {
      return typeof (PythonContext).Assembly.CodeBase;
    }
    catch (SecurityException ex)
    {
      return string.Empty;
    }
    catch (MethodAccessException ex)
    {
      return string.Empty;
    }
  }

  public override IList<string> GetMemberNames(object obj)
  {
    List<string> memberNames = new List<string>();
    foreach (object attrName in (IEnumerable<object>) PythonOps.GetAttrNames(this.SharedContext, obj))
    {
      if (attrName is string)
        memberNames.Add((string) attrName);
    }
    return (IList<string>) memberNames;
  }

  public override string FormatObject(DynamicOperations operations, object obj)
  {
    return PythonOps.Repr(this._defaultContext, obj) ?? "None";
  }

  internal object GetSystemStateValue(string name)
  {
    object obj;
    return this.SystemState.__dict__.TryGetValue((object) name, out obj) ? obj : (object) null;
  }

  internal void SetSystemStateValue(string name, object value)
  {
    this.SystemState.__dict__[(object) name] = value;
  }

  internal void DelSystemStateValue(string name) => this.SystemState.__dict__.Remove((object) name);

  private void SetStandardIO()
  {
    SharedIO sharedIo = this.DomainManager.SharedIO;
    PythonFile console1 = PythonFile.CreateConsole(this, sharedIo, ConsoleStreamType.Input, "<stdin>");
    PythonFile console2 = PythonFile.CreateConsole(this, sharedIo, ConsoleStreamType.Output, "<stdout>");
    PythonFile console3 = PythonFile.CreateConsole(this, sharedIo, ConsoleStreamType.ErrorOutput, "<stderr>");
    this.SetSystemStateValue("__stdin__", (object) console1);
    this.SetSystemStateValue("stdin", (object) console1);
    this.SetSystemStateValue("__stdout__", (object) console2);
    this.SetSystemStateValue("stdout", (object) console2);
    this.SetSystemStateValue("__stderr__", (object) console3);
    this.SetSystemStateValue("stderr", (object) console3);
  }

  internal PythonFileManager RawFileManager => this._fileManager;

  internal PythonFileManager FileManager
  {
    get
    {
      if (this._fileManager == null)
        Interlocked.CompareExchange<PythonFileManager>(ref this._fileManager, new PythonFileManager(), (PythonFileManager) null);
      return this._fileManager;
    }
  }

  public override int ExecuteProgram(SourceUnit program)
  {
    try
    {
      PythonCompilerOptions compilerOptions = (PythonCompilerOptions) this.GetCompilerOptions();
      compilerOptions.ModuleName = "__main__";
      compilerOptions.Module |= ModuleOptions.Initialize;
      program.Execute((CompilerOptions) compilerOptions, ErrorSink.Default);
    }
    catch (SystemExitException ex)
    {
      object obj;
      ref object local = ref obj;
      return ex.GetExitCode(out local);
    }
    return 0;
  }

  internal Dictionary<string, object> ErrorHandlers
  {
    get
    {
      if (this._errorHandlers == null)
        Interlocked.CompareExchange<Dictionary<string, object>>(ref this._errorHandlers, new Dictionary<string, object>(), (Dictionary<string, object>) null);
      return this._errorHandlers;
    }
  }

  internal List<object> SearchFunctions
  {
    get
    {
      if (this._searchFunctions == null)
        Interlocked.CompareExchange<List<object>>(ref this._searchFunctions, new List<object>(), (List<object>) null);
      return this._searchFunctions;
    }
  }

  internal SiteLocalStorage<T> GetGenericSiteStorage<T>()
  {
    if (this._genericSiteStorage == null)
      Interlocked.CompareExchange<Dictionary<Type, object>>(ref this._genericSiteStorage, new Dictionary<Type, object>(), (Dictionary<Type, object>) null);
    lock (this._genericSiteStorage)
    {
      object genericSiteStorage;
      if (!this._genericSiteStorage.TryGetValue(typeof (T), out genericSiteStorage))
        this._genericSiteStorage[typeof (T)] = (object) (SiteLocalStorage<T>) (genericSiteStorage = (object) new SiteLocalStorage<T>());
      return (SiteLocalStorage<T>) genericSiteStorage;
    }
  }

  internal SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> GetGenericCallSiteStorage()
  {
    return this.GetGenericSiteStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>>();
  }

  internal SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object>>> GetGenericCallSiteStorage0()
  {
    return this.GetGenericSiteStorage<CallSite<Func<CallSite, CodeContext, object, object>>>();
  }

  internal SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>> GetGenericKeywordCallSiteStorage()
  {
    return this.GetGenericSiteStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>>();
  }

  public override ConvertBinder CreateConvertBinder(Type toType, bool? explicitCast)
  {
    return explicitCast.HasValue ? (ConvertBinder) this.Convert(toType, explicitCast.Value ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast).CompatBinder : (ConvertBinder) this.Convert(toType, ConversionResultKind.ImplicitCast).CompatBinder;
  }

  public override DeleteMemberBinder CreateDeleteMemberBinder(string name, bool ignoreCase)
  {
    return ignoreCase ? (DeleteMemberBinder) new PythonDeleteMemberBinder(this, name, ignoreCase) : (DeleteMemberBinder) this.DeleteMember(name);
  }

  public override GetMemberBinder CreateGetMemberBinder(string name, bool ignoreCase)
  {
    return ignoreCase ? (GetMemberBinder) new CompatibilityGetMember(this, name, false) : (GetMemberBinder) this.CompatGetMember(name, false);
  }

  public override InvokeBinder CreateInvokeBinder(CallInfo callInfo)
  {
    return (InvokeBinder) this.CompatInvoke(callInfo);
  }

  public override BinaryOperationBinder CreateBinaryOperationBinder(ExpressionType operation)
  {
    return (BinaryOperationBinder) this.BinaryOperation(operation);
  }

  public override UnaryOperationBinder CreateUnaryOperationBinder(ExpressionType operation)
  {
    return (UnaryOperationBinder) this.UnaryOperation(operation);
  }

  public override SetMemberBinder CreateSetMemberBinder(string name, bool ignoreCase)
  {
    return ignoreCase ? (SetMemberBinder) new PythonSetMemberBinder(this, name, ignoreCase) : (SetMemberBinder) this.SetMember(name);
  }

  public override CreateInstanceBinder CreateCreateBinder(CallInfo callInfo)
  {
    return (CreateInstanceBinder) this.Create(this.CompatInvoke(callInfo), callInfo);
  }

  internal WeakRefTracker GetSystemPythonTypeWeakRef(PythonType type)
  {
    object obj;
    return !this._systemPythonTypesWeakRefs.TryGetValue((object) type, out obj) ? (WeakRefTracker) null : (WeakRefTracker) obj;
  }

  internal bool SetSystemPythonTypeWeakRef(PythonType type, WeakRefTracker value)
  {
    lock (this._systemPythonTypesWeakRefs)
      this._systemPythonTypesWeakRefs.AddNoLock((object) type, (object) value);
    return true;
  }

  internal void SetSystemPythonTypeFinalizer(PythonType type, WeakRefTracker value)
  {
    lock (this._systemPythonTypesWeakRefs)
      this._systemPythonTypesWeakRefs.AddNoLock((object) type, (object) value);
  }

  internal bool TryConvertToWeakReferenceable(object obj, out IWeakReferenceable weakref)
  {
    switch (obj)
    {
      case IWeakReferenceableByProxy referenceableByProxy:
        weakref = referenceableByProxy.GetWeakRefProxy(this);
        return true;
      case IWeakReferenceable weakReferenceable:
        weakref = weakReferenceable;
        return true;
      default:
        weakref = (IWeakReferenceable) null;
        return false;
    }
  }

  internal IWeakReferenceable ConvertToWeakReferenceable(object obj)
  {
    IWeakReferenceable weakref;
    if (this.TryConvertToWeakReferenceable(obj, out weakref))
      return weakref;
    throw PythonOps.TypeError("cannot create weak reference to '{0}' object", (object) PythonOps.GetPythonTypeName(obj));
  }

  private bool InvokeOperatorWorker(
    CodeContext context,
    UnaryOperators oper,
    object target,
    out object result)
  {
    if (this._newUnarySites == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>[]>(ref this._newUnarySites, new CallSite<Func<CallSite, CodeContext, object, object>>[4], (CallSite<Func<CallSite, CodeContext, object, object>>[]) null);
    if (this._newUnarySites[(int) oper] == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._newUnarySites[(int) oper], CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) this.InvokeNone), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
    CallSite<Func<CallSite, CodeContext, object, object>> newUnarySite = this._newUnarySites[(int) oper];
    string unarySymbol = PythonContext.GetUnarySymbol(oper);
    PythonType pythonType = DynamicHelpers.GetPythonType(target);
    PythonTypeSlot slot;
    object obj;
    if (pythonType.TryResolveMixedSlot(context, unarySymbol, out slot) && slot.TryGetValue(context, target, pythonType, out obj))
    {
      result = newUnarySite.Target((CallSite) newUnarySite, context, obj);
      return true;
    }
    result = (object) null;
    return false;
  }

  private static string GetUnarySymbol(UnaryOperators oper)
  {
    switch (oper)
    {
      case UnaryOperators.Repr:
        return "__repr__";
      case UnaryOperators.Length:
        return "__len__";
      case UnaryOperators.Hash:
        return "__hash__";
      case UnaryOperators.String:
        return "__str__";
      default:
        throw new ValueErrorException("unknown unary symbol");
    }
  }

  private bool InvokeOperatorWorker(
    CodeContext context,
    TernaryOperators oper,
    object target,
    object value1,
    object value2,
    out object result)
  {
    if (this._newTernarySites == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object>>[]>(ref this._newTernarySites, new CallSite<Func<CallSite, CodeContext, object, object, object, object>>[2], (CallSite<Func<CallSite, CodeContext, object, object, object, object>>[]) null);
    if (this._newTernarySites[(int) oper] == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object>>>(ref this._newTernarySites[(int) oper], CallSite<Func<CallSite, CodeContext, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(2))), (CallSite<Func<CallSite, CodeContext, object, object, object, object>>) null);
    CallSite<Func<CallSite, CodeContext, object, object, object, object>> newTernarySite = this._newTernarySites[(int) oper];
    string ternarySymbol = PythonContext.GetTernarySymbol(oper);
    PythonType pythonType = DynamicHelpers.GetPythonType(target);
    PythonTypeSlot slot;
    object obj;
    if (pythonType.TryResolveMixedSlot(context, ternarySymbol, out slot) && slot.TryGetValue(context, target, pythonType, out obj))
    {
      result = newTernarySite.Target((CallSite) newTernarySite, context, obj, value1, value2);
      return true;
    }
    result = (object) null;
    return false;
  }

  private static string GetTernarySymbol(TernaryOperators oper)
  {
    if (oper == TernaryOperators.SetDescriptor)
      return "__set__";
    if (oper == TernaryOperators.GetDescriptor)
      return "__get__";
    throw new ValueErrorException("unknown ternary operator");
  }

  internal static object InvokeUnaryOperator(
    CodeContext context,
    UnaryOperators oper,
    object target,
    string errorMsg)
  {
    object result;
    if (context.LanguageContext.InvokeOperatorWorker(context, oper, target, out result))
      return result;
    throw PythonOps.TypeError(errorMsg);
  }

  internal static object InvokeUnaryOperator(
    CodeContext context,
    UnaryOperators oper,
    object target)
  {
    object result;
    if (context.LanguageContext.InvokeOperatorWorker(context, oper, target, out result))
      return result;
    throw PythonOps.TypeError(string.Empty);
  }

  internal static bool TryInvokeTernaryOperator(
    CodeContext context,
    TernaryOperators oper,
    object target,
    object value1,
    object value2,
    out object res)
  {
    return context.LanguageContext.InvokeOperatorWorker(context, oper, target, value1, value2, out res);
  }

  internal CallSite<Func<CallSite, object, object, int>> CompareSite
  {
    get
    {
      if (this._compareSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, int>>>(ref this._compareSite, this.MakeSortCompareSite(), (CallSite<Func<CallSite, object, object, int>>) null);
      return this._compareSite;
    }
  }

  internal CallSite<Func<CallSite, object, object, int>> MakeSortCompareSite()
  {
    return CallSite<Func<CallSite, object, object, int>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.Compare));
  }

  internal void SetAttr(CodeContext context, object o, string name, object value)
  {
    if (this._setAttrSites == null)
      Interlocked.CompareExchange<Dictionary<PythonContext.AttrKey, CallSite<Func<CallSite, object, object, object>>>>(ref this._setAttrSites, new Dictionary<PythonContext.AttrKey, CallSite<Func<CallSite, object, object, object>>>(), (Dictionary<PythonContext.AttrKey, CallSite<Func<CallSite, object, object, object>>>) null);
    CallSite<Func<CallSite, object, object, object>> callSite;
    lock (this._setAttrSites)
    {
      PythonContext.AttrKey key = new PythonContext.AttrKey(CompilerHelpers.GetType(o), name);
      if (!this._setAttrSites.TryGetValue(key, out callSite))
        this._setAttrSites[key] = callSite = CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.SetMember(name));
    }
    object obj = callSite.Target((CallSite) callSite, o, value);
  }

  internal void DeleteAttr(CodeContext context, object o, string name)
  {
    PythonContext.AttrKey key = new PythonContext.AttrKey(CompilerHelpers.GetType(o), name);
    if (this._deleteAttrSites == null)
      Interlocked.CompareExchange<Dictionary<PythonContext.AttrKey, CallSite<Action<CallSite, object>>>>(ref this._deleteAttrSites, new Dictionary<PythonContext.AttrKey, CallSite<Action<CallSite, object>>>(), (Dictionary<PythonContext.AttrKey, CallSite<Action<CallSite, object>>>) null);
    CallSite<Action<CallSite, object>> callSite;
    lock (this._deleteAttrSites)
    {
      if (!this._deleteAttrSites.TryGetValue(key, out callSite))
        this._deleteAttrSites[key] = callSite = CallSite<Action<CallSite, object>>.Create((CallSiteBinder) this.DeleteMember(name));
    }
    callSite.Target((CallSite) callSite, o);
  }

  internal CallSite<Func<CallSite, CodeContext, object, string, PythonTuple, PythonDictionary, object>> MetaClassCallSite
  {
    get
    {
      if (this._metaClassSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, string, PythonTuple, PythonDictionary, object>>>(ref this._metaClassSite, CallSite<Func<CallSite, CodeContext, object, string, PythonTuple, PythonDictionary, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(3))), (CallSite<Func<CallSite, CodeContext, object, string, PythonTuple, PythonDictionary, object>>) null);
      return this._metaClassSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, string, object>> WriteCallSite
  {
    get
    {
      if (this._writeSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, string, object>>>(ref this._writeSite, CallSite<Func<CallSite, CodeContext, object, string, object>>.Create((CallSiteBinder) this.InvokeOne), (CallSite<Func<CallSite, CodeContext, object, string, object>>) null);
      return this._writeSite;
    }
  }

  internal CallSite<Func<CallSite, object, object, object>> GetIndexSite
  {
    get
    {
      if (this._getIndexSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object>>>(ref this._getIndexSite, CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.GetIndex(1)), (CallSite<Func<CallSite, object, object, object>>) null);
      return this._getIndexSite;
    }
  }

  internal void DelIndex(object target, object index)
  {
    if (this._delIndexSite == null)
      Interlocked.CompareExchange<CallSite<Action<CallSite, object, object>>>(ref this._delIndexSite, CallSite<Action<CallSite, object, object>>.Create((CallSiteBinder) this.DeleteIndex(1)), (CallSite<Action<CallSite, object, object>>) null);
    this._delIndexSite.Target((CallSite) this._delIndexSite, target, index);
  }

  internal void DelSlice(object target, object start, object end)
  {
    if (this._delSliceSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object, object>>>(ref this._delSliceSite, CallSite<Func<CallSite, object, object, object, object>>.Create((CallSiteBinder) this.DeleteSlice), (CallSite<Func<CallSite, object, object, object, object>>) null);
    object obj = this._delSliceSite.Target((CallSite) this._delSliceSite, target, start, end);
  }

  internal void SetIndex(object a, object b, object c)
  {
    if (this._setIndexSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object, object>>>(ref this._setIndexSite, CallSite<Func<CallSite, object, object, object, object>>.Create((CallSiteBinder) this.SetIndex(1)), (CallSite<Func<CallSite, object, object, object, object>>) null);
    object obj = this._setIndexSite.Target((CallSite) this._setIndexSite, a, b, c);
  }

  internal void SetSlice(object a, object start, object end, object value)
  {
    if (this._setSliceSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object, object, object>>>(ref this._setSliceSite, CallSite<Func<CallSite, object, object, object, object, object>>.Create((CallSiteBinder) this.SetSliceBinder), (CallSite<Func<CallSite, object, object, object, object, object>>) null);
    object obj = this._setSliceSite.Target((CallSite) this._setSliceSite, a, start, end, value);
  }

  internal CallSite<Func<CallSite, object, object, object>> EqualSite
  {
    get
    {
      if (this._equalSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object>>>(ref this._equalSite, CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.BinaryOperation(ExpressionType.Equal)), (CallSite<Func<CallSite, object, object, object>>) null);
      return this._equalSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, object>> FinalizerSite
  {
    get
    {
      if (this._finalizerSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._finalizerSite, CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) this.InvokeNone), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
      return this._finalizerSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, PythonFunction, object>> FunctionCallSite
  {
    get
    {
      if (this._functionCallSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, PythonFunction, object>>>(ref this._functionCallSite, CallSite<Func<CallSite, CodeContext, PythonFunction, object>>.Create((CallSiteBinder) this.InvokeNone), (CallSite<Func<CallSite, CodeContext, PythonFunction, object>>) null);
      return this._functionCallSite;
    }
  }

  public override string GetDocumentation(object obj)
  {
    if (this._docSite == null)
      this._docSite = CallSite<Func<CallSite, object, string>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.Documentation));
    return this._docSite.Target((CallSite) this._docSite, obj);
  }

  internal PythonSiteCache GetSiteCacheForSystemType(Type type)
  {
    if (this._systemSiteCache == null)
      Interlocked.CompareExchange<Dictionary<Type, PythonSiteCache>>(ref this._systemSiteCache, new Dictionary<Type, PythonSiteCache>(), (Dictionary<Type, PythonSiteCache>) null);
    lock (this._systemSiteCache)
    {
      PythonSiteCache cacheForSystemType;
      if (!this._systemSiteCache.TryGetValue(type, out cacheForSystemType))
        this._systemSiteCache[type] = cacheForSystemType = new PythonSiteCache();
      return cacheForSystemType;
    }
  }

  internal int ConvertToInt32(object value)
  {
    if (this._intSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, int>>>(ref this._intSite, this.MakeExplicitConvertSite<int>(), (CallSite<Func<CallSite, object, int>>) null);
    return this._intSite.Target((CallSite) this._intSite, value);
  }

  internal bool TryConvertToString(object str, out string res)
  {
    if (this._tryStringSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, string>>>(ref this._tryStringSite, this.MakeExplicitTrySite<string>(), (CallSite<Func<CallSite, object, string>>) null);
    res = this._tryStringSite.Target((CallSite) this._tryStringSite, str);
    return res != null;
  }

  internal bool TryConvertToInt32(object val, out int res)
  {
    if (this._tryIntSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object>>>(ref this._tryIntSite, this.MakeExplicitStructTrySite<int>(), (CallSite<Func<CallSite, object, object>>) null);
    object obj = this._tryIntSite.Target((CallSite) this._tryIntSite, val);
    if (obj != null)
    {
      res = (int) obj;
      return true;
    }
    res = 0;
    return false;
  }

  internal bool TryConvertToIEnumerable(object enumerable, out IEnumerable res)
  {
    if (this._tryIEnumerableSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, IEnumerable>>>(ref this._tryIEnumerableSite, this.MakeExplicitTrySite<IEnumerable>(), (CallSite<Func<CallSite, object, IEnumerable>>) null);
    res = this._tryIEnumerableSite.Target((CallSite) this._tryIEnumerableSite, enumerable);
    return res != null;
  }

  private CallSite<Func<CallSite, object, T>> MakeExplicitTrySite<T>() where T : class
  {
    return this.MakeTrySite<T, T>(ConversionResultKind.ExplicitTry);
  }

  private CallSite<Func<CallSite, object, object>> MakeExplicitStructTrySite<T>() where T : struct
  {
    return this.MakeTrySite<T, object>(ConversionResultKind.ExplicitTry);
  }

  private CallSite<Func<CallSite, object, TRet>> MakeTrySite<T, TRet>(ConversionResultKind kind)
  {
    return CallSite<Func<CallSite, object, TRet>>.Create((CallSiteBinder) this.Convert(typeof (T), kind));
  }

  internal object ImplicitConvertTo<T>(object value)
  {
    if (this._implicitConvertSites == null)
      Interlocked.CompareExchange<Dictionary<Type, CallSite<Func<CallSite, object, object>>>>(ref this._implicitConvertSites, new Dictionary<Type, CallSite<Func<CallSite, object, object>>>(), (Dictionary<Type, CallSite<Func<CallSite, object, object>>>) null);
    CallSite<Func<CallSite, object, object>> callSite;
    lock (this._implicitConvertSites)
    {
      if (!this._implicitConvertSites.TryGetValue(typeof (T), out callSite))
        this._implicitConvertSites[typeof (T)] = callSite = this.MakeImplicitConvertSite<T>();
    }
    return callSite.Target((CallSite) callSite, value);
  }

  private CallSite<Func<CallSite, object, T>> MakeExplicitConvertSite<T>()
  {
    return this.MakeConvertSite<T>(ConversionResultKind.ExplicitCast);
  }

  private CallSite<Func<CallSite, object, object>> MakeImplicitConvertSite<T>()
  {
    return CallSite<Func<CallSite, object, object>>.Create((CallSiteBinder) this.ConvertRetObject(typeof (T), ConversionResultKind.ImplicitCast));
  }

  private CallSite<Func<CallSite, object, T>> MakeConvertSite<T>(ConversionResultKind kind)
  {
    return CallSite<Func<CallSite, object, T>>.Create((CallSiteBinder) this.Convert(typeof (T), kind));
  }

  internal object Operation(PythonOperationKind operation, object self, object other)
  {
    if (this._binarySites == null)
      Interlocked.CompareExchange<Dictionary<PythonOperationKind, CallSite<Func<CallSite, object, object, object>>>>(ref this._binarySites, new Dictionary<PythonOperationKind, CallSite<Func<CallSite, object, object, object>>>(), (Dictionary<PythonOperationKind, CallSite<Func<CallSite, object, object, object>>>) null);
    CallSite<Func<CallSite, object, object, object>> callSite;
    lock (this._binarySites)
    {
      if (!this._binarySites.TryGetValue(operation, out callSite))
        this._binarySites[operation] = callSite = CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) Binders.BinaryOperationBinder(this, operation));
    }
    return callSite.Target((CallSite) callSite, self, other);
  }

  internal bool GreaterThan(object self, object other)
  {
    return this.Comparison(self, other, ExpressionType.GreaterThan, ref this._greaterThanSite);
  }

  internal bool LessThan(object self, object other)
  {
    return this.Comparison(self, other, ExpressionType.LessThan, ref this._lessThanSite);
  }

  internal bool GreaterThanOrEqual(object self, object other)
  {
    return this.Comparison(self, other, ExpressionType.GreaterThanOrEqual, ref this._greaterThanEqualSite);
  }

  internal bool LessThanOrEqual(object self, object other)
  {
    return this.Comparison(self, other, ExpressionType.LessThanOrEqual, ref this._lessThanEqualSite);
  }

  internal bool Contains(object self, object other)
  {
    return this.Comparison(self, other, PythonOperationKind.Contains, ref this._containsSite);
  }

  internal static bool Equal(object self, object other)
  {
    return DynamicHelpers.GetPythonType(self).EqualRetBool(self, other);
  }

  internal static bool NotEqual(object self, object other) => !PythonContext.Equal(self, other);

  private bool Comparison(
    object self,
    object other,
    ExpressionType operation,
    ref CallSite<Func<CallSite, object, object, bool>> comparisonSite)
  {
    if (comparisonSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, bool>>>(ref comparisonSite, this.CreateComparisonSite(operation), (CallSite<Func<CallSite, object, object, bool>>) null);
    return comparisonSite.Target((CallSite) comparisonSite, self, other);
  }

  internal CallSite<Func<CallSite, object, object, bool>> CreateComparisonSite(ExpressionType op)
  {
    return CallSite<Func<CallSite, object, object, bool>>.Create((CallSiteBinder) this.BinaryOperationRetType(this.BinaryOperation(op), this.Convert(typeof (bool), ConversionResultKind.ExplicitCast)));
  }

  private bool Comparison(
    object self,
    object other,
    PythonOperationKind operation,
    ref CallSite<Func<CallSite, object, object, bool>> comparisonSite)
  {
    if (comparisonSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, bool>>>(ref comparisonSite, this.CreateComparisonSite(operation), (CallSite<Func<CallSite, object, object, bool>>) null);
    return comparisonSite.Target((CallSite) comparisonSite, self, other);
  }

  internal CallSite<Func<CallSite, object, object, bool>> CreateComparisonSite(
    PythonOperationKind op)
  {
    return CallSite<Func<CallSite, object, object, bool>>.Create((CallSiteBinder) this.OperationRetType(this.Operation(op), this.Convert(typeof (bool), ConversionResultKind.ExplicitCast)));
  }

  internal object CallSplat(object func, params object[] args)
  {
    this.EnsureCallSplatSite();
    return this._callSplatSite.Target((CallSite) this._callSplatSite, this.SharedContext, func, args);
  }

  internal object CallWithContext(CodeContext context, object func, params object[] args)
  {
    this.EnsureCallSplatSite();
    return this._callSplatSite.Target((CallSite) this._callSplatSite, context, func, args);
  }

  internal object Call(CodeContext context, object func)
  {
    this.EnsureCall0Site();
    return this._callSite0.Target((CallSite) this._callSite0, context, func);
  }

  private void EnsureCall0SiteLightEh()
  {
    if (this._callSite0LightEh != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._callSite0LightEh, CallSite<Func<CallSite, CodeContext, object, object>>.Create(this.Invoke(new CallSignature(0)).GetLightExceptionBinder()), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
  }

  internal object CallLightEh(CodeContext context, object func)
  {
    this.EnsureCall0SiteLightEh();
    return this._callSite0LightEh.Target((CallSite) this._callSite0LightEh, context, func);
  }

  internal object Call(CodeContext context, object func, object arg0)
  {
    this.EnsureCall1Site();
    return this._callSite1.Target((CallSite) this._callSite1, context, func, arg0);
  }

  internal object Call(CodeContext context, object func, object arg0, object arg1)
  {
    this.EnsureCall2Site();
    return this._callSite2.Target((CallSite) this._callSite2, context, func, arg0, arg1);
  }

  private void EnsureCallSplatSite()
  {
    if (this._callSplatSite != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object[], object>>>(ref this._callSplatSite, this.MakeSplatSite(), (CallSite<Func<CallSite, CodeContext, object, object[], object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object[], object>> MakeSplatSite()
  {
    return CallSite<Func<CallSite, CodeContext, object, object[], object>>.Create((CallSiteBinder) Binders.InvokeSplat(this));
  }

  internal object CallWithKeywords(object func, object[] args, IDictionary<object, object> dict)
  {
    if (this._callDictSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>>(ref this._callDictSite, this.MakeKeywordSplatSite(), (CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>) null);
    return this._callDictSite.Target((CallSite) this._callDictSite, this.SharedContext, func, args, dict);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>> MakeKeywordSplatSite()
  {
    return CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>.Create((CallSiteBinder) Binders.InvokeKeywords(this));
  }

  internal object CallWithKeywords(object func, object args, object dict)
  {
    if (this._callDictSiteLooselyTyped == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object>>>(ref this._callDictSiteLooselyTyped, this.MakeKeywordSplatSiteLooselyTyped(), (CallSite<Func<CallSite, CodeContext, object, object, object, object>>) null);
    return this._callDictSiteLooselyTyped.Target((CallSite) this._callDictSiteLooselyTyped, this.SharedContext, func, args, dict);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object>> MakeKeywordSplatSiteLooselyTyped()
  {
    return CallSite<Func<CallSite, CodeContext, object, object, object, object>>.Create((CallSiteBinder) Binders.InvokeKeywords(this));
  }

  internal CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>> ImportSite
  {
    get
    {
      if (this._importSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>>>(ref this._importSite, CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>>.Create(this.Invoke(new CallSignature(5)).GetLightExceptionBinder()), (CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, int, object>>) null);
      return this._importSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>> OldImportSite
  {
    get
    {
      if (this._oldImportSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>>>(ref this._oldImportSite, CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>>.Create(this.Invoke(new CallSignature(4)).GetLightExceptionBinder()), (CallSite<Func<CallSite, CodeContext, object, string, PythonDictionary, PythonDictionary, PythonTuple, object>>) null);
      return this._oldImportSite;
    }
  }

  public override bool IsCallable(object obj)
  {
    if (this._isCallableSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, bool>>>(ref this._isCallableSite, CallSite<Func<CallSite, object, bool>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.IsCallable)), (CallSite<Func<CallSite, object, bool>>) null);
    return this._isCallableSite.Target((CallSite) this._isCallableSite, obj);
  }

  internal static int Hash(object o)
  {
    if (o != null)
    {
      switch (o.GetType().GetTypeCode())
      {
        case TypeCode.Boolean:
          return ((bool) o).GetHashCode();
        case TypeCode.SByte:
          return SByteOps.__hash__((sbyte) o);
        case TypeCode.Byte:
          return ByteOps.__hash__((byte) o);
        case TypeCode.Int16:
          return Int16Ops.__hash__((short) o);
        case TypeCode.UInt16:
          return UInt16Ops.__hash__((ushort) o);
        case TypeCode.Int32:
          return Int32Ops.__hash__((int) o);
        case TypeCode.UInt32:
          return UInt32Ops.__hash__((uint) o);
        case TypeCode.Int64:
          return Int64Ops.__hash__((long) o);
        case TypeCode.UInt64:
          return UInt64Ops.__hash__((ulong) o);
        case TypeCode.Single:
          return SingleOps.__hash__((float) o);
        case TypeCode.Double:
          return DoubleOps.__hash__((double) o);
        case TypeCode.Decimal:
          return DecimalOps.__hash__((Decimal) o);
        case TypeCode.DateTime:
          return ((DateTime) o).GetHashCode();
        case TypeCode.String:
          return ((string) o).GetHashCode();
      }
    }
    return DynamicHelpers.GetPythonType(o).Hash(o);
  }

  internal static bool IsHashable(object o)
  {
    if (o == null)
      return true;
    switch (o.GetType().GetTypeCode())
    {
      case TypeCode.Boolean:
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
      case TypeCode.DateTime:
      case TypeCode.String:
        return true;
      default:
        object ret;
        return PythonOps.TryGetBoundAttr(o, "__hash__", out ret) && ret != null || o is OldInstance oldInstance && (oldInstance.TryGetBoundCustomMember(DefaultContext.Default, "__hash__", out ret) || !oldInstance.TryGetBoundCustomMember(DefaultContext.Default, "__cmp__", out ret) && !oldInstance.TryGetBoundCustomMember(DefaultContext.Default, "__eq__", out ret)) || o is PythonType;
    }
  }

  internal object Add(object x, object y)
  {
    CallSite<Func<CallSite, object, object, object>> callSite = this.EnsureAddSite();
    return callSite.Target((CallSite) callSite, x, y);
  }

  internal CallSite<Func<CallSite, object, object, object>> EnsureAddSite()
  {
    if (this._addSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object>>>(ref this._addSite, CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.BinaryOperation(ExpressionType.Add)), (CallSite<Func<CallSite, object, object, object>>) null);
    return this._addSite;
  }

  internal object DivMod(object x, object y)
  {
    if (this._divModSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object>>>(ref this._divModSite, CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.DivMod)), (CallSite<Func<CallSite, object, object, object>>) null);
    object obj1 = this._divModSite.Target((CallSite) this._divModSite, x, y);
    if (obj1 != NotImplementedType.Value)
      return obj1;
    if (this._rdivModSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, object>>>(ref this._rdivModSite, CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.ReverseDivMod)), (CallSite<Func<CallSite, object, object, object>>) null);
    object obj2 = this._rdivModSite.Target((CallSite) this._rdivModSite, x, y);
    return obj2 != NotImplementedType.Value ? obj2 : throw PythonOps.TypeErrorForBinaryOp("divmod", x, y);
  }

  internal CompiledLoader GetCompiledLoader()
  {
    if (this._compiledLoader == null && Interlocked.CompareExchange<CompiledLoader>(ref this._compiledLoader, new CompiledLoader(), (CompiledLoader) null) == null)
    {
      object obj;
      if (!this.SystemState.__dict__.TryGetValue((object) "meta_path", out obj) || !(obj is List list))
        this.SystemState.__dict__[(object) "meta_path"] = (object) (list = new List());
      list.append((object) this._compiledLoader);
    }
    return this._compiledLoader;
  }

  internal CodeContext SharedContext => this._defaultContext;

  internal PythonOverloadResolverFactory SharedOverloadResolverFactory
  {
    get => this._sharedOverloadResolverFactory;
  }

  internal CodeContext SharedClsContext => this._defaultClsContext;

  internal IronPython.Runtime.ClrModule.ReferencesList ReferencedAssemblies
  {
    get
    {
      if (this._referencesList == null)
        Interlocked.CompareExchange<IronPython.Runtime.ClrModule.ReferencesList>(ref this._referencesList, new IronPython.Runtime.ClrModule.ReferencesList(), (IronPython.Runtime.ClrModule.ReferencesList) null);
      return this._referencesList;
    }
  }

  internal static CultureInfo CCulture
  {
    get
    {
      if (PythonContext._CCulture == null)
        Interlocked.CompareExchange<CultureInfo>(ref PythonContext._CCulture, PythonContext.MakeCCulture(), (CultureInfo) null);
      return PythonContext._CCulture;
    }
  }

  private static CultureInfo MakeCCulture()
  {
    CultureInfo cultureInfo = (CultureInfo) CultureInfo.InvariantCulture.Clone();
    cultureInfo.NumberFormat.NumberGroupSizes = new int[1];
    cultureInfo.NumberFormat.CurrencyGroupSizes = new int[1];
    return cultureInfo;
  }

  internal CultureInfo CollateCulture
  {
    get
    {
      if (this._collateCulture == null)
        this._collateCulture = PythonContext.CCulture;
      return this._collateCulture;
    }
    set => this._collateCulture = value;
  }

  internal CultureInfo CTypeCulture
  {
    get
    {
      if (this._ctypeCulture == null)
        this._ctypeCulture = PythonContext.CCulture;
      return this._ctypeCulture;
    }
    set => this._ctypeCulture = value;
  }

  internal CultureInfo TimeCulture
  {
    get
    {
      if (this._timeCulture == null)
        this._timeCulture = PythonContext.CCulture;
      return this._timeCulture;
    }
    set => this._timeCulture = value;
  }

  internal CultureInfo MonetaryCulture
  {
    get
    {
      if (this._monetaryCulture == null)
        this._monetaryCulture = PythonContext.CCulture;
      return this._monetaryCulture;
    }
    set => this._monetaryCulture = value;
  }

  internal CultureInfo NumericCulture
  {
    get
    {
      if (this._numericCulture == null)
        this._numericCulture = PythonContext.CCulture;
      return this._numericCulture;
    }
    set => this._numericCulture = value;
  }

  public Action<Action> GetSetCommandDispatcher(Action<Action> newDispatcher)
  {
    return Interlocked.Exchange<Action<Action>>(ref this._commandDispatcher, newDispatcher);
  }

  public Action<Action> GetCommandDispatcher() => this._commandDispatcher;

  public void DispatchCommand(Action command)
  {
    Action<Action> commandDispatcher = this._commandDispatcher;
    if (commandDispatcher != null)
    {
      commandDispatcher(command);
    }
    else
    {
      if (command == null)
        return;
      command();
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object>> PropertyGetSite
  {
    get
    {
      if (this._propGetSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object>>>(ref this._propGetSite, CallSite<Func<CallSite, CodeContext, object, object, object>>.Create((CallSiteBinder) this.InvokeOne), (CallSite<Func<CallSite, CodeContext, object, object, object>>) null);
      return this._propGetSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object>> PropertyDeleteSite
  {
    get
    {
      if (this._propDelSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object>>>(ref this._propDelSite, CallSite<Func<CallSite, CodeContext, object, object, object>>.Create((CallSiteBinder) this.InvokeOne), (CallSite<Func<CallSite, CodeContext, object, object, object>>) null);
      return this._propDelSite;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object>> PropertySetSite
  {
    get
    {
      if (this._propSetSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object>>>(ref this._propSetSite, CallSite<Func<CallSite, CodeContext, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(2))), (CallSite<Func<CallSite, CodeContext, object, object, object, object>>) null);
      return this._propSetSite;
    }
  }

  internal PythonBinder Binder => this._binder;

  private static CallSite<Func<CallSite, CodeContext, T, object, object, int>> MakeCompareSite<T>(
    PythonContext context)
  {
    return CallSite<Func<CallSite, CodeContext, T, object, object, int>>.Create((CallSiteBinder) context.InvokeTwoConvertToInt);
  }

  internal IComparer GetComparer(object cmp, Type type)
  {
    if (type == (Type) null)
    {
      switch (cmp)
      {
        case null:
          return (IComparer) new PythonContext.DefaultPythonComparer(this);
        case PythonFunction _:
          return (IComparer) new PythonContext.FunctionComparer<PythonFunction>(this, (PythonFunction) cmp);
        case BuiltinFunction _:
          return (IComparer) new PythonContext.FunctionComparer<BuiltinFunction>(this, (BuiltinFunction) cmp);
        default:
          return (IComparer) new PythonContext.FunctionComparer<object>(this, cmp);
      }
    }
    else
    {
      if (cmp == null)
      {
        if (this._defaultComparer == null)
          Interlocked.CompareExchange<Dictionary<Type, PythonContext.DefaultPythonComparer>>(ref this._defaultComparer, new Dictionary<Type, PythonContext.DefaultPythonComparer>(), (Dictionary<Type, PythonContext.DefaultPythonComparer>) null);
        lock (this._defaultComparer)
        {
          PythonContext.DefaultPythonComparer comparer;
          if (!this._defaultComparer.TryGetValue(type, out comparer))
            this._defaultComparer[type] = comparer = new PythonContext.DefaultPythonComparer(this);
          return (IComparer) comparer;
        }
      }
      if (cmp is PythonFunction)
      {
        if (this._sharedPythonFunctionCompareSite == null)
          this._sharedPythonFunctionCompareSite = PythonContext.MakeCompareSite<PythonFunction>(this);
        return (IComparer) new PythonContext.FunctionComparer<PythonFunction>(this, (PythonFunction) cmp, this._sharedPythonFunctionCompareSite);
      }
      if (cmp is BuiltinFunction)
      {
        if (this._sharedBuiltinFunctionCompareSite == null)
          this._sharedBuiltinFunctionCompareSite = PythonContext.MakeCompareSite<BuiltinFunction>(this);
        return (IComparer) new PythonContext.FunctionComparer<BuiltinFunction>(this, (BuiltinFunction) cmp, this._sharedBuiltinFunctionCompareSite);
      }
      if (this._sharedFunctionCompareSite == null)
        this._sharedFunctionCompareSite = PythonContext.MakeCompareSite<object>(this);
      return (IComparer) new PythonContext.FunctionComparer<object>(this, cmp, this._sharedFunctionCompareSite);
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, int, object>> GetItemCallSite
  {
    get
    {
      if (this._getItemCallSite == null)
        Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, int, object>>>(ref this._getItemCallSite, CallSite<Func<CallSite, CodeContext, object, int, object>>.Create((CallSiteBinder) new PythonInvokeBinder(this, new CallSignature(1))), (CallSite<Func<CallSite, CodeContext, object, int, object>>) null);
      return this._getItemCallSite;
    }
  }

  internal CallSite<Func<CallSite, object, object, bool>> GetEqualSite(Type type)
  {
    if (this._equalSites == null)
      Interlocked.CompareExchange<Dictionary<Type, CallSite<Func<CallSite, object, object, bool>>>>(ref this._equalSites, new Dictionary<Type, CallSite<Func<CallSite, object, object, bool>>>(), (Dictionary<Type, CallSite<Func<CallSite, object, object, bool>>>) null);
    CallSite<Func<CallSite, object, object, bool>> equalSite;
    lock (this._equalSites)
    {
      if (!this._equalSites.TryGetValue(type, out equalSite))
        this._equalSites[type] = equalSite = this.MakeEqualSite();
    }
    return equalSite;
  }

  internal CallSite<Func<CallSite, object, object, bool>> MakeEqualSite()
  {
    return this.CreateComparisonSite(ExpressionType.Equal);
  }

  internal static CallSite<Func<CallSite, object, int>> GetHashSite(PythonType type)
  {
    return type.HashSite;
  }

  internal CallSite<Func<CallSite, object, int>> MakeHashSite()
  {
    return CallSite<Func<CallSite, object, int>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.Hash));
  }

  public override IList<string> GetCallSignatures(object obj)
  {
    if (this._getSignaturesSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, IList<string>>>>(ref this._getSignaturesSite, CallSite<Func<CallSite, object, IList<string>>>.Create((CallSiteBinder) this.Operation(PythonOperationKind.CallSignatures)), (CallSite<Func<CallSite, object, IList<string>>>) null);
    return this._getSignaturesSite.Target((CallSite) this._getSignaturesSite, obj);
  }

  internal int Collect(int generation)
  {
    if (generation > GC.MaxGeneration || generation < 0)
      throw PythonOps.ValueError("invalid generation {0}", (object) generation);
    long totalMemory = GC.GetTotalMemory(false);
    for (int index = 0; index < 2; ++index)
    {
      GC.Collect(generation);
      GC.WaitForPendingFinalizers();
      if (generation == GC.MaxGeneration)
        FunctionCode.CleanFunctionCodes(this, true);
    }
    return (int) Math.Max(totalMemory - GC.GetTotalMemory(false), 0L);
  }

  public DynamicDelegateCreator DelegateCreator
  {
    get
    {
      if (this._delegateCreator == null)
        Interlocked.CompareExchange<DynamicDelegateCreator>(ref this._delegateCreator, new DynamicDelegateCreator((LanguageContext) this), (DynamicDelegateCreator) null);
      return this._delegateCreator;
    }
  }

  internal CompatibilityInvokeBinder CompatInvoke(CallInfo callInfo)
  {
    if (this._compatInvokeBinders == null)
      Interlocked.CompareExchange<Dictionary<CallSignature, CompatibilityInvokeBinder>>(ref this._compatInvokeBinders, new Dictionary<CallSignature, CompatibilityInvokeBinder>(), (Dictionary<CallSignature, CompatibilityInvokeBinder>) null);
    lock (this._compatInvokeBinders)
    {
      CallSignature signature = BindingHelpers.CallInfoToSignature(callInfo);
      CompatibilityInvokeBinder compatibilityInvokeBinder;
      if (!this._compatInvokeBinders.TryGetValue(signature, out compatibilityInvokeBinder))
        this._compatInvokeBinders[signature] = compatibilityInvokeBinder = new CompatibilityInvokeBinder(this, callInfo);
      return compatibilityInvokeBinder;
    }
  }

  internal PythonConversionBinder Convert(Type type, ConversionResultKind resultKind)
  {
    if (this._conversionBinders == null)
      Interlocked.CompareExchange<Dictionary<Type, PythonConversionBinder>[]>(ref this._conversionBinders, new Dictionary<Type, PythonConversionBinder>[4], (Dictionary<Type, PythonConversionBinder>[]) null);
    if (this._conversionBinders[(int) resultKind] == null)
      Interlocked.CompareExchange<Dictionary<Type, PythonConversionBinder>>(ref this._conversionBinders[(int) resultKind], new Dictionary<Type, PythonConversionBinder>(), (Dictionary<Type, PythonConversionBinder>) null);
    Dictionary<Type, PythonConversionBinder> conversionBinder1 = this._conversionBinders[(int) resultKind];
    lock (conversionBinder1)
    {
      PythonConversionBinder conversionBinder2;
      if (!conversionBinder1.TryGetValue(type, out conversionBinder2))
        conversionBinder1[type] = conversionBinder2 = new PythonConversionBinder(this, type, resultKind);
      return conversionBinder2;
    }
  }

  internal DynamicMetaObjectBinder ConvertRetObject(Type type, ConversionResultKind resultKind)
  {
    if (this._convertRetObjectBinders == null)
      Interlocked.CompareExchange<Dictionary<Type, DynamicMetaObjectBinder>[]>(ref this._convertRetObjectBinders, new Dictionary<Type, DynamicMetaObjectBinder>[4], (Dictionary<Type, DynamicMetaObjectBinder>[]) null);
    if (this._convertRetObjectBinders[(int) resultKind] == null)
      Interlocked.CompareExchange<Dictionary<Type, DynamicMetaObjectBinder>>(ref this._convertRetObjectBinders[(int) resultKind], new Dictionary<Type, DynamicMetaObjectBinder>(), (Dictionary<Type, DynamicMetaObjectBinder>) null);
    Dictionary<Type, DynamicMetaObjectBinder> convertRetObjectBinder = this._convertRetObjectBinders[(int) resultKind];
    lock (convertRetObjectBinder)
    {
      DynamicMetaObjectBinder metaObjectBinder;
      if (!convertRetObjectBinder.TryGetValue(type, out metaObjectBinder))
        convertRetObjectBinder[type] = metaObjectBinder = (DynamicMetaObjectBinder) new PythonConversionBinder(this, type, resultKind, true);
      return metaObjectBinder;
    }
  }

  internal CreateFallback Create(CompatibilityInvokeBinder realFallback, CallInfo callInfo)
  {
    if (this._createBinders == null)
      Interlocked.CompareExchange<Dictionary<CallSignature, CreateFallback>>(ref this._createBinders, new Dictionary<CallSignature, CreateFallback>(), (Dictionary<CallSignature, CreateFallback>) null);
    lock (this._createBinders)
    {
      CallSignature signature = BindingHelpers.CallInfoToSignature(callInfo);
      CreateFallback createFallback;
      if (!this._createBinders.TryGetValue(signature, out createFallback))
        this._createBinders[signature] = createFallback = new CreateFallback(realFallback, callInfo);
      return createFallback;
    }
  }

  internal PythonGetMemberBinder GetMember(string name) => this.GetMember(name, false);

  internal PythonGetMemberBinder GetMember(string name, bool isNoThrow)
  {
    Dictionary<string, PythonGetMemberBinder> getMemberBinders;
    if (isNoThrow)
    {
      if (this._tryGetMemberBinders == null)
        Interlocked.CompareExchange<Dictionary<string, PythonGetMemberBinder>>(ref this._tryGetMemberBinders, new Dictionary<string, PythonGetMemberBinder>(), (Dictionary<string, PythonGetMemberBinder>) null);
      getMemberBinders = this._tryGetMemberBinders;
    }
    else
    {
      if (this._getMemberBinders == null)
        Interlocked.CompareExchange<Dictionary<string, PythonGetMemberBinder>>(ref this._getMemberBinders, new Dictionary<string, PythonGetMemberBinder>(), (Dictionary<string, PythonGetMemberBinder>) null);
      getMemberBinders = this._getMemberBinders;
    }
    lock (getMemberBinders)
    {
      PythonGetMemberBinder member;
      if (!getMemberBinders.TryGetValue(name, out member))
        getMemberBinders[name] = member = new PythonGetMemberBinder(this, name, isNoThrow);
      return member;
    }
  }

  internal CompatibilityGetMember CompatGetMember(string name, bool isNoThrow)
  {
    Dictionary<string, CompatibilityGetMember> dictionary;
    if (isNoThrow)
    {
      if (this._compatGetMemberNoThrow == null)
        Interlocked.CompareExchange<Dictionary<string, CompatibilityGetMember>>(ref this._compatGetMemberNoThrow, new Dictionary<string, CompatibilityGetMember>(), (Dictionary<string, CompatibilityGetMember>) null);
      dictionary = this._compatGetMemberNoThrow;
    }
    else
    {
      if (this._compatGetMember == null)
        Interlocked.CompareExchange<Dictionary<string, CompatibilityGetMember>>(ref this._compatGetMember, new Dictionary<string, CompatibilityGetMember>(), (Dictionary<string, CompatibilityGetMember>) null);
      dictionary = this._compatGetMember;
    }
    lock (dictionary)
    {
      CompatibilityGetMember member;
      if (!dictionary.TryGetValue(name, out member))
        dictionary[name] = member = new CompatibilityGetMember(this, name, isNoThrow);
      return member;
    }
  }

  internal PythonSetMemberBinder SetMember(string name)
  {
    if (this._setMemberBinders == null)
      Interlocked.CompareExchange<Dictionary<string, PythonSetMemberBinder>>(ref this._setMemberBinders, new Dictionary<string, PythonSetMemberBinder>(), (Dictionary<string, PythonSetMemberBinder>) null);
    lock (this._setMemberBinders)
    {
      PythonSetMemberBinder pythonSetMemberBinder;
      if (!this._setMemberBinders.TryGetValue(name, out pythonSetMemberBinder))
        this._setMemberBinders[name] = pythonSetMemberBinder = new PythonSetMemberBinder(this, name);
      return pythonSetMemberBinder;
    }
  }

  internal PythonDeleteMemberBinder DeleteMember(string name)
  {
    if (this._deleteMemberBinders == null)
      Interlocked.CompareExchange<Dictionary<string, PythonDeleteMemberBinder>>(ref this._deleteMemberBinders, new Dictionary<string, PythonDeleteMemberBinder>(), (Dictionary<string, PythonDeleteMemberBinder>) null);
    lock (this._deleteMemberBinders)
    {
      PythonDeleteMemberBinder deleteMemberBinder;
      if (!this._deleteMemberBinders.TryGetValue(name, out deleteMemberBinder))
        this._deleteMemberBinders[name] = deleteMemberBinder = new PythonDeleteMemberBinder(this, name);
      return deleteMemberBinder;
    }
  }

  internal PythonInvokeBinder Invoke(CallSignature signature)
  {
    if (this._invokeBinders == null)
      Interlocked.CompareExchange<Dictionary<CallSignature, PythonInvokeBinder>>(ref this._invokeBinders, new Dictionary<CallSignature, PythonInvokeBinder>(), (Dictionary<CallSignature, PythonInvokeBinder>) null);
    lock (this._invokeBinders)
    {
      PythonInvokeBinder pythonInvokeBinder;
      if (!this._invokeBinders.TryGetValue(signature, out pythonInvokeBinder))
        this._invokeBinders[signature] = pythonInvokeBinder = new PythonInvokeBinder(this, signature);
      return pythonInvokeBinder;
    }
  }

  internal PythonInvokeBinder InvokeNone
  {
    get
    {
      if (this._invokeNoArgs == null)
        this._invokeNoArgs = this.Invoke(new CallSignature(0));
      return this._invokeNoArgs;
    }
  }

  internal PythonInvokeBinder InvokeOne
  {
    get
    {
      if (this._invokeOneArg == null)
        this._invokeOneArg = this.Invoke(new CallSignature(1));
      return this._invokeOneArg;
    }
  }

  internal DynamicMetaObjectBinder InvokeTwoConvertToInt
  {
    get
    {
      if (this._invokeTwoConvertToInt == null)
      {
        ParameterMappingInfo[] parameterMappingInfoArray = new ParameterMappingInfo[4];
        for (int index = 0; index < 4; ++index)
          parameterMappingInfoArray[index] = ParameterMappingInfo.Parameter(index);
        this._invokeTwoConvertToInt = (DynamicMetaObjectBinder) new ComboBinder(new BinderMappingInfo[2]
        {
          new BinderMappingInfo((DynamicMetaObjectBinder) this.Invoke(new CallSignature(2)), parameterMappingInfoArray),
          new BinderMappingInfo((DynamicMetaObjectBinder) this.Convert(typeof (int), ConversionResultKind.ExplicitCast), new ParameterMappingInfo[1]
          {
            ParameterMappingInfo.Action(0)
          })
        });
      }
      return this._invokeTwoConvertToInt;
    }
  }

  internal PythonOperationBinder Operation(PythonOperationKind operation)
  {
    if (this._operationBinders == null)
      Interlocked.CompareExchange<Dictionary<PythonOperationKind, PythonOperationBinder>>(ref this._operationBinders, new Dictionary<PythonOperationKind, PythonOperationBinder>(), (Dictionary<PythonOperationKind, PythonOperationBinder>) null);
    lock (this._operationBinders)
    {
      PythonOperationBinder pythonOperationBinder;
      if (!this._operationBinders.TryGetValue(operation, out pythonOperationBinder))
        this._operationBinders[operation] = pythonOperationBinder = new PythonOperationBinder(this, operation);
      return pythonOperationBinder;
    }
  }

  internal PythonUnaryOperationBinder UnaryOperation(ExpressionType operation)
  {
    if (this._unaryBinders == null)
      Interlocked.CompareExchange<Dictionary<ExpressionType, PythonUnaryOperationBinder>>(ref this._unaryBinders, new Dictionary<ExpressionType, PythonUnaryOperationBinder>(), (Dictionary<ExpressionType, PythonUnaryOperationBinder>) null);
    lock (this._unaryBinders)
    {
      PythonUnaryOperationBinder unaryOperationBinder;
      if (!this._unaryBinders.TryGetValue(operation, out unaryOperationBinder))
        this._unaryBinders[operation] = unaryOperationBinder = new PythonUnaryOperationBinder(this, operation);
      return unaryOperationBinder;
    }
  }

  internal PythonBinaryOperationBinder BinaryOperation(ExpressionType operation)
  {
    if (this._binaryBinders == null)
      Interlocked.CompareExchange<PythonBinaryOperationBinder[]>(ref this._binaryBinders, new PythonBinaryOperationBinder[85], (PythonBinaryOperationBinder[]) null);
    PythonBinaryOperationBinder binaryOperationBinder;
    return this._binaryBinders[(int) operation] ?? Interlocked.CompareExchange<PythonBinaryOperationBinder>(ref this._binaryBinders[(int) operation], binaryOperationBinder = new PythonBinaryOperationBinder(this, operation), (PythonBinaryOperationBinder) null) ?? binaryOperationBinder;
  }

  internal BinaryRetTypeBinder BinaryOperationRetType(
    PythonBinaryOperationBinder opBinder,
    PythonConversionBinder convBinder)
  {
    if (this._binaryRetTypeBinders == null)
      Interlocked.CompareExchange<Dictionary<PythonContext.OperationRetTypeKey<ExpressionType>, BinaryRetTypeBinder>>(ref this._binaryRetTypeBinders, new Dictionary<PythonContext.OperationRetTypeKey<ExpressionType>, BinaryRetTypeBinder>(), (Dictionary<PythonContext.OperationRetTypeKey<ExpressionType>, BinaryRetTypeBinder>) null);
    lock (this._binaryRetTypeBinders)
    {
      PythonContext.OperationRetTypeKey<ExpressionType> key = new PythonContext.OperationRetTypeKey<ExpressionType>(convBinder.Type, opBinder.Operation);
      BinaryRetTypeBinder binaryRetTypeBinder;
      if (!this._binaryRetTypeBinders.TryGetValue(key, out binaryRetTypeBinder))
        this._binaryRetTypeBinders[key] = binaryRetTypeBinder = new BinaryRetTypeBinder((DynamicMetaObjectBinder) opBinder, convBinder);
      return binaryRetTypeBinder;
    }
  }

  internal BinaryRetTypeBinder OperationRetType(
    PythonOperationBinder opBinder,
    PythonConversionBinder convBinder)
  {
    if (this._operationRetTypeBinders == null)
      Interlocked.CompareExchange<Dictionary<PythonContext.OperationRetTypeKey<PythonOperationKind>, BinaryRetTypeBinder>>(ref this._operationRetTypeBinders, new Dictionary<PythonContext.OperationRetTypeKey<PythonOperationKind>, BinaryRetTypeBinder>(), (Dictionary<PythonContext.OperationRetTypeKey<PythonOperationKind>, BinaryRetTypeBinder>) null);
    lock (this._operationRetTypeBinders)
    {
      PythonContext.OperationRetTypeKey<PythonOperationKind> key = new PythonContext.OperationRetTypeKey<PythonOperationKind>(convBinder.Type, opBinder.Operation);
      BinaryRetTypeBinder binaryRetTypeBinder;
      if (!this._operationRetTypeBinders.TryGetValue(key, out binaryRetTypeBinder))
        this._operationRetTypeBinders[key] = binaryRetTypeBinder = new BinaryRetTypeBinder((DynamicMetaObjectBinder) opBinder, convBinder);
      return binaryRetTypeBinder;
    }
  }

  internal PythonGetIndexBinder GetIndex(int argCount)
  {
    if (this._getIndexBinders == null)
      Interlocked.CompareExchange<PythonGetIndexBinder[]>(ref this._getIndexBinders, new PythonGetIndexBinder[argCount + 1], (PythonGetIndexBinder[]) null);
    lock (this)
    {
      if (this._getIndexBinders.Length <= argCount)
        Array.Resize<PythonGetIndexBinder>(ref this._getIndexBinders, argCount + 1);
      if (this._getIndexBinders[argCount] == null)
        this._getIndexBinders[argCount] = new PythonGetIndexBinder(this, argCount);
      return this._getIndexBinders[argCount];
    }
  }

  internal PythonSetIndexBinder SetIndex(int argCount)
  {
    if (this._setIndexBinders == null)
      Interlocked.CompareExchange<PythonSetIndexBinder[]>(ref this._setIndexBinders, new PythonSetIndexBinder[argCount + 1], (PythonSetIndexBinder[]) null);
    lock (this)
    {
      if (this._setIndexBinders.Length <= argCount)
        Array.Resize<PythonSetIndexBinder>(ref this._setIndexBinders, argCount + 1);
      if (this._setIndexBinders[argCount] == null)
        this._setIndexBinders[argCount] = new PythonSetIndexBinder(this, argCount);
      return this._setIndexBinders[argCount];
    }
  }

  internal PythonDeleteIndexBinder DeleteIndex(int argCount)
  {
    if (this._deleteIndexBinders == null)
      Interlocked.CompareExchange<PythonDeleteIndexBinder[]>(ref this._deleteIndexBinders, new PythonDeleteIndexBinder[argCount + 1], (PythonDeleteIndexBinder[]) null);
    lock (this)
    {
      if (this._deleteIndexBinders.Length <= argCount)
        Array.Resize<PythonDeleteIndexBinder>(ref this._deleteIndexBinders, argCount + 1);
      if (this._deleteIndexBinders[argCount] == null)
        this._deleteIndexBinders[argCount] = new PythonDeleteIndexBinder(this, argCount);
      return this._deleteIndexBinders[argCount];
    }
  }

  internal PythonGetSliceBinder GetSlice
  {
    get
    {
      if (this._getSlice == null)
        Interlocked.CompareExchange<PythonGetSliceBinder>(ref this._getSlice, new PythonGetSliceBinder(this), (PythonGetSliceBinder) null);
      return this._getSlice;
    }
  }

  internal PythonSetSliceBinder SetSliceBinder
  {
    get
    {
      if (this._setSlice == null)
        Interlocked.CompareExchange<PythonSetSliceBinder>(ref this._setSlice, new PythonSetSliceBinder(this), (PythonSetSliceBinder) null);
      return this._setSlice;
    }
  }

  internal PythonDeleteSliceBinder DeleteSlice
  {
    get
    {
      if (this._deleteSlice == null)
        Interlocked.CompareExchange<PythonDeleteSliceBinder>(ref this._deleteSlice, new PythonDeleteSliceBinder(this), (PythonDeleteSliceBinder) null);
      return this._deleteSlice;
    }
  }

  public static PythonContext GetPythonContext(DynamicMetaObjectBinder action)
  {
    return action is IPythonSite pythonSite ? pythonSite.Context : DefaultContext.DefaultPythonContext;
  }

  public static System.Linq.Expressions.Expression GetCodeContext(DynamicMetaObjectBinder action)
  {
    return Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(action)._defaultContext);
  }

  public static DynamicMetaObject GetCodeContextMO(DynamicMetaObjectBinder action)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(action)._defaultContext), BindingRestrictions.Empty, (object) PythonContext.GetPythonContext(action)._defaultContext);
  }

  public static DynamicMetaObject GetCodeContextMOCls(DynamicMetaObjectBinder action)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(action).SharedClsContext), BindingRestrictions.Empty, (object) PythonContext.GetPythonContext(action).SharedClsContext);
  }

  public override T ScopeGetVariable<T>(Scope scope, string name)
  {
    object obj;
    if (scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out obj))
      return this.Operations.ConvertTo<T>(obj);
    return scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out obj) ? this.Operations.ConvertTo<T>(obj) : base.ScopeGetVariable<T>(scope, name);
  }

  public override object ScopeGetVariable(Scope scope, string name)
  {
    object obj;
    return scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out obj) || scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out obj) ? obj : base.ScopeGetVariable(scope, name);
  }

  public override void ScopeSetVariable(Scope scope, string name, object value)
  {
    if (scope.Storage is ScopeStorage storage2)
      storage2.SetValue(name, false, value);
    else if (scope.Storage is StringDictionaryExpando storage1)
      storage1.Dictionary[name] = value;
    else
      base.ScopeSetVariable(scope, name, value);
  }

  public override bool ScopeTryGetVariable(Scope scope, string name, out object value)
  {
    return scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out value) || scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out value) || base.ScopeTryGetVariable(scope, name, out value);
  }

  internal DebugContext DebugContext
  {
    get
    {
      this.EnsureDebugContext();
      return this._debugContext;
    }
  }

  private void EnsureDebugContext()
  {
    if (this._debugContext != null)
      return;
    lock (this)
    {
      if (this._debugContext != null)
        return;
      try
      {
        this._debugContext = DebugContext.CreateInstance();
        this._tracePipeline = Microsoft.Scripting.Debugging.TracePipeline.CreateInstance(this._debugContext);
        this._tracePipeline.TraceCallback = (ITraceCallback) new PythonContext.PythonTracebackListenersDispatcher(this);
      }
      catch
      {
        this._debugContext = (DebugContext) null;
        this._tracePipeline = (Microsoft.Scripting.Debugging.TracePipeline) null;
        throw;
      }
    }
  }

  internal ITracePipeline TracePipeline => (ITracePipeline) this._tracePipeline;

  internal void SetTrace(object o)
  {
    if (o == null && this._debugContext == null)
      return;
    this.EnsureDebugContext();
    PythonTracebackListener tracebackListener1 = this._tracebackListeners.Value;
    PythonTracebackListener tracebackListener2 = tracebackListener1;
    if (o == null)
      this._tracebackListeners.Value = tracebackListener2 = (PythonTracebackListener) null;
    else if (PythonOps.GetFunctionStackNoCreate() == null || tracebackListener1 == null || !tracebackListener1.InTraceBack)
      this._tracebackListeners.Value = tracebackListener2 = new PythonTracebackListener(this, o);
    lock (this._codeUpdateLock)
    {
      bool enableTracing = this.EnableTracing;
      if (tracebackListener1 != null != (tracebackListener2 != null))
        this._tracebackListenersCount += tracebackListener2 != null ? 1 : -1;
      if (this.EnableTracing == enableTracing)
        return;
      FunctionCode.UpdateAllCode(this);
    }
  }

  internal object CallTracing(object func, PythonTuple args)
  {
    PythonTracebackListener tracebackListener = this._debugContext != null ? this._tracebackListeners.Value : (PythonTracebackListener) null;
    bool flag = tracebackListener != null && tracebackListener.InTraceBack;
    if (tracebackListener != null & flag)
      tracebackListener.InTraceBack = false;
    try
    {
      return PythonCalls.Call(func, args.ToArray());
    }
    finally
    {
      if (tracebackListener != null)
        tracebackListener.InTraceBack = flag;
    }
  }

  internal object GetTrace()
  {
    if (this._debugContext == null)
      return (object) null;
    return this._tracebackListeners.Value?.TraceObject;
  }

  internal ExtensionMethodSet UniqifyExtensions(ExtensionMethodSet newSet)
  {
    int index1 = -1;
    if (this._weakExtensionMethodSets == null)
      Interlocked.CompareExchange<List<WeakReference>>(ref this._weakExtensionMethodSets, new List<WeakReference>(), (List<WeakReference>) null);
    lock (this._weakExtensionMethodSets)
    {
      for (int index2 = 0; index2 < this._weakExtensionMethodSets.Count; ++index2)
      {
        ExtensionMethodSet target = (ExtensionMethodSet) this._weakExtensionMethodSets[index2].Target;
        if (target != (ExtensionMethodSet) null)
        {
          if (target == newSet)
            return target;
        }
        else
          index1 = index2;
      }
      if (index1 == -1)
        this._weakExtensionMethodSets.Add(new WeakReference((object) newSet));
      else
        this._weakExtensionMethodSets[index1].Target = (object) newSet;
      return newSet;
    }
  }

  internal CallSite<Func<CallSite, CodeContext, object, object>> CallSite0
  {
    get
    {
      this.EnsureCall0Site();
      return this._callSite0;
    }
  }

  private void EnsureCall0Site()
  {
    if (this._callSite0 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object>>>(ref this._callSite0, CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(0))), (CallSite<Func<CallSite, CodeContext, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object>> CallSite1
  {
    get
    {
      this.EnsureCall1Site();
      return this._callSite1;
    }
  }

  private void EnsureCall1Site()
  {
    if (this._callSite1 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object>>>(ref this._callSite1, CallSite<Func<CallSite, CodeContext, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(1))), (CallSite<Func<CallSite, CodeContext, object, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object>> CallSite2
  {
    get
    {
      this.EnsureCall2Site();
      return this._callSite2;
    }
  }

  private void EnsureCall2Site()
  {
    if (this._callSite2 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object>>>(ref this._callSite2, CallSite<Func<CallSite, CodeContext, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(2))), (CallSite<Func<CallSite, CodeContext, object, object, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object, object>> CallSite3
  {
    get
    {
      this.EnsureCall3Site();
      return this._callSite3;
    }
  }

  private void EnsureCall3Site()
  {
    if (this._callSite3 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object, object>>>(ref this._callSite3, CallSite<Func<CallSite, CodeContext, object, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(3))), (CallSite<Func<CallSite, CodeContext, object, object, object, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>> CallSite4
  {
    get
    {
      this.EnsureCall4Site();
      return this._callSite4;
    }
  }

  private void EnsureCall4Site()
  {
    if (this._callSite4 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>>>(ref this._callSite4, CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(4))), (CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>> CallSite5
  {
    get
    {
      this.EnsureCall5Site();
      return this._callSite5;
    }
  }

  private void EnsureCall5Site()
  {
    if (this._callSite5 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>>>(ref this._callSite5, CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(5))), (CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object>>) null);
  }

  internal CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>> CallSite6
  {
    get
    {
      this.EnsureCall6Site();
      return this._callSite6;
    }
  }

  private void EnsureCall6Site()
  {
    if (this._callSite6 != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>>>(ref this._callSite6, CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>>.Create((CallSiteBinder) this.Invoke(new CallSignature(6))), (CallSite<Func<CallSite, CodeContext, object, object, object, object, object, object, object, object>>) null);
  }

  internal sealed class PythonEqualityComparer : IEqualityComparer, IEqualityComparer<object>
  {
    public readonly PythonContext Context;

    public PythonEqualityComparer(PythonContext context) => this.Context = context;

    bool IEqualityComparer.Equals(object x, object y)
    {
      return PythonOps.EqualRetBool(this.Context._defaultContext, x, y);
    }

    bool IEqualityComparer<object>.Equals(object x, object y)
    {
      return PythonOps.EqualRetBool(this.Context._defaultContext, x, y);
    }

    int IEqualityComparer.GetHashCode(object obj) => PythonContext.Hash(obj);

    int IEqualityComparer<object>.GetHashCode(object obj) => PythonContext.Hash(obj);
  }

  private sealed class OptimizedUserHasher
  {
    private readonly PythonContext _context;
    private readonly PythonType _pt;

    public OptimizedUserHasher(PythonContext context, PythonType pt)
    {
      this._context = context;
      this._pt = pt;
    }

    public int Hasher(object o, ref HashDelegate dlg)
    {
      if (o is IPythonObject pythonObject && pythonObject.PythonType == this._pt)
        return this._pt.Hash(o);
      dlg = this._context.FallbackHasher;
      return this._context.FallbackHasher(o, ref dlg);
    }
  }

  private sealed class OptimizedBuiltinHasher
  {
    private readonly PythonContext _context;
    private readonly Type _type;
    private readonly PythonType _pt;

    public OptimizedBuiltinHasher(PythonContext context, Type type)
    {
      this._context = context;
      this._type = type;
      this._pt = DynamicHelpers.GetPythonTypeFromType(type);
    }

    public int Hasher(object o, ref HashDelegate dlg)
    {
      if (o != null && o.GetType() == this._type)
        return this._pt.Hash(o);
      dlg = this._context.FallbackHasher;
      return this._context.FallbackHasher(o, ref dlg);
    }
  }

  private class AssemblyResolveHolder
  {
    private readonly WeakReference _context;

    public AssemblyResolveHolder(PythonContext context)
    {
      this._context = new WeakReference((object) context);
    }

    internal Assembly AssemblyResolveEvent(object sender, ResolveEventArgs args)
    {
      PythonContext target = (PythonContext) this._context.Target;
      if (target != null)
        return target.CurrentDomain_AssemblyResolve(sender, args);
      AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.AssemblyResolveEvent);
      return (Assembly) null;
    }
  }

  private class AttrKey : IEquatable<PythonContext.AttrKey>
  {
    private Type _type;
    private string _name;

    public AttrKey(Type type, string name)
    {
      this._type = type;
      this._name = name;
    }

    public bool Equals(PythonContext.AttrKey other)
    {
      return other != null && this._type == other._type && this._name == other._name;
    }

    public override bool Equals(object obj) => this.Equals(obj as PythonContext.AttrKey);

    public override int GetHashCode() => this._type.GetHashCode() ^ this._name.GetHashCode();
  }

  private class DefaultPythonComparer : IComparer
  {
    private CallSite<Func<CallSite, object, object, int>> _site;

    public DefaultPythonComparer(PythonContext context)
    {
      this._site = CallSite<Func<CallSite, object, object, int>>.Create((CallSiteBinder) context.Operation(PythonOperationKind.Compare));
    }

    public int Compare(object x, object y) => this._site.Target((CallSite) this._site, x, y);
  }

  private class FunctionComparer<T> : IComparer
  {
    private T _cmpfunc;
    private CallSite<Func<CallSite, CodeContext, T, object, object, int>> _funcSite;
    private CodeContext _context;

    public FunctionComparer(PythonContext context, T cmpfunc)
      : this(context, cmpfunc, PythonContext.MakeCompareSite<T>(context))
    {
    }

    public FunctionComparer(
      PythonContext context,
      T cmpfunc,
      CallSite<Func<CallSite, CodeContext, T, object, object, int>> site)
    {
      this._cmpfunc = cmpfunc;
      this._context = context.SharedContext;
      this._funcSite = site;
    }

    public int Compare(object o1, object o2)
    {
      return this._funcSite.Target((CallSite) this._funcSite, this._context, this._cmpfunc, o1, o2);
    }
  }

  private class OperationRetTypeKey<T> : IEquatable<PythonContext.OperationRetTypeKey<T>>
  {
    public readonly Type ReturnType;
    public readonly T Operation;

    public OperationRetTypeKey(Type retType, T operation)
    {
      this.ReturnType = retType;
      this.Operation = operation;
    }

    public bool Equals(PythonContext.OperationRetTypeKey<T> other)
    {
      return other.ReturnType == this.ReturnType && other.Operation.Equals((object) this.Operation);
    }

    public override int GetHashCode()
    {
      return this.ReturnType.GetHashCode() ^ this.Operation.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return obj is PythonContext.OperationRetTypeKey<T> other && this.Equals(other);
    }
  }

  private class PythonTracebackListenersDispatcher : ITraceCallback
  {
    private readonly PythonContext _parent;

    public PythonTracebackListenersDispatcher(PythonContext parent) => this._parent = parent;

    void ITraceCallback.OnTraceEvent(
      TraceEventKind kind,
      string name,
      string sourceFileName,
      SourceSpan sourceSpan,
      Func<IDictionary<object, object>> scopeCallback,
      object payload,
      object customPayload)
    {
      PythonTracebackListener tracebackListener = this._parent._tracebackListeners.Value;
      if (tracebackListener == null && this._parent.PythonOptions.Tracing)
        this._parent._tracebackListeners.Value = tracebackListener = new PythonTracebackListener(this._parent, (object) null);
      tracebackListener?.OnTraceEvent(kind, name, sourceFileName, sourceSpan, scopeCallback, payload, customPayload);
    }
  }
}
