// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.OldInstance
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("instance")]
[DebuggerTypeProxy(typeof (OldInstance.OldInstanceDebugView))]
[DebuggerDisplay("old-style instance of {ClassName}")]
[Serializable]
public sealed class OldInstance : 
  ICodeFormattable,
  ICustomTypeDescriptor,
  ISerializable,
  IWeakReferenceable,
  IDynamicMetaObjectProvider,
  IPythonMembersList,
  IMembersList,
  IFastGettable
{
  private PythonDictionary _dict;
  internal OldClass _class;
  private WeakRefTracker _weakRef;

  private static PythonDictionary MakeDictionary(OldClass oldClass)
  {
    return new PythonDictionary((DictionaryStorage) new CustomInstanceDictionaryStorage(oldClass.OptimizedInstanceNames, oldClass.OptimizedInstanceNamesVersion));
  }

  public OldInstance(CodeContext context, OldClass @class)
  {
    this._class = @class;
    this._dict = OldInstance.MakeDictionary(@class);
    if (!this._class.HasFinalizer)
      return;
    this.AddFinalizer(context);
  }

  public OldInstance(CodeContext context, OldClass @class, PythonDictionary dict)
  {
    this._class = @class;
    this._dict = dict ?? PythonDictionary.MakeSymbolDictionary();
    if (!this._class.HasFinalizer)
      return;
    this.AddFinalizer(context);
  }

  private OldInstance(SerializationInfo info, StreamingContext context)
  {
    this._class = (OldClass) info.GetValue("__class__", typeof (OldClass));
    this._dict = OldInstance.MakeDictionary(this._class);
    List<object> objectList1 = (List<object>) info.GetValue("keys", typeof (List<object>));
    List<object> objectList2 = (List<object>) info.GetValue("values", typeof (List<object>));
    for (int index = 0; index < objectList1.Count; ++index)
      this._dict[objectList1[index]] = objectList2[index];
  }

  private void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    ContractUtils.RequiresNotNull((object) info, nameof (info));
    info.AddValue("__class__", (object) this._class);
    List<object> objectList1 = new List<object>();
    List<object> objectList2 = new List<object>();
    foreach (object key in this._dict.keys())
    {
      objectList1.Add(key);
      object obj;
      this._dict.TryGetValue(key, out obj);
      objectList2.Add(obj);
    }
    info.AddValue("keys", (object) objectList1);
    info.AddValue("values", (object) objectList2);
  }

  internal PythonDictionary Dictionary => this._dict;

  internal string ClassName => this._class.Name;

  public static bool operator true(OldInstance self)
  {
    return (bool) self.__nonzero__(DefaultContext.Default);
  }

  public static bool operator false(OldInstance self)
  {
    return !(bool) self.__nonzero__(DefaultContext.Default);
  }

  public override string ToString()
  {
    object o = OldInstance.InvokeOne(this, "__str__");
    if (o == NotImplementedType.Value)
      return this.__repr__(DefaultContext.Default);
    string result;
    return Converter.TryConvertToString(o, out result) && result != null ? result : throw PythonOps.TypeError("__str__ returned non-string type ({0})", (object) PythonTypeOps.GetName(o));
  }

  public string __repr__(CodeContext context)
  {
    object o = OldInstance.InvokeOne(this, nameof (__repr__));
    if (o == NotImplementedType.Value)
      return $"<{this._class.FullName} instance at {PythonOps.HexId((object) this)}>";
    string result;
    return Converter.TryConvertToString(o, out result) && result != null ? result : throw PythonOps.TypeError("__repr__ returned non-string type ({0})", (object) PythonTypeOps.GetName(o));
  }

  [return: MaybeNotImplemented]
  public object __divmod__(CodeContext context, object divmod)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__divmod__), out func) ? PythonCalls.Call(context, func, divmod) : (object) NotImplementedType.Value;
  }

  [return: MaybeNotImplemented]
  public static object __rdivmod__(CodeContext context, object divmod, [NotNull] OldInstance self)
  {
    object func;
    return self.TryGetBoundCustomMember(context, nameof (__rdivmod__), out func) ? PythonCalls.Call(context, func, divmod) : (object) NotImplementedType.Value;
  }

  public object __coerce__(CodeContext context, object other)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__coerce__), out func) ? PythonCalls.Call(context, func, other) : (object) NotImplementedType.Value;
  }

  public object __len__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__len__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__len__));
  }

  public object __pos__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__pos__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__pos__));
  }

  [SpecialName]
  public object GetItem(CodeContext context, object item)
  {
    return PythonOps.Invoke(context, (object) this, "__getitem__", item);
  }

  [SpecialName]
  public void SetItem(CodeContext context, object item, object value)
  {
    PythonOps.Invoke(context, (object) this, "__setitem__", item, value);
  }

  [SpecialName]
  public object DeleteItem(CodeContext context, object item)
  {
    object func;
    if (this.TryGetBoundCustomMember(context, "__delitem__", out func))
      return PythonCalls.Call(context, func, item);
    throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, "__delitem__");
  }

  [Python3Warning("in 3.x, __getslice__ has been removed; use __getitem__")]
  public object __getslice__(CodeContext context, int i, int j)
  {
    object ret;
    if (this.TryRawGetAttr(context, nameof (__getslice__), out ret))
      return PythonCalls.Call(context, ret, (object) i, (object) j);
    if (this.TryRawGetAttr(context, "__getitem__", out ret))
      return PythonCalls.Call(context, ret, (object) new Slice((object) i, (object) j));
    throw PythonOps.TypeError("instance {0} does not have __getslice__ or __getitem__", (object) this._class.Name);
  }

  [Python3Warning("in 3.x, __setslice__ has been removed; use __setitem__")]
  public void __setslice__(CodeContext context, int i, int j, object value)
  {
    object ret;
    if (this.TryRawGetAttr(context, nameof (__setslice__), out ret))
      PythonCalls.Call(context, ret, (object) i, (object) j, value);
    else if (this.TryRawGetAttr(context, "__setitem__", out ret))
      PythonCalls.Call(context, ret, (object) new Slice((object) i, (object) j), value);
    else
      throw PythonOps.TypeError("instance {0} does not have __setslice__ or __setitem__", (object) this._class.Name);
  }

  [Python3Warning("in 3.x, __delslice__ has been removed; use __delitem__")]
  public object __delslice__(CodeContext context, int i, int j)
  {
    object ret;
    if (this.TryRawGetAttr(context, nameof (__delslice__), out ret))
      return PythonCalls.Call(context, ret, (object) i, (object) j);
    if (this.TryRawGetAttr(context, "__delitem__", out ret))
      return PythonCalls.Call(context, ret, (object) new Slice((object) i, (object) j));
    throw PythonOps.TypeError("instance {0} does not have __delslice__ or __delitem__", (object) this._class.Name);
  }

  public object __index__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, "__int__", out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.TypeError("object cannot be converted to an index");
  }

  public object __neg__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__neg__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__neg__));
  }

  public object __abs__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__abs__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__abs__));
  }

  public object __invert__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__invert__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__invert__));
  }

  public object __contains__(CodeContext context, object index)
  {
    object func;
    if (this.TryGetBoundCustomMember(context, nameof (__contains__), out func))
      return PythonCalls.Call(context, func, index);
    IEnumerator enumerator = PythonOps.GetEnumerator((object) this);
    while (enumerator.MoveNext())
    {
      if (PythonOps.EqualRetBool(context, enumerator.Current, index))
        return ScriptingRuntimeHelpers.True;
    }
    return ScriptingRuntimeHelpers.False;
  }

  [SpecialName]
  public object Call(CodeContext context) => this.Call(context, ArrayUtils.EmptyObjects);

  [SpecialName]
  public object Call(CodeContext context, object args)
  {
    try
    {
      PythonOps.FunctionPushFrame(context.LanguageContext);
      object func;
      if (this.TryGetBoundCustomMember(context, "__call__", out func))
        return PythonOps.CallWithContext(context, func, args);
    }
    finally
    {
      PythonOps.FunctionPopFrame();
    }
    throw PythonOps.AttributeError("{0} instance has no __call__ method", (object) this._class.Name);
  }

  [SpecialName]
  public object Call(CodeContext context, params object[] args)
  {
    try
    {
      PythonOps.FunctionPushFrame(context.LanguageContext);
      object func;
      if (this.TryGetBoundCustomMember(context, "__call__", out func))
        return PythonOps.CallWithContext(context, func, args);
    }
    finally
    {
      PythonOps.FunctionPopFrame();
    }
    throw PythonOps.AttributeError("{0} instance has no __call__ method", (object) this._class.Name);
  }

  [SpecialName]
  public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
  {
    try
    {
      PythonOps.FunctionPushFrame(context.LanguageContext);
      object func;
      if (this.TryGetBoundCustomMember(context, "__call__", out func))
        return context.LanguageContext.CallWithKeywords(func, args, dict);
    }
    finally
    {
      PythonOps.FunctionPopFrame();
    }
    throw PythonOps.AttributeError("{0} instance has no __call__ method", (object) this._class.Name);
  }

  public object __nonzero__(CodeContext context)
  {
    object func;
    if (this.TryGetBoundCustomMember(context, nameof (__nonzero__), out func))
      return PythonOps.CallWithContext(context, func);
    if (!this.TryGetBoundCustomMember(context, "__len__", out func))
      return ScriptingRuntimeHelpers.True;
    object o = PythonOps.CallWithContext(context, func);
    switch (o)
    {
      case int _:
      case BigInteger _:
        return ScriptingRuntimeHelpers.BooleanToObject(Converter.ConvertToBoolean(o));
      default:
        throw PythonOps.TypeError("an integer is required, got {0}", (object) PythonTypeOps.GetName(o));
    }
  }

  public object __hex__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__hex__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__hex__));
  }

  public object __oct__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__oct__), out func) ? PythonOps.CallWithContext(context, func) : throw PythonOps.AttributeErrorForOldInstanceMissingAttribute(this._class.Name, nameof (__oct__));
  }

  public object __int__(CodeContext context)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, (object) this, nameof (__int__), out ret) ? PythonOps.CallWithContext(context, ret) : (object) NotImplementedType.Value;
  }

  public object __long__(CodeContext context)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, (object) this, nameof (__long__), out ret) ? PythonOps.CallWithContext(context, ret) : (object) NotImplementedType.Value;
  }

  public object __float__(CodeContext context)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, (object) this, nameof (__float__), out ret) ? PythonOps.CallWithContext(context, ret) : (object) NotImplementedType.Value;
  }

  public object __complex__(CodeContext context)
  {
    object func;
    return this.TryGetBoundCustomMember(context, nameof (__complex__), out func) ? PythonOps.CallWithContext(context, func) : (object) NotImplementedType.Value;
  }

  public object __getattribute__(CodeContext context, string name)
  {
    object obj;
    if (this.TryGetBoundCustomMember(context, name, out obj))
      return obj;
    throw PythonOps.AttributeError("{0} instance has no attribute '{1}'", this._class._name, (object) name);
  }

  internal object GetBoundMember(CodeContext context, string name)
  {
    object boundMember;
    if (this.TryGetBoundCustomMember(context, name, out boundMember))
      return boundMember;
    throw PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) PythonTypeOps.GetName((object) this), (object) name);
  }

  internal bool TryGetBoundCustomMember(CodeContext context, string name, out object value)
  {
    switch (name)
    {
      case "__dict__":
        value = (object) this._dict;
        return true;
      case "__class__":
        value = (object) this._class;
        return true;
      default:
        if (this.TryRawGetAttr(context, name, out value))
          return true;
        if (name != "__getattr__")
        {
          object ret;
          if (this.TryRawGetAttr(context, "__getattr__", out ret))
          {
            try
            {
              value = PythonCalls.Call(context, ret, (object) name);
              return true;
            }
            catch (MissingMemberException ex)
            {
            }
          }
        }
        return false;
    }
  }

  internal void SetCustomMember(CodeContext context, string name, object value)
  {
    switch (name)
    {
      case "__class__":
        this.SetClass(value);
        break;
      case "__dict__":
        this.SetDict(context, value);
        break;
      default:
        object ret;
        if (this._class.HasSetAttr && this._class.TryLookupSlot("__setattr__", out ret))
        {
          PythonCalls.Call(context, this._class.GetOldStyleDescriptor(context, ret, (object) this, (object) this._class), (object) name.ToString(), value);
          break;
        }
        if (name == "__del__")
        {
          this.SetFinalizer(context, name, value);
          break;
        }
        this._dict[(object) name] = value;
        break;
    }
  }

  private void SetFinalizer(CodeContext context, string name, object value)
  {
    if (!this.HasFinalizer())
      this.AddFinalizer(context);
    this._dict[(object) name] = value;
  }

  private void SetDict(CodeContext context, object value)
  {
    if (!(value is PythonDictionary pythonDictionary))
      throw PythonOps.TypeError("__dict__ must be set to a dictionary");
    if (this.HasFinalizer() && !this._class.HasFinalizer)
    {
      if (!pythonDictionary.ContainsKey((object) "__del__"))
        this.ClearFinalizer();
    }
    else if (pythonDictionary.ContainsKey((object) "__del__"))
      this.AddFinalizer(context);
    this._dict = pythonDictionary;
  }

  private void SetClass(object value)
  {
    this._class = value is OldClass oldClass ? oldClass : throw PythonOps.TypeError("__class__ must be set to class");
  }

  internal bool DeleteCustomMember(CodeContext context, string name)
  {
    switch (name)
    {
      case "__class__":
        throw PythonOps.TypeError("__class__ must be set to class");
      case "__dict__":
        throw PythonOps.TypeError("__dict__ must be set to a dictionary");
      default:
        object ret;
        if (this._class.HasDelAttr && this._class.TryLookupSlot("__delattr__", out ret))
        {
          PythonCalls.Call(context, this._class.GetOldStyleDescriptor(context, ret, (object) this, (object) this._class), (object) name.ToString());
          return true;
        }
        if (name == "__del__" && this.HasFinalizer() && !this._class.HasFinalizer)
          this.ClearFinalizer();
        if (!this._dict.Remove((object) name))
          throw PythonOps.AttributeError("{0} is not a valid attribute", (object) name);
        return true;
    }
  }

  IList<string> IMembersList.GetMemberNames()
  {
    return PythonOps.GetStringMemberList((IPythonMembersList) this);
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    PythonDictionary pythonDictionary = new PythonDictionary(this._dict);
    OldClass.RecurseAttrHierarchy(this._class, (IDictionary<object, object>) pythonDictionary);
    return (IList<object>) PythonOps.MakeListFromSequence((object) pythonDictionary);
  }

  [return: MaybeNotImplemented]
  public object __cmp__(CodeContext context, object other)
  {
    OldInstance oldInstance = other as OldInstance;
    object obj1 = this.InternalCompare(nameof (__cmp__), other);
    if (obj1 != NotImplementedType.Value)
      return obj1;
    if (oldInstance != null)
    {
      object obj2 = oldInstance.InternalCompare(nameof (__cmp__), (object) this);
      if (obj2 != NotImplementedType.Value)
        return (object) ((int) obj2 * -1);
    }
    return (object) NotImplementedType.Value;
  }

  private object CompareForwardReverse(object other, string forward, string reverse)
  {
    object obj = this.InternalCompare(forward, other);
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance oldInstance ? oldInstance.InternalCompare(reverse, (object) this) : (object) NotImplementedType.Value;
  }

  public static object operator >([NotNull] OldInstance self, object other)
  {
    return self.CompareForwardReverse(other, "__gt__", "__lt__");
  }

  public static object operator <([NotNull] OldInstance self, object other)
  {
    return self.CompareForwardReverse(other, "__lt__", "__gt__");
  }

  public static object operator >=([NotNull] OldInstance self, object other)
  {
    return self.CompareForwardReverse(other, "__ge__", "__le__");
  }

  public static object operator <=([NotNull] OldInstance self, object other)
  {
    return self.CompareForwardReverse(other, "__le__", "__ge__");
  }

  private object InternalCompare(string cmp, object other)
  {
    return OldInstance.InvokeOne(this, other, cmp);
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
    return CustomTypeDescHelpers.GetEvents((object) this, attributes);
  }

  EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
  {
    return CustomTypeDescHelpers.GetEvents((object) this);
  }

  PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
  {
    return CustomTypeDescHelpers.GetProperties((object) this, attributes);
  }

  PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
  {
    return CustomTypeDescHelpers.GetProperties((object) this);
  }

  object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
  {
    return CustomTypeDescHelpers.GetPropertyOwner((object) this, pd);
  }

  WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakRef;

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    this._weakRef = value;
    return true;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
  {
    ((IWeakReferenceable) this).SetWeakRef(value);
  }

  public int __hash__(CodeContext context)
  {
    object o = OldInstance.InvokeOne(this, nameof (__hash__));
    if (o != NotImplementedType.Value)
    {
      switch (o)
      {
        case BigInteger self:
          return BigIntegerOps.__hash__(self);
        case int num:
          return num;
        default:
          throw PythonOps.TypeError("expected int from __hash__, got {0}", (object) PythonTypeOps.GetName(o));
      }
    }
    else
    {
      object obj;
      if (this.TryGetBoundCustomMember(context, "__cmp__", out obj) || this.TryGetBoundCustomMember(context, "__eq__", out obj))
        throw PythonOps.TypeError("unhashable instance");
      return base.GetHashCode();
    }
  }

  public override int GetHashCode()
  {
    object obj;
    try
    {
      obj = OldInstance.InvokeOne(this, "__hash__");
    }
    catch
    {
      return base.GetHashCode();
    }
    if (obj != NotImplementedType.Value)
    {
      switch (obj)
      {
        case int hashCode:
          return hashCode;
        case BigInteger self:
          return BigIntegerOps.__hash__(self);
      }
    }
    return base.GetHashCode();
  }

  [return: MaybeNotImplemented]
  public object __eq__(object other)
  {
    object obj = this.InvokeBoth(other, nameof (__eq__));
    return obj != NotImplementedType.Value ? obj : (object) NotImplementedType.Value;
  }

  private object InvokeBoth(object other, string si)
  {
    object obj1 = OldInstance.InvokeOne(this, other, si);
    if (obj1 != NotImplementedType.Value)
      return obj1;
    if (other is OldInstance self)
    {
      object obj2 = OldInstance.InvokeOne(self, (object) this, si);
      if (obj2 != NotImplementedType.Value)
        return obj2;
    }
    return (object) NotImplementedType.Value;
  }

  private static object InvokeOne(OldInstance self, object other, string si)
  {
    object func;
    try
    {
      if (!self.TryGetBoundCustomMember(DefaultContext.Default, si, out func))
        return (object) NotImplementedType.Value;
    }
    catch (MissingMemberException ex)
    {
      return (object) NotImplementedType.Value;
    }
    return PythonOps.CallWithContext(DefaultContext.Default, func, other);
  }

  private static object InvokeOne(OldInstance self, object other, object other2, string si)
  {
    object func;
    try
    {
      if (!self.TryGetBoundCustomMember(DefaultContext.Default, si, out func))
        return (object) NotImplementedType.Value;
    }
    catch (MissingMemberException ex)
    {
      return (object) NotImplementedType.Value;
    }
    return PythonOps.CallWithContext(DefaultContext.Default, func, other, other2);
  }

  private static object InvokeOne(OldInstance self, string si)
  {
    object func;
    try
    {
      if (!self.TryGetBoundCustomMember(DefaultContext.Default, si, out func))
        return (object) NotImplementedType.Value;
    }
    catch (MissingMemberException ex)
    {
      return (object) NotImplementedType.Value;
    }
    return PythonOps.CallWithContext(DefaultContext.Default, func);
  }

  [return: MaybeNotImplemented]
  public object __ne__(object other)
  {
    object obj = this.InvokeBoth(other, nameof (__ne__));
    return obj != NotImplementedType.Value ? obj : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object Power([NotNull] OldInstance self, object other, object mod)
  {
    object obj = OldInstance.InvokeOne(self, other, mod, "__pow__");
    return obj != NotImplementedType.Value ? obj : (object) NotImplementedType.Value;
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("__class__", (object) this._class);
    info.AddValue("__dict__", (object) this._dict);
  }

  private void RecurseAttrHierarchyInt(OldClass oc, IDictionary<string, object> attrs)
  {
    foreach (KeyValuePair<object, object> keyValuePair in oc._dict._storage.GetItems())
    {
      if (keyValuePair.Key is string key && !attrs.ContainsKey(key))
        attrs.Add(key, (object) key);
    }
    if (oc.BaseClasses.Count == 0)
      return;
    foreach (OldClass baseClass in oc.BaseClasses)
      this.RecurseAttrHierarchyInt(baseClass, attrs);
  }

  private void AddFinalizer(CodeContext context)
  {
    InstanceFinalizer instanceFinalizer = new InstanceFinalizer(context, (object) this);
    this._weakRef = new WeakRefTracker((IWeakReferenceable) this, (object) instanceFinalizer, (object) instanceFinalizer);
  }

  private void ClearFinalizer()
  {
    if (this._weakRef == null)
      return;
    WeakRefTracker weakRef = this._weakRef;
    if (weakRef == null)
      return;
    for (int index = 0; index < weakRef.HandlerCount; ++index)
    {
      if (weakRef.GetHandlerCallback(index) is InstanceFinalizer)
      {
        weakRef.RemoveHandlerAt(index);
        break;
      }
    }
    if (weakRef.HandlerCount != 0)
      return;
    GC.SuppressFinalize((object) weakRef);
    this._weakRef = (WeakRefTracker) null;
  }

  private bool HasFinalizer()
  {
    if (this._weakRef != null)
    {
      WeakRefTracker weakRef = this._weakRef;
      if (weakRef != null)
      {
        for (int index = 0; index < weakRef.HandlerCount; ++index)
        {
          if (weakRef.GetHandlerCallback(index) is InstanceFinalizer)
            return true;
        }
      }
    }
    return false;
  }

  private bool TryRawGetAttr(CodeContext context, string name, out object ret)
  {
    if (this._dict._storage.TryGetValue((object) name, out ret))
      return true;
    if (!this._class.TryLookupSlot(name, out ret))
      return false;
    ret = this._class.GetOldStyleDescriptor(context, ret, (object) this, (object) this._class);
    return true;
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new MetaOldInstance(parameter, BindingRestrictions.Empty, this);
  }

  T IFastGettable.MakeGetBinding<T>(
    CallSite<T> site,
    PythonGetMemberBinder binder,
    CodeContext state,
    string name)
  {
    if (binder.IsNoThrow)
      return (T) new Func<CallSite, object, CodeContext, object>(new OldInstance.FastOldInstanceGet(name).NoThrowTarget);
    return binder.SupportsLightThrow ? (T) new Func<CallSite, object, CodeContext, object>(new OldInstance.FastOldInstanceGet(name).LightThrowTarget) : (T) new Func<CallSite, object, CodeContext, object>(new OldInstance.FastOldInstanceGet(name).Target);
  }

  public static object operator +([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__add__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__radd__") : (object) NotImplementedType.Value;
  }

  public static object operator +(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__radd__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceAdd(object other) => OldInstance.InvokeOne(this, other, "__iadd__");

  public static object operator -([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__sub__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rsub__") : (object) NotImplementedType.Value;
  }

  public static object operator -(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rsub__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceSubtract(object other) => OldInstance.InvokeOne(this, other, "__isub__");

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object Power([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__pow__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rpow__") : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object Power(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rpow__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlacePower(object other) => OldInstance.InvokeOne(this, other, "__ipow__");

  public static object operator *([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__mul__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rmul__") : (object) NotImplementedType.Value;
  }

  public static object operator *(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rmul__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceMultiply(object other) => OldInstance.InvokeOne(this, other, "__imul__");

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object FloorDivide([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__floordiv__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rfloordiv__") : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object FloorDivide(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rfloordiv__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceFloorDivide(object other)
  {
    return OldInstance.InvokeOne(this, other, "__ifloordiv__");
  }

  public static object operator /([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__div__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rdiv__") : (object) NotImplementedType.Value;
  }

  public static object operator /(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rdiv__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceDivide(object other) => OldInstance.InvokeOne(this, other, "__idiv__");

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object TrueDivide([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__truediv__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rtruediv__") : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object TrueDivide(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rtruediv__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceTrueDivide(object other)
  {
    return OldInstance.InvokeOne(this, other, "__itruediv__");
  }

  public static object operator %([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__mod__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rmod__") : (object) NotImplementedType.Value;
  }

  public static object operator %(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rmod__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceMod(object other) => OldInstance.InvokeOne(this, other, "__imod__");

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object LeftShift([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__lshift__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rlshift__") : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object LeftShift(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rlshift__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceLeftShift(object other) => OldInstance.InvokeOne(this, other, "__ilshift__");

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object RightShift([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__rshift__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rrshift__") : (object) NotImplementedType.Value;
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object RightShift(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rrshift__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceRightShift(object other)
  {
    return OldInstance.InvokeOne(this, other, "__irshift__");
  }

  public static object operator &([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__and__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rand__") : (object) NotImplementedType.Value;
  }

  public static object operator &(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rand__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceBitwiseAnd(object other) => OldInstance.InvokeOne(this, other, "__iand__");

  public static object operator |([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__or__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__ror__") : (object) NotImplementedType.Value;
  }

  public static object operator |(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__ror__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceBitwiseOr(object other) => OldInstance.InvokeOne(this, other, "__ior__");

  public static object operator ^([NotNull] OldInstance self, object other)
  {
    object obj = OldInstance.InvokeOne(self, other, "__xor__");
    if (obj != NotImplementedType.Value)
      return obj;
    return other is OldInstance self1 ? OldInstance.InvokeOne(self1, (object) self, "__rxor__") : (object) NotImplementedType.Value;
  }

  public static object operator ^(object other, [NotNull] OldInstance self)
  {
    return OldInstance.InvokeOne(self, other, "__rxor__");
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public object InPlaceExclusiveOr(object other) => OldInstance.InvokeOne(this, other, "__ixor__");

  internal class OldInstanceDebugView
  {
    private readonly OldInstance _userObject;

    public OldInstanceDebugView(OldInstance userObject) => this._userObject = userObject;

    public OldClass __class__ => this._userObject._class;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    internal List<ObjectDebugView> Members
    {
      get
      {
        List<ObjectDebugView> members = new List<ObjectDebugView>();
        if (this._userObject._dict != null)
        {
          foreach (KeyValuePair<object, object> keyValuePair in this._userObject._dict)
            members.Add(new ObjectDebugView(keyValuePair.Key, keyValuePair.Value));
        }
        return members;
      }
    }
  }

  private class FastOldInstanceGet
  {
    private readonly string _name;

    public FastOldInstanceGet(string name) => this._name = name;

    public object Target(CallSite site, object instance, CodeContext context)
    {
      if (!(instance is OldInstance oldInstance))
        return ((CallSite<Func<CallSite, object, CodeContext, object>>) site).Update(site, instance, context);
      object obj;
      if (oldInstance.TryGetBoundCustomMember(context, this._name, out obj))
        return obj;
      throw PythonOps.AttributeError("{0} instance has no attribute '{1}'", (object) oldInstance._class.Name, (object) this._name);
    }

    public object LightThrowTarget(CallSite site, object instance, CodeContext context)
    {
      if (!(instance is OldInstance oldInstance))
        return ((CallSite<Func<CallSite, object, CodeContext, object>>) site).Update(site, instance, context);
      object obj;
      if (oldInstance.TryGetBoundCustomMember(context, this._name, out obj))
        return obj;
      return LightExceptions.Throw(PythonOps.AttributeError("{0} instance has no attribute '{1}'", (object) oldInstance._class.Name, (object) this._name));
    }

    public object NoThrowTarget(CallSite site, object instance, CodeContext context)
    {
      if (!(instance is OldInstance oldInstance))
        return ((CallSite<Func<CallSite, object, CodeContext, object>>) site).Update(site, instance, context);
      object obj;
      return oldInstance.TryGetBoundCustomMember(context, this._name, out obj) ? obj : (object) OperationFailed.Value;
    }
  }
}
