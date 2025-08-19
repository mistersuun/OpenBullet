// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DynamicInstructionN
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class DynamicInstructionN : Instruction
{
  private readonly CallInstruction _targetInvocationInstruction;
  private readonly object _targetDelegate;
  private readonly CallSite _site;
  private readonly int _argumentCount;
  private readonly bool _isVoid;

  public DynamicInstructionN(Type delegateType, CallSite site)
  {
    MethodInfo method = delegateType.GetMethod("Invoke");
    ParameterInfo[] parameters = method.GetParameters();
    this._targetInvocationInstruction = CallInstruction.Create(method, parameters);
    this._site = site;
    this._argumentCount = parameters.Length - 1;
    this._targetDelegate = site.GetType().GetInheritedFields("Target").First<FieldInfo>().GetValue((object) site);
  }

  public DynamicInstructionN(Type delegateType, CallSite site, bool isVoid)
    : this(delegateType, site)
  {
    this._isVoid = isVoid;
  }

  public override int ProducedStack => !this._isVoid ? 1 : 0;

  public override int ConsumedStack => this._argumentCount;

  public override int Run(InterpretedFrame frame)
  {
    int index1 = frame.StackIndex - this._argumentCount;
    object[] objArray = new object[1 + this._argumentCount];
    objArray[0] = (object) this._site;
    for (int index2 = 0; index2 < this._argumentCount; ++index2)
      objArray[1 + index2] = frame.Data[index1 + index2];
    object obj = this._targetInvocationInstruction.InvokeInstance(this._targetDelegate, objArray);
    if (this._isVoid)
    {
      frame.StackIndex = index1;
    }
    else
    {
      frame.Data[index1] = obj;
      frame.StackIndex = index1 + 1;
    }
    return 1;
  }

  public override string ToString() => $"DynamicInstructionN({(object) this._site})";

  internal static Type GetDynamicInstructionType(Type delegateType)
  {
    Type[] genericArguments = delegateType.GetGenericArguments();
    if (genericArguments.Length == 0)
      return (Type) null;
    Type[] typeArray = ArrayUtils.RemoveFirst<Type>(genericArguments);
    Type type;
    switch (typeArray.Length)
    {
      case 1:
        type = typeof (DynamicInstruction<>);
        break;
      case 2:
        type = typeof (DynamicInstruction<,>);
        break;
      case 3:
        type = typeof (DynamicInstruction<,,>);
        break;
      case 4:
        type = typeof (DynamicInstruction<,,,>);
        break;
      case 5:
        type = typeof (DynamicInstruction<,,,,>);
        break;
      case 6:
        type = typeof (DynamicInstruction<,,,,,>);
        break;
      case 7:
        type = typeof (DynamicInstruction<,,,,,,>);
        break;
      case 8:
        type = typeof (DynamicInstruction<,,,,,,,>);
        break;
      case 9:
        type = typeof (DynamicInstruction<,,,,,,,,>);
        break;
      case 10:
        type = typeof (DynamicInstruction<,,,,,,,,,>);
        break;
      case 11:
        type = typeof (DynamicInstruction<,,,,,,,,,,>);
        break;
      case 12:
        type = typeof (DynamicInstruction<,,,,,,,,,,,>);
        break;
      case 13:
        type = typeof (DynamicInstruction<,,,,,,,,,,,,>);
        break;
      case 14:
        type = typeof (DynamicInstruction<,,,,,,,,,,,,,>);
        break;
      case 15:
        type = typeof (DynamicInstruction<,,,,,,,,,,,,,,>);
        break;
      case 16 /*0x10*/:
        type = typeof (DynamicInstruction<,,,,,,,,,,,,,,,>);
        break;
      default:
        throw Assert.Unreachable;
    }
    return type.MakeGenericType(typeArray);
  }

  internal static Instruction CreateUntypedInstruction(CallSiteBinder binder, int argCount)
  {
    switch (argCount)
    {
      case 0:
        return DynamicInstruction<object>.Factory(binder);
      case 1:
        return DynamicInstruction<object, object>.Factory(binder);
      case 2:
        return DynamicInstruction<object, object, object>.Factory(binder);
      case 3:
        return DynamicInstruction<object, object, object, object>.Factory(binder);
      case 4:
        return DynamicInstruction<object, object, object, object, object>.Factory(binder);
      case 5:
        return DynamicInstruction<object, object, object, object, object, object>.Factory(binder);
      case 6:
        return DynamicInstruction<object, object, object, object, object, object, object>.Factory(binder);
      case 7:
        return DynamicInstruction<object, object, object, object, object, object, object, object>.Factory(binder);
      case 8:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 9:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 10:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 11:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 12:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 13:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 14:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      case 15:
        return DynamicInstruction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Factory(binder);
      default:
        return (Instruction) null;
    }
  }
}
