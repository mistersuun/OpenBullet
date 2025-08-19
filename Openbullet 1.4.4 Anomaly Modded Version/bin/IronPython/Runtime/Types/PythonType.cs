// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonType
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[DebuggerDisplay("PythonType: {Name}")]
[DebuggerTypeProxy(typeof (PythonType.DebugProxy))]
[PythonType("type")]
[Documentation("type(object) -> gets the type of the object\r\ntype(name, bases, dict) -> creates a new type instance with the given name, base classes, and members from the dictionary")]
public class PythonType : 
  IPythonMembersList,
  IMembersList,
  IDynamicMetaObjectProvider,
  IWeakReferenceable,
  IWeakReferenceableByProxy,
  ICodeFormattable,
  IFastGettable,
  IFastSettable,
  IFastInvokable
{
  private Type _underlyingSystemType;
  private string _name;
  private Dictionary<string, PythonTypeSlot> _dict;
  private PythonType.PythonTypeAttributes _attrs;
  private int _flags;
  private int _version = PythonType.GetNextVersion();
  private List<WeakReference> _subtypes;
  private PythonContext _pythonContext;
  private bool? _objectNew;
  private bool? _objectInit;
  internal Dictionary<CachedGetKey, FastGetBase> _cachedGets;
  internal Dictionary<CachedGetKey, FastGetBase> _cachedTryGets;
  internal Dictionary<SetMemberKey, FastSetBase> _cachedSets;
  internal Dictionary<string, TypeGetBase> _cachedTypeGets;
  internal Dictionary<string, TypeGetBase> _cachedTypeTryGets;
  private List<PythonType> _resolutionOrder;
  private PythonType[] _bases;
  private BuiltinFunction _ctor;
  private Type _finalSystemType;
  private WeakRefTracker _weakrefTracker;
  private WeakReference _weakRef;
  private string[] _slots;
  private OldClass _oldClass;
  private int _originalSlotCount;
  private InstanceCreator _instanceCtor;
  private CallSite<Func<CallSite, object, int>> _hashSite;
  private CallSite<Func<CallSite, object, object, bool>> _eqSite;
  private CallSite<Func<CallSite, object, object, int>> _compareSite;
  private Dictionary<CallSignature, LateBoundInitBinder> _lateBoundInitBinders;
  private string[] _optimizedInstanceNames;
  private int _optimizedInstanceVersion;
  private Dictionary<string, List<MethodInfo>> _extensionMethods;
  private PythonSiteCache _siteCache = new PythonSiteCache();
  private PythonTypeSlot _lenSlot;
  internal Func<string, Exception> _makeException = PythonType.DefaultMakeException;
  private static int MasterVersion = 1;
  private static readonly CommonDictionaryStorage _pythonTypes = new CommonDictionaryStorage();
  internal static readonly PythonType _pythonTypeType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonType));
  private static readonly WeakReference[] _emptyWeakRef = new WeakReference[0];
  private static object _subtypesLock = new object();
  internal static readonly Func<string, Exception> DefaultMakeException = (Func<string, Exception>) (message => new Exception(message));
  private static Dictionary<Type, Dictionary<Type, Delegate>> _fastBindCtors = new Dictionary<Type, Dictionary<Type, Delegate>>();
  private static Dictionary<Type, BuiltinFunction> _userTypeCtors = new Dictionary<Type, BuiltinFunction>();
  private const int TypeFlagHeapType = 512 /*0x0200*/;
  private const int TypeFlagAbstractMethodsDefined = 524288 /*0x080000*/;
  private const int TypeFlagAbstractMethodsNonEmpty = 1048576 /*0x100000*/;
  [SlotField]
  public static PythonTypeSlot __dict__ = (PythonTypeSlot) new PythonTypeDictSlot(PythonType._pythonTypeType);

  public PythonType(CodeContext context, string name, PythonTuple bases, PythonDictionary dict)
    : this(context, name, bases, dict, string.Empty)
  {
  }

  internal PythonType(
    CodeContext context,
    string name,
    PythonTuple bases,
    PythonDictionary dict,
    string selfNames)
  {
    this.InitializeUserType(context, name, bases, dict, selfNames);
  }

  internal PythonType()
  {
  }

  internal PythonType(Type underlyingSystemType)
  {
    this._underlyingSystemType = underlyingSystemType;
    this.InitializeSystemType();
  }

  internal PythonType(PythonType baseType, string name, Func<string, Exception> exceptionMaker)
  {
    this._underlyingSystemType = baseType.UnderlyingSystemType;
    this.IsSystemType = baseType.IsSystemType;
    this.IsPythonType = baseType.IsPythonType;
    this.Name = name;
    this._bases = new PythonType[1]{ baseType };
    this.ResolutionOrder = (IList<PythonType>) Mro.Calculate(this, (IList<PythonType>) this._bases);
    this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
    this._makeException = exceptionMaker;
  }

  internal PythonType(PythonType[] baseTypes, string name)
  {
    bool flag1 = false;
    bool flag2 = false;
    foreach (PythonType baseType in baseTypes)
    {
      flag1 |= baseType.IsSystemType;
      flag2 |= baseType.IsPythonType;
    }
    this.IsSystemType = flag1;
    this.IsPythonType = flag2;
    this.Name = name;
    this._bases = baseTypes;
    this.ResolutionOrder = (IList<PythonType>) Mro.Calculate(this, (IList<PythonType>) this._bases);
    this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
  }

  internal PythonType(
    PythonType[] baseTypes,
    Type underlyingType,
    string name,
    Func<string, Exception> exceptionMaker)
    : this(baseTypes, name)
  {
    this._underlyingSystemType = underlyingType;
    this._makeException = exceptionMaker;
  }

  internal PythonType(
    PythonContext context,
    PythonType baseType,
    string name,
    string module,
    string doc,
    Func<string, Exception> exceptionMaker)
    : this(baseType, name, exceptionMaker)
  {
    this.EnsureDict();
    this._dict["__doc__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) doc, true);
    this._dict["__module__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) module, true);
    this.IsSystemType = false;
    this.IsPythonType = false;
    this._pythonContext = context;
    this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
  }

  internal PythonType(
    PythonContext context,
    PythonType[] baseTypes,
    string name,
    string module,
    string doc)
    : this(baseTypes, name)
  {
    this.EnsureDict();
    this._dict["__doc__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) doc, true);
    this._dict["__module__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) module, true);
    this._pythonContext = context;
    this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
  }

  internal PythonType(
    PythonContext context,
    PythonType[] baseTypes,
    Type underlyingType,
    string name,
    string module,
    string doc,
    Func<string, Exception> exceptionMaker)
    : this(baseTypes, underlyingType, name, exceptionMaker)
  {
    this.EnsureDict();
    this._dict["__doc__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) doc, true);
    this._dict["__module__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) module, true);
    this.IsSystemType = false;
    this.IsPythonType = false;
    this._pythonContext = context;
    this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
  }

  internal PythonType(OldClass oc)
  {
    this.EnsureDict();
    this._underlyingSystemType = typeof (OldInstance);
    this.Name = oc.Name;
    this.OldClass = oc;
    List<PythonType> pythonTypeList1 = new List<PythonType>(oc.BaseClasses.Count);
    foreach (OldClass baseClass in oc.BaseClasses)
      pythonTypeList1.Add(baseClass.TypeObject);
    List<PythonType> pythonTypeList2 = new List<PythonType>();
    pythonTypeList2.Add(this);
    this._bases = pythonTypeList1.ToArray();
    this._resolutionOrder = pythonTypeList2;
    this.AddSlot("__class__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) this, true));
  }

  internal BuiltinFunction Ctor
  {
    get
    {
      this.EnsureConstructor();
      return this._ctor;
    }
  }

  public static object __new__(
    CodeContext context,
    PythonType cls,
    string name,
    PythonTuple bases,
    PythonDictionary dict)
  {
    return PythonType.__new__(context, cls, name, bases, dict, string.Empty);
  }

  internal static object __new__(
    CodeContext context,
    PythonType cls,
    string name,
    PythonTuple bases,
    PythonDictionary dict,
    string selfNames)
  {
    if (name == null)
      throw PythonOps.TypeError("type() argument 1 must be string, not None");
    if (bases == null)
      throw PythonOps.TypeError("type() argument 2 must be tuple, not None");
    if (dict == null)
      throw PythonOps.TypeError("TypeError: type() argument 3 must be dict, not None");
    PythonType.EnsureModule(context, dict);
    PythonType metaClass = PythonType.FindMetaClass(cls, bases);
    if (metaClass == TypeCache.OldInstance || metaClass == TypeCache.PythonType)
      return (object) new PythonType(context, name, bases, dict, selfNames);
    if (metaClass == cls)
      return metaClass.CreateInstance(context, (object) name, (object) bases, (object) dict);
    return PythonCalls.Call(context, (object) metaClass, (object) name, (object) bases, (object) dict);
  }

  public void __init__(string name, PythonTuple bases, PythonDictionary dict)
  {
  }

  internal static PythonType FindMetaClass(PythonType cls, PythonTuple bases)
  {
    PythonType other = cls;
    foreach (object o in bases)
    {
      PythonType pythonType = DynamicHelpers.GetPythonType(o);
      if (pythonType != TypeCache.OldClass && !other.IsSubclassOf(pythonType))
        other = pythonType.IsSubclassOf(other) ? pythonType : throw PythonOps.TypeError("Error when calling the metaclass bases\n    metaclass conflict: the metaclass of a derived class must be a (non-strict) subclass of the metaclasses of all its bases");
    }
    return other;
  }

  public static object __new__(CodeContext context, object cls, object o)
  {
    return (object) DynamicHelpers.GetPythonType(o);
  }

  public void __init__(object o)
  {
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static PythonTuple Get__bases__(CodeContext context, PythonType type)
  {
    return type.GetBasesTuple();
  }

  private PythonTuple GetBasesTuple()
  {
    object[] objArray = new object[this.BaseTypes.Count];
    IList<PythonType> baseTypes = this.BaseTypes;
    for (int index = 0; index < baseTypes.Count; ++index)
    {
      PythonType pythonType = baseTypes[index];
      objArray[index] = !pythonType.IsOldClass ? (object) pythonType : (object) pythonType.OldClass;
    }
    return PythonTuple.MakeTuple(objArray);
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static PythonType Get__base__(CodeContext context, PythonType type)
  {
    foreach (object obj in PythonType.Get__bases__(context, type))
    {
      if (obj is PythonType pythonType)
        return pythonType;
    }
    return (PythonType) null;
  }

  private bool SetAbstractMethodFlags(string name, object value)
  {
    if (name != "__abstractmethods__")
      return false;
    int num = this._flags | 524288 /*0x080000*/;
    IEnumerator enumerator;
    this._flags = value == null || !PythonOps.TryGetEnumerator(DefaultContext.Default, value, out enumerator) || !enumerator.MoveNext() ? num & -1048577 : num | 1048576 /*0x100000*/;
    return true;
  }

  internal bool IsIterable(CodeContext context)
  {
    object ret = (object) null;
    return PythonOps.TryGetBoundAttr(context, (object) this, "__iter__", out ret) && ret != NotImplementedType.Value && PythonOps.TryGetBoundAttr(context, (object) this, "next", out ret) && ret != NotImplementedType.Value;
  }

  private void ClearAbstractMethodFlags(string name)
  {
    if (!(name == "__abstractmethods__"))
      return;
    this._flags &= -1572865;
  }

  internal bool HasAbstractMethods(CodeContext context)
  {
    object enumerable;
    IEnumerator enumerator;
    return (this._flags & 1048576 /*0x100000*/) != 0 && this.TryGetBoundCustomMember(context, "__abstractmethods__", out enumerable) && PythonOps.TryGetEnumerator(context, enumerable, out enumerator) && enumerator.MoveNext();
  }

  internal string GetAbstractErrorMessage(CodeContext context)
  {
    if ((this._flags & 1048576 /*0x100000*/) == 0)
      return (string) null;
    object enumerable;
    IEnumerator enumerator;
    if (!this.TryGetBoundCustomMember(context, "__abstractmethods__", out enumerable) || !PythonOps.TryGetEnumerator(context, enumerable, out enumerator) || !enumerator.MoveNext())
      return (string) null;
    string str = "";
    StringBuilder stringBuilder = new StringBuilder("Can't instantiate abstract class ");
    stringBuilder.Append(this.Name);
    stringBuilder.Append(" with abstract methods ");
    int num = 0;
    do
    {
      if (!(enumerator.Current is string current1) && enumerator.Current is Extensible<string> current2)
        current1 = current2.Value;
      if (current1 == null)
        return $"sequence item {num}: expected string, {PythonTypeOps.GetName(enumerator.Current)} found";
      stringBuilder.Append(str);
      stringBuilder.Append(enumerator.Current);
      str = ", ";
      ++num;
    }
    while (enumerator.MoveNext());
    return stringBuilder.ToString();
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static int Get__flags__(CodeContext context, PythonType type)
  {
    int flags = type._flags;
    if (type.IsSystemType)
      flags |= 512 /*0x0200*/;
    return flags;
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static void Set__bases__(CodeContext context, PythonType type, object value)
  {
    if (!(value is PythonTuple bases))
      throw PythonOps.TypeError("expected tuple of types or old-classes, got '{0}'", (object) PythonTypeOps.GetName(value));
    List<PythonType> ldt = new List<PythonType>();
    foreach (object o in bases)
    {
      switch (o)
      {
        case PythonType typeObject:
label_7:
          ldt.Add(typeObject);
          continue;
        case OldClass oldClass:
          typeObject = oldClass.TypeObject;
          goto label_7;
        default:
          throw PythonOps.TypeError("expected tuple of types, got '{0}'", (object) PythonTypeOps.GetName(o));
      }
    }
    Type newType = NewTypeMaker.GetNewType(type.Name, bases);
    if (type.UnderlyingSystemType != newType)
      throw PythonOps.TypeErrorForIncompatibleObjectLayout("__bases__ assignment", type, newType);
    List<PythonType> mro = PythonType.CalculateMro(type, (IList<PythonType>) ldt);
    type.BaseTypes = (IList<PythonType>) ldt;
    type._resolutionOrder = mro;
  }

  private static List<PythonType> CalculateMro(PythonType type, IList<PythonType> ldt)
  {
    return Mro.Calculate(type, ldt);
  }

  private static bool TryReplaceExtensibleWithBase(Type curType, out Type newType)
  {
    if (curType.IsGenericType && curType.GetGenericTypeDefinition() == typeof (Extensible<>))
    {
      newType = curType.GetGenericArguments()[0];
      return true;
    }
    newType = (Type) null;
    return false;
  }

  public object __call__(CodeContext context, params object[] args)
  {
    return PythonTypeOps.CallParams(context, this, args);
  }

  public object __call__(
    CodeContext context,
    [ParamDictionary] IDictionary<string, object> kwArgs,
    params object[] args)
  {
    return PythonTypeOps.CallWorker(context, this, kwArgs, args);
  }

  public int __cmp__([NotNull] PythonType other)
  {
    if (other == this)
      return 0;
    int num = this.Name.CompareTo(other.Name);
    if (num != 0)
      return num;
    return IdDispenser.GetId((object) this) > IdDispenser.GetId((object) other) ? 1 : -1;
  }

  public bool __eq__([NotNull] PythonType other) => this.__cmp__(other) == 0;

  public bool __ne__([NotNull] PythonType other) => this.__cmp__(other) != 0;

  [Python3Warning("type inequality comparisons not supported in 3.x")]
  public static bool operator >(PythonType self, PythonType other) => self.__cmp__(other) > 0;

  [Python3Warning("type inequality comparisons not supported in 3.x")]
  public static bool operator <(PythonType self, PythonType other) => self.__cmp__(other) < 0;

  [Python3Warning("type inequality comparisons not supported in 3.x")]
  public static bool operator >=(PythonType self, PythonType other) => self.__cmp__(other) >= 0;

  [Python3Warning("type inequality comparisons not supported in 3.x")]
  public static bool operator <=(PythonType self, PythonType other) => self.__cmp__(other) <= 0;

  public void __delattr__(CodeContext context, string name)
  {
    this.DeleteCustomMember(context, name);
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static object Get__doc__(CodeContext context, PythonType self)
  {
    PythonTypeSlot slot;
    object doc;
    if (self.TryLookupSlot(context, "__doc__", out slot) && slot.TryGetValue(context, (object) null, self, out doc))
      return doc;
    return self.IsSystemType ? (object) PythonTypeOps.GetDocumentation(self.UnderlyingSystemType) : (object) null;
  }

  public object __getattribute__(CodeContext context, string name)
  {
    object obj;
    if (this.TryGetBoundCustomMember(context, name, out obj))
      return obj;
    throw PythonOps.AttributeError("type object '{0}' has no attribute '{1}'", (object) this.Name, (object) name);
  }

  public PythonType this[params Type[] args]
  {
    get
    {
      if (this.UnderlyingSystemType == typeof (Array))
        return args.Length == 1 ? DynamicHelpers.GetPythonTypeFromType(args[0].MakeArrayType()) : throw PythonOps.TypeError("expected one argument to make array type, got {0}", (object) args.Length);
      return this.UnderlyingSystemType.IsGenericTypeDefinition() ? DynamicHelpers.GetPythonTypeFromType(this.UnderlyingSystemType.MakeGenericType(args)) : throw new InvalidOperationException("MakeGenericType on non-generic type");
    }
  }

  public object this[string member]
  {
    get
    {
      if (!this.UnderlyingSystemType.IsEnum)
        throw PythonOps.TypeError("'type' object is not subscriptable");
      if (member == null)
        throw PythonOps.KeyError(member);
      try
      {
        return Enum.Parse(this.UnderlyingSystemType, member);
      }
      catch (ArgumentException ex)
      {
        throw PythonOps.KeyError(member);
      }
    }
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static object Get__module__(CodeContext context, PythonType self)
  {
    PythonTypeSlot pythonTypeSlot;
    object obj;
    return self._dict != null && self._dict.TryGetValue("__module__", out pythonTypeSlot) && pythonTypeSlot.TryGetValue(context, (object) self, DynamicHelpers.GetPythonType((object) self), out obj) ? obj : (object) PythonTypeOps.GetModuleName(context, self.UnderlyingSystemType);
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [PythonHidden(new PlatformID[] {})]
  [SpecialName]
  public static string Get__clr_assembly__(PythonType self)
  {
    return $"{self.UnderlyingSystemType.Namespace} in {self.UnderlyingSystemType.Assembly.FullName}";
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static void Set__module__(CodeContext context, PythonType self, object value)
  {
    if (self.IsSystemType)
      throw PythonOps.TypeError("can't set {0}.__module__", (object) self.Name);
    self._dict["__module__"] = (PythonTypeSlot) new PythonTypeUserDescriptorSlot(value);
    self.UpdateVersion();
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static void Delete__module__(CodeContext context, PythonType self)
  {
    throw PythonOps.TypeError("can't delete {0}.__module__", (object) self.Name);
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static PythonTuple Get__mro__(PythonType type)
  {
    return PythonTypeOps.MroToPython(type.ResolutionOrder);
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static string Get__name__(PythonType type) => type.Name;

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static void Set__name__(PythonType type, string name)
  {
    if (type.IsSystemType)
      throw PythonOps.TypeError("can't set attributes of built-in/extension type '{0}'", (object) type.Name);
    type.Name = name;
  }

  public string __repr__(CodeContext context)
  {
    string name = this.Name;
    if (this.IsSystemType)
    {
      if (PythonTypeOps.IsRuntimeAssembly(this.UnderlyingSystemType.Assembly) || this.IsPythonType)
      {
        object obj = PythonType.Get__module__(context, this);
        if (!obj.Equals((object) "__builtin__"))
          return $"<type '{obj}.{this.Name}'>";
      }
      return $"<type '{this.Name}'>";
    }
    string str = "unknown";
    PythonTypeSlot slot;
    object obj1;
    if (this.TryLookupSlot(context, "__module__", out slot) && slot.TryGetValue(context, (object) this, this, out obj1))
      str = obj1 as string;
    return $"<class '{str}.{name}'>";
  }

  internal string GetTypeDebuggerDisplay()
  {
    string str = "unknown";
    PythonTypeSlot slot;
    object obj;
    if (this.TryLookupSlot(this.Context.SharedContext, "__module__", out slot) && slot.TryGetValue(this.Context.SharedContext, (object) this, this, out obj))
      str = obj as string;
    return $"{str}.{this.Name} instance";
  }

  public void __setattr__(CodeContext context, string name, object value)
  {
    this.SetCustomMember(context, name, value);
  }

  public List __subclasses__(CodeContext context)
  {
    List list = new List();
    IList<WeakReference> subTypes = this.SubTypes;
    if (subTypes != null)
    {
      PythonContext languageContext = context.LanguageContext;
      foreach (WeakReference weakReference in (IEnumerable<WeakReference>) subTypes)
      {
        if (weakReference.IsAlive)
        {
          PythonType target = (PythonType) weakReference.Target;
          if (target.PythonContext == null || target.PythonContext == languageContext)
            list.AddNoLock(weakReference.Target);
        }
      }
    }
    return list;
  }

  public virtual List mro() => new List((ICollection) PythonType.Get__mro__(this));

  public virtual bool __instancecheck__(object instance)
  {
    return this.SubclassImpl(DynamicHelpers.GetPythonType(instance));
  }

  public virtual bool __subclasscheck__(PythonType sub) => this.SubclassImpl(sub);

  private bool SubclassImpl(PythonType sub)
  {
    return this.UnderlyingSystemType.IsInterface() && this.UnderlyingSystemType.IsAssignableFrom(sub.UnderlyingSystemType) || sub.IsSubclassOf(this);
  }

  public virtual bool __subclasscheck__(OldClass sub) => this.IsSubclassOf(sub.TypeObject);

  public static implicit operator Type(PythonType self) => self.UnderlyingSystemType;

  public static implicit operator TypeTracker(PythonType self)
  {
    return ReflectionCache.GetTypeTracker(self.UnderlyingSystemType);
  }

  internal bool IsMixedNewStyleOldStyle()
  {
    if (!this.IsOldClass)
    {
      foreach (PythonType pythonType in (IEnumerable<PythonType>) this.ResolutionOrder)
      {
        if (pythonType.IsOldClass)
          return true;
      }
    }
    return false;
  }

  internal int SlotCount => this._originalSlotCount;

  internal string Name
  {
    get => this._name;
    set => this._name = value;
  }

  internal int Version => this._version;

  internal bool IsNull => this.UnderlyingSystemType == typeof (DynamicNull);

  internal IList<PythonType> ResolutionOrder
  {
    get => (IList<PythonType>) this._resolutionOrder;
    set
    {
      lock (this.SyncRoot)
        this._resolutionOrder = new List<PythonType>((IEnumerable<PythonType>) value);
    }
  }

  internal static PythonType GetPythonType(Type type)
  {
    object pythonType;
    if (!PythonType._pythonTypes.TryGetValue((object) type, out pythonType))
    {
      lock (PythonType._pythonTypes)
      {
        if (!PythonType._pythonTypes.TryGetValue((object) type, out pythonType))
        {
          pythonType = (object) new PythonType(type);
          PythonType._pythonTypes.Add((object) type, pythonType);
        }
      }
    }
    return (PythonType) pythonType;
  }

  internal static PythonType SetPythonType(Type type, PythonType pyType)
  {
    lock (PythonType._pythonTypes)
      PythonType._pythonTypes.Add((object) type, (object) pyType);
    return pyType;
  }

  internal object CreateInstance(CodeContext context)
  {
    this.EnsureInstanceCtor();
    return this._instanceCtor.CreateInstance(context);
  }

  internal object CreateInstance(CodeContext context, object arg0)
  {
    this.EnsureInstanceCtor();
    return this._instanceCtor.CreateInstance(context, arg0);
  }

  internal object CreateInstance(CodeContext context, object arg0, object arg1)
  {
    this.EnsureInstanceCtor();
    return this._instanceCtor.CreateInstance(context, arg0, arg1);
  }

  internal object CreateInstance(CodeContext context, object arg0, object arg1, object arg2)
  {
    this.EnsureInstanceCtor();
    return this._instanceCtor.CreateInstance(context, arg0, arg1, arg2);
  }

  internal object CreateInstance(CodeContext context, params object[] args)
  {
    this.EnsureInstanceCtor();
    switch (args.Length)
    {
      case 0:
        return this._instanceCtor.CreateInstance(context);
      case 1:
        return this._instanceCtor.CreateInstance(context, args[0]);
      case 2:
        return this._instanceCtor.CreateInstance(context, args[0], args[1]);
      case 3:
        return this._instanceCtor.CreateInstance(context, args[0], args[1], args[2]);
      default:
        return this._instanceCtor.CreateInstance(context, args);
    }
  }

  internal object CreateInstance(CodeContext context, object[] args, string[] names)
  {
    this.EnsureInstanceCtor();
    return this._instanceCtor.CreateInstance(context, args, names);
  }

  internal int Hash(object o)
  {
    this.EnsureHashSite();
    return this._hashSite.Target((CallSite) this._hashSite, o);
  }

  internal bool TryGetLength(CodeContext context, object o, out int length)
  {
    CallSite<Func<CallSite, CodeContext, object, object>> callSite = !this.IsSystemType ? this._siteCache.GetLenSite(context) : context.LanguageContext.GetSiteCacheForSystemType(this.UnderlyingSystemType).GetLenSite(context);
    PythonTypeSlot slot = this._lenSlot;
    if (slot == null && !PythonOps.TryResolveTypeSlot(context, this, "__len__", out slot))
    {
      length = 0;
      return false;
    }
    object obj;
    if (!slot.TryGetValue(context, o, this, out obj))
    {
      length = 0;
      return false;
    }
    if (!(callSite.Target((CallSite) callSite, context, obj) is int num))
      throw PythonOps.ValueError("__len__ must return int");
    length = num;
    return true;
  }

  internal bool EqualRetBool(object self, object other)
  {
    if (this._eqSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, bool>>>(ref this._eqSite, this.Context.CreateComparisonSite(PythonOperationKind.Equal), (CallSite<Func<CallSite, object, object, bool>>) null);
    return this._eqSite.Target((CallSite) this._eqSite, self, other);
  }

  internal int Compare(object self, object other)
  {
    if (this._compareSite == null)
      Interlocked.CompareExchange<CallSite<Func<CallSite, object, object, int>>>(ref this._compareSite, this.Context.MakeSortCompareSite(), (CallSite<Func<CallSite, object, object, int>>) null);
    return this._compareSite.Target((CallSite) this._compareSite, self, other);
  }

  internal bool TryGetBoundAttr(CodeContext context, object o, string name, out object ret)
  {
    CallSite<Func<CallSite, object, CodeContext, object>> callSite = !this.IsSystemType ? this._siteCache.GetTryGetMemberSite(context, name) : context.LanguageContext.GetSiteCacheForSystemType(this.UnderlyingSystemType).GetTryGetMemberSite(context, name);
    try
    {
      ret = callSite.Target((CallSite) callSite, o, context);
      return ret != OperationFailed.Value;
    }
    catch (MissingMemberException ex)
    {
      ret = (object) null;
      return false;
    }
  }

  internal CallSite<Func<CallSite, object, int>> HashSite
  {
    get
    {
      this.EnsureHashSite();
      return this._hashSite;
    }
  }

  private void EnsureHashSite()
  {
    if (this._hashSite != null)
      return;
    Interlocked.CompareExchange<CallSite<Func<CallSite, object, int>>>(ref this._hashSite, CallSite<Func<CallSite, object, int>>.Create((CallSiteBinder) this.Context.Operation(PythonOperationKind.Hash)), (CallSite<Func<CallSite, object, int>>) null);
  }

  internal Type UnderlyingSystemType => this._underlyingSystemType;

  internal Type FinalSystemType
  {
    get
    {
      Type finalSystemType = this._finalSystemType;
      return (object) finalSystemType != null ? finalSystemType : (this._finalSystemType = PythonTypeOps.GetFinalSystemType(this._underlyingSystemType));
    }
  }

  internal Type ExtensionType
  {
    get
    {
      if (!this._underlyingSystemType.IsEnum())
      {
        switch (this._underlyingSystemType.GetTypeCode())
        {
          case TypeCode.Object:
            if (this._underlyingSystemType == typeof (BigInteger))
              return typeof (Extensible<BigInteger>);
            if (this._underlyingSystemType == typeof (Complex))
              return typeof (ExtensibleComplex);
            break;
          case TypeCode.Int32:
            return typeof (Extensible<int>);
          case TypeCode.Double:
            return typeof (Extensible<double>);
          case TypeCode.String:
            return typeof (ExtensibleString);
        }
      }
      return this._underlyingSystemType;
    }
  }

  internal IList<PythonType> BaseTypes
  {
    get => (IList<PythonType>) this._bases;
    set
    {
      foreach (PythonType pythonType in (IEnumerable<PythonType>) value)
      {
        if (pythonType == null)
          throw new ArgumentNullException(nameof (value), "a PythonType was null while assigning base classes");
      }
      lock (this._bases)
      {
        foreach (PythonType pythonType in this._bases)
          pythonType.RemoveSubType(this);
        List<PythonType> pythonTypeList = new List<PythonType>((IEnumerable<PythonType>) value);
        foreach (PythonType pythonType in pythonTypeList)
          pythonType.AddSubType(this);
        this.UpdateVersion();
        this._bases = pythonTypeList.ToArray();
      }
    }
  }

  internal bool IsSubclassOf(PythonType other)
  {
    return other == this || other.UnderlyingSystemType == typeof (ValueType) && this.UnderlyingSystemType.IsValueType() || this.IsSubclassWorker(other);
  }

  private bool IsSubclassWorker(PythonType other)
  {
    for (int index = 0; index < this._bases.Length; ++index)
    {
      PythonType pythonType = this._bases[index];
      if (pythonType == other || pythonType.IsSubclassWorker(other))
        return true;
    }
    return false;
  }

  internal bool IsSystemType
  {
    get => (this._attrs & PythonType.PythonTypeAttributes.SystemType) != 0;
    set
    {
      if (value)
        this._attrs |= PythonType.PythonTypeAttributes.SystemType;
      else
        this._attrs &= ~PythonType.PythonTypeAttributes.SystemType;
    }
  }

  internal bool IsWeakReferencable
  {
    get => (this._attrs & PythonType.PythonTypeAttributes.WeakReferencable) != 0;
    set
    {
      if (value)
        this._attrs |= PythonType.PythonTypeAttributes.WeakReferencable;
      else
        this._attrs &= ~PythonType.PythonTypeAttributes.WeakReferencable;
    }
  }

  internal bool HasDictionary
  {
    get => (this._attrs & PythonType.PythonTypeAttributes.HasDictionary) != 0;
    set
    {
      if (value)
        this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
      else
        this._attrs &= ~PythonType.PythonTypeAttributes.HasDictionary;
    }
  }

  internal bool HasSystemCtor => (this._attrs & PythonType.PythonTypeAttributes.SystemCtor) != 0;

  internal void SetConstructor(BuiltinFunction ctor) => this._ctor = ctor;

  internal bool IsPythonType
  {
    get => (this._attrs & PythonType.PythonTypeAttributes.IsPythonType) != 0;
    set
    {
      if (value)
        this._attrs |= PythonType.PythonTypeAttributes.IsPythonType;
      else
        this._attrs &= ~PythonType.PythonTypeAttributes.IsPythonType;
    }
  }

  internal OldClass OldClass
  {
    get => this._oldClass;
    set => this._oldClass = value;
  }

  internal bool IsOldClass => this._oldClass != null;

  internal PythonContext PythonContext => this._pythonContext;

  internal PythonContext Context => this._pythonContext ?? DefaultContext.DefaultPythonContext;

  internal object SyncRoot => (object) this;

  internal bool IsHiddenMember(string name)
  {
    PythonTypeSlot slot;
    return !this.TryResolveSlot(DefaultContext.Default, name, out slot) && this.TryResolveSlot(DefaultContext.DefaultCLS, name, out slot);
  }

  internal LateBoundInitBinder GetLateBoundInitBinder(CallSignature signature)
  {
    if (this._lateBoundInitBinders == null)
      Interlocked.CompareExchange<Dictionary<CallSignature, LateBoundInitBinder>>(ref this._lateBoundInitBinders, new Dictionary<CallSignature, LateBoundInitBinder>(), (Dictionary<CallSignature, LateBoundInitBinder>) null);
    lock (this._lateBoundInitBinders)
    {
      LateBoundInitBinder lateBoundInitBinder;
      if (!this._lateBoundInitBinders.TryGetValue(signature, out lateBoundInitBinder))
        this._lateBoundInitBinders[signature] = lateBoundInitBinder = new LateBoundInitBinder(this, signature);
      return lateBoundInitBinder;
    }
  }

  internal Dictionary<string, List<MethodInfo>> ExtensionMethods
  {
    get
    {
      if (this._extensionMethods == null)
      {
        Dictionary<string, List<MethodInfo>> dictionary = new Dictionary<string, List<MethodInfo>>();
        foreach (MethodInfo method in this.UnderlyingSystemType.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
          if (method.IsExtension())
          {
            List<MethodInfo> methodInfoList;
            if (!dictionary.TryGetValue(method.Name, out methodInfoList))
              dictionary[method.Name] = methodInfoList = new List<MethodInfo>();
            methodInfoList.Add(method);
          }
        }
        this._extensionMethods = dictionary;
      }
      return this._extensionMethods;
    }
  }

  internal bool TryLookupSlot(CodeContext context, string name, out PythonTypeSlot slot)
  {
    return this.IsSystemType ? PythonBinder.GetBinder(context).TryLookupSlot(context, this, name, out slot) : this._dict.TryGetValue(name, out slot);
  }

  internal bool TryResolveSlot(CodeContext context, string name, out PythonTypeSlot slot)
  {
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonType type = this._resolutionOrder[index];
      if (type.IsSystemType && !type.UnderlyingSystemType.IsInterface())
        return PythonBinder.GetBinder(context).TryResolveSlot(context, type, this, name, out slot);
      if (type.TryLookupSlot(context, name, out slot))
        return true;
    }
    if (this.UnderlyingSystemType.IsInterface())
      return TypeCache.Object.TryResolveSlot(context, name, out slot);
    slot = (PythonTypeSlot) null;
    return false;
  }

  internal bool TryResolveMixedSlot(CodeContext context, string name, out PythonTypeSlot slot)
  {
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonType pythonType = this._resolutionOrder[index];
      if (pythonType.TryLookupSlot(context, name, out slot))
        return true;
      object ret;
      if (pythonType.OldClass != null && pythonType.OldClass.TryLookupSlot(name, out ret))
      {
        slot = PythonType.ToTypeSlot(ret);
        return true;
      }
    }
    slot = (PythonTypeSlot) null;
    return false;
  }

  internal void AddSlot(string name, PythonTypeSlot slot)
  {
    this._dict[name] = slot;
    switch (name)
    {
      case "__new__":
        this._objectNew = new bool?();
        this.ClearObjectNewInSubclasses(this);
        break;
      case "__init__":
        this._objectInit = new bool?();
        this.ClearObjectInitInSubclasses(this);
        break;
    }
  }

  private void ClearObjectNewInSubclasses(PythonType pt)
  {
    lock (PythonType._subtypesLock)
    {
      if (pt._subtypes == null)
        return;
      foreach (WeakReference subtype in pt._subtypes)
      {
        if (subtype.Target is PythonType target)
        {
          target._objectNew = new bool?();
          this.ClearObjectNewInSubclasses(target);
        }
      }
    }
  }

  private void ClearObjectInitInSubclasses(PythonType pt)
  {
    lock (PythonType._subtypesLock)
    {
      if (pt._subtypes == null)
        return;
      foreach (WeakReference subtype in pt._subtypes)
      {
        if (subtype.Target is PythonType target)
        {
          target._objectInit = new bool?();
          this.ClearObjectInitInSubclasses(target);
        }
      }
    }
  }

  internal bool TryGetCustomSetAttr(CodeContext context, out PythonTypeSlot pts)
  {
    return context.LanguageContext.Binder.TryResolveSlot(context, DynamicHelpers.GetPythonType((object) this), this, "__setattr__", out pts) && pts is BuiltinMethodDescriptor && ((BuiltinMethodDescriptor) pts).DeclaringType != typeof (PythonType);
  }

  internal void SetCustomMember(CodeContext context, string name, object value)
  {
    PythonTypeSlot slot;
    if (this.TryResolveSlot(context, name, out slot) && slot.TrySetValue(context, (object) null, this, value) || PythonType._pythonTypeType.TryResolveSlot(context, name, out slot) && slot.TrySetValue(context, (object) this, PythonType._pythonTypeType, value))
      return;
    if (this.IsSystemType)
      throw new MissingMemberException($"'{this.Name}' object has no attribute '{name}'");
    PythonTypeSlot pythonTypeSlot;
    if (!(value is PythonTypeSlot) && this._dict.TryGetValue(name, out pythonTypeSlot) && pythonTypeSlot is PythonTypeUserDescriptorSlot)
    {
      if (this.SetAbstractMethodFlags(name, value))
        this.UpdateVersion();
      ((PythonTypeUserDescriptorSlot) pythonTypeSlot).Value = value;
    }
    else
    {
      this.SetAbstractMethodFlags(name, value);
      this.AddSlot(name, PythonType.ToTypeSlot(value));
      this.UpdateVersion();
    }
  }

  internal static PythonTypeSlot ToTypeSlot(object value)
  {
    if (value is PythonTypeSlot typeSlot)
      return typeSlot;
    return value != null ? (PythonTypeSlot) new PythonTypeUserDescriptorSlot(value) : (PythonTypeSlot) new PythonTypeUserDescriptorSlot(value, true);
  }

  internal bool DeleteCustomMember(CodeContext context, string name)
  {
    PythonTypeSlot slot;
    if (this.TryResolveSlot(context, name, out slot) && slot.TryDeleteValue(context, (object) null, this))
      return true;
    if (this.IsSystemType)
      throw new MissingMemberException($"can't delete attributes of built-in/extension type '{this.Name}'");
    if (!this._dict.Remove(name))
      throw new MissingMemberException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MemberDoesNotExist, (object) name.ToString()));
    this.ClearAbstractMethodFlags(name);
    this.UpdateVersion();
    return true;
  }

  internal bool TryGetBoundCustomMember(CodeContext context, string name, out object value)
  {
    PythonTypeSlot slot;
    if (this.TryResolveSlot(context, name, out slot) && slot.TryGetValue(context, (object) null, this, out value))
      return true;
    PythonType pythonType = DynamicHelpers.GetPythonType((object) this);
    if (pythonType.TryResolveSlot(context, name, out slot) && slot.TryGetValue(context, (object) this, pythonType, out value))
      return true;
    value = (object) null;
    return false;
  }

  T IFastGettable.MakeGetBinding<T>(
    CallSite<T> site,
    PythonGetMemberBinder binder,
    CodeContext context,
    string name)
  {
    return (T) new MetaPythonType.FastGetBinderHelper(this, context, binder).GetBinding();
  }

  internal object GetMember(CodeContext context, object instance, string name)
  {
    object member;
    if (this.TryGetMember(context, instance, name, out member))
      return member;
    throw new MissingMemberException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CantFindMember, (object) name));
  }

  internal void SetMember(CodeContext context, object instance, string name, object value)
  {
    if (!this.TrySetMember(context, instance, name, value))
      throw new MissingMemberException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Slot_CantSet, (object) name));
  }

  internal void DeleteMember(CodeContext context, object instance, string name)
  {
    if (!this.TryDeleteMember(context, instance, name))
      throw new MissingMemberException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "couldn't delete member {0}", (object) name));
  }

  internal bool TryGetMember(CodeContext context, object instance, string name, out object value)
  {
    if (this.TryGetNonCustomMember(context, instance, name, out value))
      return true;
    try
    {
      if (PythonTypeOps.TryInvokeBinaryOperator(context, instance, (object) name, "__getattr__", out value))
        return true;
    }
    catch (MissingMemberException ex)
    {
    }
    return false;
  }

  internal bool TryGetNonCustomMember(
    CodeContext context,
    object instance,
    string name,
    out object value)
  {
    bool nonCustomMember = false;
    value = (object) null;
    switch (instance)
    {
      case PythonType pythonType:
        PythonTypeSlot slot1;
        if (pythonType.TryLookupSlot(context, name, out slot1))
        {
          nonCustomMember = slot1.TryGetValue(context, (object) null, this, out value);
          break;
        }
        break;
      case IPythonObject pythonObject:
        PythonDictionary dict = pythonObject.Dict;
        nonCustomMember = dict != null && dict.TryGetValue((object) name, out value);
        break;
    }
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonTypeSlot slot2;
      if (this._resolutionOrder[index].TryLookupSlot(context, name, out slot2) && (!nonCustomMember || slot2.IsSetDescriptor(context, this)))
      {
        object obj;
        if (slot2.TryGetValue(context, instance, this, out obj))
          value = obj;
        return true;
      }
    }
    return nonCustomMember;
  }

  internal bool TryGetBoundMember(
    CodeContext context,
    object instance,
    string name,
    out object value)
  {
    object getattr;
    if (!this.TryResolveNonObjectSlot(context, instance, "__getattribute__", out getattr))
      return this.TryGetNonCustomBoundMember(context, instance, name, out value);
    value = this.InvokeGetAttributeMethod(context, name, getattr);
    return true;
  }

  private object InvokeGetAttributeMethod(CodeContext context, string name, object getattr)
  {
    CallSite<Func<CallSite, CodeContext, object, string, object>> callSite = !this.IsSystemType ? this._siteCache.GetGetAttributeSite(context) : context.LanguageContext.GetSiteCacheForSystemType(this.UnderlyingSystemType).GetGetAttributeSite(context);
    return callSite.Target((CallSite) callSite, context, getattr, name);
  }

  internal bool TryGetNonCustomBoundMember(
    CodeContext context,
    object instance,
    string name,
    out object value)
  {
    if (instance is IPythonObject pythonObject)
    {
      PythonDictionary dict = pythonObject.Dict;
      if (dict != null && dict.TryGetValue((object) name, out value))
        return true;
    }
    if (this.TryResolveSlot(context, instance, name, out value))
      return true;
    try
    {
      object getattr;
      if (this.TryResolveNonObjectSlot(context, instance, "__getattr__", out getattr))
      {
        value = this.InvokeGetAttributeMethod(context, name, getattr);
        return true;
      }
    }
    catch (MissingMemberException ex)
    {
    }
    value = (object) null;
    return false;
  }

  private bool TryResolveSlot(CodeContext context, object instance, string name, out object value)
  {
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonTypeSlot slot;
      if (this._resolutionOrder[index].TryLookupSlot(context, name, out slot) && slot.TryGetValue(context, instance, this, out value))
        return true;
    }
    value = (object) null;
    return false;
  }

  private bool TryResolveNonObjectSlot(
    CodeContext context,
    object instance,
    string name,
    out object value)
  {
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonType pythonType = this._resolutionOrder[index];
      if (pythonType != TypeCache.Object)
      {
        PythonTypeSlot slot;
        if (pythonType.TryLookupSlot(context, name, out slot) && slot.TryGetValue(context, instance, this, out value))
          return true;
      }
      else
        break;
    }
    value = (object) null;
    return false;
  }

  internal bool TrySetMember(CodeContext context, object instance, string name, object value)
  {
    object obj1;
    if (!this.TryResolveNonObjectSlot(context, instance, "__setattr__", out obj1))
      return this.TrySetNonCustomMember(context, instance, name, value);
    CallSite<Func<CallSite, CodeContext, object, object, string, object, object>> callSite = !this.IsSystemType ? this._siteCache.GetSetAttrSite(context) : context.LanguageContext.GetSiteCacheForSystemType(this.UnderlyingSystemType).GetSetAttrSite(context);
    object obj2 = callSite.Target((CallSite) callSite, context, obj1, instance, name, value);
    return true;
  }

  internal bool TrySetNonCustomMember(
    CodeContext context,
    object instance,
    string name,
    object value)
  {
    PythonTypeSlot slot;
    if (this.TryResolveSlot(context, name, out slot) && slot.TrySetValue(context, instance, this, value))
      return true;
    if (!(instance is IPythonObject pythonObject))
      return false;
    PythonDictionary pythonDictionary = pythonObject.Dict;
    if (pythonDictionary == null && pythonObject.PythonType.HasDictionary)
    {
      PythonDictionary dict = this.MakeDictionary();
      if ((pythonDictionary = pythonObject.SetDict(dict)) == null)
        return false;
    }
    pythonDictionary[(object) name] = value;
    return true;
  }

  internal bool TryDeleteMember(CodeContext context, object instance, string name)
  {
    try
    {
      object getattr;
      if (this.TryResolveNonObjectSlot(context, instance, "__delattr__", out getattr))
      {
        this.InvokeGetAttributeMethod(context, name, getattr);
        return true;
      }
    }
    catch (MissingMemberException ex)
    {
    }
    return this.TryDeleteNonCustomMember(context, instance, name);
  }

  internal bool TryDeleteNonCustomMember(CodeContext context, object instance, string name)
  {
    PythonTypeSlot slot;
    if (this.TryResolveSlot(context, name, out slot) && slot.TryDeleteValue(context, instance, this))
      return true;
    if (!(instance is IPythonObject pythonObject))
      return false;
    PythonDictionary pythonDictionary = pythonObject.Dict;
    if (pythonDictionary == null && pythonObject.PythonType.HasDictionary)
    {
      PythonDictionary dict = this.MakeDictionary();
      if ((pythonDictionary = pythonObject.SetDict(dict)) == null)
        return false;
    }
    return pythonDictionary.Remove((object) name);
  }

  internal List GetMemberNames(CodeContext context) => this.GetMemberNames(context, (object) null);

  internal List GetMemberNames(CodeContext context, object self)
  {
    List customDir = this.TryGetCustomDir(context, self);
    if (customDir != null)
      return customDir;
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    List res = new List();
    for (int index = 0; index < this._resolutionOrder.Count; ++index)
    {
      PythonType pythonType = this._resolutionOrder[index];
      if (pythonType.IsSystemType)
        PythonBinder.GetBinder(context).ResolveMemberNames(context, pythonType, this, dictionary);
      else
        PythonType.AddUserTypeMembers(context, dictionary, pythonType, res);
    }
    ExtensionMethodSet extensionMethods = context.ModuleContext.ExtensionMethods;
    if (extensionMethods != ExtensionMethodSet.Empty)
    {
      foreach (MethodInfo extensionMethod in extensionMethods.GetExtensionMethods(this))
        dictionary[extensionMethod.Name] = extensionMethod.Name;
    }
    return PythonType.AddInstanceMembers(self, dictionary, res);
  }

  private List TryGetCustomDir(CodeContext context, object self)
  {
    object obj;
    if (self == null || !this.TryResolveNonObjectSlot(context, self, "__dir__", out obj))
      return (List) null;
    CallSite<Func<CallSite, CodeContext, object, object>> callSite = !this.IsSystemType ? this._siteCache.GetDirSite(context) : context.LanguageContext.GetSiteCacheForSystemType(this.UnderlyingSystemType).GetDirSite(context);
    return new List(callSite.Target((CallSite) callSite, context, obj));
  }

  private static void AddUserTypeMembers(
    CodeContext context,
    Dictionary<string, string> keys,
    PythonType dt,
    List res)
  {
    if (dt.OldClass != null)
    {
      foreach (KeyValuePair<object, object> keyValuePair in dt.OldClass._dict)
        PythonType.AddOneMember(keys, res, keyValuePair.Key);
    }
    else
    {
      foreach (KeyValuePair<string, PythonTypeSlot> keyValuePair in dt._dict)
      {
        if (!keys.ContainsKey(keyValuePair.Key))
          keys[keyValuePair.Key] = keyValuePair.Key;
      }
    }
  }

  private static void AddOneMember(Dictionary<string, string> keys, List res, object name)
  {
    if (name is string key)
      keys[key] = key;
    else
      res.Add(name);
  }

  private static List AddInstanceMembers(object self, Dictionary<string, string> keys, List res)
  {
    if (self is IPythonObject pythonObject)
    {
      PythonDictionary dict = pythonObject.Dict;
      if (dict != null)
      {
        lock (dict)
        {
          foreach (object key in (IEnumerable<object>) dict.Keys)
            PythonType.AddOneMember(keys, res, key);
        }
      }
    }
    List<string> seq = new List<string>((IEnumerable<string>) keys.Keys);
    seq.Sort();
    res.extend((object) seq);
    return res;
  }

  internal PythonDictionary GetMemberDictionary(CodeContext context)
  {
    return this.GetMemberDictionary(context, true);
  }

  internal PythonDictionary GetMemberDictionary(CodeContext context, bool excludeDict)
  {
    PythonDictionary memberNames = PythonDictionary.MakeSymbolDictionary();
    if (this.IsSystemType)
    {
      PythonBinder.GetBinder(context).LookupMembers(context, this, memberNames);
    }
    else
    {
      foreach (KeyValuePair<string, PythonTypeSlot> keyValuePair in this._dict)
      {
        if (!excludeDict || !(keyValuePair.Key == "__dict__"))
        {
          object obj = !(keyValuePair.Value is PythonTypeUserDescriptorSlot userDescriptorSlot) ? (object) keyValuePair.Value : userDescriptorSlot.Value;
          memberNames[(object) keyValuePair.Key] = obj;
        }
      }
    }
    return memberNames;
  }

  private void InitializeUserType(
    CodeContext context,
    string name,
    PythonTuple bases,
    PythonDictionary vars,
    string selfNames)
  {
    if (vars.ContainsKey((object) "__mro__"))
      throw new NotImplementedException("Overriding __mro__ of built-in types is not implemented");
    if (vars.ContainsKey((object) "mro"))
    {
      foreach (object obj in bases)
      {
        if (obj is PythonType pythonType && pythonType.IsSubclassOf(TypeCache.PythonType))
          throw new NotImplementedException("Overriding type.mro is not implemented");
      }
    }
    bases = PythonType.ValidateBases(bases);
    this._name = name;
    this._bases = PythonType.GetBasesAsList(bases).ToArray();
    this._pythonContext = context.LanguageContext;
    this._resolutionOrder = PythonType.CalculateMro(this, (IList<PythonType>) this._bases);
    bool flag1 = false;
    foreach (PythonType pythonType in this._bases)
    {
      if (pythonType.GetUsedSlotCount() != 0)
        flag1 = !flag1 ? true : throw PythonOps.TypeError("multiple bases have instance lay-out conflict");
      pythonType.AddSubType(this);
    }
    HashSet<string> collection = (HashSet<string>) null;
    foreach (PythonType pythonType in this._resolutionOrder)
    {
      this._originalSlotCount += pythonType.GetUsedSlotCount();
      if (pythonType._optimizedInstanceNames != null)
      {
        if (collection == null)
          collection = new HashSet<string>();
        collection.UnionWith((IEnumerable<string>) pythonType._optimizedInstanceNames);
      }
    }
    if (!string.IsNullOrEmpty(selfNames))
    {
      if (collection == null)
        collection = new HashSet<string>();
      collection.UnionWith((IEnumerable<string>) selfNames.Split(','));
    }
    if (collection != null)
    {
      this._optimizedInstanceVersion = CustomInstanceDictionaryStorage.AllocateVersion();
      this._optimizedInstanceNames = new List<string>((IEnumerable<string>) collection).ToArray();
    }
    this.EnsureDict();
    this.PopulateDictionary(context, name, bases, vars);
    this._underlyingSystemType = NewTypeMaker.GetNewType(name, bases);
    this._underlyingSystemType = this.__clrtype__();
    if (this._underlyingSystemType == (Type) null)
      throw PythonOps.ValueError("__clrtype__ must return a type, not None");
    lock (PythonType._userTypeCtors)
    {
      if (!PythonType._userTypeCtors.TryGetValue(this._underlyingSystemType, out this._ctor))
      {
        ConstructorInfo[] constructors = this._underlyingSystemType.GetConstructors();
        bool flag2 = false;
        foreach (MethodBase methodBase in constructors)
        {
          ParameterInfo[] parameters = methodBase.GetParameters();
          if (parameters.Length > 1 && parameters[0].ParameterType == typeof (CodeContext) && parameters[1].ParameterType == typeof (PythonType) || parameters.Length != 0 && parameters[0].ParameterType == typeof (PythonType))
          {
            flag2 = true;
            break;
          }
        }
        this._ctor = BuiltinFunction.MakeFunction(this.Name, (MethodBase[]) constructors, this._underlyingSystemType);
        if (flag2)
        {
          PythonType._userTypeCtors[this._underlyingSystemType] = this._ctor;
        }
        else
        {
          this._instanceCtor = (InstanceCreator) new SystemInstanceCreator(this);
          this._attrs |= PythonType.PythonTypeAttributes.SystemCtor;
        }
      }
    }
    this.UpdateObjectNewAndInit(context);
  }

  internal PythonDictionary MakeDictionary()
  {
    return this._optimizedInstanceNames != null ? new PythonDictionary((DictionaryStorage) new CustomInstanceDictionaryStorage(this._optimizedInstanceNames, this._optimizedInstanceVersion)) : PythonDictionary.MakeSymbolDictionary();
  }

  internal IList<string> GetOptimizedInstanceNames()
  {
    return (IList<string>) this._optimizedInstanceNames;
  }

  internal int GetOptimizedInstanceVersion() => this._optimizedInstanceVersion;

  internal IList<string> GetTypeSlots()
  {
    PythonTypeSlot pythonTypeSlot;
    return this._dict != null && this._dict.TryGetValue("__slots__", out pythonTypeSlot) && pythonTypeSlot is PythonTypeUserDescriptorSlot ? (IList<string>) PythonType.SlotsToList(((PythonTypeUserDescriptorSlot) pythonTypeSlot).Value) : (IList<string>) ArrayUtils.EmptyStrings;
  }

  internal static List<string> GetSlots(PythonDictionary dict)
  {
    List<string> slots1 = (List<string>) null;
    object slots2;
    if (dict != null && dict.TryGetValue((object) "__slots__", out slots2))
      slots1 = PythonType.SlotsToList(slots2);
    return slots1;
  }

  internal static List<string> SlotsToList(object slots)
  {
    List<string> stringList = new List<string>();
    List<string> list;
    if (slots is IList<object> objectList)
    {
      list = new List<string>(objectList.Count);
      for (int index = 0; index < objectList.Count; ++index)
        list.Add(PythonType.GetSlotName(objectList[index]));
      list.Sort();
    }
    else
    {
      list = new List<string>(1);
      list.Add(PythonType.GetSlotName(slots));
    }
    return list;
  }

  internal bool HasObjectNew(CodeContext context)
  {
    if (!this._objectNew.HasValue)
      this.UpdateObjectNewAndInit(context);
    return this._objectNew.Value;
  }

  internal bool HasObjectInit(CodeContext context)
  {
    if (!this._objectInit.HasValue)
      this.UpdateObjectNewAndInit(context);
    return this._objectInit.Value;
  }

  private void UpdateObjectNewAndInit(CodeContext context)
  {
    foreach (PythonType pythonType in this._bases)
    {
      if (pythonType != TypeCache.Object)
      {
        if (!pythonType._objectNew.HasValue || !pythonType._objectInit.HasValue)
          pythonType.UpdateObjectNewAndInit(context);
        if (!pythonType._objectNew.Value)
          this._objectNew = new bool?(false);
        if (!pythonType._objectInit.Value)
          this._objectInit = new bool?(false);
      }
    }
    PythonTypeSlot slot;
    object obj;
    if (!this._objectInit.HasValue)
      this._objectInit = new bool?(this.TryResolveSlot(context, "__init__", out slot) && slot.TryGetValue(context, (object) null, this, out obj) && obj == InstanceOps.Init);
    if (this._objectNew.HasValue)
      return;
    this._objectNew = new bool?(this.TryResolveSlot(context, "__new__", out slot) && slot.TryGetValue(context, (object) null, this, out obj) && obj == InstanceOps.New);
  }

  private static string GetSlotName(object o)
  {
    string result;
    if (!Converter.TryConvertToString(o, out result) || string.IsNullOrEmpty(result))
      throw PythonOps.TypeError("slots must be one string or a list of strings");
    for (int index = 0; index < result.Length; ++index)
    {
      if ((result[index] < 'a' || result[index] > 'z') && (result[index] < 'A' || result[index] > 'Z') && (index == 0 || result[index] < '0' || result[index] > '9') && result[index] != '_')
        throw PythonOps.TypeError("__slots__ must be valid identifiers");
    }
    return result;
  }

  private int GetUsedSlotCount()
  {
    int usedSlotCount = 0;
    if (this._slots != null)
    {
      usedSlotCount = this._slots.Length;
      if (Array.IndexOf<string>(this._slots, "__weakref__") != -1)
        --usedSlotCount;
      if (Array.IndexOf<string>(this._slots, "__dict__") != -1)
        --usedSlotCount;
    }
    return usedSlotCount;
  }

  private void PopulateDictionary(
    CodeContext context,
    string name,
    PythonTuple bases,
    PythonDictionary vars)
  {
    this.PopulateSlot("__doc__", (object) null);
    List<string> slots = PythonType.GetSlots(vars);
    if (slots != null)
    {
      this._slots = slots.ToArray();
      int originalSlotCount = this._originalSlotCount;
      string privatePrefix = Parser.GetPrivatePrefix(name);
      for (int index = 0; index < slots.Count; ++index)
      {
        string name1 = slots[index];
        if (name1.StartsWith("__") && !name1.EndsWith("__"))
          name1 = $"_{privatePrefix}{name1}";
        this.AddSlot(name1, (PythonTypeSlot) new ReflectedSlotProperty(name1, name, index + originalSlotCount));
      }
      this._originalSlotCount += slots.Count;
    }
    if (this.CheckForSlotWithDefault(context, (IList<PythonType>) this._resolutionOrder, slots, "__weakref__"))
    {
      this._attrs |= PythonType.PythonTypeAttributes.WeakReferencable;
      this.AddSlot("__weakref__", (PythonTypeSlot) new PythonTypeWeakRefSlot(this));
    }
    if (this.CheckForSlotWithDefault(context, (IList<PythonType>) this._resolutionOrder, slots, "__dict__"))
    {
      this._attrs |= PythonType.PythonTypeAttributes.HasDictionary;
      bool flag = false;
      for (int index = 1; index < this._resolutionOrder.Count; ++index)
      {
        if (this._resolutionOrder[index].TryResolveSlot(context, "__dict__", out PythonTypeSlot _))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.AddSlot("__dict__", (PythonTypeSlot) new PythonTypeDictSlot(this));
    }
    object obj;
    if (context.TryGetVariable("__name__", out obj))
      this.PopulateSlot("__module__", obj);
    foreach (KeyValuePair<object, object> var in vars)
    {
      if (var.Key is string)
        this.PopulateSlot((string) var.Key, var.Value);
    }
    PythonTypeSlot func;
    if (!this._dict.TryGetValue("__new__", out func) || !(func is PythonFunction))
      return;
    this.AddSlot("__new__", (PythonTypeSlot) new staticmethod((object) func));
  }

  private bool CheckForSlotWithDefault(
    CodeContext context,
    IList<PythonType> resolutionOrder,
    List<string> slots,
    string name)
  {
    if (slots == null)
      return true;
    if (slots.Contains(name))
    {
      if (resolutionOrder.Count > 1)
      {
        PythonType pythonType = resolutionOrder[1];
        if (pythonType != null && pythonType.TryLookupSlot(context, name, out PythonTypeSlot _))
          throw PythonOps.TypeError(name + " slot disallowed: we already got one");
      }
      return true;
    }
    foreach (PythonType pythonType in resolutionOrder.Skip<PythonType>(1))
    {
      if (pythonType.TryLookupSlot(context, name, out PythonTypeSlot _))
        return true;
    }
    return false;
  }

  [PythonHidden(new PlatformID[] {})]
  public virtual Type __clrtype__() => this._underlyingSystemType;

  private void PopulateSlot(string key, object value)
  {
    this.AddSlot(key, PythonType.ToTypeSlot(value));
  }

  private static List<PythonType> GetBasesAsList(PythonTuple bases)
  {
    List<PythonType> basesAsList = new List<PythonType>();
    foreach (object obj in bases)
    {
      if (!(obj is PythonType pythonType))
        pythonType = ((OldClass) obj).TypeObject;
      basesAsList.Add(pythonType);
    }
    return basesAsList;
  }

  private static PythonTuple ValidateBases(PythonTuple bases)
  {
    PythonTuple pythonTuple = PythonTypeOps.EnsureBaseType(bases);
    for (int index1 = 0; index1 < pythonTuple.__len__(); ++index1)
    {
      for (int index2 = 0; index2 < pythonTuple.__len__(); ++index2)
      {
        if (index1 != index2 && pythonTuple[index1] == pythonTuple[index2])
        {
          if (pythonTuple[index1] is OldClass oldClass)
            throw PythonOps.TypeError("duplicate base class {0}", (object) oldClass.Name);
          throw PythonOps.TypeError("duplicate base class {0}", (object) ((PythonType) pythonTuple[index1]).Name);
        }
      }
    }
    return pythonTuple;
  }

  private static void EnsureModule(CodeContext context, PythonDictionary dict)
  {
    object obj;
    if (dict.ContainsKey((object) "__module__") || !context.TryGetVariable("__name__", out obj))
      return;
    dict[(object) "__module__"] = obj;
  }

  private void InitializeSystemType()
  {
    this.IsSystemType = true;
    this.IsPythonType = PythonBinder.IsPythonType(this._underlyingSystemType);
    this._name = NameConverter.GetTypeName(this._underlyingSystemType);
    this.AddSystemBases();
  }

  private void AddSystemBases()
  {
    List<PythonType> mro = new List<PythonType>();
    mro.Add(this);
    if (this._underlyingSystemType.GetBaseType() != (Type) null)
    {
      Type type1 = !(this._underlyingSystemType == typeof (bool)) ? (!(this._underlyingSystemType.GetBaseType() == typeof (ValueType)) ? this._underlyingSystemType.GetBaseType() : typeof (object)) : typeof (int);
      while (type1.IsDefined(typeof (PythonHiddenBaseClassAttribute), false))
        type1 = type1.GetBaseType();
      this._bases = new PythonType[1]
      {
        PythonType.GetPythonType(type1)
      };
      for (Type type2 = type1; type2 != (Type) null; type2 = type2.GetBaseType())
      {
        Type newType;
        if (PythonType.TryReplaceExtensibleWithBase(type2, out newType))
          mro.Add(DynamicHelpers.GetPythonTypeFromType(newType));
        else if (!type2.IsDefined(typeof (PythonHiddenBaseClassAttribute), false))
          mro.Add(DynamicHelpers.GetPythonTypeFromType(type2));
      }
      if (!this.IsPythonType)
        this.AddSystemInterfaces(mro);
    }
    else if (this._underlyingSystemType.IsInterface())
    {
      Type[] interfaces = this._underlyingSystemType.GetInterfaces();
      PythonType[] pythonTypeArray = new PythonType[interfaces.Length];
      for (int index = 0; index < interfaces.Length; ++index)
      {
        PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(interfaces[index]);
        mro.Add(pythonTypeFromType);
        pythonTypeArray[index] = pythonTypeFromType;
      }
      this._bases = pythonTypeArray;
    }
    else
      this._bases = new PythonType[0];
    this._resolutionOrder = mro;
  }

  private void AddSystemInterfaces(List<PythonType> mro)
  {
    if (this._underlyingSystemType.IsArray)
    {
      mro.Add(DynamicHelpers.GetPythonTypeFromType(typeof (IList)));
      mro.Add(DynamicHelpers.GetPythonTypeFromType(typeof (ICollection)));
      mro.Add(DynamicHelpers.GetPythonTypeFromType(typeof (IEnumerable)));
    }
    else
    {
      Type[] interfaces = this._underlyingSystemType.GetInterfaces();
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
      bool flag = false;
      List<Type> typeList = new List<Type>((IEnumerable<Type>) interfaces);
      PythonType[] destinationArray = new PythonType[this._bases.Length + interfaces.Length];
      Array.Copy((Array) this._bases, (Array) destinationArray, this._bases.Length);
      for (int index = 0; index < interfaces.Length; ++index)
        destinationArray[index + this._bases.Length] = PythonType.GetPythonType(interfaces[index]);
      lock (this._bases)
        this._bases = destinationArray;
      foreach (Type interfaceType in interfaces)
      {
        InterfaceMapping interfaceMap = this._underlyingSystemType.GetInterfaceMap(interfaceType);
        for (int index = 0; index < interfaceMap.TargetMethods.Length; ++index)
        {
          MethodInfo targetMethod = interfaceMap.TargetMethods[index];
          if (!(targetMethod == (MethodInfo) null))
          {
            if (!targetMethod.IsPrivate)
              dictionary[targetMethod.Name] = (Type) null;
            else
              flag = true;
          }
        }
        if (flag)
        {
          for (int index = 0; index < interfaceMap.TargetMethods.Length; ++index)
          {
            MethodInfo targetMethod = interfaceMap.TargetMethods[index];
            MethodInfo interfaceMethod = interfaceMap.InterfaceMethods[index];
            if (targetMethod != (MethodInfo) null && targetMethod.IsPrivate)
            {
              flag = true;
              Type type;
              if (dictionary.TryGetValue(interfaceMethod.Name, out type))
              {
                if (type != (Type) null)
                {
                  typeList.Remove(interfaceType);
                  typeList.Remove(dictionary[interfaceMethod.Name]);
                  break;
                }
              }
              else
                dictionary[interfaceMethod.Name] = interfaceType;
            }
          }
        }
      }
      if (!flag)
        return;
      foreach (Type type in typeList)
        mro.Add(DynamicHelpers.GetPythonTypeFromType(type));
    }
  }

  private void AddSystemConstructors()
  {
    if (typeof (Delegate).IsAssignableFrom(this._underlyingSystemType))
    {
      this.SetConstructor(BuiltinFunction.MakeFunction(this._underlyingSystemType.Name, (MethodBase[]) new MethodInfo[1]
      {
        typeof (DelegateOps).GetMethod("__new__")
      }, this._underlyingSystemType));
    }
    else
    {
      if (this._underlyingSystemType.IsAbstract())
        return;
      BuiltinFunction constructors = this.GetConstructors();
      if (constructors == null)
        return;
      this.SetConstructor(constructors);
    }
  }

  private BuiltinFunction GetConstructors()
  {
    return PythonTypeOps.GetConstructorFunction(this._underlyingSystemType, this.Name);
  }

  private void EnsureConstructor()
  {
    if (this._ctor != null)
      return;
    this.AddSystemConstructors();
    if (this._ctor == null)
      throw PythonOps.TypeError(this._underlyingSystemType.FullName + " does not define any public constructors.");
  }

  private void EnsureInstanceCtor()
  {
    if (this._instanceCtor != null)
      return;
    this._instanceCtor = InstanceCreator.Make(this);
  }

  private void UpdateVersion()
  {
    foreach (WeakReference subType in (IEnumerable<WeakReference>) this.SubTypes)
    {
      if (subType.IsAlive)
        ((PythonType) subType.Target).UpdateVersion();
    }
    this._lenSlot = (PythonTypeSlot) null;
    this._version = PythonType.GetNextVersion();
  }

  private static int GetNextVersion()
  {
    return PythonType.MasterVersion >= 0 ? Interlocked.Increment(ref PythonType.MasterVersion) : throw new InvalidOperationException(Resources.TooManyVersions);
  }

  private void EnsureDict()
  {
    if (this._dict != null)
      return;
    Interlocked.CompareExchange<Dictionary<string, PythonTypeSlot>>(ref this._dict, new Dictionary<string, PythonTypeSlot>((IEqualityComparer<string>) StringComparer.Ordinal), (Dictionary<string, PythonTypeSlot>) null);
  }

  private void AddSubType(PythonType subtype)
  {
    if (this._subtypes == null)
      Interlocked.CompareExchange<List<WeakReference>>(ref this._subtypes, new List<WeakReference>(), (List<WeakReference>) null);
    lock (PythonType._subtypesLock)
      this._subtypes.Add(new WeakReference((object) subtype));
  }

  private void RemoveSubType(PythonType subtype)
  {
    int index = 0;
    if (this._subtypes == null)
      return;
    lock (PythonType._subtypesLock)
    {
      while (index < this._subtypes.Count)
      {
        if (!this._subtypes[index].IsAlive || this._subtypes[index].Target == subtype)
          this._subtypes.RemoveAt(index);
        else
          ++index;
      }
    }
  }

  private IList<WeakReference> SubTypes
  {
    get
    {
      if (this._subtypes == null)
        return (IList<WeakReference>) PythonType._emptyWeakRef;
      lock (PythonType._subtypesLock)
        return (IList<WeakReference>) this._subtypes.ToArray();
    }
  }

  IList<string> IMembersList.GetMemberNames()
  {
    return PythonOps.GetStringMemberList((IPythonMembersList) this);
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    List memberNames = this.GetMemberNames(context);
    object[] array = new object[memberNames.Count];
    memberNames.CopyTo(array, 0);
    Array.Sort<object>(array);
    return (IList<object>) array;
  }

  WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakrefTracker;

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    return !this.IsSystemType && Interlocked.CompareExchange<WeakRefTracker>(ref this._weakrefTracker, value, (WeakRefTracker) null) == null;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
  {
    if (this.IsSystemType)
      return;
    this._weakrefTracker = value;
  }

  IWeakReferenceable IWeakReferenceableByProxy.GetWeakRefProxy(PythonContext context)
  {
    return (IWeakReferenceable) new PythonType.WeakReferenceProxy(context, this);
  }

  [PythonHidden(new PlatformID[] {})]
  public DynamicMetaObject GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new MetaPythonType(parameter, BindingRestrictions.Empty, this);
  }

  internal WeakReference GetSharedWeakReference()
  {
    if (this._weakRef == null)
      this._weakRef = new WeakReference((object) this);
    return this._weakRef;
  }

  T IFastSettable.MakeSetBinding<T>(CallSite<T> site, PythonSetMemberBinder binder)
  {
    if (!this.IsSystemType && !this.TryGetCustomSetAttr(this.Context.SharedContext, out PythonTypeSlot _))
    {
      CodeContext sharedContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) binder).SharedContext;
      string name = binder.Name;
      Type type = typeof (T);
      if (type == typeof (Func<CallSite, object, object, object>))
        return (T) PythonType.MakeFastSet<object>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, string, object>))
        return (T) PythonType.MakeFastSet<string>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, int, object>))
        return (T) PythonType.MakeFastSet<int>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, double, object>))
        return (T) PythonType.MakeFastSet<double>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, List, object>))
        return (T) PythonType.MakeFastSet<List>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, PythonTuple, object>))
        return (T) PythonType.MakeFastSet<PythonTuple>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, PythonDictionary, object>))
        return (T) PythonType.MakeFastSet<PythonDictionary>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, SetCollection, object>))
        return (T) PythonType.MakeFastSet<SetCollection>(sharedContext, name);
      if (type == typeof (Func<CallSite, object, FrozenSetCollection, object>))
        return (T) PythonType.MakeFastSet<FrozenSetCollection>(sharedContext, name);
    }
    return default (T);
  }

  private static Func<CallSite, object, T, object> MakeFastSet<T>(CodeContext context, string name)
  {
    return new Func<CallSite, object, T, object>(new PythonType.Setter<T>(context, name).Target);
  }

  FastBindResult<T> IFastInvokable.MakeInvokeBinding<T>(
    CallSite<T> site,
    PythonInvokeBinder binder,
    CodeContext context,
    object[] args)
  {
    ParameterInfo[] parameters = typeof (T).GetMethod("Invoke").GetParameters();
    if (parameters[2].ParameterType != typeof (object))
      return new FastBindResult<T>();
    if (binder.Signature.IsSimple)
    {
      if (this == TypeCache.PythonType && args.Length == 1 && parameters[3].ParameterType == typeof (object))
        return new FastBindResult<T>((T) new Func<CallSite, CodeContext, object, object, object>(this.GetPythonType), true);
      if (this == TypeCache.Set && args.Length == 0)
        return new FastBindResult<T>((T) new Func<CallSite, CodeContext, object, object>(this.EmptySet), true);
      if (this == TypeCache.Object && args.Length == 0)
        return new FastBindResult<T>((T) new Func<CallSite, CodeContext, object, object>(this.NewObject), true);
    }
    if (this.IsSystemType || this.IsMixedNewStyleOldStyle() || args.Length > 5 || this.HasSystemCtor || this.GetType() != typeof (PythonType) || this.HasAbstractMethods(context))
      return new FastBindResult<T>();
    Type[] genTypeArgs = new Type[parameters.Length - 3];
    for (int index = 0; index < parameters.Length - 3; ++index)
      genTypeArgs[index] = parameters[index + 3].ParameterType;
    PythonType.FastBindingBuilderBase bindingBuilderBase;
    if (genTypeArgs.Length == 0)
    {
      bindingBuilderBase = (PythonType.FastBindingBuilderBase) new PythonType.FastBindingBuilder(context, this, binder, typeof (T), genTypeArgs);
    }
    else
    {
      Type type;
      switch (genTypeArgs.Length)
      {
        case 1:
          type = typeof (PythonType.FastBindingBuilder<>);
          break;
        case 2:
          type = typeof (PythonType.FastBindingBuilder<,>);
          break;
        case 3:
          type = typeof (PythonType.FastBindingBuilder<,,>);
          break;
        case 4:
          type = typeof (PythonType.FastBindingBuilder<,,,>);
          break;
        case 5:
          type = typeof (PythonType.FastBindingBuilder<,,,,>);
          break;
        default:
          throw new NotImplementedException();
      }
      bindingBuilderBase = (PythonType.FastBindingBuilderBase) Activator.CreateInstance(type.MakeGenericType(genTypeArgs), (object) context, (object) this, (object) binder, (object) typeof (T), (object) genTypeArgs);
    }
    return new FastBindResult<T>((T) bindingBuilderBase.MakeBindingResult(), true);
  }

  private object GetPythonType(CallSite site, CodeContext context, object type, object instance)
  {
    return type == TypeCache.PythonType ? (object) DynamicHelpers.GetPythonType(instance) : ((CallSite<Func<CallSite, CodeContext, object, object, object>>) site).Update(site, context, type, instance);
  }

  private object EmptySet(CallSite site, CodeContext context, object type)
  {
    return type == TypeCache.Set ? (object) new SetCollection() : ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, type);
  }

  private object NewObject(CallSite site, CodeContext context, object type)
  {
    return type == TypeCache.Object ? new object() : ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, type);
  }

  [Flags]
  private enum PythonTypeAttributes
  {
    None = 0,
    Immutable = 1,
    SystemType = 2,
    IsPythonType = 4,
    WeakReferencable = 8,
    HasDictionary = 16, // 0x00000010
    SystemCtor = 32, // 0x00000020
  }

  private class WeakReferenceProxy : IWeakReferenceable
  {
    private readonly PythonContext context;
    private readonly PythonType type;

    internal WeakReferenceProxy(PythonContext context, PythonType type)
    {
      this.type = type;
      this.context = context;
    }

    public WeakRefTracker GetWeakRef()
    {
      return this.type.IsSystemType ? this.context.GetSystemPythonTypeWeakRef(this.type) : ((IWeakReferenceable) this.type).GetWeakRef();
    }

    public void SetFinalizer(WeakRefTracker value)
    {
      if (this.type.IsSystemType)
        this.context.SetSystemPythonTypeFinalizer(this.type, value);
      else
        ((IWeakReferenceable) this.type).SetFinalizer(value);
    }

    public bool SetWeakRef(WeakRefTracker value)
    {
      return this.type.IsSystemType ? this.context.SetSystemPythonTypeWeakRef(this.type, value) : ((IWeakReferenceable) this.type).SetWeakRef(value);
    }
  }

  private class Setter<T> : FastSetBase<T>
  {
    private readonly CodeContext _context;
    private readonly string _name;

    public Setter(CodeContext context, string name)
      : base(-1)
    {
      this._context = context;
      this._name = name;
    }

    public object Target(CallSite site, object self, T value)
    {
      if (!(self is PythonType pythonType) || pythonType.IsSystemType)
        return FastSetBase<T>.Update(site, self, value);
      pythonType.SetCustomMember(this._context, this._name, (object) value);
      return (object) value;
    }
  }

  internal class DebugProxy
  {
    private readonly PythonType _type;

    public DebugProxy(PythonType type) => this._type = type;

    public PythonType[] __bases__
    {
      get => ArrayUtils.ToArray<PythonType>((ICollection<PythonType>) this._type.BaseTypes);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<ObjectDebugView> Members
    {
      get
      {
        List<ObjectDebugView> members = new List<ObjectDebugView>();
        if (this._type._dict != null)
        {
          foreach (KeyValuePair<string, PythonTypeSlot> keyValuePair in this._type._dict)
          {
            if (keyValuePair.Value is PythonTypeUserDescriptorSlot)
              members.Add(new ObjectDebugView((object) keyValuePair.Key, ((PythonTypeUserDescriptorSlot) keyValuePair.Value).Value));
            else
              members.Add(new ObjectDebugView((object) keyValuePair.Key, (object) keyValuePair.Value));
          }
        }
        return members;
      }
    }
  }

  private abstract class FastBindingBuilderBase
  {
    private readonly CodeContext _context;
    private readonly PythonInvokeBinder _binder;
    private readonly PythonType _type;
    private readonly Type _siteType;
    private readonly Type[] _genTypeArgs;

    public FastBindingBuilderBase(
      CodeContext context,
      PythonType type,
      PythonInvokeBinder binder,
      Type siteType,
      Type[] genTypeArgs)
    {
      this._context = context;
      this._type = type;
      this._binder = binder;
      this._siteType = siteType;
      this._genTypeArgs = genTypeArgs;
    }

    public virtual Delegate MakeBindingResult()
    {
      int version = this._type.Version;
      PythonTypeSlot slot1;
      this._type.TryResolveSlot(this._context, "__new__", out slot1);
      PythonTypeSlot slot2;
      this._type.TryResolveSlot(this._context, "__init__", out slot2);
      Delegate newDlg;
      if (slot1 == InstanceOps.New)
      {
        if (this._genTypeArgs.Length != 0 && slot2 == InstanceOps.Init)
          return (Delegate) null;
        newDlg = this.GetOrCreateFastNew();
      }
      else
        newDlg = !(slot1.GetType() == typeof (staticmethod)) || !(((staticmethod) slot1)._func is PythonFunction) ? (Delegate) null : this.GetNewSiteDelegate(this._binder.Context.Invoke(this._binder.Signature.InsertArgument(Argument.Simple)), ((staticmethod) slot1)._func);
      return (object) newDlg == null ? (Delegate) null : this.MakeDelegate(version, newDlg, this._type.GetLateBoundInitBinder(this._binder.Signature));
    }

    private Delegate GetOrCreateFastNew()
    {
      Delegate fastNew;
      lock (PythonType._fastBindCtors)
      {
        Dictionary<Type, Delegate> dictionary;
        if (!PythonType._fastBindCtors.TryGetValue(this._type.UnderlyingSystemType, out dictionary))
          dictionary = PythonType._fastBindCtors[this._type.UnderlyingSystemType] = new Dictionary<Type, Delegate>();
        else if (dictionary.TryGetValue(this._siteType, out fastNew))
          return fastNew;
        ConstructorInfo[] constructors = this._type.UnderlyingSystemType.GetConstructors();
        if (constructors.Length != 1 || constructors[0].GetParameters().Length != 1 || !(constructors[0].GetParameters()[0].ParameterType == typeof (PythonType)))
          return (Delegate) null;
        ParameterExpression parameterExpression1 = Expression.Parameter(typeof (CodeContext));
        ParameterExpression parameterExpression2 = Expression.Parameter(typeof (object));
        ParameterExpression[] parameterExpressionArray = new ParameterExpression[this._genTypeArgs.Length + 2];
        parameterExpressionArray[0] = parameterExpression1;
        parameterExpressionArray[1] = parameterExpression2;
        for (int index = 0; index < this._genTypeArgs.Length; ++index)
          parameterExpressionArray[index + 2] = Expression.Parameter(this._genTypeArgs[index]);
        fastNew = Expression.Lambda((Expression) Expression.Convert((Expression) Expression.New(constructors[0], (Expression) Expression.Convert((Expression) parameterExpression2, typeof (PythonType))), typeof (object)), parameterExpressionArray).Compile();
        dictionary[this._siteType] = fastNew;
      }
      return fastNew;
    }

    protected abstract Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func);

    protected abstract Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder);
  }

  private class FastBindingBuilder(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, object>(new PythonType.NewSite(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object>(new PythonType.FastTypeSite(version, (Func<CodeContext, object, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(CallSite site, CodeContext context, object type)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, type);
      object obj1 = this._new(context, type);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1);
      return obj1;
    }
  }

  private class NewSite
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(CodeContext context, object type)
    {
      return this._site.Target((CallSite) this._site, context, this._target, type);
    }
  }

  private class FastBindingBuilder<T0>(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, T0, object>(new PythonType.NewSite<T0>(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, object>(new PythonType.FastTypeSite<T0>(version, (Func<CodeContext, object, T0, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite<T0>
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, T0, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, T0, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, T0, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, T0, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(CallSite site, CodeContext context, object type, T0 arg0)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, object>>) site).Update(site, context, type, arg0);
      object obj1 = this._new(context, type, arg0);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1, arg0);
      return obj1;
    }
  }

  private class NewSite<T0>
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, T0, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(CodeContext context, object typeOrInstance, T0 arg0)
    {
      return this._site.Target((CallSite) this._site, context, this._target, typeOrInstance, arg0);
    }
  }

  private class FastBindingBuilder<T0, T1>(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, T0, T1, object>(new PythonType.NewSite<T0, T1>(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, object>(new PythonType.FastTypeSite<T0, T1>(version, (Func<CodeContext, object, T0, T1, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite<T0, T1>
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, T0, T1, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, T0, T1, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, T0, T1, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, T0, T1, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(CallSite site, CodeContext context, object type, T0 arg0, T1 arg1)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, object>>) site).Update(site, context, type, arg0, arg1);
      object obj1 = this._new(context, type, arg0, arg1);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1, arg0, arg1);
      return obj1;
    }
  }

  private class NewSite<T0, T1>
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, T0, T1, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(CodeContext context, object typeOrInstance, T0 arg0, T1 arg1)
    {
      return this._site.Target((CallSite) this._site, context, this._target, typeOrInstance, arg0, arg1);
    }
  }

  private class FastBindingBuilder<T0, T1, T2>(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, T0, T1, T2, object>(new PythonType.NewSite<T0, T1, T2>(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, object>(new PythonType.FastTypeSite<T0, T1, T2>(version, (Func<CodeContext, object, T0, T1, T2, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite<T0, T1, T2>
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, T0, T1, T2, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, T0, T1, T2, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object type,
      T0 arg0,
      T1 arg1,
      T2 arg2)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>>) site).Update(site, context, type, arg0, arg1, arg2);
      object obj1 = this._new(context, type, arg0, arg1, arg2);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1, arg0, arg1, arg2);
      return obj1;
    }
  }

  private class NewSite<T0, T1, T2>
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(CodeContext context, object typeOrInstance, T0 arg0, T1 arg1, T2 arg2)
    {
      return this._site.Target((CallSite) this._site, context, this._target, typeOrInstance, arg0, arg1, arg2);
    }
  }

  private class FastBindingBuilder<T0, T1, T2, T3>(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, T0, T1, T2, T3, object>(new PythonType.NewSite<T0, T1, T2, T3>(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>(new PythonType.FastTypeSite<T0, T1, T2, T3>(version, (Func<CodeContext, object, T0, T1, T2, T3, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite<T0, T1, T2, T3>
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, T0, T1, T2, T3, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, T0, T1, T2, T3, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object type,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, type, arg0, arg1, arg2, arg3);
      object obj1 = this._new(context, type, arg0, arg1, arg2, arg3);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1, arg0, arg1, arg2, arg3);
      return obj1;
    }
  }

  private class NewSite<T0, T1, T2, T3>
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(
      CodeContext context,
      object typeOrInstance,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3)
    {
      return this._site.Target((CallSite) this._site, context, this._target, typeOrInstance, arg0, arg1, arg2, arg3);
    }
  }

  private class FastBindingBuilder<T0, T1, T2, T3, T4>(
    CodeContext context,
    PythonType type,
    PythonInvokeBinder binder,
    Type siteType,
    Type[] genTypeArgs) : PythonType.FastBindingBuilderBase(context, type, binder, siteType, genTypeArgs)
  {
    protected override Delegate GetNewSiteDelegate(PythonInvokeBinder binder, object func)
    {
      return (Delegate) new Func<CodeContext, object, T0, T1, T2, T3, T4, object>(new PythonType.NewSite<T0, T1, T2, T3, T4>(binder, func).Call);
    }

    protected override Delegate MakeDelegate(
      int version,
      Delegate newDlg,
      LateBoundInitBinder initBinder)
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>(new PythonType.FastTypeSite<T0, T1, T2, T3, T4>(version, (Func<CodeContext, object, T0, T1, T2, T3, T4, object>) newDlg, initBinder).CallTarget);
    }
  }

  private class FastTypeSite<T0, T1, T2, T3, T4>
  {
    private readonly int _version;
    private readonly Func<CodeContext, object, T0, T1, T2, T3, T4, object> _new;
    private readonly CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>> _initSite;

    public FastTypeSite(
      int version,
      Func<CodeContext, object, T0, T1, T2, T3, T4, object> @new,
      LateBoundInitBinder initBinder)
    {
      this._version = version;
      this._new = @new;
      this._initSite = CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>.Create((CallSiteBinder) initBinder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object type,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4)
    {
      if (!(type is PythonType pythonType) || pythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, type, arg0, arg1, arg2, arg3, arg4);
      object obj1 = this._new(context, type, arg0, arg1, arg2, arg3, arg4);
      object obj2 = this._initSite.Target((CallSite) this._initSite, context, obj1, arg0, arg1, arg2, arg3, arg4);
      return obj1;
    }
  }

  private class NewSite<T0, T1, T2, T3, T4>
  {
    private readonly CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>> _site;
    private readonly object _target;

    public NewSite(PythonInvokeBinder binder, object target)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>>.Create((CallSiteBinder) binder);
      this._target = target;
    }

    public object Call(
      CodeContext context,
      object typeOrInstance,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4)
    {
      return this._site.Target((CallSite) this._site, context, this._target, typeOrInstance, arg0, arg1, arg2, arg3, arg4);
    }
  }
}
