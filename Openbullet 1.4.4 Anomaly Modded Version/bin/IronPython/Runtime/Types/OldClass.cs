// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.OldClass
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("classobj")]
[DontMapGetMemberNamesToDir]
[DebuggerTypeProxy(typeof (OldClass.OldClassDebugView))]
[DebuggerDisplay("old-style class {Name}")]
[Serializable]
public sealed class OldClass : 
  ICustomTypeDescriptor,
  ICodeFormattable,
  IMembersList,
  IDynamicMetaObjectProvider,
  IPythonMembersList,
  IWeakReferenceable
{
  [NonSerialized]
  private List<OldClass> _bases;
  private PythonType _type;
  internal PythonDictionary _dict;
  private int _attrs;
  internal object _name;
  private int _optimizedInstanceNamesVersion;
  private string[] _optimizedInstanceNames;
  private WeakRefTracker _tracker;
  public static string __doc__ = "classobj(name, bases, dict)";

  public static object __new__(
    CodeContext context,
    [NotNull] PythonType cls,
    string name,
    PythonTuple bases,
    PythonDictionary dict)
  {
    if (dict == null)
      throw PythonOps.TypeError("dict must be a dictionary");
    if (cls != TypeCache.OldClass)
      throw PythonOps.TypeError("{0} is not a subtype of classobj", (object) cls.Name);
    object res;
    if (!dict.ContainsKey((object) "__module__") && context.TryGetGlobalVariable("__name__", out res))
      dict[(object) "__module__"] = res;
    foreach (object obj in bases)
    {
      if (obj is PythonType)
        return PythonOps.MakeClass(context, name, bases._data, string.Empty, dict);
    }
    return (object) new OldClass(name, bases, dict, string.Empty);
  }

  internal OldClass(string name, PythonTuple bases, PythonDictionary dict, string instanceNames)
  {
    this._bases = this.ValidateBases((object) bases);
    this.Init(name, dict, instanceNames);
  }

  private void Init(string name, PythonDictionary dict, string instanceNames)
  {
    this._name = (object) name;
    this.InitializeInstanceNames(instanceNames);
    this._dict = dict;
    if (!this._dict._storage.Contains((object) "__doc__"))
      this._dict._storage.Add(ref this._dict._storage, (object) "__doc__", (object) null);
    this.CheckSpecialMethods(this._dict);
  }

  private void CheckSpecialMethods(PythonDictionary dict)
  {
    if (dict._storage.Contains((object) "__del__"))
      this.HasFinalizer = true;
    if (dict._storage.Contains((object) "__setattr__"))
      this.HasSetAttr = true;
    if (dict._storage.Contains((object) "__delattr__"))
      this.HasDelAttr = true;
    foreach (OldClass oldClass in this._bases)
    {
      if (oldClass.HasDelAttr)
        this.HasDelAttr = true;
      if (oldClass.HasSetAttr)
        this.HasSetAttr = true;
      if (oldClass.HasFinalizer)
        this.HasFinalizer = true;
    }
  }

  private OldClass(SerializationInfo info, StreamingContext context)
  {
    this._bases = (List<OldClass>) info.GetValue("__class__", typeof (List<OldClass>));
    this._name = info.GetValue("__name__", typeof (object));
    this._dict = new PythonDictionary();
    this.InitializeInstanceNames("");
    List<object> objectList1 = (List<object>) info.GetValue("keys", typeof (List<object>));
    List<object> objectList2 = (List<object>) info.GetValue("values", typeof (List<object>));
    for (int index = 0; index < objectList1.Count; ++index)
      this._dict[objectList1[index]] = objectList2[index];
    if (!this._dict.has_key((object) "__del__"))
      return;
    this.HasFinalizer = true;
  }

  private void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    ContractUtils.RequiresNotNull((object) info, nameof (info));
    info.AddValue("__bases__", (object) this._bases);
    info.AddValue("__name__", this._name);
    List<object> objectList1 = new List<object>();
    List<object> objectList2 = new List<object>();
    foreach (KeyValuePair<object, object> keyValuePair in this._dict._storage.GetItems())
    {
      objectList1.Add(keyValuePair.Key);
      objectList2.Add(keyValuePair.Value);
    }
    info.AddValue("keys", (object) objectList1);
    info.AddValue("values", (object) objectList2);
  }

  private void InitializeInstanceNames(string instanceNames)
  {
    if (instanceNames.Length == 0)
    {
      this._optimizedInstanceNames = ArrayUtils.EmptyStrings;
      this._optimizedInstanceNamesVersion = 0;
    }
    else
    {
      string[] strArray = instanceNames.Split(',');
      this._optimizedInstanceNames = new string[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        this._optimizedInstanceNames[index] = strArray[index];
      this._optimizedInstanceNamesVersion = CustomInstanceDictionaryStorage.AllocateVersion();
    }
  }

  internal string[] OptimizedInstanceNames => this._optimizedInstanceNames;

  internal int OptimizedInstanceNamesVersion => this._optimizedInstanceNamesVersion;

  internal string Name => this._name.ToString();

  internal bool TryLookupSlot(string name, out object ret)
  {
    if (this._dict._storage.TryGetValue((object) name, out ret))
      return true;
    foreach (OldClass oldClass in this._bases)
    {
      if (oldClass.TryLookupSlot(name, out ret))
        return true;
    }
    ret = (object) null;
    return false;
  }

  internal bool TryLookupOneSlot(PythonType lookingType, string name, out object ret)
  {
    if (!this._dict._storage.TryGetValue((object) name, out ret))
      return false;
    ret = OldClass.GetOldStyleDescriptor(this.TypeObject.Context.SharedContext, ret, (object) null, lookingType);
    return true;
  }

  internal string FullName => $"{this._dict[(object) "__module__"].ToString()}.{this._name}";

  internal List<OldClass> BaseClasses => this._bases;

  internal object GetOldStyleDescriptor(
    CodeContext context,
    object self,
    object instance,
    object type)
  {
    object obj;
    return self is PythonTypeSlot pythonTypeSlot && pythonTypeSlot.TryGetValue(context, instance, this.TypeObject, out obj) ? obj : PythonOps.GetUserDescriptor(self, instance, type);
  }

  internal static object GetOldStyleDescriptor(
    CodeContext context,
    object self,
    object instance,
    PythonType type)
  {
    object obj;
    return self is PythonTypeSlot pythonTypeSlot && pythonTypeSlot.TryGetValue(context, instance, type, out obj) ? obj : PythonOps.GetUserDescriptor(self, instance, (object) type);
  }

  internal bool HasFinalizer
  {
    get => (this._attrs & 1) != 0;
    set
    {
      int attrs;
      do
      {
        attrs = this._attrs;
      }
      while (Interlocked.CompareExchange(ref this._attrs, value ? attrs | 1 : attrs & -2, attrs) != attrs);
    }
  }

  internal bool HasSetAttr
  {
    get => (this._attrs & 2) != 0;
    set
    {
      int attrs;
      do
      {
        attrs = this._attrs;
      }
      while (Interlocked.CompareExchange(ref this._attrs, value ? attrs | 2 : attrs & -3, attrs) != attrs);
    }
  }

  internal bool HasDelAttr
  {
    get => (this._attrs & 4) != 0;
    set
    {
      int attrs;
      do
      {
        attrs = this._attrs;
      }
      while (Interlocked.CompareExchange(ref this._attrs, value ? attrs | 4 : attrs & -5, attrs) != attrs);
    }
  }

  public override string ToString() => this.FullName;

  [SpecialName]
  public object Call(CodeContext context, [NotNull] params object[] argsø)
  {
    OldInstance instance = new OldInstance(context, this);
    object ret;
    if (this.TryLookupSlot("__init__", out ret))
      PythonOps.CallWithContext(context, this.GetOldStyleDescriptor(context, ret, (object) instance, (object) this), argsø);
    else if (argsø.Length != 0)
      OldClass.MakeCallError();
    return (object) instance;
  }

  [SpecialName]
  public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dictø, [NotNull] params object[] argsø)
  {
    OldInstance o = new OldInstance(context, this);
    object ret;
    if (PythonOps.TryGetBoundAttr((object) o, "__init__", out ret))
    {
      PythonCalls.CallWithKeywordArgs(context, ret, argsø, dictø);
      return (object) o;
    }
    if (dictø.Count <= 0 && argsø.Length == 0)
      return (object) o;
    OldClass.MakeCallError();
    return (object) o;
  }

  internal PythonType TypeObject
  {
    get
    {
      if (this._type == null)
        Interlocked.CompareExchange<PythonType>(ref this._type, new PythonType(this), (PythonType) null);
      return this._type;
    }
  }

  private List<OldClass> ValidateBases(object value)
  {
    List<OldClass> oldClassList = value is PythonTuple pythonTuple ? new List<OldClass>(pythonTuple.__len__()) : throw PythonOps.TypeError("__bases__ must be a tuple object");
    foreach (object o in pythonTuple)
    {
      if (!(o is OldClass oldClass))
        throw PythonOps.TypeError("__bases__ items must be classes (got {0})", (object) PythonTypeOps.GetName(o));
      if (oldClass.IsSubclassOf((object) this))
        throw PythonOps.TypeError("a __bases__ item causes an inheritance cycle");
      oldClassList.Add(oldClass);
    }
    return oldClassList;
  }

  internal object GetMember(CodeContext context, string name)
  {
    object member;
    if (!this.TryGetBoundCustomMember(context, name, out member))
      throw PythonOps.AttributeError("type object '{0}' has no attribute '{1}'", (object) this.Name, (object) name);
    return member;
  }

  internal bool TryGetBoundCustomMember(CodeContext context, string name, out object value)
  {
    switch (name)
    {
      case "__bases__":
        value = (object) PythonTuple.Make((object) this._bases);
        return true;
      case "__name__":
        value = this._name;
        return true;
      case "__dict__":
        this.HasDelAttr = this.HasSetAttr = true;
        value = (object) this._dict;
        return true;
      default:
        if (!this.TryLookupSlot(name, out value))
          return false;
        value = this.GetOldStyleDescriptor(context, value, (object) null, (object) this);
        return true;
    }
  }

  internal bool DeleteCustomMember(CodeContext context, string name)
  {
    if (!this._dict._storage.Remove(ref this._dict._storage, (object) name))
      throw PythonOps.AttributeError("{0} is not a valid attribute", (object) name);
    if (name == "__del__")
      this.HasFinalizer = false;
    if (name == "__setattr__")
      this.HasSetAttr = false;
    if (name == "__delattr__")
      this.HasDelAttr = false;
    return true;
  }

  internal static void RecurseAttrHierarchy(OldClass oc, IDictionary<object, object> attrs)
  {
    foreach (KeyValuePair<object, object> keyValuePair in oc._dict._storage.GetItems())
    {
      if (!attrs.ContainsKey(keyValuePair.Key))
        attrs.Add(keyValuePair.Key, keyValuePair.Key);
    }
    if (oc._bases.Count == 0)
      return;
    foreach (OldClass oc1 in oc._bases)
      OldClass.RecurseAttrHierarchy(oc1, attrs);
  }

  IList<string> IMembersList.GetMemberNames()
  {
    return PythonOps.GetStringMemberList((IPythonMembersList) this);
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    PythonDictionary pythonDictionary = new PythonDictionary(this._dict);
    OldClass.RecurseAttrHierarchy(this, (IDictionary<object, object>) pythonDictionary);
    return (IList<object>) PythonOps.MakeListFromSequence((object) pythonDictionary);
  }

  internal bool IsSubclassOf(object other)
  {
    if (this == other)
      return true;
    if (!(other is OldClass))
      return false;
    foreach (OldClass oldClass in this._bases)
    {
      if (oldClass.IsSubclassOf(other))
        return true;
    }
    return false;
  }

  AttributeCollection ICustomTypeDescriptor.GetAttributes()
  {
    return CustomTypeDescHelpers.GetAttributes((object) this);
  }

  string ICustomTypeDescriptor.GetClassName() => CustomTypeDescHelpers.GetClassName((object) this);

  string ICustomTypeDescriptor.GetComponentName()
  {
    return CustomTypeDescHelpers.GetComponentName((object) this);
  }

  TypeConverter ICustomTypeDescriptor.GetConverter()
  {
    return CustomTypeDescHelpers.GetConverter((object) this);
  }

  EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
  {
    return CustomTypeDescHelpers.GetDefaultEvent((object) this);
  }

  PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
  {
    return CustomTypeDescHelpers.GetDefaultProperty((object) this);
  }

  object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
  {
    return CustomTypeDescHelpers.GetEditor((object) this, editorBaseType);
  }

  EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
  {
    return CustomTypeDescHelpers.GetEvents((object) attributes);
  }

  EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
  {
    return CustomTypeDescHelpers.GetEvents((object) this);
  }

  PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
  {
    return CustomTypeDescHelpers.GetProperties((object) attributes);
  }

  PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
  {
    return CustomTypeDescHelpers.GetProperties((object) this);
  }

  object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
  {
    return CustomTypeDescHelpers.GetPropertyOwner((object) this, pd);
  }

  public string __repr__(CodeContext context)
  {
    return $"<class {this.FullName} at {PythonOps.HexId((object) this)}>";
  }

  internal bool TryLookupInit(object inst, out object ret)
  {
    if (!this.TryLookupSlot("__init__", out ret))
      return false;
    ret = this.GetOldStyleDescriptor(DefaultContext.Default, ret, inst, (object) this);
    return true;
  }

  internal static object MakeCallError()
  {
    throw PythonOps.TypeError("this constructor takes no arguments");
  }

  internal void SetBases(object value) => this._bases = this.ValidateBases(value);

  internal void SetName(object value)
  {
    this._name = value is string str ? (object) str : throw PythonOps.TypeError("TypeError: __name__ must be a string object");
  }

  internal void SetDictionary(object value)
  {
    this._dict = value is PythonDictionary pythonDictionary ? pythonDictionary : throw PythonOps.TypeError("__dict__ must be set to dictionary");
  }

  internal void SetNameHelper(string name, object value)
  {
    this._dict._storage.Add(ref this._dict._storage, (object) name, value);
    switch (name)
    {
      case "__del__":
        this.HasFinalizer = true;
        break;
      case "__setattr__":
        this.HasSetAttr = true;
        break;
      case "__delattr__":
        this.HasDelAttr = true;
        break;
    }
  }

  internal object LookupValue(CodeContext context, string name)
  {
    object obj;
    if (this.TryLookupValue(context, name, out obj))
      return obj;
    throw PythonOps.AttributeErrorForMissingAttribute((object) this, name);
  }

  internal bool TryLookupValue(CodeContext context, string name, out object value)
  {
    if (!this.TryLookupSlot(name, out value))
      return false;
    value = this.GetOldStyleDescriptor(context, value, (object) null, (object) this);
    return true;
  }

  internal void DictionaryIsPublic()
  {
    this.HasDelAttr = true;
    this.HasSetAttr = true;
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new MetaOldClass(parameter, BindingRestrictions.Empty, this);
  }

  WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    this._tracker = value;
    return true;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._tracker = value;

  internal class OldClassDebugView
  {
    private readonly OldClass _class;

    public OldClassDebugView(OldClass klass) => this._class = klass;

    public List<OldClass> __bases__ => this._class.BaseClasses;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    internal List<ObjectDebugView> Members
    {
      get
      {
        List<ObjectDebugView> members = new List<ObjectDebugView>();
        if (this._class._dict != null)
        {
          foreach (KeyValuePair<object, object> keyValuePair in this._class._dict)
            members.Add(new ObjectDebugView(keyValuePair.Key, keyValuePair.Value));
        }
        return members;
      }
    }
  }
}
