// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.NewTypeMaker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

internal sealed class NewTypeMaker
{
  private Type _baseType;
  private IList<Type> _interfaceTypes;
  private TypeBuilder _tg;
  private FieldInfo _typeField;
  private FieldInfo _dictField;
  private FieldInfo _slotsField;
  private FieldInfo _explicitMO;
  private ILGen _cctor;
  private int _site;
  private static int _typeCount;
  public const string VtableNamesField = "#VTableNames#";
  public const string TypePrefix = "IronPython.NewTypes.";
  public const string BaseMethodPrefix = "#base#";
  public const string FieldGetterPrefix = "#field_get#";
  public const string FieldSetterPrefix = "#field_set#";
  public const string ClassFieldName = ".class";
  public const string DictFieldName = ".dict";
  public const string SlotsAndWeakRefFieldName = ".slots_and_weakref";
  private const string _constructorTypeName = "PythonCachedTypeConstructor";
  private const string _constructorMethodName = "GetTypeInfo";
  internal static readonly Publisher<NewTypeInfo, Type> _newTypes = new Publisher<NewTypeInfo, Type>();
  private static readonly Dictionary<Type, Dictionary<string, List<MethodInfo>>> _overriddenMethods = new Dictionary<Type, Dictionary<string, List<MethodInfo>>>();
  private static readonly Dictionary<Type, Dictionary<string, List<ExtensionPropertyTracker>>> _overriddenProperties = new Dictionary<Type, Dictionary<string, List<ExtensionPropertyTracker>>>();
  private const MethodAttributes MethodAttributesReservedMask = MethodAttributes.ReservedMask;
  private const MethodAttributes MethodAttributesToEraseInOveride = MethodAttributes.ReservedMask | MethodAttributes.Abstract;

  private NewTypeMaker(NewTypeInfo typeInfo)
  {
    this._baseType = typeInfo.BaseType;
    this._interfaceTypes = typeInfo.InterfaceTypes;
  }

  public static Type GetNewType(string typeName, PythonTuple bases)
  {
    NewTypeInfo typeInfo = NewTypeInfo.GetTypeInfo(typeName, bases);
    if (typeInfo.BaseType.IsValueType())
      throw PythonOps.TypeError("cannot derive from {0} because it is a value type", (object) typeInfo.BaseType.FullName);
    if (typeInfo.BaseType.IsSealed())
      throw PythonOps.TypeError("cannot derive from {0} because it is sealed", (object) typeInfo.BaseType.FullName);
    return NewTypeMaker._newTypes.GetOrCreateValue(typeInfo, (Func<Type>) (() => typeInfo.InterfaceTypes.Count == 0 && ((IEnumerable<object>) typeInfo.BaseType.GetCustomAttributes(typeof (DynamicBaseTypeAttribute), false)).Any<object>() ? typeInfo.BaseType : new NewTypeMaker(typeInfo).CreateNewType()));
  }

