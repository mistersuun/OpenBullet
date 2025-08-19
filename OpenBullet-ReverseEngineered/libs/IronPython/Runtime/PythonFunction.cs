// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[PythonType("function")]
[DontMapGetMemberNamesToDir]
[DebuggerDisplay("function {__name__} in {__module__}")]
public sealed class PythonFunction : 
  PythonTypeSlot,
  IWeakReferenceable,
  IPythonMembersList,
  IMembersList,
  IDynamicMetaObjectProvider,
  ICodeFormattable,
  IFastInvokable
{
  private readonly CodeContext _context;
  [PythonHidden(new PlatformID[] {})]
  public readonly MutableTuple Closure;
  private object[] _defaults;
  internal PythonDictionary _dict;
  private object _module;
  internal int _id;
  internal int _compat;
  private FunctionCode _code;
  private string _name;
  private object _doc;
  private static int[] _depth_fast = new int[20];
  [ThreadStatic]
  private static int DepthSlow;
  private static int _CurrentId = 1;
  private static Dictionary<PythonFunction.FunctionCallerKey, FunctionCaller> _functionCallers = new Dictionary<PythonFunction.FunctionCallerKey, FunctionCaller>();

  public PythonFunction(CodeContext context, FunctionCode code, PythonDictionary globals)
    : this(context, code, globals, code.PythonCode.Name, (PythonTuple) null, (PythonTuple) null)
  {
  }

  public PythonFunction(
    CodeContext context,
    FunctionCode code,
    PythonDictionary globals,
    string name)
    : this(context, code, globals, name, (PythonTuple) null, (PythonTuple) null)
  {
  }

  public PythonFunction(
    CodeContext context,
    FunctionCode code,
    PythonDictionary globals,
    string name,
    PythonTuple defaults)
    : this(context, code, globals, name, defaults, (PythonTuple) null)
  {
  }

  public PythonFunction(
    CodeContext context,
    FunctionCode code,
    PythonDictionary globals,
    string name,
    PythonTuple defaults,
    PythonTuple closure)
  {
    if (closure != null && closure.__len__() != 0)
      throw new NotImplementedException("non empty closure argument is not supported");
    if (globals == context.GlobalDict)
    {
      this._module = (object) context.Module.GetName();
      this._context = context;
    }
    else
    {
      this._module = (object) null;
      this._context = new CodeContext(new PythonDictionary(), new ModuleContext(globals, DefaultContext.DefaultPythonContext));
    }
    this._defaults = defaults == null ? ArrayUtils.EmptyObjects : defaults.ToArray();
    this._code = code;
    this._name = name;
    this._doc = (object) code._initialDoc;
    this.Closure = (MutableTuple) null;
    ScopeStatement pythonCode = this._code.PythonCode;
    if (pythonCode.IsClosure)
      throw new NotImplementedException("code containing closures is not supported");
    pythonCode.RewriteBody((ExpressionVisitor) FunctionDefinition.ArbitraryGlobalsVisitorInstance);
    this._compat = this.CalculatedCachedCompat();
  }

  internal PythonFunction(
    CodeContext context,
    FunctionCode funcInfo,
    object modName,
    object[] defaults,
    MutableTuple closure)
  {
    this._context = context;
    this._defaults = defaults ?? ArrayUtils.EmptyObjects;
    this._code = funcInfo;
    this._doc = (object) funcInfo._initialDoc;
    this._name = funcInfo.co_name;
    if (modName != Uninitialized.Instance)
      this._module = modName;
    this.Closure = closure;
    this._compat = this.CalculatedCachedCompat();
  }

  public object __globals__
  {
    get => this.func_globals;
    set => throw PythonOps.TypeError("readonly attribute");
  }

  public object func_globals
  {
    get => (object) this._context.GlobalDict;
    set => throw PythonOps.TypeError("readonly attribute");
  }

  [PropertyMethod]
  [SpecialName]
  public void Deletefunc_globals() => throw PythonOps.TypeError("readonly attribute");

  public PythonTuple __defaults__
  {
    get => this.func_defaults;
    set => this.func_defaults = value;
  }

  public PythonTuple func_defaults
  {
    get
    {
      return this._defaults.Length == 0 ? (PythonTuple) null : new PythonTuple((object) this._defaults);
    }
    set
    {
      this._defaults = value != null ? value.ToArray() : ArrayUtils.EmptyObjects;
      this._compat = this.CalculatedCachedCompat();
    }
  }

  public PythonTuple __closure__
  {
    get => this.func_closure;
    set => this.func_closure = value;
  }

  public PythonTuple func_closure
  {
    get
    {
      if (!(this.Context.Dict._storage is RuntimeVariablesDictionaryStorage storage))
        return (PythonTuple) null;
      object[] objArray = new object[storage.Names.Length];
      for (int i = 0; i < objArray.Length; ++i)
        objArray[i] = (object) storage.GetCell(i);
      return PythonTuple.MakeTuple(objArray);
    }
    set => throw PythonOps.TypeError("readonly attribute");
  }

  public string __name__
  {
    get => this.func_name;
    set => this.func_name = value;
  }

  public string func_name
  {
    get => this._name;
    set
    {
      this._name = value != null ? value : throw PythonOps.TypeError("func_name must be set to a string object");
    }
  }

  public PythonDictionary __dict__
  {
    get => this.func_dict;
    set => this.func_dict = value;
  }

  public PythonDictionary func_dict
  {
    get => this.EnsureDict();
    set
    {
      this._dict = value != null ? value : throw PythonOps.TypeError("setting function's dictionary to non-dict");
    }
  }

  public object __doc__
  {
    get => this._doc;
    set => this._doc = value;
  }

  public object func_doc
  {
    get => this.__doc__;
    set => this.__doc__ = value;
  }

  public object __module__
  {
    get => this._module;
    set => this._module = value;
  }

  public FunctionCode __code__
  {
    get => this.func_code;
    set => this.func_code = value;
  }

  public FunctionCode func_code
  {
    get => this._code;
    set
    {
      this._code = value != null ? value : throw PythonOps.TypeError("func_code must be set to a code object");
      this._compat = this.CalculatedCachedCompat();
    }
  }

  public object __call__(CodeContext context, params object[] args)
  {
    return PythonCalls.Call(context, (object) this, args);
  }

  public object __call__(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> dict,
    params object[] args)
  {
    return PythonCalls.CallWithKeywordArgs(context, (object) this, args, dict);
  }

  internal SourceSpan Span => this.func_code.Span;

  internal string[] ArgNames => this.func_code.ArgNames;

  internal CodeContext Context => this._context;

  internal string GetSignatureString()
  {
    StringBuilder stringBuilder = new StringBuilder(this.__name__);
    stringBuilder.Append('(');
    for (int index = 0; index < this._code.ArgNames.Length; ++index)
    {
      if (index != 0)
        stringBuilder.Append(", ");
      if (index == this.ExpandDictPosition)
        stringBuilder.Append("**");
      else if (index == this.ExpandListPosition)
        stringBuilder.Append("*");
      stringBuilder.Append(this.ArgNames[index]);
      if (index < this.NormalArgumentCount)
      {
        int num = this.NormalArgumentCount - this.Defaults.Length;
        if (index - num >= 0)
        {
          stringBuilder.Append('=');
          stringBuilder.Append(PythonOps.Repr(this.Context, this.Defaults[index - num]));
        }
      }
    }
    stringBuilder.Append(')');
    return stringBuilder.ToString();
  }

  internal int FunctionCompatibility => this._compat;

  private int CalculatedCachedCompat()
  {
    return this.NormalArgumentCount | this.Defaults.Length << 14 | (this.ExpandDictPosition != -1 ? 1073741824 /*0x40000000*/ : 0) | (this.ExpandListPosition != -1 ? 536870912 /*0x20000000*/ : 0);
  }

  internal bool IsGeneratorWithExceptionHandling
  {
    get
    {
      return (this._code.Flags & (FunctionAttributes.Generator | FunctionAttributes.CanSetSysExcInfo)) == (FunctionAttributes.Generator | FunctionAttributes.CanSetSysExcInfo);
    }
  }

  internal int FunctionID => this._id;

  internal int ExpandListPosition
  {
    get
    {
      return (this._code.Flags & FunctionAttributes.ArgumentList) != FunctionAttributes.None ? this._code.co_argcount : -1;
    }
  }

  internal int ExpandDictPosition
  {
    get
    {
      if ((this._code.Flags & FunctionAttributes.KeywordDictionary) == FunctionAttributes.None)
        return -1;
      return (this._code.Flags & FunctionAttributes.ArgumentList) != FunctionAttributes.None ? this._code.co_argcount + 1 : this._code.co_argcount;
    }
  }

  internal int NormalArgumentCount => this._code.co_argcount;

  internal int ExtraArguments
  {
    get
    {
      return (this._code.Flags & FunctionAttributes.ArgumentList) != FunctionAttributes.None ? ((this._code.Flags & FunctionAttributes.KeywordDictionary) != FunctionAttributes.None ? 2 : 1) : ((this._code.Flags & FunctionAttributes.KeywordDictionary) != FunctionAttributes.None ? 1 : 0);
    }
  }

  internal FunctionAttributes Flags => this._code.Flags;

  internal object[] Defaults => this._defaults;

  internal Exception BadArgumentError(int count)
  {
    return (Exception) BinderOps.TypeErrorForIncorrectArgumentCount(this.__name__, this.NormalArgumentCount, this.Defaults.Length, count, this.ExpandListPosition != -1, false);
  }

  internal Exception BadKeywordArgumentError(int count)
  {
    return (Exception) BinderOps.TypeErrorForIncorrectArgumentCount(this.__name__, this.NormalArgumentCount, this.Defaults.Length, count, this.ExpandListPosition != -1, true);
  }

  IList<string> IMembersList.GetMemberNames()
  {
    return PythonOps.GetStringMemberList((IPythonMembersList) this);
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    List memberNames = this._dict != null ? PythonOps.MakeListFromSequence((object) this._dict) : PythonOps.MakeList();
    memberNames.AddNoLock((object) "__module__");
    memberNames.extend(TypeCache.Function.GetMemberNames(context, (object) this));
    return (IList<object>) memberNames;
  }

  WeakRefTracker IWeakReferenceable.GetWeakRef()
  {
    object obj;
    return this._dict != null && this._dict.TryGetValue((object) "__weakref__", out obj) ? obj as WeakRefTracker : (WeakRefTracker) null;
  }

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    this.EnsureDict();
    this._dict[(object) "__weakref__"] = (object) value;
    return true;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
  {
    ((IWeakReferenceable) this).SetWeakRef(value);
  }

  internal PythonDictionary EnsureDict()
  {
    if (this._dict == null)
      Interlocked.CompareExchange<PythonDictionary>(ref this._dict, PythonDictionary.MakeSymbolDictionary(), (PythonDictionary) null);
    return this._dict;
  }

  internal static int AddRecursionDepth(int change)
  {
    uint managedThreadId = (uint) Thread.CurrentThread.ManagedThreadId;
    return (long) managedThreadId < (long) PythonFunction._depth_fast.Length ? (PythonFunction._depth_fast[(int) managedThreadId] += change) : (PythonFunction.DepthSlow += change);
  }

  internal void EnsureID()
  {
    if (this._id != 0)
      return;
    Interlocked.CompareExchange(ref this._id, Interlocked.Increment(ref PythonFunction._CurrentId), 0);
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = (object) new Method((object) this, instance, (object) owner);
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  public string __repr__(CodeContext context)
  {
    return $"<function {this.func_name} at {PythonOps.HexId((object) this)}>";
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
  {
    return (DynamicMetaObject) new MetaPythonFunction(parameter, BindingRestrictions.Empty, this);
  }

  FastBindResult<T> IFastInvokable.MakeInvokeBinding<T>(
    CallSite<T> site,
    PythonInvokeBinder binder,
    CodeContext state,
    object[] args)
  {
    if (this.CanOptimizeCall(binder, args))
    {
      int functionCompatibility = this.FunctionCompatibility;
      ParameterInfo[] parameters = typeof (T).GetMethod("Invoke").GetParameters();
      Type[] array = ArrayUtils.ConvertAll<ParameterInfo, Type>(parameters, (Func<ParameterInfo, Type>) (inp => inp.ParameterType));
      if (array[2] != typeof (object))
        return new FastBindResult<T>();
      Type[] typeArray1 = ArrayUtils.Append<Type>(array, typeof (object));
      Type[] typeArray2 = new Type[parameters.Length - 3];
      for (int index = 3; index < parameters.Length; ++index)
        typeArray2[index - 3] = parameters[index].ParameterType;
      string str = "";
      if (args.Length != this.NormalArgumentCount)
        str = "Default" + (object) (this.NormalArgumentCount - args.Length);
      switch (args.Length)
      {
        case 0:
          if (string.IsNullOrEmpty(str))
            return new FastBindResult<T>((T) new Func<CallSite, CodeContext, object, object>(new FunctionCaller(functionCompatibility).Call0), true);
          FunctionCaller functionCaller1 = new FunctionCaller(functionCompatibility);
          return new FastBindResult<T>((T) typeof (FunctionCaller).GetMethod(str + "Call0").CreateDelegate(typeof (Func<CallSite, CodeContext, object, object>), (object) functionCaller1), true);
        case 1:
          Type callerType1 = typeof (FunctionCaller<>).MakeGenericType(typeArray2);
          MethodInfo method1 = callerType1.GetMethod(str + "Call1");
          FunctionCaller functionCaller2 = PythonFunction.GetFunctionCaller(callerType1, functionCompatibility);
          Type type1 = typeof (Func<,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method1.CreateDelegate(type1, (object) functionCaller2), true);
        case 2:
          Type callerType2 = typeof (FunctionCaller<,>).MakeGenericType(typeArray2);
          MethodInfo method2 = callerType2.GetMethod(str + "Call2");
          FunctionCaller functionCaller3 = PythonFunction.GetFunctionCaller(callerType2, functionCompatibility);
          Type type2 = typeof (Func<,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method2.CreateDelegate(type2, (object) functionCaller3), true);
        case 3:
          Type callerType3 = typeof (FunctionCaller<,,>).MakeGenericType(typeArray2);
          MethodInfo method3 = callerType3.GetMethod(str + "Call3");
          FunctionCaller functionCaller4 = PythonFunction.GetFunctionCaller(callerType3, functionCompatibility);
          Type type3 = typeof (Func<,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method3.CreateDelegate(type3, (object) functionCaller4), true);
        case 4:
          Type callerType4 = typeof (FunctionCaller<,,,>).MakeGenericType(typeArray2);
          MethodInfo method4 = callerType4.GetMethod(str + "Call4");
          FunctionCaller functionCaller5 = PythonFunction.GetFunctionCaller(callerType4, functionCompatibility);
          Type type4 = typeof (Func<,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method4.CreateDelegate(type4, (object) functionCaller5), true);
        case 5:
          Type callerType5 = typeof (FunctionCaller<,,,,>).MakeGenericType(typeArray2);
          MethodInfo method5 = callerType5.GetMethod(str + "Call5");
          FunctionCaller functionCaller6 = PythonFunction.GetFunctionCaller(callerType5, functionCompatibility);
          Type type5 = typeof (Func<,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method5.CreateDelegate(type5, (object) functionCaller6), true);
        case 6:
          Type callerType6 = typeof (FunctionCaller<,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method6 = callerType6.GetMethod(str + "Call6");
          FunctionCaller functionCaller7 = PythonFunction.GetFunctionCaller(callerType6, functionCompatibility);
          Type type6 = typeof (Func<,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method6.CreateDelegate(type6, (object) functionCaller7), true);
        case 7:
          Type callerType7 = typeof (FunctionCaller<,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method7 = callerType7.GetMethod(str + "Call7");
          FunctionCaller functionCaller8 = PythonFunction.GetFunctionCaller(callerType7, functionCompatibility);
          Type type7 = typeof (Func<,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method7.CreateDelegate(type7, (object) functionCaller8), true);
        case 8:
          Type callerType8 = typeof (FunctionCaller<,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method8 = callerType8.GetMethod(str + "Call8");
          FunctionCaller functionCaller9 = PythonFunction.GetFunctionCaller(callerType8, functionCompatibility);
          Type type8 = typeof (Func<,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method8.CreateDelegate(type8, (object) functionCaller9), true);
        case 9:
          Type callerType9 = typeof (FunctionCaller<,,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method9 = callerType9.GetMethod(str + "Call9");
          FunctionCaller functionCaller10 = PythonFunction.GetFunctionCaller(callerType9, functionCompatibility);
          Type type9 = typeof (Func<,,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method9.CreateDelegate(type9, (object) functionCaller10), true);
        case 10:
          Type callerType10 = typeof (FunctionCaller<,,,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method10 = callerType10.GetMethod(str + "Call10");
          FunctionCaller functionCaller11 = PythonFunction.GetFunctionCaller(callerType10, functionCompatibility);
          Type type10 = typeof (Func<,,,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method10.CreateDelegate(type10, (object) functionCaller11), true);
        case 11:
          Type callerType11 = typeof (FunctionCaller<,,,,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method11 = callerType11.GetMethod(str + "Call11");
          FunctionCaller functionCaller12 = PythonFunction.GetFunctionCaller(callerType11, functionCompatibility);
          Type type11 = typeof (Func<,,,,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method11.CreateDelegate(type11, (object) functionCaller12), true);
        case 12:
          Type callerType12 = typeof (FunctionCaller<,,,,,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method12 = callerType12.GetMethod(str + "Call12");
          FunctionCaller functionCaller13 = PythonFunction.GetFunctionCaller(callerType12, functionCompatibility);
          Type type12 = typeof (Func<,,,,,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method12.CreateDelegate(type12, (object) functionCaller13), true);
        case 13:
          Type callerType13 = typeof (FunctionCaller<,,,,,,,,,,,,>).MakeGenericType(typeArray2);
          MethodInfo method13 = callerType13.GetMethod(str + "Call13");
          FunctionCaller functionCaller14 = PythonFunction.GetFunctionCaller(callerType13, functionCompatibility);
          Type type13 = typeof (Func<,,,,,,,,,,,,,,,,>).MakeGenericType(typeArray1);
          return new FastBindResult<T>((T) method13.CreateDelegate(type13, (object) functionCaller14), true);
      }
    }
    return new FastBindResult<T>();
  }

  private bool CanOptimizeCall(PythonInvokeBinder binder, object[] args)
  {
    if (args.Length >= this.NormalArgumentCount - this._defaults.Length && args.Length <= this.NormalArgumentCount && this.ArgNames.Length < 14)
    {
      CallSignature signature = binder.Signature;
      if (!signature.HasDictionaryArgument())
      {
        signature = binder.Signature;
        if (!signature.HasKeywordArgument())
        {
          signature = binder.Signature;
          if (!signature.HasListArgument() && (this.Flags & (FunctionAttributes.ArgumentList | FunctionAttributes.KeywordDictionary)) == FunctionAttributes.None)
            return !binder.SupportsLightThrow();
        }
      }
    }
    return false;
  }

  private static FunctionCaller GetFunctionCaller(Type callerType, int funcCompat)
  {
    FunctionCaller functionCaller;
    lock (PythonFunction._functionCallers)
    {
      PythonFunction.FunctionCallerKey key1 = new PythonFunction.FunctionCallerKey(callerType, funcCompat);
      if (!PythonFunction._functionCallers.TryGetValue(key1, out functionCaller))
      {
        Dictionary<PythonFunction.FunctionCallerKey, FunctionCaller> functionCallers = PythonFunction._functionCallers;
        PythonFunction.FunctionCallerKey key2 = key1;
        Type type = callerType;
        object[] objArray = new object[1]
        {
          (object) funcCompat
        };
        FunctionCaller instance;
        functionCaller = instance = (FunctionCaller) Activator.CreateInstance(type, objArray);
        functionCallers[key2] = instance;
      }
    }
    return functionCaller;
  }

  private class FunctionCallerKey : IEquatable<PythonFunction.FunctionCallerKey>
  {
    public readonly Type CallerType;
    public readonly int FunctionCompat;

    public FunctionCallerKey(Type callerType, int compat)
    {
      this.CallerType = callerType;
      this.FunctionCompat = compat;
    }

    public bool Equals(PythonFunction.FunctionCallerKey other)
    {
      return this.CallerType == other.CallerType && this.FunctionCompat == other.FunctionCompat;
    }

    public override int GetHashCode() => this.CallerType.GetHashCode() ^ this.FunctionCompat;

    public override bool Equals(object obj)
    {
      return obj is PythonFunction.FunctionCallerKey other && this.Equals(other);
    }
  }
}
