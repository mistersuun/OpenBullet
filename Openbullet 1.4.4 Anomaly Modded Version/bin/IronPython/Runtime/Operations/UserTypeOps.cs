// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UserTypeOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class UserTypeOps
{
  public static string ToStringReturnHelper(object o)
  {
    return o is string && o != null ? (string) o : throw PythonOps.TypeError("__str__ returned non-string type ({0})", (object) PythonTypeOps.GetName(o));
  }

  public static PythonDictionary SetDictHelper(ref PythonDictionary dict, PythonDictionary value)
  {
    return Interlocked.CompareExchange<PythonDictionary>(ref dict, value, (PythonDictionary) null) == null ? value : dict;
  }

  public static object GetPropertyHelper(object prop, object instance, string name)
  {
    if (!(prop is PythonTypeSlot pythonTypeSlot))
      throw PythonOps.TypeError("Expected property for {0}, but found {1}", (object) name.ToString(), (object) DynamicHelpers.GetPythonType(prop).Name);
    object propertyHelper;
    pythonTypeSlot.TryGetValue(DefaultContext.Default, instance, DynamicHelpers.GetPythonType(instance), out propertyHelper);
    return propertyHelper;
  }

  public static void SetPropertyHelper(object prop, object instance, object newValue, string name)
  {
    if (!(prop is PythonTypeSlot pythonTypeSlot))
      throw PythonOps.TypeError("Expected settable property for {0}, but found {1}", (object) name.ToString(), (object) DynamicHelpers.GetPythonType(prop).Name);
    pythonTypeSlot.TrySetValue(DefaultContext.Default, instance, DynamicHelpers.GetPythonType(instance), newValue);
  }

  public static bool SetWeakRefHelper(IPythonObject obj, WeakRefTracker value)
  {
    if (!obj.PythonType.IsWeakReferencable)
      return false;
    object[] slotsCreate = obj.GetSlotsCreate();
    slotsCreate[slotsCreate.Length - 1] = (object) value;
    return true;
  }

  public static WeakRefTracker GetWeakRefHelper(IPythonObject obj)
  {
    object[] slots = obj.GetSlots();
    return slots == null ? (WeakRefTracker) null : (WeakRefTracker) slots[slots.Length - 1];
  }

  public static void SetFinalizerHelper(IPythonObject obj, WeakRefTracker value)
  {
    object[] slotsCreate = obj.GetSlotsCreate();
    if (Interlocked.CompareExchange(ref slotsCreate[slotsCreate.Length - 1], (object) value, (object) null) == null)
      return;
    GC.SuppressFinalize((object) value);
  }

  public static object[] GetSlotsCreate(IPythonObject obj, ref object[] slots)
  {
    if (slots != null)
      return slots;
    Interlocked.CompareExchange<object[]>(ref slots, new object[obj.PythonType.SlotCount + 1], (object[]) null);
    return slots;
  }

  public static void AddRemoveEventHelper(
    object method,
    IPythonObject instance,
    object eventValue,
    string name)
  {
    object obj = method;
    PythonType pythonType = instance.PythonType;
    if (method is PythonTypeSlot pythonTypeSlot && !pythonTypeSlot.TryGetValue(DefaultContext.Default, (object) instance, pythonType, out obj))
      throw PythonOps.AttributeErrorForMissingAttribute(pythonType.Name, name);
    if (!PythonOps.IsCallable(DefaultContext.Default, obj))
      throw PythonOps.TypeError("Expected callable value for {0}, but found {1}", (object) name.ToString(), (object) PythonTypeOps.GetName(method));
    PythonCalls.Call(obj, eventValue);
  }

  public static DynamicMetaObject GetMetaObjectHelper(
    IPythonObject self,
    Expression parameter,
    DynamicMetaObject baseMetaObject)
  {
    return (DynamicMetaObject) new MetaUserObject(parameter, BindingRestrictions.Empty, baseMetaObject, self);
  }

  public static bool TryGetMixedNewStyleOldStyleSlot(
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
    PythonType pythonType1 = DynamicHelpers.GetPythonType(instance);
    foreach (PythonType pythonType2 in (IEnumerable<PythonType>) pythonType1.ResolutionOrder)
    {
      if (pythonType2 != TypeCache.Object && pythonType2.OldClass != null)
      {
        OldClass oldClass = pythonType2.OldClass;
        if (oldClass.TryGetBoundCustomMember(context, name, out value))
        {
          value = oldClass.GetOldStyleDescriptor(context, value, instance, (object) oldClass);
          return true;
        }
      }
      else
      {
        PythonTypeSlot slot;
        if (pythonType2.TryLookupSlot(context, name, out slot))
          return slot.TryGetValue(context, instance, pythonType1, out value);
      }
    }
    value = (object) null;
    return false;
  }

  public static bool TryGetDictionaryValue(
    PythonDictionary dict,
    string name,
    int keyVersion,
    int keyIndex,
    out object res)
  {
    if (dict != null)
    {
      if (dict._storage is CustomInstanceDictionaryStorage storage && storage.KeyVersion == keyVersion)
      {
        if (storage.TryGetValue(keyIndex, out res))
          return true;
      }
      else if (dict.TryGetValue((object) name, out res))
        return true;
    }
    res = (object) null;
    return false;
  }

  public static object SetDictionaryValue(IPythonObject self, string name, object value)
  {
    return UserTypeOps.GetDictionary(self)[(object) name] = value;
  }

  public static object SetDictionaryValueOptimized(
    IPythonObject ipo,
    string name,
    object value,
    int keysVersion,
    int index)
  {
    PythonDictionary dictionary = UserTypeOps.GetDictionary(ipo);
    if (dictionary._storage is CustomInstanceDictionaryStorage storage && storage.KeyVersion == keysVersion)
      storage.SetExtraValue(index, value);
    else
      dictionary[(object) name] = value;
    return value;
  }

  public static object FastSetDictionaryValue(ref PythonDictionary dict, string name, object value)
  {
    if (dict == null)
      Interlocked.CompareExchange<PythonDictionary>(ref dict, PythonDictionary.MakeSymbolDictionary(), (PythonDictionary) null);
    return dict[(object) name] = value;
  }

  public static object FastSetDictionaryValueOptimized(
    PythonType type,
    ref PythonDictionary dict,
    string name,
    object value,
    int keysVersion,
    int index)
  {
    if (dict == null)
      Interlocked.CompareExchange<PythonDictionary>(ref dict, type.MakeDictionary(), (PythonDictionary) null);
    if (!(dict._storage is CustomInstanceDictionaryStorage storage) || storage.KeyVersion != keysVersion)
      return dict[(object) name] = value;
    storage.SetExtraValue(index, value);
    return value;
  }

  public static object RemoveDictionaryValue(IPythonObject self, string name)
  {
    PythonDictionary dict = self.Dict;
    if (dict != null && dict.Remove((object) name))
      return (object) null;
    throw PythonOps.AttributeErrorForMissingAttribute((object) self.PythonType, name);
  }

  internal static PythonDictionary GetDictionary(IPythonObject self)
  {
    PythonDictionary dictionary = self.Dict;
    if (dictionary == null && self.PythonType.HasDictionary)
      dictionary = self.SetDict(self.PythonType.MakeDictionary());
    return dictionary;
  }

  public static string ToStringHelper(IPythonObject o)
  {
    return ObjectOps.__str__(DefaultContext.Default, (object) o);
  }

  public static bool TryGetNonInheritedMethodHelper(
    PythonType dt,
    object instance,
    string name,
    out object callTarget)
  {
    foreach (PythonType dt1 in (IEnumerable<PythonType>) dt.ResolutionOrder)
    {
      if (!dt1.IsSystemType)
      {
        if (UserTypeOps.LookupValue(dt1, instance, name, out callTarget))
          return true;
      }
      else
        break;
    }
    PythonDictionary dict;
    if (instance is IPythonObject pythonObject && (dict = pythonObject.Dict) != null && dict.TryGetValue((object) name, out callTarget))
      return true;
    callTarget = (object) null;
    return false;
  }

  private static bool LookupValue(PythonType dt, object instance, string name, out object value)
  {
    PythonTypeSlot slot;
    if (dt.TryLookupSlot(DefaultContext.Default, name, out slot) && slot.TryGetValue(DefaultContext.Default, instance, dt, out value))
      return true;
    value = (object) null;
    return false;
  }

  public static bool TryGetNonInheritedValueHelper(
    IPythonObject instance,
    string name,
    out object callTarget)
  {
    foreach (PythonType pythonType in (IEnumerable<PythonType>) instance.PythonType.ResolutionOrder)
    {
      if (!pythonType.IsSystemType)
      {
        PythonTypeSlot slot;
        if (pythonType.TryLookupSlot(DefaultContext.Default, name, out slot))
        {
          callTarget = (object) slot;
          return true;
        }
      }
      else
        break;
    }
    IPythonObject pythonObject = instance;
    PythonDictionary dict;
    if (pythonObject != null && (dict = pythonObject.Dict) != null && dict.TryGetValue((object) name, out callTarget))
      return true;
    callTarget = (object) null;
    return false;
  }

  public static object GetAttribute(
    CodeContext context,
    object self,
    string name,
    PythonTypeSlot getAttributeSlot,
    PythonTypeSlot getAttrSlot,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, string, object>>> callSite)
  {
    if (callSite.Data == null)
      callSite.Data = UserTypeOps.MakeGetAttrSite(context);
    object obj;
    try
    {
      if (getAttributeSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj))
        return callSite.Data.Target((CallSite) callSite.Data, context, obj, name);
    }
    catch (MissingMemberException ex)
    {
      if (getAttrSlot != null && getAttrSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj))
        return callSite.Data.Target((CallSite) callSite.Data, context, obj, name);
      throw;
    }
    if (getAttrSlot != null && getAttrSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj))
      return callSite.Data.Target((CallSite) callSite.Data, context, obj, name);
    throw PythonOps.AttributeError(name);
  }

  public static object GetAttributeNoThrow(
    CodeContext context,
    object self,
    string name,
    PythonTypeSlot getAttributeSlot,
    PythonTypeSlot getAttrSlot,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, string, object>>> callSite)
  {
    if (callSite.Data == null)
      callSite.Data = UserTypeOps.MakeGetAttrSite(context);
    object obj;
    try
    {
      if (getAttributeSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj))
        return callSite.Data.Target((CallSite) callSite.Data, context, obj, name);
    }
    catch (MissingMemberException ex1)
    {
      try
      {
        return getAttrSlot != null && getAttrSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj) ? callSite.Data.Target((CallSite) callSite.Data, context, obj, name) : (object) OperationFailed.Value;
      }
      catch (MissingMemberException ex2)
      {
        return (object) OperationFailed.Value;
      }
    }
    try
    {
      if (getAttrSlot != null)
      {
        if (getAttrSlot.TryGetValue(context, self, ((IPythonObject) self).PythonType, out obj))
          return callSite.Data.Target((CallSite) callSite.Data, context, obj, name);
      }
    }
    catch (MissingMemberException ex)
    {
    }
    return (object) OperationFailed.Value;
  }

  private static CallSite<Func<CallSite, CodeContext, object, string, object>> MakeGetAttrSite(
    CodeContext context)
  {
    return CallSite<Func<CallSite, CodeContext, object, string, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeOne);
  }

  internal static FastBindResult<T> MakeGetBinding<T>(
    CodeContext codeContext,
    CallSite<T> site,
    IPythonObject self,
    PythonGetMemberBinder getBinder)
    where T : class
  {
    return typeof (IDynamicMetaObjectProvider).IsAssignableFrom(self.PythonType.FinalSystemType) && !(self is IFastGettable) ? new FastBindResult<T>() : (FastBindResult<T>) (ValueType) new MetaUserObject.FastGetBinderHelper(codeContext, (CallSite<Func<CallSite, object, CodeContext, object>>) site, self, getBinder).GetBinding(codeContext, getBinder.Name);
  }

  internal static FastBindResult<T> MakeSetBinding<T>(
    CodeContext codeContext,
    CallSite<T> site,
    IPythonObject self,
    object value,
    PythonSetMemberBinder setBinder)
    where T : class
  {
    if (typeof (IDynamicMetaObjectProvider).IsAssignableFrom(self.GetType().BaseType))
      return new FastBindResult<T>();
    Type type = typeof (T);
    if (type == typeof (Func<CallSite, object, object, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<object>(codeContext, self, value, setBinder).MakeSet();
    if (type == typeof (Func<CallSite, object, string, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<string>(codeContext, self, value, setBinder).MakeSet();
    if (type == typeof (Func<CallSite, object, int, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<int>(codeContext, self, value, setBinder).MakeSet();
    if (type == typeof (Func<CallSite, object, double, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<double>(codeContext, self, value, setBinder).MakeSet();
    if (type == typeof (Func<CallSite, object, List, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<List>(codeContext, self, value, setBinder).MakeSet();
    if (type == typeof (Func<CallSite, object, PythonTuple, object>))
      return (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<PythonTuple>(codeContext, self, value, setBinder).MakeSet();
    return type == typeof (Func<CallSite, object, PythonDictionary, object>) ? (FastBindResult<T>) (ValueType) new MetaUserObject.FastSetBinderHelper<PythonDictionary>(codeContext, self, value, setBinder).MakeSet() : new FastBindResult<T>();
  }
}