  public static void SaveNewTypes(string assemblyName, IList<PythonTuple> types)
  {
    AssemblyGen ag = new AssemblyGen(new AssemblyName(assemblyName), ".", ".dll", false);
    TypeBuilder typeBuilder = ag.DefinePublicType("PythonCachedTypeConstructor", typeof (object), true);
    typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (PythonCachedTypeInfoAttribute).GetConstructor(ReflectionUtils.EmptyTypes), new object[0]));
    ILGenerator ilGenerator = typeBuilder.DefineMethod("GetTypeInfo", MethodAttributes.Public | MethodAttributes.Static, typeof (CachedNewTypeInfo[]), ReflectionUtils.EmptyTypes).GetILGenerator();
    NewTypeMaker.EmitInt(ilGenerator, types.Count);
    ilGenerator.Emit(OpCodes.Newarr, typeof (CachedNewTypeInfo));
    int num = 0;
    foreach (PythonTuple type in (IEnumerable<PythonTuple>) types)
    {
      NewTypeInfo typeInfo = NewTypeInfo.GetTypeInfo(string.Empty, type);
      KeyValuePair<Type, Dictionary<string, string[]>> keyValuePair1 = new NewTypeMaker(typeInfo).SaveType(ag, $"Python{(object) NewTypeMaker._typeCount++}${typeInfo.BaseType.Name}");
      ilGenerator.Emit(OpCodes.Dup);
      NewTypeMaker.EmitInt(ilGenerator, num++);
      ilGenerator.Emit(OpCodes.Ldtoken, keyValuePair1.Key);
      ilGenerator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetTypeFromHandle"));
      ilGenerator.Emit(OpCodes.Newobj, typeof (Dictionary<string, string[]>).GetConstructor(new Type[0]));
      foreach (KeyValuePair<string, string[]> keyValuePair2 in keyValuePair1.Value)
      {
        ilGenerator.Emit(OpCodes.Dup);
        ilGenerator.Emit(OpCodes.Ldstr, keyValuePair2.Key);
        int length = keyValuePair2.Value.Length;
        NewTypeMaker.EmitInt(ilGenerator, length);
        ilGenerator.Emit(OpCodes.Newarr, typeof (string));
        for (int iVal = 0; iVal < keyValuePair2.Value.Length; ++iVal)
        {
          ilGenerator.Emit(OpCodes.Dup);
          NewTypeMaker.EmitInt(ilGenerator, iVal);
          ilGenerator.Emit(OpCodes.Ldstr, keyValuePair2.Value[0]);
          ilGenerator.Emit(OpCodes.Stelem_Ref);
        }
        ilGenerator.Emit(OpCodes.Call, typeof (Dictionary<string, string[]>).GetMethod("set_Item"));
      }
      if (typeInfo.InterfaceTypes.Count != 0)
      {
        NewTypeMaker.EmitInt(ilGenerator, typeInfo.InterfaceTypes.Count);
        ilGenerator.Emit(OpCodes.Newarr, typeof (Type));
        for (int index = 0; index < typeInfo.InterfaceTypes.Count; ++index)
        {
          ilGenerator.Emit(OpCodes.Dup);
          NewTypeMaker.EmitInt(ilGenerator, index);
          ilGenerator.Emit(OpCodes.Ldtoken, typeInfo.InterfaceTypes[index]);
          ilGenerator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetTypeFromHandle"));
          ilGenerator.Emit(OpCodes.Stelem_Ref);
        }
      }
      else
        ilGenerator.Emit(OpCodes.Ldnull);
      ilGenerator.Emit(OpCodes.Newobj, typeof (CachedNewTypeInfo).GetConstructors()[0]);
      ilGenerator.Emit(OpCodes.Stelem_Ref);
    }
    ilGenerator.Emit(OpCodes.Ret);
    typeBuilder.CreateType();
    ag.SaveAssembly();
  }

  public static void LoadNewTypes(Assembly asm)
  {
    Type type = asm.GetType("PythonCachedTypeConstructor");
    if (type == (Type) null || !type.IsDefined(typeof (PythonCachedTypeInfoAttribute), false))
      return;
    foreach (CachedNewTypeInfo cachedNewTypeInfo in (CachedNewTypeInfo[]) type.GetMethod("GetTypeInfo").Invoke((object) null, new object[0]))
    {
      CachedNewTypeInfo v = cachedNewTypeInfo;
      NewTypeMaker._newTypes.GetOrCreateValue(new NewTypeInfo(v.Type.GetBaseType(), v.InterfaceTypes), (Func<Type>) (() =>
      {
        NewTypeMaker.AddBaseMethods(v.Type, v.SpecialNames);
        return v.Type;
      }));
    }
  }

  public static bool IsInstanceType(Type type)
  {
    if (type.FullName.IndexOf("IronPython.NewTypes.") == 0)
      return true;
    return type.GetBaseType() != (Type) null && NewTypeMaker.IsInstanceType(type.GetBaseType());
  }

  private Type CreateNewType()
  {
    string name = this.GetName();
    this._tg = Snippets.Shared.DefinePublicType("IronPython.NewTypes." + name, this._baseType);
    Dictionary<string, string[]> specialNames = this.ImplementType();
    Type finishedType = this.FinishType();
    NewTypeMaker.AddBaseMethods(finishedType, specialNames);
    return finishedType;
  }

  private string GetName()
  {
    StringBuilder stringBuilder = new StringBuilder(this._baseType.Namespace);
    stringBuilder.Append('.');
    stringBuilder.Append(this._baseType.Name);
    foreach (Type interfaceType in (IEnumerable<Type>) this._interfaceTypes)
    {
      stringBuilder.Append("#");
      stringBuilder.Append(interfaceType.Name);
    }
    stringBuilder.Append("_");
    stringBuilder.Append(Interlocked.Increment(ref NewTypeMaker._typeCount));
    return stringBuilder.ToString();
  }

  private Dictionary<string, string[]> ImplementType()
  {
    this.DefineInterfaces();
    this.ImplementPythonObject();
    this.ImplementConstructors();
    Dictionary<string, string[]> specialNames = new Dictionary<string, string[]>();
    this.OverrideMethods(this._baseType, specialNames);
    this.ImplementProtectedFieldAccessors(specialNames);
    Dictionary<Type, bool> doneTypes = new Dictionary<Type, bool>();
    foreach (Type interfaceType in (IEnumerable<Type>) this._interfaceTypes)
      this.DoInterfaceType(interfaceType, doneTypes, specialNames);
    return specialNames;
  }

  private void DefineInterfaces()
  {
    foreach (Type interfaceType in (IEnumerable<Type>) this._interfaceTypes)
      this.ImplementInterface(interfaceType);
  }

  private void ImplementInterface(Type interfaceType)
  {
    this._tg.AddInterfaceImplementation(interfaceType);
  }

  private void ImplementPythonObject()
  {
    this.ImplementIPythonObject();
    this.ImplementDynamicObject();
    this.ImplementCustomTypeDescriptor();
    this.ImplementWeakReference();
    this.AddDebugView();
  }

  private void AddDebugView()
  {
    this._tg.SetCustomAttribute(new CustomAttributeBuilder(typeof (DebuggerTypeProxyAttribute).GetConstructor(new Type[1]
    {
      typeof (Type)
    }), new object[1]{ (object) typeof (UserTypeDebugView) }));
    this._tg.SetCustomAttribute(new CustomAttributeBuilder(typeof (DebuggerDisplayAttribute).GetConstructor(new Type[1]
    {
      typeof (string)
    }), new object[1]
    {
      (object) "{get_PythonType().GetTypeDebuggerDisplay()}"
    }));
  }

  private void EmitGetDict(ILGen gen) => gen.EmitFieldGet(this._dictField);

  private void EmitSetDict(ILGen gen) => gen.EmitFieldSet(this._dictField);

  private ParameterInfoWrapper[] GetOverrideCtorSignature(ParameterInfo[] originalParameterInfo)
  {
    ParameterInfoWrapper[] array = ((IEnumerable<ParameterInfo>) originalParameterInfo).Select<ParameterInfo, ParameterInfoWrapper>((Func<ParameterInfo, ParameterInfoWrapper>) (o => new ParameterInfoWrapper(o))).ToArray<ParameterInfoWrapper>();
    if (typeof (IPythonObject).IsAssignableFrom(this._baseType))
      return array;
    ParameterInfoWrapper[] destinationArray = new ParameterInfoWrapper[array.Length + 1];
    if (array.Length == 0 || array[0].ParameterType != typeof (CodeContext))
    {
      destinationArray[0] = new ParameterInfoWrapper(typeof (PythonType), "cls");
      Array.Copy((Array) array, 0, (Array) destinationArray, 1, destinationArray.Length - 1);
    }
    else
    {
      destinationArray[0] = array[0];
      destinationArray[1] = new ParameterInfoWrapper(typeof (PythonType), "cls");
      Array.Copy((Array) array, 1, (Array) destinationArray, 2, destinationArray.Length - 2);
    }
    return destinationArray;
  }

  private void ImplementConstructors()
  {
    foreach (ConstructorInfo constructor in this._baseType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      if (constructor.IsPublic || constructor.IsProtected())
        this.OverrideConstructor(constructor);
    }
  }

  private static bool CanOverrideMethod(MethodInfo mi) => true;

  private void DoInterfaceType(
    Type interfaceType,
    Dictionary<Type, bool> doneTypes,
    Dictionary<string, string[]> specialNames)
  {
    if (interfaceType == typeof (IDynamicMetaObjectProvider) || doneTypes.ContainsKey(interfaceType))
      return;
    doneTypes.Add(interfaceType, true);
    this.OverrideMethods(interfaceType, specialNames);
    foreach (Type interfaceType1 in interfaceType.GetInterfaces())
      this.DoInterfaceType(interfaceType1, doneTypes, specialNames);
  }

  private void OverrideConstructor(ConstructorInfo parentConstructor)
  {
    ParameterInfo[] parameters = parentConstructor.GetParameters();
    if (parameters.Length == 0 && typeof (IPythonObject).IsAssignableFrom(this._baseType))
      return;
    ParameterInfoWrapper[] overrideCtorSignature = this.GetOverrideCtorSignature(parameters);
    Type[] parameterTypes = new Type[overrideCtorSignature.Length];
    string[] strArray = new string[overrideCtorSignature.Length];
    for (int index = 0; index < overrideCtorSignature.Length; ++index)
    {
      parameterTypes[index] = overrideCtorSignature[index].ParameterType;
      strArray[index] = overrideCtorSignature[index].Name;
    }
    ConstructorBuilder constructorBuilder = this._tg.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);
    for (int i = 0; i < overrideCtorSignature.Length; ++i)
    {
      ParameterBuilder parameterBuilder = constructorBuilder.DefineParameter(i + 1, overrideCtorSignature[i].Attributes, overrideCtorSignature[i].Name);
      int originalIndex = NewTypeMaker.GetOriginalIndex(parameters, overrideCtorSignature, i);
      if (originalIndex >= 0)
      {
        ParameterInfo parameterInfo = parameters[originalIndex];
        if (parameterInfo.IsDefined(typeof (ParamArrayAttribute), false))
          parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (ParamArrayAttribute).GetConstructor(ReflectionUtils.EmptyTypes), ArrayUtils.EmptyObjects));
        else if (parameterInfo.IsDefined(typeof (ParamDictionaryAttribute), false))
          parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (ParamDictionaryAttribute).GetConstructor(ReflectionUtils.EmptyTypes), ArrayUtils.EmptyObjects));
        else if (parameterInfo.IsDefined(typeof (BytesConversionAttribute), false))
          parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (BytesConversionAttribute).GetConstructor(ReflectionUtils.EmptyTypes), ArrayUtils.EmptyObjects));
        else if (parameterInfo.IsDefined(typeof (BytesConversionNoStringAttribute), false))
          parameterBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (BytesConversionNoStringAttribute).GetConstructor(ReflectionUtils.EmptyTypes), ArrayUtils.EmptyObjects));
        if ((parameterInfo.Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.None)
        {
          if (parameterInfo.DefaultValue == null || parameterInfo.ParameterType.IsAssignableFrom(parameterInfo.DefaultValue.GetType()))
            parameterBuilder.SetConstant(parameterInfo.DefaultValue);
          else
            parameterBuilder.SetConstant(Convert.ChangeType(parameterInfo.DefaultValue, parameterInfo.ParameterType, (IFormatProvider) CultureInfo.CurrentCulture));
        }
      }
    }
    ILGen il = new ILGen(constructorBuilder.GetILGenerator());
    int index1 = parameters.Length == 0 || parameters[0].ParameterType != typeof (CodeContext) ? 1 : 2;
    if (!typeof (IPythonObject).IsAssignableFrom(this._baseType))
    {
      il.EmitLoadArg(0);
      il.EmitLoadArg(index1);
      il.EmitFieldSet(this._typeField);
    }
    if (this._explicitMO != (FieldInfo) null)
    {
      il.Emit(OpCodes.Ldarg_0);
      il.EmitNew(this._explicitMO.FieldType.GetConstructor(ReflectionUtils.EmptyTypes));
      il.Emit(OpCodes.Stfld, this._explicitMO);
    }
    MethodInfo method = typeof (PythonOps).GetMethod("InitializeUserTypeSlots");
    il.EmitLoadArg(0);
    il.EmitLoadArg(index1);
    il.EmitCall(method);
    il.EmitFieldSet(this._slotsField);
    NewTypeMaker.CallBaseConstructor(parentConstructor, parameters, overrideCtorSignature, il);
  }

  private static int GetOriginalIndex(
    ParameterInfo[] pis,
    ParameterInfoWrapper[] overrideParams,
    int i)
  {
    if (pis.Length == 0 || pis[0].ParameterType != typeof (CodeContext))
      return i - (overrideParams.Length - pis.Length);
    if (i == 1)
      return -1;
    return i == 0 ? 0 : i - (overrideParams.Length - pis.Length);
  }

  private static void CallBaseConstructor(
    ConstructorInfo parentConstructor,
    ParameterInfo[] pis,
    ParameterInfoWrapper[] overrideParams,
    ILGen il)
  {
    il.EmitLoadArg(0);
    for (int i = 0; i < overrideParams.Length; ++i)
    {
      if (NewTypeMaker.GetOriginalIndex(pis, overrideParams, i) >= 0)
        il.EmitLoadArg(i + 1);
    }
    il.Emit(OpCodes.Call, parentConstructor);
    il.Emit(OpCodes.Ret);
  }

  private ILGen GetCCtor()
  {
    if (this._cctor == null)
      this._cctor = new ILGen(this._tg.DefineTypeInitializer().GetILGenerator());
    return this._cctor;
  }

  private void ImplementCustomTypeDescriptor()
  {
    this.ImplementInterface(typeof (ICustomTypeDescriptor));
    foreach (MethodInfo method in typeof (ICustomTypeDescriptor).GetMethods())
      this.ImplementCTDOverride(method);
  }

  private void ImplementCTDOverride(MethodInfo m)
  {
    MethodBuilder builder;
    ILGen ilGen = this.DefineExplicitInterfaceImplementation(m, out builder);
    ilGen.EmitLoadArg(0);
    ParameterInfo[] parameters = m.GetParameters();
    Type[] paramTypes = new Type[parameters.Length + 1];
    paramTypes[0] = typeof (object);
    for (int index = 0; index < parameters.Length; ++index)
    {
      ilGen.EmitLoadArg(index + 1);
      paramTypes[index + 1] = parameters[index].ParameterType;
    }
    ilGen.EmitCall(typeof (CustomTypeDescHelpers), m.Name, paramTypes);
    ilGen.EmitBoxing(m.ReturnType);
    ilGen.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) builder, m);
  }

  private bool NeedsPythonObject => !typeof (IPythonObject).IsAssignableFrom(this._baseType);

  private void ImplementDynamicObject()
  {
    bool flag1 = false;
    foreach (Type interfaceType in (IEnumerable<Type>) this._interfaceTypes)
    {
      if (interfaceType == typeof (IDynamicMetaObjectProvider))
      {
        flag1 = true;
        break;
      }
    }
    bool flag2 = typeof (IDynamicMetaObjectProvider).IsAssignableFrom(this._baseType);
    if (flag2 && this._baseType.GetInterfaceMap(typeof (IDynamicMetaObjectProvider)).TargetMethods[0].IsPrivate)
    {
      if (this._baseType.IsDefined(typeof (DynamicBaseTypeAttribute), true))
        return;
      flag2 = false;
    }
    this.ImplementInterface(typeof (IDynamicMetaObjectProvider));
    MethodInfo decl;
    MethodBuilder impl;
    ILGen il = this.DefineMethodOverride(MethodAttributes.Private, typeof (IDynamicMetaObjectProvider), "GetMetaObject", out decl, out impl);
    MethodInfo method = typeof (UserTypeOps).GetMethod("GetMetaObjectHelper");
    LocalBuilder localBuilder = il.DeclareLocal(typeof (DynamicMetaObject));
    Label label1 = il.DefineLabel();
    if (flag1)
    {
      this._explicitMO = (FieldInfo) this._tg.DefineField("__gettingMO", typeof (Microsoft.Scripting.Utils.ThreadLocal<bool>), FieldAttributes.Private | FieldAttributes.InitOnly);
      Label label2 = il.DefineLabel();
      Label label3 = il.DefineLabel();
      Label label4 = il.DefineLabel();
      PropertyInfo declaredProperty = typeof (Microsoft.Scripting.Utils.ThreadLocal<bool>).GetDeclaredProperty("Value");
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Ldfld, this._explicitMO);
      il.EmitPropertyGet(declaredProperty);
      il.Emit(OpCodes.Brtrue, label2);
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Ldfld, this._explicitMO);
      il.Emit(OpCodes.Ldc_I4_1);
      il.EmitPropertySet(declaredProperty);
      il.BeginExceptionBlock();
      LocalBuilder callTarget = this.EmitNonInheritedMethodLookup("GetMetaObject", il);
      il.Emit(OpCodes.Brfalse, label3);
      this.EmitClrCallStub(il, typeof (IDynamicMetaObjectProvider).GetMethod("GetMetaObject"), callTarget);
      il.Emit(OpCodes.Dup);
      il.Emit(OpCodes.Ldnull);
      il.Emit(OpCodes.Beq, label4);
      il.Emit(OpCodes.Stloc_S, localBuilder.LocalIndex);
      il.Emit(OpCodes.Leave, label1);
      il.MarkLabel(label4);
      il.Emit(OpCodes.Pop);
      il.MarkLabel(label3);
      il.BeginFinallyBlock();
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Ldfld, this._explicitMO);
      il.Emit(OpCodes.Ldc_I4_0);
      il.EmitPropertySet(typeof (Microsoft.Scripting.Utils.ThreadLocal<bool>).GetDeclaredProperty("Value"));
      il.EndExceptionBlock();
      il.MarkLabel(label2);
    }
    il.EmitLoadArg(0);
    il.EmitLoadArg(1);
    if (flag2)
    {
      InterfaceMapping interfaceMap = this._baseType.GetInterfaceMap(typeof (IDynamicMetaObjectProvider));
      il.EmitLoadArg(0);
      il.EmitLoadArg(1);
      il.EmitCall(interfaceMap.TargetMethods[0]);
    }
    else
      il.EmitNull();
    il.EmitCall(method);
    il.Emit(OpCodes.Stloc, localBuilder.LocalIndex);
    il.MarkLabel(label1);
    il.Emit(OpCodes.Ldloc, localBuilder.LocalIndex);
    il.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, decl);
  }

  private void ImplementIPythonObject()
  {
    MethodInfo decl;
    MethodBuilder impl;
    if (this.NeedsPythonObject)
    {
      this._typeField = (FieldInfo) this._tg.DefineField(".class", typeof (PythonType), FieldAttributes.Public);
      this._dictField = (FieldInfo) this._tg.DefineField(".dict", typeof (PythonDictionary), FieldAttributes.Public);
      this.ImplementInterface(typeof (IPythonObject));
      MethodAttributes extra = MethodAttributes.Private;
      ILGen gen1 = this.DefineMethodOverride(extra, typeof (IPythonObject), "get_Dict", out decl, out impl);
      gen1.EmitLoadArg(0);
      this.EmitGetDict(gen1);
      gen1.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) impl, decl);
      ILGen gen2 = this.DefineMethodOverride(extra, typeof (IPythonObject), "ReplaceDict", out decl, out impl);
      gen2.EmitLoadArg(0);
      gen2.EmitLoadArg(1);
      this.EmitSetDict(gen2);
      gen2.EmitBoolean(true);
      gen2.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) impl, decl);
      ILGen ilGen1 = this.DefineMethodOverride(extra, typeof (IPythonObject), "SetDict", out decl, out impl);
      ilGen1.EmitLoadArg(0);
      ilGen1.EmitFieldAddress(this._dictField);
      ilGen1.EmitLoadArg(1);
      ilGen1.EmitCall(typeof (UserTypeOps), "SetDictHelper");
      ilGen1.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) impl, decl);
      ILGen ilGen2 = this.DefineMethodOverride(extra, typeof (IPythonObject), "get_PythonType", out decl, out impl);
      ilGen2.EmitLoadArg(0);
      ilGen2.EmitFieldGet(this._typeField);
      ilGen2.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) impl, decl);
      ILGen ilGen3 = this.DefineMethodOverride(extra, typeof (IPythonObject), "SetPythonType", out decl, out impl);
      ilGen3.EmitLoadArg(0);
      ilGen3.EmitLoadArg(1);
      ilGen3.EmitFieldSet(this._typeField);
      ilGen3.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) impl, decl);
    }
    this._slotsField = (FieldInfo) this._tg.DefineField(".slots_and_weakref", typeof (object[]), FieldAttributes.Public);
    ILGen ilGen4 = this.DefineMethodOverride(MethodAttributes.Private, typeof (IPythonObject), "GetSlots", out decl, out impl);
    ilGen4.EmitLoadArg(0);
    ilGen4.EmitFieldGet(this._slotsField);
    ilGen4.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, decl);
    ILGen ilGen5 = this.DefineMethodOverride(MethodAttributes.Private, typeof (IPythonObject), "GetSlotsCreate", out decl, out impl);
    ilGen5.EmitLoadArg(0);
    ilGen5.EmitLoadArg(0);
    ilGen5.EmitFieldAddress(this._slotsField);
    ilGen5.EmitCall(typeof (UserTypeOps).GetMethod("GetSlotsCreate"));
    ilGen5.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, decl);
  }

  private void DefineHelperInterface(Type intf)
  {
    this.ImplementInterface(intf);
    foreach (MethodInfo method1 in intf.GetMethods())
    {
      MethodBuilder builder;
      ILGen ilGen = this.DefineExplicitInterfaceImplementation(method1, out builder);
      ParameterInfo[] parameters = method1.GetParameters();
      MethodInfo method2 = typeof (UserTypeOps).GetMethod(method1.Name + "Helper");
      int num = 0;
      if (parameters.Length != 0 && parameters[0].ParameterType == typeof (CodeContext))
      {
        num = 1;
        ilGen.EmitLoadArg(1);
      }
      ilGen.EmitLoadArg(0);
      for (int index = num; index < parameters.Length; ++index)
        ilGen.EmitLoadArg(index + 1);
      ilGen.EmitCall(method2);
      ilGen.Emit(OpCodes.Ret);
      this._tg.DefineMethodOverride((MethodInfo) builder, method1);
    }
  }

  private void ImplementWeakReference()
  {
    if (typeof (IWeakReferenceable).IsAssignableFrom(this._baseType))
      return;
    this.DefineHelperInterface(typeof (IWeakReferenceable));
  }

  private void ImplementProtectedFieldAccessors(Dictionary<string, string[]> specialNames)
  {
    foreach (FieldInfo inheritedField in this._baseType.GetInheritedFields(flattenHierarchy: true))
    {
      if (inheritedField.IsProtected())
      {
        List<string> stringList = new List<string>();
        PropertyBuilder propertyBuilder = this._tg.DefineProperty(inheritedField.Name, PropertyAttributes.None, inheritedField.FieldType, ReflectionUtils.EmptyTypes);
        MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
        if (inheritedField.IsStatic)
          attributes |= MethodAttributes.Static;
        MethodBuilder mdBuilder1 = this._tg.DefineMethod("#field_get#" + inheritedField.Name, attributes, inheritedField.FieldType, ReflectionUtils.EmptyTypes);
        ILGen ilGen1 = new ILGen(mdBuilder1.GetILGenerator());
        if (!inheritedField.IsStatic)
          ilGen1.EmitLoadArg(0);
        if (inheritedField.IsLiteral)
        {
          object rawConstantValue = inheritedField.GetRawConstantValue();
          switch (inheritedField.FieldType.GetTypeCode())
          {
            case TypeCode.Boolean:
              if ((bool) rawConstantValue)
              {
                ilGen1.Emit(OpCodes.Ldc_I4_1);
                break;
              }
              ilGen1.Emit(OpCodes.Ldc_I4_0);
              break;
            case TypeCode.Char:
              ilGen1.Emit(OpCodes.Ldc_I4, (int) (char) rawConstantValue);
              break;
            case TypeCode.SByte:
              ilGen1.Emit(OpCodes.Ldc_I4, (sbyte) rawConstantValue);
              break;
            case TypeCode.Byte:
              ilGen1.Emit(OpCodes.Ldc_I4, (byte) rawConstantValue);
              break;
            case TypeCode.Int16:
              ilGen1.Emit(OpCodes.Ldc_I4, (short) rawConstantValue);
              break;
            case TypeCode.UInt16:
              ilGen1.Emit(OpCodes.Ldc_I4, (int) (ushort) rawConstantValue);
              break;
            case TypeCode.Int32:
              ilGen1.Emit(OpCodes.Ldc_I4, (int) rawConstantValue);
              break;
            case TypeCode.UInt32:
              ilGen1.Emit(OpCodes.Ldc_I4, (long) (uint) rawConstantValue);
              break;
            case TypeCode.Int64:
              ilGen1.Emit(OpCodes.Ldc_I8, (long) rawConstantValue);
              break;
            case TypeCode.UInt64:
              ilGen1.Emit(OpCodes.Ldc_I8, (float) (ulong) rawConstantValue);
              break;
            case TypeCode.Single:
              ilGen1.Emit(OpCodes.Ldc_R4, (float) rawConstantValue);
              break;
            case TypeCode.Double:
              ilGen1.Emit(OpCodes.Ldc_R8, (double) rawConstantValue);
              break;
            case TypeCode.String:
              ilGen1.Emit(OpCodes.Ldstr, (string) rawConstantValue);
              break;
          }
        }
        else
          ilGen1.EmitFieldGet(inheritedField);
        ilGen1.Emit(OpCodes.Ret);
        propertyBuilder.SetGetMethod(mdBuilder1);
        stringList.Add(mdBuilder1.Name);
        if (!inheritedField.IsLiteral && !inheritedField.IsInitOnly)
        {
          MethodBuilder mdBuilder2 = this._tg.DefineMethod("#field_set#" + inheritedField.Name, attributes, (Type) null, new Type[1]
          {
            inheritedField.FieldType
          });
          mdBuilder2.DefineParameter(1, ParameterAttributes.None, "value");
          ILGen ilGen2 = new ILGen(mdBuilder2.GetILGenerator());
          ilGen2.EmitLoadArg(0);
          if (!inheritedField.IsStatic)
            ilGen2.EmitLoadArg(1);
          ilGen2.EmitFieldSet(inheritedField);
          ilGen2.Emit(OpCodes.Ret);
          propertyBuilder.SetSetMethod(mdBuilder2);
          stringList.Add(mdBuilder2.Name);
        }
        specialNames[inheritedField.Name] = stringList.ToArray();
      }
    }
  }

  private void OverrideMethods(Type type, Dictionary<string, string[]> specialNames)
  {
    Dictionary<KeyValuePair<string, MethodSignatureInfo>, MethodInfo> dictionary = new Dictionary<KeyValuePair<string, MethodSignatureInfo>, MethodInfo>();
    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
    {
      KeyValuePair<string, MethodSignatureInfo> key = new KeyValuePair<string, MethodSignatureInfo>(method.Name, new MethodSignatureInfo(method));
      MethodInfo methodInfo;
      if (!dictionary.TryGetValue(key, out methodInfo))
        dictionary[key] = method;
      else if (methodInfo.DeclaringType.IsAssignableFrom(method.DeclaringType))
        dictionary[key] = method;
    }
    if (type.IsAbstract() && !type.IsInterface())
    {
      foreach (Type interfaceType in type.GetInterfaces())
      {
        InterfaceMapping interfaceMap = type.GetInterfaceMap(interfaceType);
        for (int index = 0; index < interfaceMap.TargetMethods.Length; ++index)
        {
          if (interfaceMap.TargetMethods[index] == (MethodInfo) null)
          {
            MethodInfo interfaceMethod = interfaceMap.InterfaceMethods[index];
            KeyValuePair<string, MethodSignatureInfo> key = new KeyValuePair<string, MethodSignatureInfo>(interfaceMethod.Name, new MethodSignatureInfo(interfaceMethod));
            dictionary[key] = interfaceMethod;
          }
        }
      }
    }
    Dictionary<PropertyInfo, PropertyBuilder> overridden = new Dictionary<PropertyInfo, PropertyBuilder>();
    foreach (MethodInfo methodInfo in dictionary.Values)
    {
      if (NewTypeMaker.CanOverrideMethod(methodInfo) && (methodInfo.IsPublic || methodInfo.IsProtected()))
      {
        if (methodInfo.IsSpecialName)
          this.OverrideSpecialName(methodInfo, specialNames, overridden);
        else
          this.OverrideBaseMethod(methodInfo, specialNames);
      }
    }
  }

  private void OverrideSpecialName(
    MethodInfo mi,
    Dictionary<string, string[]> specialNames,
    Dictionary<PropertyInfo, PropertyBuilder> overridden)
  {
    if (!mi.IsVirtual || mi.IsFinal)
    {
      if (!mi.IsProtected() && !mi.IsSpecialName || !mi.Name.StartsWith("get_") && !mi.Name.StartsWith("set_"))
        return;
      specialNames[mi.Name] = new string[1]{ mi.Name };
      MethodBuilder superCallHelper = this.CreateSuperCallHelper(mi);
      foreach (PropertyInfo property in mi.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (property.GetGetMethod(true).MemberEquals((MemberInfo) mi) || property.GetSetMethod(true).MemberEquals((MemberInfo) mi))
        {
          this.AddPublicProperty(mi, overridden, superCallHelper, property);
          break;
        }
      }
    }
    else
    {
      if (this.TryOverrideProperty(mi, specialNames, overridden))
        return;
      foreach (EventInfo ei in mi.DeclaringType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        string name;
        if (ei.GetAddMethod().MemberEquals((MemberInfo) mi))
        {
          if (NameConverter.TryGetName(DynamicHelpers.GetPythonTypeFromType(mi.DeclaringType), ei, mi, out name) == NameType.None)
            return;
          this.CreateVTableEventOverride(mi, mi.Name);
          return;
        }
        if (ei.GetRemoveMethod().MemberEquals((MemberInfo) mi))
        {
          if (NameConverter.TryGetName(DynamicHelpers.GetPythonTypeFromType(mi.DeclaringType), ei, mi, out name) == NameType.None)
            return;
          this.CreateVTableEventOverride(mi, mi.Name);
          return;
        }
      }
      this.OverrideBaseMethod(mi, specialNames);
    }
  }

  private bool TryOverrideProperty(
    MethodInfo mi,
    Dictionary<string, string[]> specialNames,
    Dictionary<PropertyInfo, PropertyBuilder> overridden)
  {
    PropertyInfo[] properties = mi.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    specialNames[mi.Name] = new string[1]{ mi.Name };
    MethodBuilder mb = (MethodBuilder) null;
    PropertyInfo foundProperty = (PropertyInfo) null;
    foreach (PropertyInfo pi in properties)
    {
      if (pi.GetIndexParameters().Length != 0)
      {
        if (mi.MemberEquals((MemberInfo) pi.GetGetMethod(true)))
        {
          mb = this.CreateVTableMethodOverride(mi, "__getitem__");
          if (!mi.IsAbstract)
            this.CreateSuperCallHelper(mi);
          foundProperty = pi;
          break;
        }
        if (mi.MemberEquals((MemberInfo) pi.GetSetMethod(true)))
        {
          mb = this.CreateVTableMethodOverride(mi, "__setitem__");
          if (!mi.IsAbstract)
            this.CreateSuperCallHelper(mi);
          foundProperty = pi;
          break;
        }
      }
      else
      {
        if (mi.MemberEquals((MemberInfo) pi.GetGetMethod(true)))
        {
          if (mi.Name != "get_PythonType")
          {
            string name;
            if (NameConverter.TryGetName(this.GetBaseTypeForMethod(mi), pi, mi, out name) == NameType.None)
              return true;
            mb = this.CreateVTableGetterOverride(mi, name);
            if (!mi.IsAbstract)
              this.CreateSuperCallHelper(mi);
          }
          foundProperty = pi;
          break;
        }
        if (mi.MemberEquals((MemberInfo) pi.GetSetMethod(true)))
        {
          string name;
          if (NameConverter.TryGetName(this.GetBaseTypeForMethod(mi), pi, mi, out name) == NameType.None)
            return true;
          mb = this.CreateVTableSetterOverride(mi, name);
          if (!mi.IsAbstract)
            this.CreateSuperCallHelper(mi);
          foundProperty = pi;
          break;
        }
      }
    }
    if (!(foundProperty != (PropertyInfo) null))
      return false;
    this.AddPublicProperty(mi, overridden, mb, foundProperty);
    return true;
  }

  private void AddPublicProperty(
    MethodInfo mi,
    Dictionary<PropertyInfo, PropertyBuilder> overridden,
    MethodBuilder mb,
    PropertyInfo foundProperty)
  {
    MethodInfo getMethod = foundProperty.GetGetMethod(true);
    MethodInfo setMethod = foundProperty.GetSetMethod(true);
    if ((!(getMethod != (MethodInfo) null) || !getMethod.IsProtected()) && (!(setMethod != (MethodInfo) null) || !setMethod.IsProtected()))
      return;
    PropertyBuilder propertyBuilder;
    if (!overridden.TryGetValue(foundProperty, out propertyBuilder))
    {
      ParameterInfo[] indexParameters = foundProperty.GetIndexParameters();
      Type[] parameterTypes = new Type[indexParameters.Length];
      for (int index = 0; index < parameterTypes.Length; ++index)
        parameterTypes[index] = indexParameters[index].ParameterType;
      overridden[foundProperty] = propertyBuilder = this._tg.DefineProperty(foundProperty.Name, foundProperty.Attributes, foundProperty.PropertyType, parameterTypes);
    }
    if (foundProperty.GetGetMethod(true).MemberEquals((MemberInfo) mi))
    {
      propertyBuilder.SetGetMethod(mb);
    }
    else
    {
      if (!foundProperty.GetSetMethod(true).MemberEquals((MemberInfo) mi))
        return;
      propertyBuilder.SetSetMethod(mb);
    }
  }

  private static void EmitBaseMethodDispatch(MethodInfo mi, ILGen il)
  {
    if (!mi.IsAbstract)
    {
      int num = 0;
      if (!mi.IsStatic)
      {
        il.EmitLoadArg(0);
        num = 1;
      }
      ParameterInfo[] parameters = mi.GetParameters();
      for (int index = 0; index < parameters.Length; ++index)
        il.EmitLoadArg(index + num);
      il.EmitCall(OpCodes.Call, mi, (Type[]) null);
      il.Emit(OpCodes.Ret);
    }
    else
    {
      il.EmitLoadArg(0);
      il.EmitString(mi.Name);
      il.EmitCall(typeof (PythonOps), "MissingInvokeMethodException");
      il.Emit(OpCodes.Throw);
    }
  }

  private void OverrideBaseMethod(MethodInfo mi, Dictionary<string, string[]> specialNames)
  {
    if ((!mi.IsVirtual || mi.IsFinal) && !mi.IsProtected())
      return;
    PythonType baseTypeForMethod = this.GetBaseTypeForMethod(mi);
    string name = (string) null;
    MethodInfo mi1 = mi;
    ref string local = ref name;
    if (NameConverter.TryGetName(baseTypeForMethod, mi1, out local) == NameType.None || mi.DeclaringType == typeof (object) && mi.Name == "Finalize")
      return;
    specialNames[mi.Name] = new string[1]{ mi.Name };
    if (!mi.IsStatic)
      this.CreateVTableMethodOverride(mi, name);
    if (mi.IsAbstract)
      return;
    this.CreateSuperCallHelper(mi);
  }

  private PythonType GetBaseTypeForMethod(MethodInfo mi)
  {
    return this._baseType == mi.DeclaringType || this._baseType.IsSubclassOf(mi.DeclaringType) ? DynamicHelpers.GetPythonTypeFromType(this._baseType) : DynamicHelpers.GetPythonTypeFromType(mi.DeclaringType);
  }

  private LocalBuilder EmitBaseClassCallCheckForProperties(
    ILGen il,
    MethodInfo baseMethod,
    string name)
  {
    Label label1 = il.DefineLabel();
    LocalBuilder local = il.DeclareLocal(typeof (object));
    il.EmitLoadArg(0);
    il.EmitString(name);
    il.Emit(OpCodes.Ldloca, local);
    il.EmitCall(typeof (UserTypeOps), "TryGetNonInheritedValueHelper");
    il.Emit(OpCodes.Brtrue, label1);
    LocalBuilder callTarget = this.EmitNonInheritedMethodLookup(baseMethod.Name, il);
    Label label2 = il.DefineLabel();
    il.Emit(OpCodes.Brtrue, label2);
    NewTypeMaker.EmitBaseMethodDispatch(baseMethod, il);
    il.MarkLabel(label2);
    this.EmitClrCallStub(il, baseMethod, callTarget);
    il.Emit(OpCodes.Ret);
    il.MarkLabel(label1);
    return local;
  }

  private MethodBuilder CreateVTableGetterOverride(MethodInfo mi, string name)
  {
    MethodBuilder impl;
    ILGen il;
    this.DefineVTableMethodOverride(mi, out impl, out il);
    LocalBuilder local = this.EmitBaseClassCallCheckForProperties(il, mi, name);
    il.Emit(OpCodes.Ldloc, local);
    il.EmitLoadArg(0);
    il.EmitString(name);
    il.EmitCall(typeof (UserTypeOps), "GetPropertyHelper");
    if (!il.TryEmitImplicitCast(typeof (object), mi.ReturnType))
      NewTypeMaker.EmitConvertFromObject(il, mi.ReturnType);
    il.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, mi);
    return impl;
  }

  private static void EmitConvertFromObject(ILGen il, Type toType)
  {
    if (toType == typeof (object))
      return;
    if (toType.IsGenericParameter)
    {
      il.EmitCall(typeof (PythonOps).GetMethod("ConvertFromObject").MakeGenericMethod(toType));
    }
    else
    {
      MethodInfo fastConvertMethod = PythonBinder.GetFastConvertMethod(toType);
      if (fastConvertMethod != (MethodInfo) null)
        il.EmitCall(fastConvertMethod);
      else if (toType == typeof (void))
        il.Emit(OpCodes.Pop);
      else if (typeof (Delegate).IsAssignableFrom(toType))
      {
        il.EmitType(toType);
        il.EmitCall(typeof (Converter), "ConvertToDelegate");
        il.Emit(OpCodes.Castclass, toType);
      }
      else
      {
        Label label = il.DefineLabel();
        il.Emit(OpCodes.Dup);
        il.Emit(OpCodes.Isinst, toType);
        il.Emit(OpCodes.Brtrue_S, label);
        il.Emit(OpCodes.Ldtoken, toType);
        il.EmitCall(PythonBinder.GetGenericConvertMethod(toType));
        il.MarkLabel(label);
        il.Emit(OpCodes.Unbox_Any, toType);
      }
    }
  }

  private MethodBuilder CreateVTableSetterOverride(MethodInfo mi, string name)
  {
    MethodBuilder impl;
    ILGen il;
    this.DefineVTableMethodOverride(mi, out impl, out il);
    LocalBuilder local = this.EmitBaseClassCallCheckForProperties(il, mi, name);
    il.Emit(OpCodes.Ldloc, local);
    il.EmitLoadArg(0);
    il.EmitLoadArg(1);
    il.EmitBoxing(mi.GetParameters()[0].ParameterType);
    il.EmitString(name);
    il.EmitCall(typeof (UserTypeOps), "SetPropertyHelper");
    il.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, mi);
    return impl;
  }

  private void CreateVTableEventOverride(MethodInfo mi, string name)
  {
    MethodBuilder impl;
    ILGen il = this.DefineMethodOverride(mi, out impl);
    LocalBuilder local = NewTypeMaker.EmitBaseClassCallCheckForEvents(il, mi, name);
    il.Emit(OpCodes.Ldloc, local);
    il.EmitLoadArg(0);
    il.EmitLoadArg(1);
    il.EmitBoxing(mi.GetParameters()[0].ParameterType);
    il.EmitString(name);
    il.EmitCall(typeof (UserTypeOps), "AddRemoveEventHelper");
    il.Emit(OpCodes.Ret);
    this._tg.DefineMethodOverride((MethodInfo) impl, mi);
  }

  private static LocalBuilder EmitBaseClassCallCheckForEvents(
    ILGen il,
    MethodInfo baseMethod,
    string name)
  {
    Label label = il.DefineLabel();
    LocalBuilder local = il.DeclareLocal(typeof (object));
    il.EmitLoadArg(0);
    il.EmitString(name);
    il.Emit(OpCodes.Ldloca, local);
    il.EmitCall(typeof (UserTypeOps), "TryGetNonInheritedValueHelper");
    il.Emit(OpCodes.Brtrue, label);
    NewTypeMaker.EmitBaseMethodDispatch(baseMethod, il);
    il.MarkLabel(label);
    return local;
  }

  private MethodBuilder CreateVTableMethodOverride(MethodInfo mi, string name)
  {
    MethodBuilder impl;
    ILGen il;
    this.DefineVTableMethodOverride(mi, out impl, out il);
    LocalBuilder callTarget = this.EmitNonInheritedMethodLookup(name, il);
    Label label = il.DefineLabel();
    il.Emit(OpCodes.Brtrue, label);
    NewTypeMaker.EmitBaseMethodDispatch(mi, il);
    il.MarkLabel(label);
    this.EmitClrCallStub(il, mi, callTarget);
    il.Emit(OpCodes.Ret);
    if (mi.IsVirtual && !mi.IsFinal)
      this._tg.DefineMethodOverride((MethodInfo) impl, mi);
    return impl;
  }

  private void DefineVTableMethodOverride(MethodInfo mi, out MethodBuilder impl, out ILGen il)
  {
    if (mi.IsVirtual && !mi.IsFinal)
    {
      il = this.DefineMethodOverride(MethodAttributes.Public, mi, out impl);
    }
    else
    {
      impl = this._tg.DefineMethod(mi.Name, mi.IsVirtual ? mi.Attributes | MethodAttributes.VtableLayoutMask : mi.Attributes & ~MethodAttributes.MemberAccessMask | MethodAttributes.Public, mi.CallingConvention);
      ReflectionUtils.CopyMethodSignature(mi, impl, false);
      il = new ILGen(impl.GetILGenerator());
    }
  }

  private LocalBuilder EmitNonInheritedMethodLookup(string name, ILGen il)
  {
    LocalBuilder local = il.DeclareLocal(typeof (object));
    il.EmitLoadArg(0);
    if (typeof (IPythonObject).IsAssignableFrom(this._baseType))
      il.EmitPropertyGet(PythonTypeInfo._IPythonObject.PythonType);
    else
      il.EmitFieldGet(this._typeField);
    il.EmitLoadArg(0);
    il.EmitString(name);
    il.Emit(OpCodes.Ldloca, local);
    il.EmitCall(typeof (UserTypeOps), "TryGetNonInheritedMethodHelper");
    return local;
  }

  private MethodBuilder CreateSuperCallHelper(MethodInfo mi)
  {
    MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
    if (mi.IsStatic)
      attributes |= MethodAttributes.Static;
    MethodBuilder to = this._tg.DefineMethod("#base#" + mi.Name, attributes, mi.CallingConvention);
    ReflectionUtils.CopyMethodSignature(mi, to, true);
    NewTypeMaker.EmitBaseMethodDispatch(mi, new ILGen(to.GetILGenerator()));
    return to;
  }

  private KeyValuePair<Type, Dictionary<string, string[]>> SaveType(AssemblyGen ag, string name)
  {
    this._tg = ag.DefinePublicType("IronPython.NewTypes." + name, this._baseType, true);
    Dictionary<string, string[]> dictionary = this.ImplementType();
    return new KeyValuePair<Type, Dictionary<string, string[]>>(this.FinishType(), dictionary);
  }

  private Type FinishType()
  {
    if (this._cctor != null)
      this._cctor.Emit(OpCodes.Ret);
    return this._tg.CreateType();
  }

  private ILGen DefineExplicitInterfaceImplementation(
    MethodInfo baseMethod,
    out MethodBuilder builder)
  {
    MethodAttributes attributes = baseMethod.Attributes & ~(MethodAttributes.Public | MethodAttributes.Abstract) | MethodAttributes.Final | MethodAttributes.VtableLayoutMask;
    Type[] parameterTypes = ReflectionUtils.GetParameterTypes(baseMethod.GetParameters());
    builder = this._tg.DefineMethod($"{baseMethod.DeclaringType.Name}.{baseMethod.Name}", attributes, baseMethod.ReturnType, parameterTypes);
    return new ILGen(builder.GetILGenerator());
  }

  private ILGen DefineMethodOverride(
    MethodAttributes extra,
    Type type,
    string name,
    out MethodInfo decl,
    out MethodBuilder impl)
  {
    decl = type.GetMethod(name);
    return this.DefineMethodOverride(extra, decl, out impl);
  }

  private ILGen DefineMethodOverride(MethodInfo decl, out MethodBuilder impl)
  {
    return this.DefineMethodOverride(MethodAttributes.PrivateScope, decl, out impl);
  }

  private ILGen DefineMethodOverride(
    MethodAttributes extra,
    MethodInfo decl,
    out MethodBuilder impl)
  {
    impl = ReflectionUtils.DefineMethodOverride(this._tg, extra, decl);
    return new ILGen(impl.GetILGenerator());
  }

  private void EmitClrCallStub(ILGen il, MethodInfo mi, LocalBuilder callTarget)
  {
    int num1 = 0;
    bool flag = false;
    bool context = false;
    ParameterInfo[] parameters = mi.GetParameters();
    if (parameters.Length != 0)
    {
      if (parameters[0].ParameterType == typeof (CodeContext))
      {
        num1 = 1;
        context = true;
      }
      if (parameters[parameters.Length - 1].IsDefined(typeof (ParamArrayAttribute), false))
        flag = true;
    }
    ParameterInfo[] parameterInfoArray = parameters;
    int num2 = parameterInfoArray.Length - num1;
    Type[] genericArguments = mi.GetGenericArguments();
    ILGen cctor = this.GetCCtor();
    if (flag || genericArguments.Length != 0)
    {
      cctor.EmitInt(num2);
      cctor.EmitBoolean(flag);
      cctor.EmitInt(genericArguments.Length);
      cctor.Emit(OpCodes.Newarr, typeof (string));
      for (int index = 0; index < genericArguments.Length; ++index)
      {
        cctor.Emit(OpCodes.Dup);
        cctor.EmitInt(index);
        cctor.Emit(OpCodes.Ldelema, typeof (string));
        cctor.Emit(OpCodes.Ldstr, genericArguments[index].Name);
        cctor.Emit(OpCodes.Stobj, typeof (string));
      }
      cctor.EmitCall(typeof (PythonOps).GetMethod("MakeComplexCallAction"));
    }
    else
    {
      cctor.EmitInt(num2);
      cctor.EmitCall(typeof (PythonOps).GetMethod("MakeSimpleCallAction"));
    }
    Type type = CompilerHelpers.MakeCallSiteType(NewTypeMaker.MakeSiteSignature(num2 + genericArguments.Length));
    FieldBuilder fi = this._tg.DefineField("site$" + (object) this._site++, type, FieldAttributes.Private | FieldAttributes.Static);
    cctor.EmitCall(type.GetMethod("Create"));
    cctor.EmitFieldSet((FieldInfo) fi);
    List<ReturnFixer> returnFixerList = new List<ReturnFixer>(0);
    il.EmitFieldGet((FieldInfo) fi);
    FieldInfo declaredField = type.GetDeclaredField("Target");
    il.EmitFieldGet(declaredField);
    il.EmitFieldGet((FieldInfo) fi);
    NewTypeMaker.EmitCodeContext(il, context);
    il.Emit(OpCodes.Ldloc, callTarget);
    for (int index = num1; index < parameterInfoArray.Length; ++index)
    {
      ReturnFixer returnFixer = ReturnFixer.EmitArgument(il, parameterInfoArray[index], index + 1);
      if (returnFixer != null)
        returnFixerList.Add(returnFixer);
    }
    for (int index = 0; index < genericArguments.Length; ++index)
    {
      il.EmitType(genericArguments[index]);
      il.EmitCall(typeof (DynamicHelpers).GetMethod("GetPythonTypeFromType"));
    }
    il.EmitCall(declaredField.FieldType, "Invoke");
    foreach (ReturnFixer returnFixer in returnFixerList)
      returnFixer.FixReturn(il);
    NewTypeMaker.EmitConvertFromObject(il, mi.ReturnType);
  }

  private static void EmitCodeContext(ILGen il, bool context)
  {
    if (context)
      il.EmitLoadArg(1);
    else
      il.EmitPropertyGet(typeof (DefaultContext).GetProperty("Default"));
  }

  private static void EmitInt(ILGenerator ilg, int iVal)
  {
    switch (iVal)
    {
      case 0:
        ilg.Emit(OpCodes.Ldc_I4_0);
        break;
      case 1:
        ilg.Emit(OpCodes.Ldc_I4_1);
        break;
      case 2:
        ilg.Emit(OpCodes.Ldc_I4_2);
        break;
      case 3:
        ilg.Emit(OpCodes.Ldc_I4_3);
        break;
      case 4:
        ilg.Emit(OpCodes.Ldc_I4_4);
        break;
      case 5:
        ilg.Emit(OpCodes.Ldc_I4_5);
        break;
      case 6:
        ilg.Emit(OpCodes.Ldc_I4_6);
        break;
      case 7:
        ilg.Emit(OpCodes.Ldc_I4_7);
        break;
      case 8:
        ilg.Emit(OpCodes.Ldc_I4_8);
        break;
      default:
        ilg.Emit(OpCodes.Ldc_I4, iVal);
        break;
    }
  }

  private static Type[] MakeSiteSignature(int nargs)
  {
    Type[] typeArray = new Type[nargs + 4];
    typeArray[0] = typeof (CallSite);
    typeArray[1] = typeof (CodeContext);
    for (int index = 2; index < typeArray.Length; ++index)
      typeArray[index] = typeof (object);
    return typeArray;
  }

  private static void AddBaseMethods(Type finishedType, Dictionary<string, string[]> specialNames)
  {
    foreach (MethodInfo method in finishedType.GetMethods())
    {
      if (!NewTypeMaker.IsInstanceType(finishedType.BaseType) || !NewTypeMaker.IsInstanceType(method.DeclaringType))
      {
        string name = method.Name;
        if (name.StartsWith("#base#") || name.StartsWith("#field_get#") || name.StartsWith("#field_set#"))
        {
          foreach (string newName in NewTypeMaker.GetBaseName(method, specialNames))
          {
            if (method.IsSpecialName && (newName.StartsWith("get_") || newName.StartsWith("set_")))
            {
              string str = newName.Substring(4);
              MemberInfo[] defaultMembers = finishedType.BaseType.GetDefaultMembers();
              if (defaultMembers.Length != 0)
              {
                foreach (MemberInfo memberInfo in defaultMembers)
                {
                  if (memberInfo.Name == str)
                  {
                    NewTypeMaker.StoreOverriddenMethod(method, newName);
                    break;
                  }
                }
              }
              NewTypeMaker.StoreOverriddenProperty(method, newName);
            }
            else if (method.IsSpecialName && (newName.StartsWith("#field_get#") || newName.StartsWith("#field_set#")))
              NewTypeMaker.StoreOverriddenField(method, newName);
            else
              NewTypeMaker.StoreOverriddenMethod(method, newName);
          }
        }
      }
    }
  }

  private static void StoreOverriddenField(MethodInfo mi, string newName)
  {
    Type baseType = mi.DeclaringType.GetBaseType();
    string propName = newName.Substring("#field_get#".Length);
    lock (PythonTypeOps._propertyCache)
    {
      foreach (FieldInfo field in baseType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (field.Name == propName)
        {
          if (newName.StartsWith("#field_get#"))
            NewTypeMaker.AddPropertyInfo(field.DeclaringType, propName, mi, (MethodInfo) null);
          else if (newName.StartsWith("#field_set#"))
            NewTypeMaker.AddPropertyInfo(field.DeclaringType, propName, (MethodInfo) null, mi);
        }
      }
    }
  }

  private static ExtensionPropertyTracker AddPropertyInfo(
    Type baseType,
    string propName,
    MethodInfo get,
    MethodInfo set)
  {
    MethodInfo methodInfo = get;
    if ((object) methodInfo == null)
      methodInfo = set;
    MethodInfo getter1 = methodInfo;
    Dictionary<string, List<ExtensionPropertyTracker>> dictionary;
    if (!NewTypeMaker._overriddenProperties.TryGetValue(baseType, out dictionary))
      NewTypeMaker._overriddenProperties[baseType] = dictionary = new Dictionary<string, List<ExtensionPropertyTracker>>();
    List<ExtensionPropertyTracker> extensionPropertyTrackerList1;
    if (!dictionary.TryGetValue(propName, out extensionPropertyTrackerList1))
      dictionary[propName] = extensionPropertyTrackerList1 = new List<ExtensionPropertyTracker>();
    for (int index1 = 0; index1 < extensionPropertyTrackerList1.Count; ++index1)
    {
      if (extensionPropertyTrackerList1[index1].DeclaringType == getter1.DeclaringType)
      {
        List<ExtensionPropertyTracker> extensionPropertyTrackerList2 = extensionPropertyTrackerList1;
        int index2 = index1;
        string name = propName;
        MethodInfo getter2 = get;
        if ((object) getter2 == null)
          getter2 = extensionPropertyTrackerList1[index1].GetGetMethod();
        MethodInfo setter = set;
        if ((object) setter == null)
          setter = extensionPropertyTrackerList1[index1].GetSetMethod();
        Type declaringType = getter1.DeclaringType;
        ExtensionPropertyTracker extensionPropertyTracker1;
        ExtensionPropertyTracker extensionPropertyTracker2 = extensionPropertyTracker1 = new ExtensionPropertyTracker(name, getter2, setter, (MethodInfo) null, declaringType);
        extensionPropertyTrackerList2[index2] = extensionPropertyTracker1;
        return extensionPropertyTracker2;
      }
    }
    ExtensionPropertyTracker extensionPropertyTracker;
    extensionPropertyTrackerList1.Add(extensionPropertyTracker = new ExtensionPropertyTracker(propName, getter1, (MethodInfo) null, (MethodInfo) null, getter1.DeclaringType));
    return extensionPropertyTracker;
  }

  private static void StoreOverriddenMethod(MethodInfo mi, string newName)
  {
    Type declaringType = ((MethodInfo) mi.DeclaringType.GetBaseType().GetMember(newName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)[0]).GetBaseDefinition().DeclaringType;
    string key = newName;
    switch (newName)
    {
      case "get_Item":
        key = "__getitem__";
        break;
      case "set_Item":
        key = "__setitem__";
        break;
    }
    lock (PythonTypeOps._functions)
    {
      foreach (BuiltinFunction builtinFunction in PythonTypeOps._functions.Values)
      {
        if (builtinFunction.Name == key && builtinFunction.DeclaringType == declaringType)
        {
          builtinFunction.AddMethod(mi);
          break;
        }
      }
      Dictionary<string, List<MethodInfo>> dictionary;
      if (!NewTypeMaker._overriddenMethods.TryGetValue(declaringType, out dictionary))
        NewTypeMaker._overriddenMethods[declaringType] = dictionary = new Dictionary<string, List<MethodInfo>>();
      List<MethodInfo> methodInfoList;
      if (!dictionary.TryGetValue(key, out methodInfoList))
        dictionary[key] = methodInfoList = new List<MethodInfo>();
      methodInfoList.Add(mi);
    }
  }

  private static void StoreOverriddenProperty(MethodInfo mi, string newName)
  {
    Type baseType = mi.DeclaringType.GetBaseType();
    lock (PythonTypeOps._propertyCache)
    {
      string propName = newName.Substring(4);
      ExtensionPropertyTracker extensionPropertyTracker = (ExtensionPropertyTracker) null;
      foreach (PropertyInfo property in baseType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (property.Name == propName)
        {
          Type declaringType = property.DeclaringType;
          if (newName.StartsWith("get_"))
            extensionPropertyTracker = NewTypeMaker.AddPropertyInfo(declaringType, propName, mi, (MethodInfo) null);
          else if (newName.StartsWith("set_"))
            extensionPropertyTracker = NewTypeMaker.AddPropertyInfo(declaringType, propName, (MethodInfo) null, mi);
        }
      }
      if (extensionPropertyTracker == null)
        return;
      foreach (ReflectedGetterSetter reflectedGetterSetter in PythonTypeOps._propertyCache.Values)
      {
        if (!(reflectedGetterSetter.DeclaringType != baseType) && !(reflectedGetterSetter.__name__ != extensionPropertyTracker.Name))
        {
          if (extensionPropertyTracker.GetGetMethod(true) != (MethodInfo) null)
            reflectedGetterSetter.AddGetter(extensionPropertyTracker.GetGetMethod(true));
          if (extensionPropertyTracker.GetSetMethod(true) != (MethodInfo) null)
            reflectedGetterSetter.AddSetter(extensionPropertyTracker.GetSetMethod(true));
        }
      }
    }
  }

  private static IEnumerable<string> GetBaseName(
    MethodInfo mi,
    Dictionary<string, string[]> specialNames)
  {
    string key;
    if (mi.Name.StartsWith("#base#"))
      key = mi.Name.Substring("#base#".Length);
    else if (mi.Name.StartsWith("#field_get#"))
    {
      key = mi.Name.Substring("#field_get#".Length);
    }
    else
    {
      if (!mi.Name.StartsWith("#field_set#"))
        throw new InvalidOperationException();
      key = mi.Name.Substring("#field_set#".Length);
    }
    return (IEnumerable<string>) specialNames[key];
  }

  internal static IList<MethodInfo> GetOverriddenMethods(Type type, string name)
  {
    List<MethodInfo> methodInfoList1 = (List<MethodInfo>) null;
    for (Type type1 = type; type1 != (Type) null; type1 = type1.GetBaseType())
    {
      Dictionary<string, List<MethodInfo>> dictionary;
      List<MethodInfo> methodInfoList2;
      if (NewTypeMaker._overriddenMethods.TryGetValue(type1, out dictionary) && dictionary.TryGetValue(name, out methodInfoList2))
      {
        if (methodInfoList1 == null)
          methodInfoList1 = new List<MethodInfo>(methodInfoList2.Count);
        foreach (MethodInfo methodInfo in methodInfoList2)
        {
          if (type.IsAssignableFrom(methodInfo.DeclaringType))
            methodInfoList1.Add(methodInfo);
        }
      }
    }
    return (IList<MethodInfo>) methodInfoList1 ?? (IList<MethodInfo>) new MethodInfo[0];
  }

  internal static IList<ExtensionPropertyTracker> GetOverriddenProperties(Type type, string name)
  {
    lock (NewTypeMaker._overriddenProperties)
    {
      Dictionary<string, List<ExtensionPropertyTracker>> dictionary;
      if (NewTypeMaker._overriddenProperties.TryGetValue(type, out dictionary))
      {
        List<ExtensionPropertyTracker> overriddenProperties;
        if (dictionary.TryGetValue(name, out overriddenProperties))
          return (IList<ExtensionPropertyTracker>) overriddenProperties;
      }
    }
    return (IList<ExtensionPropertyTracker>) new ExtensionPropertyTracker[0];
  }
}
