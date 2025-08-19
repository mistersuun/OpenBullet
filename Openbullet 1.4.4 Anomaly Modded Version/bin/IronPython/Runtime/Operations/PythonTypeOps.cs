// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.PythonTypeOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Operations;

internal static class PythonTypeOps
{
  private static readonly Dictionary<FieldInfo, PythonTypeSlot> _fieldCache = new Dictionary<FieldInfo, PythonTypeSlot>();
  private static readonly Dictionary<BuiltinFunction, BuiltinMethodDescriptor> _methodCache = new Dictionary<BuiltinFunction, BuiltinMethodDescriptor>();
  private static readonly Dictionary<BuiltinFunction, ClassMethodDescriptor> _classMethodCache = new Dictionary<BuiltinFunction, ClassMethodDescriptor>();
  internal static readonly Dictionary<PythonTypeOps.BuiltinFunctionKey, BuiltinFunction> _functions = new Dictionary<PythonTypeOps.BuiltinFunctionKey, BuiltinFunction>();
  private static readonly Dictionary<ReflectionCache.MethodBaseCache, ConstructorFunction> _ctors = new Dictionary<ReflectionCache.MethodBaseCache, ConstructorFunction>();
  private static readonly Dictionary<EventTracker, ReflectedEvent> _eventCache = new Dictionary<EventTracker, ReflectedEvent>();
  internal static readonly Dictionary<PropertyTracker, ReflectedGetterSetter> _propertyCache = new Dictionary<PropertyTracker, ReflectedGetterSetter>();

  internal static PythonTuple MroToPython(IList<PythonType> types)
  {
    List<object> o = new List<object>(types.Count);
    foreach (PythonType type in (IEnumerable<PythonType>) types)
    {
      if (!(type.UnderlyingSystemType == typeof (ValueType)))
      {
        if (type.OldClass != null)
          o.Add((object) type.OldClass);
        else
          o.Add((object) type);
      }
    }
    return PythonTuple.Make((object) o);
  }

  internal static string GetModuleName(CodeContext context, Type type)
  {
    for (Type key = type; key != (Type) null; key = key.DeclaringType)
    {
      string moduleName;
      if (context.LanguageContext.BuiltinModuleNames.TryGetValue(key, out moduleName))
        return moduleName;
    }
    FieldInfo field = type.GetField("__module__");
    return field != (FieldInfo) null && field.IsLiteral && field.FieldType == typeof (string) ? (string) field.GetRawConstantValue() : "__builtin__";
  }

  internal static object CallParams(CodeContext context, PythonType cls, params object[] argsτ)
  {
    if (argsτ == null)
      argsτ = ArrayUtils.EmptyObjects;
    return PythonTypeOps.CallWorker(context, cls, argsτ);
  }

  internal static object CallWorker(CodeContext context, PythonType dt, object[] args)
  {
    object obj = PythonOps.CallWithContext(context, PythonTypeOps.GetTypeNew(context, dt), ArrayUtils.Insert<object>((object) dt, args));
    if (PythonTypeOps.ShouldInvokeInit(dt, DynamicHelpers.GetPythonType(obj), args.Length))
    {
      PythonOps.CallWithContext(context, PythonTypeOps.GetInitMethod(context, dt, obj), args);
      PythonTypeOps.AddFinalizer(context, dt, obj);
    }
    return obj;
  }

  internal static object CallWorker(
    CodeContext context,
    PythonType dt,
    IDictionary<string, object> kwArgs,
    object[] args)
  {
    object[] args1 = ArrayOps.CopyArray(args, kwArgs.Count + args.Length);
    string[] names = new string[kwArgs.Count];
    int length = args.Length;
    foreach (KeyValuePair<string, object> kwArg in (IEnumerable<KeyValuePair<string, object>>) kwArgs)
    {
      args1[length] = kwArg.Value;
      names[length++ - args.Length] = kwArg.Key;
    }
    return PythonTypeOps.CallWorker(context, dt, new KwCallInfo(args1, names));
  }

