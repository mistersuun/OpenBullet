// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonExtensionBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonExtensionBinder : PythonBinder
{
  private readonly ExtensionMethodSet _extMethodSet;

  public PythonExtensionBinder(PythonBinder binder, ExtensionMethodSet extensionMethods)
    : base(binder)
  {
    this._extMethodSet = extensionMethods;
  }

  public override MemberGroup GetMember(MemberRequestKind actionKind, Type type, string name)
  {
    MemberGroup member = base.GetMember(actionKind, type, name);
    if (member.Count == 0)
    {
      List<MemberTracker> memberTrackerList = new List<MemberTracker>();
      foreach (MethodInfo extensionMethod in this._extMethodSet.GetExtensionMethods(name))
      {
        ParameterInfo[] parameters = extensionMethod.GetParameters();
        if (parameters.Length != 0)
        {
          Type parameterType = parameters[0].ParameterType;
          if (PythonExtensionBinder.IsApplicableExtensionMethod(type, parameterType))
            memberTrackerList.Add(MemberTracker.FromMemberInfo((MemberInfo) extensionMethod, parameterType));
        }
      }
      if (memberTrackerList.Count > 0)
        return new MemberGroup(memberTrackerList.ToArray());
    }
    return member;
  }

  internal static bool IsApplicableExtensionMethod(Type instanceType, Type extensionMethodThisType)
  {
    if (extensionMethodThisType.ContainsGenericParameters())
    {
      Dictionary<Type, Type> binding = new Dictionary<Type, Type>();
      if (extensionMethodThisType.IsArray)
      {
        if (!instanceType.IsArray)
          return false;
        extensionMethodThisType = extensionMethodThisType.GetElementType();
        instanceType = instanceType.GetElementType();
        return TypeInferer.GetInferedType(extensionMethodThisType, extensionMethodThisType, instanceType, instanceType, binding) != (Type) null;
      }
      Type[] typeArray1;
      if (!extensionMethodThisType.IsGenericParameter)
        typeArray1 = extensionMethodThisType.GetGenericArguments();
      else
        typeArray1 = new Type[1]{ extensionMethodThisType };
      Type[] typeArray2 = typeArray1;
      Type[] typeArray3 = new Type[typeArray2.Length];
      for (int index = 0; index < typeArray2.Length; ++index)
      {
        if ((typeArray3[index] = TypeInferer.GetInferedType(typeArray2[index], extensionMethodThisType, instanceType, instanceType, binding)) == (Type) null)
        {
          typeArray3 = (Type[]) null;
          break;
        }
      }
      if (typeArray3 != null)
      {
        if (extensionMethodThisType.IsGenericParameter)
          extensionMethodThisType = typeArray3[0];
        else if (typeArray3.Length != 0)
          extensionMethodThisType = extensionMethodThisType.GetGenericTypeDefinition().MakeGenericType(typeArray3);
      }
    }
    return extensionMethodThisType.IsAssignableFrom(instanceType);
  }
}
