// Decompiled with JetBrains decompiler
// Type: InteropDotNet.InteropRuntimeImplementer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace InteropDotNet;

internal static class InteropRuntimeImplementer
{
  public static T CreateInstance<T>() where T : class
  {
    Type interfaceType = typeof (T);
    if (!typeof (T).IsInterface)
      throw new Exception($"The type {interfaceType.Name} should be an interface");
    string str = interfaceType.IsPublic ? InteropRuntimeImplementer.GetAssemblyName(interfaceType) : throw new Exception($"The interface {interfaceType.Name} should be public");
    ModuleBuilder moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(str), AssemblyBuilderAccess.Run).DefineDynamicModule(str);
    string implementationTypeName = InteropRuntimeImplementer.GetImplementationTypeName(str, interfaceType);
    TypeBuilder typeBuilder = moduleBuilder.DefineType(implementationTypeName, TypeAttributes.Public, typeof (object), new Type[1]
    {
      interfaceType
    });
    InteropRuntimeImplementer.MethodItem[] methods = InteropRuntimeImplementer.BuildMethods(interfaceType);
    InteropRuntimeImplementer.ImplementDelegates(str, moduleBuilder, (IEnumerable<InteropRuntimeImplementer.MethodItem>) methods);
    InteropRuntimeImplementer.ImplementFields(typeBuilder, (IEnumerable<InteropRuntimeImplementer.MethodItem>) methods);
    InteropRuntimeImplementer.ImplementMethods(typeBuilder, (IEnumerable<InteropRuntimeImplementer.MethodItem>) methods);
    InteropRuntimeImplementer.ImplementConstructor(typeBuilder, methods);
    return (T) Activator.CreateInstance(typeBuilder.CreateType(), (object) LibraryLoader.Instance);
  }

  private static InteropRuntimeImplementer.MethodItem[] BuildMethods(Type interfaceType)
  {
    MethodInfo[] methods = interfaceType.GetMethods();
    InteropRuntimeImplementer.MethodItem[] methodItemArray = new InteropRuntimeImplementer.MethodItem[methods.Length];
    for (int index = 0; index < methods.Length; ++index)
    {
      methodItemArray[index] = new InteropRuntimeImplementer.MethodItem()
      {
        Info = methods[index]
      };
      methodItemArray[index].DllImportAttribute = InteropRuntimeImplementer.GetRuntimeDllImportAttribute(methods[index]) ?? throw new Exception($"Method '{methods[index].Name}' of interface '{interfaceType.Name}' should be marked with the RuntimeDllImport attribute");
    }
    return methodItemArray;
  }

  private static void ImplementDelegates(
    string assemblyName,
    ModuleBuilder moduleBuilder,
    IEnumerable<InteropRuntimeImplementer.MethodItem> methods)
  {
    foreach (InteropRuntimeImplementer.MethodItem method in methods)
      method.DelegateType = InteropRuntimeImplementer.ImplementMethodDelegate(assemblyName, moduleBuilder, method);
  }

  private static Type ImplementMethodDelegate(
    string assemblyName,
    ModuleBuilder moduleBuilder,
    InteropRuntimeImplementer.MethodItem method)
  {
    string delegateName = InteropRuntimeImplementer.GetDelegateName(assemblyName, method.Info);
    TypeBuilder typeBuilder = moduleBuilder.DefineType(delegateName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass, typeof (MulticastDelegate));
    RuntimeDllImportAttribute dllImportAttribute = method.DllImportAttribute;
    ConstructorInfo constructor = typeof (UnmanagedFunctionPointerAttribute).GetConstructor(new Type[1]
    {
      typeof (CallingConvention)
    });
    if (constructor == (ConstructorInfo) null)
      throw new Exception("There is no the target constructor of the UnmanagedFunctionPointerAttribute");
    CustomAttributeBuilder customBuilder = new CustomAttributeBuilder(constructor, new object[1]
    {
      (object) dllImportAttribute.CallingConvention
    }, new FieldInfo[4]
    {
      typeof (UnmanagedFunctionPointerAttribute).GetField("CharSet"),
      typeof (UnmanagedFunctionPointerAttribute).GetField("BestFitMapping"),
      typeof (UnmanagedFunctionPointerAttribute).GetField("ThrowOnUnmappableChar"),
      typeof (UnmanagedFunctionPointerAttribute).GetField("SetLastError")
    }, new object[4]
    {
      (object) dllImportAttribute.CharSet,
      (object) dllImportAttribute.BestFitMapping,
      (object) dllImportAttribute.ThrowOnUnmappableChar,
      (object) dllImportAttribute.SetLastError
    });
    typeBuilder.SetCustomAttribute(customBuilder);
    ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[2]
    {
      typeof (object),
      typeof (IntPtr)
    });
    constructorBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    constructorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "object");
    constructorBuilder.DefineParameter(2, ParameterAttributes.HasDefault, nameof (method));
    InteropRuntimeImplementer.LightParameterInfo[] parameterInfoArray1 = InteropRuntimeImplementer.GetParameterInfoArray(method.Info);
    InteropRuntimeImplementer.DefineMethod(typeBuilder, "Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, method.ReturnType, parameterInfoArray1).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    InteropRuntimeImplementer.LightParameterInfo[] parameterInfoArray2 = InteropRuntimeImplementer.GetParameterInfoArray(method.Info, InteropRuntimeImplementer.InfoArrayMode.BeginInvoke);
    InteropRuntimeImplementer.DefineMethod(typeBuilder, "BeginInvoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, typeof (IAsyncResult), parameterInfoArray2).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    InteropRuntimeImplementer.LightParameterInfo[] parameterInfoArray3 = InteropRuntimeImplementer.GetParameterInfoArray(method.Info, InteropRuntimeImplementer.InfoArrayMode.EndInvoke);
    InteropRuntimeImplementer.DefineMethod(typeBuilder, "EndInvoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, method.ReturnType, parameterInfoArray3).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    return typeBuilder.CreateType();
  }

  private static void ImplementFields(
    TypeBuilder typeBuilder,
    IEnumerable<InteropRuntimeImplementer.MethodItem> methods)
  {
    foreach (InteropRuntimeImplementer.MethodItem method in methods)
    {
      string fieldName = method.Info.Name + "Field";
      FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, method.DelegateType, FieldAttributes.Private);
      method.FieldInfo = (FieldInfo) fieldBuilder;
    }
  }

  private static void ImplementMethods(
    TypeBuilder typeBuilder,
    IEnumerable<InteropRuntimeImplementer.MethodItem> methods)
  {
    foreach (InteropRuntimeImplementer.MethodItem method in methods)
    {
      InteropRuntimeImplementer.LightParameterInfo[] parameterInfoArray = InteropRuntimeImplementer.GetParameterInfoArray(method.Info);
      MethodBuilder methodInfoBody = InteropRuntimeImplementer.DefineMethod(typeBuilder, method.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, method.ReturnType, parameterInfoArray);
      ILGenerator ilGenerator = methodInfoBody.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, method.FieldInfo);
      for (int index = 0; index < parameterInfoArray.Length; ++index)
        InteropRuntimeImplementer.LdArg(ilGenerator, index + 1);
      ilGenerator.Emit(OpCodes.Callvirt, method.DelegateType.GetMethod("Invoke"));
      ilGenerator.Emit(OpCodes.Ret);
      typeBuilder.DefineMethodOverride((MethodInfo) methodInfoBody, method.Info);
    }
  }

  private static void ImplementConstructor(
    TypeBuilder typeBuilder,
    InteropRuntimeImplementer.MethodItem[] methods)
  {
    ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[1]
    {
      typeof (LibraryLoader)
    });
    constructorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "loader");
    ConstructorInfo con = !(typeBuilder.BaseType == (Type) null) ? typeBuilder.BaseType.GetConstructor(new Type[0]) : throw new Exception("There is no a BaseType of typeBuilder");
    if (con == (ConstructorInfo) null)
      throw new Exception("There is no a default constructor of BaseType of typeBuilder");
    List<string> stringList = new List<string>();
    foreach (InteropRuntimeImplementer.MethodItem method in methods)
    {
      string libraryFileName = method.DllImportAttribute.LibraryFileName;
      if (!stringList.Contains(libraryFileName))
        stringList.Add(libraryFileName);
    }
    ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
    for (int index = 0; index < stringList.Count; ++index)
      ilGenerator.DeclareLocal(typeof (IntPtr));
    ilGenerator.DeclareLocal(typeof (IntPtr));
    ilGenerator.Emit(OpCodes.Ldarg_0);
    ilGenerator.Emit(OpCodes.Call, con);
    for (int index = 0; index < stringList.Count; ++index)
    {
      string str = stringList[index];
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Ldstr, str);
      ilGenerator.Emit(OpCodes.Ldnull);
      ilGenerator.Emit(OpCodes.Callvirt, typeof (LibraryLoader).GetMethod("LoadLibrary"));
      ilGenerator.Emit(OpCodes.Stloc, index);
    }
    foreach (InteropRuntimeImplementer.MethodItem method in methods)
    {
      int num = stringList.IndexOf(method.DllImportAttribute.LibraryFileName);
      string str = method.DllImportAttribute.EntryPoint ?? method.Info.Name;
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Ldloc, num);
      ilGenerator.Emit(OpCodes.Ldstr, str);
      ilGenerator.Emit(OpCodes.Callvirt, typeof (LibraryLoader).GetMethod("GetProcAddress"));
      ilGenerator.Emit(OpCodes.Stloc, stringList.Count);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldloc_1);
      ilGenerator.Emit(OpCodes.Ldtoken, method.DelegateType);
      ilGenerator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetTypeFromHandle"));
      ilGenerator.Emit(OpCodes.Call, typeof (Marshal).GetMethod("GetDelegateForFunctionPointer", new Type[2]
      {
        typeof (IntPtr),
        typeof (Type)
      }));
      ilGenerator.Emit(OpCodes.Castclass, method.DelegateType);
      ilGenerator.Emit(OpCodes.Stfld, method.FieldInfo);
    }
    ilGenerator.Emit(OpCodes.Ret);
  }

  private static RuntimeDllImportAttribute GetRuntimeDllImportAttribute(MethodInfo methodInfo)
  {
    object[] customAttributes = methodInfo.GetCustomAttributes(typeof (RuntimeDllImportAttribute), true);
    return customAttributes.Length != 0 ? (RuntimeDllImportAttribute) customAttributes[0] : throw new Exception($"RuntimeDllImportAttribute for method '{methodInfo.Name}' not found");
  }

  private static void LdArg(ILGenerator ilGen, int index)
  {
    switch (index)
    {
      case 0:
        ilGen.Emit(OpCodes.Ldarg_0);
        break;
      case 1:
        ilGen.Emit(OpCodes.Ldarg_1);
        break;
      case 2:
        ilGen.Emit(OpCodes.Ldarg_2);
        break;
      case 3:
        ilGen.Emit(OpCodes.Ldarg_3);
        break;
      default:
        ilGen.Emit(OpCodes.Ldarg, index);
        break;
    }
  }

  private static MethodBuilder DefineMethod(
    TypeBuilder typeBuilder,
    string name,
    MethodAttributes attributes,
    Type returnType,
    InteropRuntimeImplementer.LightParameterInfo[] infoArray)
  {
    MethodBuilder methodBuilder = typeBuilder.DefineMethod(name, attributes, returnType, InteropRuntimeImplementer.GetParameterTypeArray(infoArray));
    for (int index = 0; index < infoArray.Length; ++index)
      methodBuilder.DefineParameter(index + 1, infoArray[index].Attributes, infoArray[index].Name);
    return methodBuilder;
  }

  private static InteropRuntimeImplementer.LightParameterInfo[] GetParameterInfoArray(
    MethodInfo methodInfo,
    InteropRuntimeImplementer.InfoArrayMode mode = InteropRuntimeImplementer.InfoArrayMode.Invoke)
  {
    ParameterInfo[] parameters = methodInfo.GetParameters();
    List<InteropRuntimeImplementer.LightParameterInfo> lightParameterInfoList = new List<InteropRuntimeImplementer.LightParameterInfo>();
    for (int index = 0; index < parameters.Length; ++index)
    {
      if (mode != InteropRuntimeImplementer.InfoArrayMode.EndInvoke || parameters[index].ParameterType.IsByRef)
        lightParameterInfoList.Add(new InteropRuntimeImplementer.LightParameterInfo(parameters[index]));
    }
    if (mode == InteropRuntimeImplementer.InfoArrayMode.BeginInvoke)
    {
      lightParameterInfoList.Add(new InteropRuntimeImplementer.LightParameterInfo(typeof (AsyncCallback), "callback"));
      lightParameterInfoList.Add(new InteropRuntimeImplementer.LightParameterInfo(typeof (object), "object"));
    }
    if (mode == InteropRuntimeImplementer.InfoArrayMode.EndInvoke)
      lightParameterInfoList.Add(new InteropRuntimeImplementer.LightParameterInfo(typeof (IAsyncResult), "result"));
    InteropRuntimeImplementer.LightParameterInfo[] parameterInfoArray = new InteropRuntimeImplementer.LightParameterInfo[lightParameterInfoList.Count];
    for (int index = 0; index < lightParameterInfoList.Count; ++index)
      parameterInfoArray[index] = lightParameterInfoList[index];
    return parameterInfoArray;
  }

  private static Type[] GetParameterTypeArray(
    InteropRuntimeImplementer.LightParameterInfo[] infoArray)
  {
    Type[] parameterTypeArray = new Type[infoArray.Length];
    for (int index = 0; index < infoArray.Length; ++index)
      parameterTypeArray[index] = infoArray[index].Type;
    return parameterTypeArray;
  }

  private static string GetAssemblyName(Type interfaceType)
  {
    return $"InteropRuntimeImplementer.{InteropRuntimeImplementer.GetSubstantialName(interfaceType)}Instance";
  }

  private static string GetImplementationTypeName(string assemblyName, Type interfaceType)
  {
    return $"{assemblyName}.{InteropRuntimeImplementer.GetSubstantialName(interfaceType)}Implementation";
  }

  private static string GetSubstantialName(Type interfaceType)
  {
    string substantialName = interfaceType.Name;
    if (substantialName.StartsWith("I"))
      substantialName = substantialName.Substring(1);
    return substantialName;
  }

  private static string GetDelegateName(string assemblyName, MethodInfo methodInfo)
  {
    return $"{assemblyName}.{methodInfo.Name}Delegate";
  }

  private class MethodItem
  {
    public MethodInfo Info { get; set; }

    public RuntimeDllImportAttribute DllImportAttribute { get; set; }

    public Type DelegateType { get; set; }

    public FieldInfo FieldInfo { get; set; }

    public string Name => this.Info.Name;

    public Type ReturnType => this.Info.ReturnType;
  }

  private class LightParameterInfo
  {
    public LightParameterInfo(ParameterInfo info)
    {
      this.Type = info.ParameterType;
      this.Name = info.Name;
      this.Attributes = info.Attributes;
    }

    public LightParameterInfo(Type type, string name)
    {
      this.Type = type;
      this.Name = name;
      this.Attributes = ParameterAttributes.HasDefault;
    }

    public Type Type { get; private set; }

    public string Name { get; private set; }

    public ParameterAttributes Attributes { get; private set; }
  }

  private enum InfoArrayMode
  {
    Invoke,
    BeginInvoke,
    EndInvoke,
  }
}