  internal static object CallWorker(CodeContext context, PythonType dt, KwCallInfo args)
  {
    object[] args1 = ArrayUtils.Insert<object>((object) dt, args.Arguments);
    object obj = PythonOps.CallWithKeywordArgs(context, PythonTypeOps.GetTypeNew(context, dt), args1, args.Names);
    if (obj == null)
      return (object) null;
    if (PythonTypeOps.ShouldInvokeInit(dt, DynamicHelpers.GetPythonType(obj), args.Arguments.Length))
    {
      PythonOps.CallWithKeywordArgs(context, PythonTypeOps.GetInitMethod(context, dt, obj), args.Arguments, args.Names);
      PythonTypeOps.AddFinalizer(context, dt, obj);
    }
    return obj;
  }

  private static object GetInitMethod(CodeContext context, PythonType dt, object newObject)
  {
    for (int index = 0; index < dt.ResolutionOrder.Count; ++index)
    {
      PythonType dt1 = dt.ResolutionOrder[index];
      object self;
      if (dt1.IsOldClass && PythonOps.ToPythonType(dt1) is OldClass pythonType && pythonType.TryGetBoundCustomMember(context, "__init__", out self))
        return pythonType.GetOldStyleDescriptor(context, self, newObject, (object) pythonType);
      PythonTypeSlot slot;
      if (dt1.TryLookupSlot(context, "__init__", out slot) && slot.TryGetValue(context, newObject, dt, out self))
        return self;
    }
    return (object) null;
  }

  private static void AddFinalizer(CodeContext context, PythonType dt, object newObject)
  {
    if (!dt.TryResolveSlot(context, "__del__", out PythonTypeSlot _))
      return;
    IWeakReferenceable weakReferenceable = context.LanguageContext.ConvertToWeakReferenceable(newObject);
    InstanceFinalizer instanceFinalizer = new InstanceFinalizer(context, newObject);
    weakReferenceable.SetFinalizer(new WeakRefTracker(weakReferenceable, (object) instanceFinalizer, (object) instanceFinalizer));
  }

  private static object GetTypeNew(CodeContext context, PythonType dt)
  {
    PythonTypeSlot slot;
    if (!dt.TryResolveSlot(context, "__new__", out slot))
      throw PythonOps.TypeError("cannot create instances of {0}", (object) dt.Name);
    object typeNew;
    slot.TryGetValue(context, (object) dt, dt, out typeNew);
    return typeNew;
  }

  internal static bool IsRuntimeAssembly(Assembly assembly)
  {
    return assembly == typeof (PythonOps).Assembly || assembly == typeof (LightCompiler).Assembly || assembly == typeof (DynamicMetaObject).Assembly || new AssemblyName(assembly.FullName).Name.Equals("IronPython.Modules");
  }

  private static bool ShouldInvokeInit(PythonType cls, PythonType newObjectType, int argCnt)
  {
    if (cls.IsSystemType && !cls.IsPythonType || !newObjectType.IsSubclassOf(cls))
      return false;
    return cls != TypeCache.PythonType || argCnt > 1;
  }

  internal static string GetName(object o)
  {
    return o is NamespaceTracker ? ((MemberTracker) o).Name : DynamicHelpers.GetPythonType(o).Name;
  }

  internal static string GetOldName(object o)
  {
    return !(o is OldInstance) ? PythonTypeOps.GetName(o) : PythonTypeOps.GetOldName((OldInstance) o);
  }

  internal static string GetOldName(OldInstance instance) => instance._class.Name;

  internal static PythonType[] ObjectTypes(object[] args)
  {
    PythonType[] pythonTypeArray = new PythonType[args.Length];
    for (int index = 0; index < args.Length; ++index)
      pythonTypeArray[index] = DynamicHelpers.GetPythonType(args[index]);
    return pythonTypeArray;
  }

  internal static Type[] ConvertToTypes(PythonType[] pythonTypes)
  {
    Type[] types = new Type[pythonTypes.Length];
    for (int index = 0; index < pythonTypes.Length; ++index)
      types[index] = PythonTypeOps.ConvertToType(pythonTypes[index]);
    return types;
  }

  private static Type ConvertToType(PythonType pythonType)
  {
    return pythonType.IsNull ? typeof (DynamicNull) : pythonType.UnderlyingSystemType;
  }

