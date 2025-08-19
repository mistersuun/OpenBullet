// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DelegateInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class DelegateInfo
{
  private const int TargetIndex = 0;
  private const int CallSiteIndex = 1;
  private const int ConvertSiteIndex = 2;
  private static readonly object TargetPlaceHolder = new object();
  private static readonly object CallSitePlaceHolder = new object();
  private static readonly object ConvertSitePlaceHolder = new object();
  private WeakDictionary<object, WeakReference> _closureMap = new WeakDictionary<object, WeakReference>();
  private readonly Type _returnType;
  private readonly Type[] _parameterTypes;
  private readonly MethodInfo _method;
  private readonly InvokeBinder _invokeBinder;
  private readonly ConvertBinder _convertBinder;

  public DelegateInfo(LanguageContext context, Type returnType, Type[] parameters)
  {
    this._returnType = returnType;
    this._parameterTypes = parameters;
    if (this._returnType != typeof (void))
      this._convertBinder = context.CreateConvertBinder(this._returnType, new bool?(true));
    this._invokeBinder = context.CreateInvokeBinder(new CallInfo(this._parameterTypes.Length, new string[0]));
    Type[] parameterTypes = new Type[1 + this._parameterTypes.Length];
    parameterTypes[0] = typeof (object[]);
    for (int index = 0; index < this._parameterTypes.Length; ++index)
      parameterTypes[1 + index] = this._parameterTypes[index];
    this.EmitClrCallStub(returnType, parameterTypes, out this._method);
  }

  public Delegate CreateDelegate(Type delegateType, object dynamicObject)
  {
    object[] target;
    lock (this._closureMap)
    {
      WeakReference weakReference;
      if (this._closureMap.TryGetValue(dynamicObject, out weakReference))
      {
        if ((target = (object[]) weakReference.Target) != null)
          goto label_9;
      }
      target = new object[3]
      {
        DelegateInfo.TargetPlaceHolder,
        DelegateInfo.CallSitePlaceHolder,
        DelegateInfo.ConvertSitePlaceHolder
      };
      this._closureMap[dynamicObject] = new WeakReference((object) target);
      CallSite callSite1 = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(DelegateInfo.MakeSiteSignature(this._parameterTypes)), (CallSiteBinder) this._invokeBinder);
      CallSite callSite2 = (CallSite) null;
      if (this._returnType != typeof (void))
        callSite2 = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(typeof (object), this._returnType), (CallSiteBinder) this._convertBinder);
      target[0] = dynamicObject;
      target[1] = (object) callSite1;
      target[2] = (object) callSite2;
    }
label_9:
    return this._method.CreateDelegate(delegateType, (object) target);
  }

  private void EmitClrCallStub(Type returnType, Type[] parameterTypes, out MethodInfo method)
  {
    DynamicILGen dynamicMethod = Snippets.Shared.CreateDynamicMethod("_Scripting_", returnType, parameterTypes, false);
    this.EmitClrCallStub((ILGen) dynamicMethod);
    method = dynamicMethod.Finish();
  }

  private void EmitClrCallStub(ILGen cg)
  {
    List<ReturnFixer> returnFixerList = new List<ReturnFixer>(0);
    Type type1 = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(DelegateInfo.MakeSiteSignature(this._parameterTypes)), (CallSiteBinder) this._invokeBinder).GetType();
    Type type2 = (Type) null;
    if (this._returnType != typeof (void))
      type2 = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(typeof (object), this._returnType), (CallSiteBinder) this._convertBinder).GetType();
    FieldInfo fi = (FieldInfo) null;
    if (this._returnType != typeof (void))
    {
      LocalBuilder local = cg.DeclareLocal(type2);
      DelegateInfo.EmitConstantGet(cg, 2, type2);
      cg.Emit(OpCodes.Dup);
      cg.Emit(OpCodes.Stloc, local);
      fi = type2.GetDeclaredField("Target");
      cg.EmitFieldGet(fi);
      cg.Emit(OpCodes.Ldloc, local);
    }
    LocalBuilder local1 = cg.DeclareLocal(type1);
    DelegateInfo.EmitConstantGet(cg, 1, type1);
    cg.Emit(OpCodes.Dup);
    cg.Emit(OpCodes.Stloc, local1);
    FieldInfo declaredField = type1.GetDeclaredField("Target");
    cg.EmitFieldGet(declaredField);
    cg.Emit(OpCodes.Ldloc, local1);
    DelegateInfo.EmitConstantGet(cg, 0, typeof (object));
    for (int index = 0; index < this._parameterTypes.Length; ++index)
    {
      if (this._parameterTypes[index].IsByRef)
      {
        ReturnFixer returnFixer = ReturnFixer.EmitArgument(cg, index + 1, this._parameterTypes[index]);
        if (returnFixer != null)
          returnFixerList.Add(returnFixer);
      }
      else
        cg.EmitLoadArg(index + 1);
    }
    cg.EmitCall(declaredField.FieldType, "Invoke");
    if (this._returnType == typeof (void))
      cg.Emit(OpCodes.Pop);
    else
      cg.EmitCall(fi.FieldType, "Invoke");
    foreach (ReturnFixer returnFixer in returnFixerList)
      returnFixer.FixReturn(cg);
    cg.Emit(OpCodes.Ret);
  }

  private static void EmitConstantGet(ILGen il, int index, Type type)
  {
    il.Emit(OpCodes.Ldarg_0);
    il.EmitInt(index);
    il.Emit(OpCodes.Ldelem_Ref);
    if (!(type != typeof (object)))
      return;
    il.Emit(OpCodes.Castclass, type);
  }

  private static Type[] MakeSiteSignature(Type[] parameterTypes)
  {
    Type[] typeArray = new Type[parameterTypes.Length + 2];
    typeArray[0] = typeof (object);
    for (int index = 0; index < parameterTypes.Length; ++index)
      typeArray[index + 1] = !parameterTypes[index].IsByRef ? parameterTypes[index] : typeof (object);
    typeArray[typeArray.Length - 1] = typeof (object);
    return typeArray;
  }
}