  internal static TrackerTypes GetMemberType(MemberGroup members)
  {
    TrackerTypes memberType = TrackerTypes.All;
    for (int index = 0; index < members.Count; ++index)
    {
      MemberTracker member = members[index];
      if (member.MemberType != memberType)
      {
        if (memberType != TrackerTypes.All)
          return TrackerTypes.All;
        memberType = member.MemberType;
      }
    }
    return memberType;
  }

  internal static PythonTypeSlot GetSlot(MemberGroup group, string name, bool privateBinding)
  {
    if (group.Count == 0)
      return (PythonTypeSlot) null;
    group = PythonTypeOps.FilterNewSlots(group);
    TrackerTypes memberType = PythonTypeOps.GetMemberType(group);
    switch (memberType)
    {
      case TrackerTypes.Constructor:
        return (PythonTypeSlot) PythonTypeOps.GetConstructorFunction(group[0].DeclaringType, privateBinding);
      case TrackerTypes.Event:
        return (PythonTypeSlot) PythonTypeOps.GetReflectedEvent((EventTracker) group[0]);
      case TrackerTypes.Field:
        return PythonTypeOps.GetReflectedField(((FieldTracker) group[0]).Field);
      case TrackerTypes.Method:
        bool checkStatic = false;
        List<MemberInfo> memberInfoList = new List<MemberInfo>();
        foreach (MethodTracker methodTracker in group)
        {
          memberInfoList.Add((MemberInfo) methodTracker.Method);
          checkStatic |= methodTracker.IsStatic;
        }
        Type declaringType = group[0].DeclaringType;
        MemberInfo[] array = memberInfoList.ToArray();
        FunctionType methodFunctionType = PythonTypeOps.GetMethodFunctionType(declaringType, array, checkStatic);
        return PythonTypeOps.GetFinalSlotForFunction(PythonTypeOps.GetBuiltinFunction(declaringType, group[0].Name, name, new FunctionType?(methodFunctionType), array));
      case TrackerTypes.Property:
        return (PythonTypeSlot) PythonTypeOps.GetReflectedProperty((PropertyTracker) group[0], group, privateBinding);
      case TrackerTypes.Type:
        TypeTracker existingTypeEntity = (TypeTracker) group[0];
        for (int index = 1; index < group.Count; ++index)
          existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, (TypeTracker) group[index]);
        return existingTypeEntity is TypeGroup ? (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) existingTypeEntity, true) : (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) DynamicHelpers.GetPythonTypeFromType(existingTypeEntity.Type), true);
      case TrackerTypes.Custom:
        return ((PythonCustomTracker) group[0]).GetSlot();
      default:
        throw new InvalidOperationException($"Bad member type {memberType.ToString()} on {group[0].DeclaringType}.{name}");
    }
  }

  internal static MemberGroup FilterNewSlots(MemberGroup group)
  {
    if (PythonTypeOps.GetMemberType(group) == TrackerTypes.All)
    {
      Type declaringType = group[0].DeclaringType;
      for (int index = 1; index < group.Count; ++index)
      {
        if (group[index].DeclaringType != declaringType && group[index].DeclaringType.IsSubclassOf(declaringType))
          declaringType = group[index].DeclaringType;
      }
      List<MemberTracker> memberTrackerList = new List<MemberTracker>();
      for (int index = 0; index < group.Count; ++index)
      {
        if (group[index].DeclaringType == declaringType)
          memberTrackerList.Add(group[index]);
      }
      if (memberTrackerList.Count != group.Count)
        return new MemberGroup(memberTrackerList.ToArray());
    }
    return group;
  }

  private static BuiltinFunction GetConstructorFunction(Type t, bool privateBinding)
  {
    BuiltinFunction nonDefaultNewInst = InstanceOps.NonDefaultNewInst;
    MethodBase[] constructors = PythonTypeOps.GetConstructors(t, privateBinding, true);
    return (BuiltinFunction) PythonTypeOps.GetConstructor(t, nonDefaultNewInst, constructors);
  }

  internal static MethodBase[] GetConstructors(Type t, bool privateBinding, bool includeProtected = false)
  {
    MethodBase[] first = CompilerHelpers.GetConstructors(t, privateBinding, includeProtected);
    if (t.IsEnum())
    {
      MethodInfo methodInfo = ReflectionUtils.GetDeclaredMethods(typeof (PythonTypeOps), "CreateEnum").Single<MethodInfo>().MakeGenericMethod(t);
      first = ((IEnumerable<MethodBase>) first).Concat<MethodBase>((IEnumerable<MethodBase>) new MethodInfo[1]
      {
        methodInfo
      }).ToArray<MethodBase>();
    }
    return first;
  }

  private static T CreateEnum<T>(object value)
  {
    if (value == null)
      throw PythonOps.ValueError("None is not a valid " + PythonOps.ToString((object) typeof (T)));
    try
    {
      return (T) Enum.ToObject(typeof (T), value);
    }
    catch (ArgumentException ex)
    {
      throw PythonOps.ValueError($"{PythonOps.ToString(value)} is not a valid {PythonOps.ToString((object) typeof (T))}");
    }
  }

  internal static bool IsDefaultNew(MethodBase[] targets)
  {
    if (targets.Length == 1)
    {
      ParameterInfo[] parameters = targets[0].GetParameters();
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].ParameterType == typeof (CodeContext))
        return true;
    }
    return false;
  }

  internal static BuiltinFunction GetConstructorFunction(Type type, string name)
  {
    List<MethodBase> methodBaseList = new List<MethodBase>();
    bool flag = false;
    foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
    {
      if (constructor.IsPublic)
      {
        if (constructor.GetParameters().Length == 0)
          flag = true;
        methodBaseList.Add((MethodBase) constructor);
      }
    }
    if (type.IsValueType() && !flag)
    {
      if (type != typeof (void))
      {
        try
        {
          methodBaseList.Add((MethodBase) typeof (ScriptingRuntimeHelpers).GetMethod("CreateInstance", ReflectionUtils.EmptyTypes).MakeGenericMethod(type));
        }
        catch (BadImageFormatException ex)
        {
        }
      }
    }
    return methodBaseList.Count > 0 ? BuiltinFunction.MakeFunction(name, methodBaseList.ToArray(), type) : (BuiltinFunction) null;
  }

  internal static ReflectedEvent GetReflectedEvent(EventTracker tracker)
  {
    ReflectedEvent reflectedEvent;
    lock (PythonTypeOps._eventCache)
    {
      if (!PythonTypeOps._eventCache.TryGetValue(tracker, out reflectedEvent))
        PythonTypeOps._eventCache[tracker] = !PythonBinder.IsExtendedType(tracker.DeclaringType) ? (reflectedEvent = new ReflectedEvent(tracker, false)) : (reflectedEvent = new ReflectedEvent(tracker, true));
    }
    return reflectedEvent;
  }

  internal static PythonTypeSlot GetFinalSlotForFunction(BuiltinFunction func)
  {
    if ((func.FunctionType & FunctionType.Method) != FunctionType.None)
    {
      lock (PythonTypeOps._methodCache)
      {
        BuiltinMethodDescriptor finalSlotForFunction;
        if (!PythonTypeOps._methodCache.TryGetValue(func, out finalSlotForFunction))
          PythonTypeOps._methodCache[func] = finalSlotForFunction = new BuiltinMethodDescriptor(func);
        return (PythonTypeSlot) finalSlotForFunction;
      }
    }
    if (!func.Targets[0].IsDefined(typeof (ClassMethodAttribute), true))
      return (PythonTypeSlot) func;
    lock (PythonTypeOps._classMethodCache)
    {
      ClassMethodDescriptor finalSlotForFunction;
      if (!PythonTypeOps._classMethodCache.TryGetValue(func, out finalSlotForFunction))
        PythonTypeOps._classMethodCache[func] = finalSlotForFunction = new ClassMethodDescriptor(func);
      return (PythonTypeSlot) finalSlotForFunction;
    }
  }

  internal static BuiltinFunction GetBuiltinFunction(Type type, string name, MemberInfo[] mems)
  {
    return PythonTypeOps.GetBuiltinFunction(type, name, new FunctionType?(), mems);
  }

  public static MethodBase[] GetNonBaseHelperMethodInfos(MemberInfo[] members)
  {
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MemberInfo member in members)
    {
      MethodBase methodBase = member as MethodBase;
      if (methodBase != (MethodBase) null && !methodBase.Name.StartsWith("#base#"))
        methodBaseList.Add(methodBase);
    }
    return methodBaseList.ToArray();
  }

  public static MemberInfo[] GetNonBaseHelperMemberInfos(MemberInfo[] members)
  {
    List<MemberInfo> memberInfoList = new List<MemberInfo>(members.Length);
    foreach (MemberInfo member in members)
    {
      MethodBase methodBase = member as MethodBase;
      if (methodBase == (MethodBase) null || !methodBase.Name.StartsWith("#base#"))
        memberInfoList.Add(member);
    }
    return memberInfoList.ToArray();
  }

  internal static BuiltinFunction GetBuiltinFunction(
    Type type,
    string name,
    FunctionType? funcType,
    params MemberInfo[] mems)
  {
    return PythonTypeOps.GetBuiltinFunction(type, name, name, funcType, mems);
  }

  internal static BuiltinFunction GetBuiltinFunction(
    Type type,
    string cacheName,
    string pythonName,
    FunctionType? funcType,
    params MemberInfo[] mems)
  {
    BuiltinFunction builtinFunction = (BuiltinFunction) null;
    if (mems.Length != 0)
    {
      FunctionType functionType = (FunctionType) ((int) funcType ?? (int) PythonTypeOps.GetMethodFunctionType(type, mems));
      type = PythonTypeOps.GetBaseDeclaringType(type, mems);
      PythonTypeOps.BuiltinFunctionKey key = new PythonTypeOps.BuiltinFunctionKey(type, new ReflectionCache.MethodBaseCache(cacheName, PythonTypeOps.GetNonBaseHelperMethodInfos(mems)), functionType);
      lock (PythonTypeOps._functions)
      {
        if (!PythonTypeOps._functions.TryGetValue(key, out builtinFunction))
        {
          if (PythonTypeOps.GetFinalSystemType(type) == type)
          {
            IList<MethodInfo> overriddenMethods = NewTypeMaker.GetOverriddenMethods(type, cacheName);
            if (overriddenMethods.Count > 0)
            {
              List<MemberInfo> memberInfoList = new List<MemberInfo>((IEnumerable<MemberInfo>) mems);
              foreach (MethodInfo methodInfo in (IEnumerable<MethodInfo>) overriddenMethods)
                memberInfoList.Add((MemberInfo) methodInfo);
              mems = memberInfoList.ToArray();
            }
          }
          PythonTypeOps._functions[key] = builtinFunction = BuiltinFunction.MakeMethod(pythonName, ReflectionUtils.GetMethodInfos(mems), type, functionType);
        }
      }
    }
    return builtinFunction;
  }

  private static Type GetCommonBaseType(Type xType, Type yType)
  {
    if (xType.IsSubclassOf(yType))
      return yType;
    if (yType.IsSubclassOf(xType) || xType == yType)
      return xType;
    Type baseType1 = xType.GetBaseType();
    Type baseType2 = yType.GetBaseType();
    if (baseType1 != (Type) null)
    {
      Type commonBaseType = PythonTypeOps.GetCommonBaseType(baseType1, yType);
      if (commonBaseType != (Type) null)
        return commonBaseType;
    }
    if (baseType2 != (Type) null)
    {
      Type commonBaseType = PythonTypeOps.GetCommonBaseType(xType, baseType2);
      if (commonBaseType != (Type) null)
        return commonBaseType;
    }
    return (Type) null;
  }

  private static Type GetBaseDeclaringType(Type type, MemberInfo[] mems)
  {
    Array.Sort<MemberInfo>(mems, (Comparison<MemberInfo>) ((x, y) =>
    {
      if (x.DeclaringType.IsSubclassOf(y.DeclaringType))
        return -1;
      if (y.DeclaringType.IsSubclassOf(x.DeclaringType))
        return 1;
      if (x.DeclaringType == y.DeclaringType)
        return 0;
      Type type1 = PythonTypeOps.GetCommonBaseType(x.DeclaringType, y.DeclaringType);
      if ((object) type1 == null)
        type1 = typeof (object);
      type = type1;
      if (x.DeclaringType.FullName == null)
        return -1;
      return y.DeclaringType.FullName == null ? 1 : x.DeclaringType.FullName.CompareTo(y.DeclaringType.FullName);
    }));
    foreach (MemberInfo mem in mems)
    {
      if (mem.DeclaringType.IsAssignableFrom(type) && (type == mem.DeclaringType || type.IsSubclassOf(mem.DeclaringType)))
      {
        type = mem.DeclaringType;
        break;
      }
    }
    return type;
  }

  internal static ConstructorFunction GetConstructor(
    Type type,
    BuiltinFunction realTarget,
    params MethodBase[] mems)
  {
    ConstructorFunction constructor = (ConstructorFunction) null;
    if (mems.Length != 0)
    {
      ReflectionCache.MethodBaseCache key = new ReflectionCache.MethodBaseCache("__new__", mems);
      lock (PythonTypeOps._ctors)
      {
        if (!PythonTypeOps._ctors.TryGetValue(key, out constructor))
          PythonTypeOps._ctors[key] = constructor = new ConstructorFunction(realTarget, (IList<MethodBase>) mems);
      }
    }
    return constructor;
  }

  internal static FunctionType GetMethodFunctionType(Type type, MemberInfo[] methods)
  {
    return PythonTypeOps.GetMethodFunctionType(type, methods, true);
  }

  internal static FunctionType GetMethodFunctionType(
    Type type,
    MemberInfo[] methods,
    bool checkStatic)
  {
    FunctionType methodFunctionType = FunctionType.None;
    foreach (MethodInfo method in methods)
    {
      if (method.IsStatic && method.IsSpecialName)
      {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length == 2 && parameters[0].ParameterType != typeof (CodeContext) || parameters.Length == 3 && parameters[0].ParameterType == typeof (CodeContext))
        {
          methodFunctionType |= FunctionType.BinaryOperator;
          if (parameters[parameters.Length - 2].ParameterType != type && parameters[parameters.Length - 1].ParameterType == type)
            methodFunctionType |= FunctionType.ReversedOperator;
        }
      }
      if (checkStatic && PythonTypeOps.IsStaticFunction(type, method))
        methodFunctionType |= FunctionType.Function;
      else
        methodFunctionType |= FunctionType.Method;
    }
    if (PythonTypeOps.IsMethodAlwaysVisible(type, methods))
      methodFunctionType |= FunctionType.AlwaysVisible;
    return methodFunctionType;
  }

  private static bool IsMethodAlwaysVisible(Type type, MemberInfo[] methods)
  {
    bool flag = true;
    if (PythonBinder.IsPythonType(type))
    {
      foreach (MethodInfo method in methods)
      {
        if (PythonBinder.IsExtendedType(method.DeclaringType) || PythonBinder.IsExtendedType(method.GetBaseDefinition().DeclaringType) || PythonHiddenAttribute.IsHidden((MemberInfo) method))
        {
          flag = false;
          break;
        }
      }
    }
    else if (typeof (IPythonObject).IsAssignableFrom(type))
    {
      foreach (MemberInfo method in methods)
      {
        if (PythonBinder.IsExtendedType(method.DeclaringType))
        {
          flag = false;
          break;
        }
      }
    }
    return flag;
  }

  private static bool IsStaticFunction(Type type, MethodInfo mi)
  {
    if (!mi.IsStatic || mi.IsDefined(typeof (WrapperDescriptorAttribute), false))
      return false;
    return mi.DeclaringType.IsAssignableFrom(type) || mi.IsDefined(typeof (StaticExtensionMethodAttribute), false);
  }

  internal static PythonTypeSlot GetReflectedField(FieldInfo info)
  {
    NameType nameType = NameType.Field;
    if (!PythonBinder.IsExtendedType(info.DeclaringType) && !PythonHiddenAttribute.IsHidden((MemberInfo) info))
      nameType |= NameType.PythonField;
    PythonTypeSlot reflectedField;
    lock (PythonTypeOps._fieldCache)
    {
      if (!PythonTypeOps._fieldCache.TryGetValue(info, out reflectedField))
      {
        reflectedField = nameType != NameType.PythonField || !info.IsLiteral ? (PythonTypeSlot) new ReflectedField(info, nameType) : (!(info.FieldType == typeof (int)) ? (!(info.FieldType == typeof (bool)) ? (PythonTypeSlot) new PythonTypeUserDescriptorSlot(info.GetValue((object) null), true) : (PythonTypeSlot) new PythonTypeUserDescriptorSlot(ScriptingRuntimeHelpers.BooleanToObject((bool) info.GetRawConstantValue()), true)) : (PythonTypeSlot) new PythonTypeUserDescriptorSlot(ScriptingRuntimeHelpers.Int32ToObject((int) info.GetRawConstantValue()), true));
        PythonTypeOps._fieldCache[info] = reflectedField;
      }
    }
    return reflectedField;
  }

  internal static string GetDocumentation(Type type)
  {
    object[] customAttributes = type.GetCustomAttributes(typeof (DocumentationAttribute), false);
    if (customAttributes != null && ((IEnumerable<object>) customAttributes).Any<object>())
      return ((DocumentationAttribute) ((IEnumerable<object>) customAttributes).First<object>()).Documentation;
    if (type == typeof (DynamicNull))
      return (string) null;
    string autoDoc = DocBuilder.CreateAutoDoc(type);
    string documentation = autoDoc != null ? autoDoc + Environment.NewLine + Environment.NewLine : string.Empty;
    foreach (ConstructorInfo constructor in type.GetConstructors())
      documentation = documentation + PythonTypeOps.FixCtorDoc(type, DocBuilder.CreateAutoDoc((MethodBase) constructor, DynamicHelpers.GetPythonTypeFromType(type).Name, 0)) + Environment.NewLine;
    return documentation;
  }

  private static string FixCtorDoc(Type type, string autoDoc)
  {
    return autoDoc.Replace("__new__(cls)", DynamicHelpers.GetPythonTypeFromType(type).Name + "()").Replace("__new__(cls, ", DynamicHelpers.GetPythonTypeFromType(type).Name + "(");
  }

  internal static ReflectedGetterSetter GetReflectedProperty(
    PropertyTracker pt,
    MemberGroup allProperties,
    bool privateBinding)
  {
    lock (PythonTypeOps._propertyCache)
    {
      ReflectedGetterSetter reflectedProperty1;
      if (PythonTypeOps._propertyCache.TryGetValue(pt, out reflectedProperty1))
        return reflectedProperty1;
      NameType nt = NameType.PythonProperty;
      MethodInfo m1 = PythonTypeOps.FilterProtectedGetterOrSetter(pt.GetGetMethod(true), privateBinding);
      MethodInfo m2 = PythonTypeOps.FilterProtectedGetterOrSetter(pt.GetSetMethod(true), privateBinding);
      if (m1 != (MethodInfo) null && PythonHiddenAttribute.IsHidden((MemberInfo) m1, true) || m2 != (MethodInfo) null && PythonHiddenAttribute.IsHidden((MemberInfo) m2, true))
        nt = NameType.Property;
      ReflectedGetterSetter reflectedProperty2;
      if (!(pt is ExtensionPropertyTracker))
      {
        ReflectedPropertyTracker reflectedPropertyTracker = pt as ReflectedPropertyTracker;
        if (PythonBinder.IsExtendedType(pt.DeclaringType) || PythonHiddenAttribute.IsHidden((MemberInfo) reflectedPropertyTracker.Property, true))
          nt = NameType.Property;
        if (pt.GetIndexParameters().Length == 0)
        {
          List<MethodInfo> methodInfoList1 = new List<MethodInfo>();
          List<MethodInfo> methodInfoList2 = new List<MethodInfo>();
          MethodInfo methodInfo = m1;
          if ((object) methodInfo == null)
            methodInfo = m2;
          foreach (ExtensionPropertyTracker overriddenProperty in (IEnumerable<ExtensionPropertyTracker>) NewTypeMaker.GetOverriddenProperties(methodInfo.DeclaringType, pt.Name))
          {
            MethodInfo getMethod = overriddenProperty.GetGetMethod(privateBinding);
            if (getMethod != (MethodInfo) null)
              methodInfoList1.Add(getMethod);
            MethodInfo setMethod = overriddenProperty.GetSetMethod(privateBinding);
            if (setMethod != (MethodInfo) null)
              methodInfoList2.Add(setMethod);
          }
          foreach (PropertyTracker allProperty in allProperties)
          {
            MethodInfo getMethod = allProperty.GetGetMethod(privateBinding);
            if (getMethod != (MethodInfo) null)
              methodInfoList1.Add(getMethod);
            MethodInfo setMethod = allProperty.GetSetMethod(privateBinding);
            if (setMethod != (MethodInfo) null)
              methodInfoList2.Add(setMethod);
          }
          reflectedProperty2 = (ReflectedGetterSetter) new ReflectedProperty(reflectedPropertyTracker.Property, methodInfoList1.ToArray(), methodInfoList2.ToArray(), nt);
        }
        else
          reflectedProperty2 = (ReflectedGetterSetter) new ReflectedIndexer(((ReflectedPropertyTracker) pt).Property, NameType.Property, privateBinding);
      }
      else
      {
        Type declaringType = pt.DeclaringType;
        MethodInfo mi = m1;
        if ((object) mi == null)
          mi = m2;
        reflectedProperty2 = (ReflectedGetterSetter) new ReflectedExtensionProperty(new ExtensionPropertyInfo(declaringType, mi), nt);
      }
      PythonTypeOps._propertyCache[pt] = reflectedProperty2;
      return reflectedProperty2;
    }
  }

  private static MethodInfo FilterProtectedGetterOrSetter(MethodInfo info, bool privateBinding)
  {
    return info != (MethodInfo) null && (privateBinding || info.IsPublic || info.IsProtected()) ? info : (MethodInfo) null;
  }

  internal static bool TryInvokeUnaryOperator(
    CodeContext context,
    object o,
    string name,
    out object value)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(o);
    PythonTypeSlot slot;
    object func;
    if (pythonType.TryResolveMixedSlot(context, name, out slot) && slot.TryGetValue(context, o, pythonType, out func))
    {
      value = PythonCalls.Call(context, func);
      return true;
    }
    value = (object) null;
    return false;
  }

  internal static bool TryInvokeBinaryOperator(
    CodeContext context,
    object o,
    object arg1,
    string name,
    out object value)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(o);
    PythonTypeSlot slot;
    object func;
    if (pythonType.TryResolveMixedSlot(context, name, out slot) && slot.TryGetValue(context, o, pythonType, out func))
    {
      value = PythonCalls.Call(context, func, arg1);
      return true;
    }
    value = (object) null;
    return false;
  }

  internal static bool TryInvokeTernaryOperator(
    CodeContext context,
    object o,
    object arg1,
    object arg2,
    string name,
    out object value)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(o);
    PythonTypeSlot slot;
    object func;
    if (pythonType.TryResolveMixedSlot(context, name, out slot) && slot.TryGetValue(context, o, pythonType, out func))
    {
      value = PythonCalls.Call(context, func, arg1, arg2);
      return true;
    }
    value = (object) null;
    return false;
  }

  internal static PythonTuple EnsureBaseType(PythonTuple bases)
  {
    bool flag = false;
    foreach (object obj in bases)
    {
      if (!(obj is OldClass))
      {
        if (!(obj as PythonType).UnderlyingSystemType.IsInterface())
          return bases;
        flag = true;
      }
    }
    if (flag || bases.Count == 0)
      return new PythonTuple(bases, (object) TypeCache.Object);
    throw PythonOps.TypeError("a new-style class can't have only classic bases");
  }

  internal static Type GetFinalSystemType(Type type)
  {
    while (typeof (IPythonObject).IsAssignableFrom(type) && !type.IsDefined(typeof (DynamicBaseTypeAttribute), false))
      type = type.GetBaseType();
    return type;
  }

  internal struct BuiltinFunctionKey(
    Type declaringType,
    ReflectionCache.MethodBaseCache cache,
    FunctionType funcType)
  {
    private Type DeclaringType = declaringType;
    private ReflectionCache.MethodBaseCache Cache = cache;
    private FunctionType FunctionType = funcType;
  }
}
