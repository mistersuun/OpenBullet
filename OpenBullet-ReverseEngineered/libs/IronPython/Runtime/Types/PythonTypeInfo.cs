// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

internal static class PythonTypeInfo
{
  private static readonly PythonTypeInfo.MemberResolver[] _resolvers = PythonTypeInfo.MakeResolverTable();
  private static PythonTypeInfo.DocumentationDescriptor _docDescr;
  internal static Dictionary<string, PythonOperationKind> _pythonOperatorTable;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _ComplexResolver;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _FloatResolver;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _IntResolver;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _BigIntegerResolver;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _GetItemResolver;
  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _SetItemResolver;
  private static readonly string[] CastNames = new string[2]
  {
    "op_Implicit",
    "op_Explicit"
  };

  public static MemberGroup GetMemberAll(
    PythonBinder binder,
    MemberRequestKind action,
    Type type,
    string name)
  {
    return PythonTypeInfo.GetMemberGroup((PythonTypeInfo.MemberBinder) new PythonTypeInfo.ResolveBinder(binder), action, type, name);
  }

  public static IList<ResolvedMember> GetMembersAll(
    PythonBinder binder,
    MemberRequestKind action,
    Type type)
  {
    return PythonTypeInfo.GetResolvedMembers((PythonTypeInfo.MemberBinder) new PythonTypeInfo.ResolveBinder(binder), action, type);
  }

  public static MemberGroup GetMember(
    PythonBinder binder,
    MemberRequestKind action,
    Type type,
    string name)
  {
    return PythonTypeInfo.GetMemberGroup((PythonTypeInfo.MemberBinder) new PythonTypeInfo.LookupBinder(binder), action, type, name);
  }

  public static IList<ResolvedMember> GetMembers(
    PythonBinder binder,
    MemberRequestKind action,
    Type type)
  {
    return PythonTypeInfo.GetResolvedMembers((PythonTypeInfo.MemberBinder) new PythonTypeInfo.LookupBinder(binder), action, type);
  }

  private static PythonTypeInfo.MemberResolver[] MakeResolverTable()
  {
    return new PythonTypeInfo.MemberResolver[41]
    {
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__str__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.StringResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__new__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.NewResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__repr__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.ReprResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__hash__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.HashResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__iter__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.IterResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__reduce_ex__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.SerializationResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.StandardResolver(),
      (PythonTypeInfo.MemberResolver) PythonTypeInfo.EqualityResolver.Instance,
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.ComparisonResolver(typeof (IStructuralComparable), "StructuralComparable"),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__all__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.AllResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__contains__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.ContainsResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__dir__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.DirResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__doc__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.DocResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__enter__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.EnterResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__exit__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.ExitResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__len__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.LengthResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__format__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.FormatResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("next", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.NextResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__complex__", PythonTypeInfo.ComplexResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__float__", PythonTypeInfo.FloatResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__int__", PythonTypeInfo.IntResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__long__", PythonTypeInfo.BigIntegerResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__truediv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("TrueDivide", "__truediv__", PythonOperationKind.TrueDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__rtruediv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("TrueDivide", "__rtruediv__", PythonOperationKind.ReverseTrueDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__itruediv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("InPlaceTrueDivide", "__itruediv__", PythonOperationKind.InPlaceTrueDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__floordiv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("FloorDivide", "__floordiv__", PythonOperationKind.FloorDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__rfloordiv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("FloorDivide", "__rfloordiv__", PythonOperationKind.ReverseFloorDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__ifloordiv__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("InPlaceFloorDivide", "__ifloordiv__", PythonOperationKind.InPlaceFloorDivide).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__pow__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffPowerBinder("__pow__", PythonOperationKind.Power).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__rpow__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffPowerBinder("__rpow__", PythonOperationKind.ReversePower).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__ipow__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("InPlacePower", "__ipow__", PythonOperationKind.InPlacePower).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__abs__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("Abs", "__abs__", PythonOperationKind.AbsoluteValue).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__divmod__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("DivMod", "__divmod__", PythonOperationKind.DivMod).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__rdivmod__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(new PythonTypeInfo.OneOffOperatorBinder("DivMod", "__rdivmod__", PythonOperationKind.DivMod).Resolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OperatorResolver(),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__getitem__", PythonTypeInfo.GetItemResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__setitem__", PythonTypeInfo.SetItemResolver),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.OneOffResolver("__ne__", new Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>(PythonTypeInfo.FallbackInequalityResolver)),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.ComparisonResolver(typeof (IComparable), "Comparable"),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.ProtectedMemberResolver(),
      (PythonTypeInfo.MemberResolver) new PythonTypeInfo.PrivateBindingResolver()
    };
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> ComplexResolver
  {
    get
    {
      if (PythonTypeInfo._ComplexResolver != null)
        return PythonTypeInfo._ComplexResolver;
      PythonTypeInfo._ComplexResolver = PythonTypeInfo.MakeConversionResolver(new List<Type>()
      {
        typeof (Complex),
        typeof (ExtensibleComplex),
        typeof (Extensible<Complex>),
        typeof (double),
        typeof (Extensible<double>)
      });
      return PythonTypeInfo._ComplexResolver;
    }
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> FloatResolver
  {
    get
    {
      if (PythonTypeInfo._FloatResolver != null)
        return PythonTypeInfo._FloatResolver;
      PythonTypeInfo._FloatResolver = PythonTypeInfo.MakeConversionResolver(new List<Type>()
      {
        typeof (double),
        typeof (Extensible<double>)
      });
      return PythonTypeInfo._FloatResolver;
    }
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> IntResolver
  {
    get
    {
      if (PythonTypeInfo._IntResolver != null)
        return PythonTypeInfo._IntResolver;
      PythonTypeInfo._IntResolver = PythonTypeInfo.MakeConversionResolver(new List<Type>()
      {
        typeof (int),
        typeof (Extensible<int>),
        typeof (BigInteger),
        typeof (Extensible<BigInteger>)
      });
      return PythonTypeInfo._IntResolver;
    }
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> BigIntegerResolver
  {
    get
    {
      if (PythonTypeInfo._BigIntegerResolver != null)
        return PythonTypeInfo._BigIntegerResolver;
      PythonTypeInfo._BigIntegerResolver = PythonTypeInfo.MakeConversionResolver(new List<Type>()
      {
        typeof (BigInteger),
        typeof (Extensible<BigInteger>),
        typeof (int),
        typeof (Extensible<int>)
      });
      return PythonTypeInfo._BigIntegerResolver;
    }
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> GetItemResolver
  {
    get
    {
      if (PythonTypeInfo._GetItemResolver == null)
        PythonTypeInfo._GetItemResolver = PythonTypeInfo.MakeIndexerResolver(false);
      return PythonTypeInfo._GetItemResolver;
    }
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> SetItemResolver
  {
    get
    {
      if (PythonTypeInfo._SetItemResolver == null)
        PythonTypeInfo._SetItemResolver = PythonTypeInfo.MakeIndexerResolver(true);
      return PythonTypeInfo._SetItemResolver;
    }
  }

  private static MemberGroup StringResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type != typeof (double) && type != typeof (float) && type != typeof (Complex))
    {
      MethodInfo method = type.GetMethod("ToString", ReflectionUtils.EmptyTypes);
      if (method != (MethodInfo) null && method.DeclaringType != typeof (object))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "ToStringMethod");
    }
    return MemberGroup.EmptyGroup;
  }

  private static MemberGroup ReprResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (PythonBinder.IsPythonType(type) || type.IsSealed() && type.IsAbstract())
      return MemberGroup.EmptyGroup;
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
    {
      if (contributingType == typeof (ObjectOps))
      {
        if (type != typeof (object))
          break;
      }
      if (contributingType.GetMember("__repr__").Length != 0)
        return MemberGroup.EmptyGroup;
    }
    return binder.GetBaseInstanceMethod(type, "FancyRepr");
  }

  private static MemberGroup SerializationResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsSerializable && !PythonBinder.IsPythonType(type))
    {
      string methodName = "__reduce_ex__";
      if (!PythonTypeInfo.TypeOverridesMethod(binder, type, methodName))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "SerializeReduce");
    }
    return MemberGroup.EmptyGroup;
  }

  private static bool TypeOverridesMethod(
    PythonTypeInfo.MemberBinder binder,
    Type type,
    string methodName)
  {
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
    {
      if (!PythonBinder.IsPythonType(type) && contributingType == typeof (ObjectOps))
      {
        if (type != typeof (object))
          break;
      }
      if (contributingType.GetMember(methodName).Length != 0)
        return true;
    }
    return false;
  }

  private static MemberGroup HashResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (!typeof (IStructuralEquatable).IsAssignableFrom(type) || type.IsInterface())
      return MemberGroup.EmptyGroup;
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
    {
      if (!(contributingType == typeof (ObjectOps)))
      {
        if (!(contributingType == typeof (object)))
        {
          if (contributingType.GetMember("__hash__").Length != 0)
            return MemberGroup.EmptyGroup;
        }
        else
          break;
      }
      else
        break;
    }
    return PythonTypeInfo.GetInstanceOpsMethod(type, "StructuralHashMethod");
  }

  private static MemberGroup NewResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsSealed() && type.IsAbstract())
      return MemberGroup.EmptyGroup;
    bool flag = typeof (IPythonObject).IsAssignableFrom(type);
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
    {
      if (!flag && contributingType == typeof (ObjectOps))
      {
        if (type != typeof (object))
          break;
      }
      MemberInfo[] member = contributingType.GetMember("__new__");
      if (member.Length != 0)
        return PythonTypeInfo.GetExtensionMemberGroup(type, member);
    }
    ConstructorInfo[] array = CompilerHelpers.FilterConstructorsToPublicAndProtected((IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).ToArray<ConstructorInfo>();
    return !PythonTypeOps.IsDefaultNew((MethodBase[]) array) ? new MemberGroup((MemberInfo[]) array) : MemberGroup.EmptyGroup;
  }

  internal static MemberGroup GetExtensionMemberGroup(Type type, MemberInfo[] news)
  {
    List<MemberTracker> memberTrackerList = new List<MemberTracker>();
    foreach (MemberInfo member in news)
    {
      if (member.MemberType == MemberTypes.Method)
      {
        if (member.DeclaringType.IsAssignableFrom(type))
          memberTrackerList.Add(MemberTracker.FromMemberInfo(member));
        else
          memberTrackerList.Add(MemberTracker.FromMemberInfo(member, type));
      }
    }
    return new MemberGroup(memberTrackerList.ToArray());
  }

  private static MemberGroup NextResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (!typeof (IEnumerator).IsAssignableFrom(type))
      return MemberGroup.EmptyGroup;
    return PythonTypeInfo.GetInstanceOpsMethod(type, "NextMethod");
  }

  private static MemberGroup LengthResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (!type.IsDefined(typeof (DontMapICollectionToLenAttribute), true))
    {
      if (binder.GetInterfaces(type).Contains(typeof (ICollection)))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "LengthMethod");
      foreach (Type type1 in (IEnumerable<Type>) binder.GetInterfaces(type))
      {
        if (type1.IsGenericType() && type1.GetGenericTypeDefinition() == typeof (ICollection<>))
          return new MemberGroup(new MemberTracker[1]
          {
            MemberTracker.FromMemberInfo((MemberInfo) typeof (InstanceOps).GetMethod("GenericLengthMethod").MakeGenericMethod(type1.GetGenericArguments()), type)
          });
      }
    }
    return MemberGroup.EmptyGroup;
  }

  private static MemberGroup IterResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type == typeof (string))
    {
      if (!binder.Binder.Context.PythonOptions.Python30)
        return MemberGroup.EmptyGroup;
      return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForString");
    }
    if (typeof (Bytes).IsAssignableFrom(type))
    {
      if (!binder.Binder.Context.PythonOptions.Python30)
        return MemberGroup.EmptyGroup;
      return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForBytes");
    }
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
    {
      if (contributingType.GetMember("__iter__").Length != 0)
        return MemberGroup.EmptyGroup;
    }
    if (!type.IsDefined(typeof (DontMapIEnumerableToIterAttribute), true))
    {
      if (typeof (IEnumerable<>).IsAssignableFrom(type))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForGenericEnumerable");
      if (typeof (IEnumerable).IsAssignableFrom(type))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForEnumerable");
      if (typeof (IEnumerator<>).IsAssignableFrom(type))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForGenericEnumerator");
      if (typeof (IEnumerator).IsAssignableFrom(type))
        return PythonTypeInfo.GetInstanceOpsMethod(type, "IterMethodForEnumerator");
    }
    return MemberGroup.EmptyGroup;
  }

  private static MemberGroup FallbackInequalityResolver(
    PythonTypeInfo.MemberBinder binder,
    Type type)
  {
    if (PythonTypeInfo.IncludeOperatorMethod(type, PythonOperationKind.NotEqual))
    {
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        foreach (MemberTracker memberTracker in binder.GetMember(contributingType, "Equals"))
        {
          if (memberTracker.MemberType == TrackerTypes.Method && !(memberTracker.DeclaringType == typeof (object)))
          {
            MethodTracker methodTracker = (MethodTracker) memberTracker;
            if ((methodTracker.Method.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.PrivateScope && !PythonHiddenAttribute.IsHidden((MemberInfo) methodTracker.Method))
            {
              ParameterInfo[] parameters = methodTracker.Method.GetParameters();
              if (parameters.Length == 1 && parameters[0].ParameterType == typeof (object))
                return new MemberGroup(new MemberTracker[1]
                {
                  MemberTracker.FromMemberInfo((MemberInfo) typeof (InstanceOps).GetMethod("NotEqualsMethod"), contributingType)
                });
            }
          }
        }
      }
    }
    return MemberGroup.EmptyGroup;
  }

  private static MemberGroup AllResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (!type.IsAbstract() || !type.IsSealed())
      return MemberGroup.EmptyGroup;
    return new MemberGroup(new MemberTracker[1]
    {
      (MemberTracker) new ExtensionPropertyTracker("__all__", typeof (InstanceOps).GetMethod("Get__all__").MakeGenericMethod(type), (MethodInfo) null, (MethodInfo) null, type)
    });
  }

  private static MemberGroup DirResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsDefined(typeof (DontMapGetMemberNamesToDirAttribute), true))
      return MemberGroup.EmptyGroup;
    MemberGroup memberGroup = binder.GetMember(type, "GetMemberNames");
    if (memberGroup == MemberGroup.EmptyGroup && !typeof (IPythonObject).IsAssignableFrom(type) && typeof (IDynamicMetaObjectProvider).IsAssignableFrom(type))
      memberGroup = PythonTypeInfo.GetInstanceOpsMethod(type, "DynamicDir");
    return memberGroup;
  }

  private static MemberGroup DocResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (PythonTypeInfo._docDescr == null)
      PythonTypeInfo._docDescr = new PythonTypeInfo.DocumentationDescriptor();
    return new MemberGroup(new MemberTracker[1]
    {
      (MemberTracker) new CustomAttributeTracker(type, "__doc__", (PythonTypeSlot) PythonTypeInfo._docDescr)
    });
  }

  private static MemberGroup EnterResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsDefined(typeof (DontMapIDisposableToContextManagerAttribute), true) || !typeof (IDisposable).IsAssignableFrom(type))
      return MemberGroup.EmptyGroup;
    return PythonTypeInfo.GetInstanceOpsMethod(type, "EnterMethod");
  }

  private static MemberGroup ExitResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsDefined(typeof (DontMapIDisposableToContextManagerAttribute), true) || !typeof (IDisposable).IsAssignableFrom(type))
      return MemberGroup.EmptyGroup;
    return PythonTypeInfo.GetInstanceOpsMethod(type, "ExitMethod");
  }

  private static MemberGroup FormatResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (!typeof (IFormattable).IsAssignableFrom(type))
      return MemberGroup.EmptyGroup;
    return PythonTypeInfo.GetInstanceOpsMethod(type, "Format");
  }

  private static MemberGroup ContainsResolver(PythonTypeInfo.MemberBinder binder, Type type)
  {
    if (type.IsDefined(typeof (DontMapIEnumerableToContainsAttribute), true))
      return MemberGroup.EmptyGroup;
    List<MemberTracker> containsMembers = (List<MemberTracker>) null;
    IList<Type> interfaces = binder.GetInterfaces(type);
    bool hasObjectContains = false;
    foreach (Type type1 in (IEnumerable<Type>) interfaces)
    {
      if (type1.IsGenericType() && type1.GetGenericTypeDefinition() == typeof (IDictionary<,>))
      {
        if (type1.GetGenericArguments()[0] == typeof (object))
          hasObjectContains = true;
        if (containsMembers == null)
          containsMembers = new List<MemberTracker>();
        containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) type1.GetMethod("ContainsKey")));
      }
    }
    if (containsMembers == null)
    {
      foreach (Type type2 in (IEnumerable<Type>) interfaces)
      {
        if (type2.IsGenericType() && type2.GetGenericTypeDefinition() == typeof (ICollection<>))
        {
          if (type2.GetGenericArguments()[0] == typeof (object))
            hasObjectContains = true;
          if (containsMembers == null)
            containsMembers = new List<MemberTracker>();
          containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) type2.GetMethod("Contains")));
        }
      }
    }
    if (!hasObjectContains)
    {
      if (interfaces.Contains(typeof (IList)))
      {
        if (containsMembers == null)
          containsMembers = new List<MemberTracker>();
        containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) typeof (IList).GetMethod("Contains")));
      }
      else if (interfaces.Contains(typeof (IDictionary)))
      {
        if (containsMembers == null)
          containsMembers = new List<MemberTracker>();
        containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) typeof (IDictionary).GetMethod("Contains")));
      }
      else if (containsMembers == null)
      {
        PythonTypeInfo.GetEnumeratorContains(type, interfaces, ref containsMembers, ref hasObjectContains, typeof (IEnumerable<>), typeof (IEnumerable), string.Empty);
        if (containsMembers == null)
          PythonTypeInfo.GetEnumeratorContains(type, interfaces, ref containsMembers, ref hasObjectContains, typeof (IEnumerator<>), typeof (IEnumerator), "IEnumerator");
      }
    }
    return containsMembers != null ? new MemberGroup(containsMembers.ToArray()) : MemberGroup.EmptyGroup;
  }

  private static void GetEnumeratorContains(
    Type type,
    IList<Type> intf,
    ref List<MemberTracker> containsMembers,
    ref bool hasObjectContains,
    Type ienumOfT,
    Type ienum,
    string name)
  {
    foreach (Type type1 in (IEnumerable<Type>) intf)
    {
      if (type1.IsGenericType() && type1.GetGenericTypeDefinition() == ienumOfT)
      {
        if (type1.GetGenericArguments()[0] == typeof (object))
          hasObjectContains = true;
        if (containsMembers == null)
          containsMembers = new List<MemberTracker>();
        containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) typeof (InstanceOps).GetMethod("ContainsGenericMethod" + name).MakeGenericMethod(type1.GetGenericArguments()[0]), type1));
      }
    }
    if (!intf.Contains(type) || hasObjectContains)
      return;
    if (containsMembers == null)
      containsMembers = new List<MemberTracker>();
    containsMembers.Add(MemberTracker.FromMemberInfo((MemberInfo) typeof (InstanceOps).GetMethod("ContainsMethod" + name), typeof (IEnumerable)));
  }

  private static MethodTracker[] GetEqualityMethods(Type type, string name)
  {
    MethodInfo[] methodSet = PythonTypeInfo.GetMethodSet(name, 3);
    MethodTracker[] equalityMethods = new MethodTracker[methodSet.Length];
    for (int index = 0; index < methodSet.Length; ++index)
      equalityMethods[index] = (MethodTracker) MemberTracker.FromMemberInfo((MemberInfo) methodSet[index].MakeGenericMethod(type), type);
    return equalityMethods;
  }

  private static MemberGroup GetMemberGroup(
    PythonTypeInfo.MemberBinder memberBinder,
    MemberRequestKind action,
    Type type,
    string name)
  {
    foreach (PythonTypeInfo.MemberResolver resolver in PythonTypeInfo._resolvers)
    {
      MemberGroup memberGroup = resolver.ResolveMember(memberBinder, action, type, name);
      if (memberGroup.Count > 0)
        return memberGroup;
    }
    return MemberGroup.EmptyGroup;
  }

  private static IList<ResolvedMember> GetResolvedMembers(
    PythonTypeInfo.MemberBinder memberBinder,
    MemberRequestKind action,
    Type type)
  {
    List<ResolvedMember> resolvedMembers = new List<ResolvedMember>();
    foreach (PythonTypeInfo.MemberResolver resolver in PythonTypeInfo._resolvers)
      resolvedMembers.AddRange((IEnumerable<ResolvedMember>) resolver.ResolveMembers(memberBinder, action, type));
    return (IList<ResolvedMember>) resolvedMembers;
  }

  private static MemberGroup GetInstanceOpsMethod(Type extends, params string[] names)
  {
    MethodTracker[] methodTrackerArray = new MethodTracker[names.Length];
    for (int index = 0; index < names.Length; ++index)
      methodTrackerArray[index] = (MethodTracker) MemberTracker.FromMemberInfo((MemberInfo) typeof (InstanceOps).GetMethod(names[index]), extends);
    return new MemberGroup((MemberTracker[]) methodTrackerArray);
  }

  private static MethodInfo FindCastMethod(
    PythonTypeInfo.MemberBinder binder,
    Type fromType,
    List<Type> toTypes)
  {
    MethodInfo castMethod = (MethodInfo) null;
    ParameterInfo[] parameterInfoArray = (ParameterInfo[]) null;
    foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(fromType))
    {
      foreach (string castName in PythonTypeInfo.GetCastNames(fromType, toTypes[0]))
      {
        foreach (MemberInfo memberInfo in contributingType.GetMember(castName))
        {
          if (memberInfo.MemberType == MemberTypes.Method)
          {
            MethodInfo methodInfo = (MethodInfo) memberInfo;
            ParameterInfo[] parameters;
            if (toTypes.Contains(methodInfo.ReturnType) && (parameters = methodInfo.GetParameters()).Length == 1)
            {
              if (castMethod == (MethodInfo) null || methodInfo.DeclaringType.IsSubclassOf(castMethod.DeclaringType))
              {
                castMethod = methodInfo;
                parameterInfoArray = parameters;
              }
              else if (!(methodInfo.DeclaringType != castMethod.DeclaringType))
              {
                if (parameters[0].ParameterType.IsSubclassOf(parameterInfoArray[0].ParameterType))
                {
                  castMethod = methodInfo;
                  parameterInfoArray = parameters;
                }
                else if (!(parameterInfoArray[0].ParameterType != parameters[0].ParameterType))
                {
                  if (methodInfo.Name != castMethod.Name)
                  {
                    if (methodInfo.Name == "op_Implicit")
                    {
                      castMethod = methodInfo;
                      parameterInfoArray = parameters;
                    }
                  }
                  else
                  {
                    foreach (Type toType in toTypes)
                    {
                      if (methodInfo.ReturnType == toType)
                      {
                        castMethod = methodInfo;
                        parameterInfoArray = parameters;
                      }
                      else if (castMethod.ReturnType == toType)
                        break;
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    return castMethod;
  }

  private static string[] GetCastNames(Type fromType, Type toType)
  {
    if (PythonBinder.IsPythonType(fromType))
      return PythonTypeInfo.CastNames;
    return new string[3]
    {
      "op_Implicit",
      "op_Explicit",
      "ConvertTo" + toType.Name
    };
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> MakeConversionResolver(
    List<Type> castPrec)
  {
    return (Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>) ((binder, type) =>
    {
      MethodInfo castMethod = PythonTypeInfo.FindCastMethod(binder, type, castPrec);
      if (!(castMethod != (MethodInfo) null))
        return MemberGroup.EmptyGroup;
      return new MemberGroup(new MemberTracker[1]
      {
        MemberTracker.FromMemberInfo((MemberInfo) castMethod, type)
      });
    });
  }

  private static Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> MakeIndexerResolver(bool set)
  {
    return (Func<PythonTypeInfo.MemberBinder, Type, MemberGroup>) ((binder, type) =>
    {
      List<MemberInfo> memberInfoList = (List<MemberInfo>) null;
      foreach (MemberInfo defaultMember in type.GetDefaultMembers())
      {
        PropertyInfo propertyInfo = defaultMember as PropertyInfo;
        if (propertyInfo != (PropertyInfo) null)
        {
          MethodInfo methodInfo = set ? propertyInfo.GetSetMethod() : propertyInfo.GetGetMethod();
          if (methodInfo != (MethodInfo) null)
          {
            memberInfoList = memberInfoList ?? new List<MemberInfo>();
            memberInfoList.Add((MemberInfo) methodInfo);
          }
        }
      }
      return memberInfoList == null ? MemberGroup.EmptyGroup : new MemberGroup(memberInfoList.ToArray());
    });
  }

  internal static bool IncludeOperatorMethod(Type t, PythonOperationKind op)
  {
    if (t == typeof (string) && op == PythonOperationKind.Compare)
      return false;
    if (t == typeof (bool) || Converter.IsNumeric(t) && t != typeof (Complex) && t != typeof (double) && t != typeof (float))
    {
      switch (op)
      {
        case PythonOperationKind.LessThan:
        case PythonOperationKind.GreaterThan:
        case PythonOperationKind.LessThanOrEqual:
        case PythonOperationKind.GreaterThanOrEqual:
        case PythonOperationKind.Equal:
        case PythonOperationKind.NotEqual:
          return false;
      }
    }
    return true;
  }

  private static MemberGroup FilterFieldAndEvent(MemberGroup members)
  {
    TrackerTypes trackerTypes = TrackerTypes.None;
    foreach (MemberTracker member in members)
      trackerTypes |= member.MemberType;
    if (trackerTypes != (TrackerTypes.Event | TrackerTypes.Field))
      return members;
    List<MemberTracker> memberTrackerList = new List<MemberTracker>();
    foreach (MemberTracker member in members)
    {
      if (member.MemberType == TrackerTypes.Event)
        memberTrackerList.Add(member);
    }
    return new MemberGroup(memberTrackerList.ToArray());
  }

  private static bool ProtectedOnly(MemberInfo input)
  {
    switch (input.MemberType)
    {
      case MemberTypes.Event:
        MethodInfo addMethod = ((EventInfo) input).GetAddMethod(true);
        return addMethod != (MethodInfo) null && PythonTypeInfo.ProtectedOnly((MemberInfo) addMethod);
      case MemberTypes.Field:
        return ((FieldInfo) input).IsProtected();
      case MemberTypes.Method:
        return ((MethodBase) input).IsProtected();
      case MemberTypes.Property:
        MethodInfo getMethod = ((PropertyInfo) input).GetGetMethod(true);
        return getMethod != (MethodInfo) null && PythonTypeInfo.ProtectedOnly((MemberInfo) getMethod);
      case MemberTypes.NestedType:
        return ((Type) input).IsProtected();
      default:
        return false;
    }
  }

  internal static bool IsReverseOperator(PythonOperationKind op)
  {
    return (op & PythonOperationKind.Reversed) != 0;
  }

  private static MemberGroup FilterForwardReverseMethods(
    string name,
    MemberGroup group,
    Type type,
    PythonOperationKind oper)
  {
    List<MethodTracker> methodTrackerList = new List<MethodTracker>(group.Count);
    PythonOperationKind reverseOperator = Symbols.OperatorToReverseOperator(oper);
    foreach (MemberTracker memberTracker in group)
    {
      if (memberTracker.MemberType == TrackerTypes.Method)
      {
        MethodTracker methodTracker = (MethodTracker) memberTracker;
        if (reverseOperator == PythonOperationKind.None)
        {
          methodTrackerList.Add(methodTracker);
        }
        else
        {
          MethodInfo method = methodTracker.Method;
          if (!method.IsStatic)
          {
            if (!PythonTypeInfo.IsReverseOperator(oper))
              methodTrackerList.Add(methodTracker);
          }
          else
          {
            ParameterInfo[] parameters = method.GetParameters();
            int index = parameters.Length == 0 || !(parameters[0].ParameterType == typeof (CodeContext)) ? 0 : 1;
            if (parameters.Length - index == 2)
            {
              Type parameterType1 = parameters[index].ParameterType;
              Type parameterType2 = parameters[1 + index].ParameterType;
              bool flag1;
              bool flag2;
              if (parameterType1 == typeof (object) && parameterType2 == typeof (object))
              {
                flag1 = !PythonTypeInfo.IsReverseOperator(oper);
                flag2 = PythonTypeInfo.IsReverseOperator(oper);
              }
              else
              {
                flag1 = parameters.Length != 0 && PythonTypeInfo.AreTypesCompatible(parameterType1, type);
                flag2 = (oper & PythonOperationKind.Comparison) == PythonOperationKind.None && parameters.Length > 1 && PythonTypeInfo.AreTypesCompatible(parameterType2, type);
              }
              if (PythonTypeInfo.IsReverseOperator(oper))
              {
                if (flag2)
                  methodTrackerList.Add(methodTracker);
              }
              else if (flag1)
                methodTrackerList.Add(methodTracker);
            }
            else
              methodTrackerList.Add(methodTracker);
          }
        }
      }
    }
    if (methodTrackerList.Count == 0)
      return MemberGroup.EmptyGroup;
    return new MemberGroup(new MemberTracker[1]
    {
      (MemberTracker) new OperatorTracker(type, name, PythonTypeInfo.IsReverseOperator(oper), methodTrackerList.ToArray())
    });
  }

  private static bool AreTypesCompatible(Type paramType, Type declaringType)
  {
    if (paramType == typeof (object))
      return declaringType == typeof (object);
    if (paramType == declaringType || declaringType.IsSubclassOf(paramType))
      return true;
    return declaringType.IsSubclassOf(typeof (Extensible<>).MakeGenericType(paramType)) || DynamicHelpers.GetPythonTypeFromType(declaringType).IsSubclassOf(DynamicHelpers.GetPythonTypeFromType(paramType));
  }

  private static void EnsureOperatorTable()
  {
    if (PythonTypeInfo._pythonOperatorTable != null)
      return;
    PythonTypeInfo._pythonOperatorTable = PythonTypeInfo.InitializeOperatorTable();
  }

  private static MemberGroup FilterSpecialNames(
    MemberGroup group,
    string name,
    MemberRequestKind action)
  {
    bool flag1 = true;
    if (action == MemberRequestKind.Invoke || action == MemberRequestKind.Convert || action == MemberRequestKind.Operation)
      flag1 = false;
    if (!PythonTypeInfo.IsPythonRecognizedOperator(name))
      flag1 = false;
    List<MemberTracker> mts = (List<MemberTracker>) null;
    for (int index = 0; index < group.Count; ++index)
    {
      MemberTracker memberTracker = group[index];
      bool flag2 = false;
      if (memberTracker.MemberType == TrackerTypes.Method)
      {
        MethodTracker meth = (MethodTracker) memberTracker;
        if (meth.Method.IsSpecialName && memberTracker.Name != "op_Implicit" && memberTracker.Name != "op_Explicit" && !PythonTypeInfo.IsPropertyWithParameters(meth))
          flag2 = true;
        if (meth.Method.IsDefined(typeof (ClassMethodAttribute), true))
          return new MemberGroup(new MemberTracker[1]
          {
            (MemberTracker) new ClassMethodTracker(group)
          });
      }
      else if (memberTracker.MemberType == TrackerTypes.Property)
      {
        PropertyTracker pt = (PropertyTracker) memberTracker;
        if (name == pt.Name && pt.GetIndexParameters().Length != 0 && PythonTypeInfo.IsPropertyDefaultMember(pt))
          flag2 = true;
      }
      else if (memberTracker.MemberType == TrackerTypes.Field)
      {
        FieldInfo field = ((FieldTracker) memberTracker).Field;
        if (field.IsDefined(typeof (SlotFieldAttribute), false) && mts == null)
        {
          mts = PythonTypeInfo.MakeListWithPreviousMembers(group, mts, index);
          memberTracker = (MemberTracker) new CustomAttributeTracker(memberTracker.DeclaringType, memberTracker.Name, (PythonTypeSlot) field.GetValue((object) null));
        }
      }
      if (flag2 & flag1)
      {
        if (mts == null)
          mts = PythonTypeInfo.MakeListWithPreviousMembers(group, mts, index);
      }
      else
        mts?.Add(memberTracker);
    }
    if (mts == null)
      return group;
    return mts.Count == 0 ? MemberGroup.EmptyGroup : new MemberGroup(mts.ToArray());
  }

  private static bool IsPropertyWithParameters(MethodTracker meth)
  {
    if (meth.Method.Name.StartsWith("get_"))
    {
      if (!PythonTypeInfo.IsMethodDefaultMember(meth) && meth.Method.GetParameters().Length != 0)
        return true;
    }
    else if (meth.Method.Name.StartsWith("set_") && !PythonTypeInfo.IsMethodDefaultMember(meth) && meth.Method.GetParameters().Length > 1)
      return true;
    return false;
  }

  private static bool IsPythonRecognizedOperator(string name)
  {
    if (name.StartsWith("get_") || name.StartsWith("set_") || name == "Abs" || name == "TrueDivide" || name == "FloorDivide" || name == "Power" || name == "DivMod")
      return true;
    bool flag = false;
    OperatorMapping operatorMapping = OperatorMapping.GetOperatorMapping(name);
    if (operatorMapping != null)
    {
      PythonTypeInfo.EnsureOperatorTable();
      if (PythonTypeInfo._pythonOperatorTable.ContainsValue(operatorMapping.Operator))
        flag = true;
    }
    return flag;
  }

  private static bool IsPropertyDefaultMember(PropertyTracker pt)
  {
    foreach (MemberInfo defaultMember in pt.DeclaringType.GetDefaultMembers())
    {
      if (defaultMember.Name == pt.Name)
        return true;
    }
    return false;
  }

  private static bool IsMethodDefaultMember(MethodTracker pt)
  {
    foreach (MemberInfo defaultMember in pt.DeclaringType.GetDefaultMembers())
    {
      if (defaultMember.MemberType == MemberTypes.Property)
      {
        PropertyInfo propertyInfo = (PropertyInfo) defaultMember;
        if (propertyInfo.GetGetMethod() == pt.Method || propertyInfo.GetSetMethod() == pt.Method)
          return true;
      }
    }
    return false;
  }

  private static List<MemberTracker> MakeListWithPreviousMembers(
    MemberGroup group,
    List<MemberTracker> mts,
    int i)
  {
    mts = new List<MemberTracker>(i);
    for (int index = 0; index < i; ++index)
      mts.Add(group[index]);
    return mts;
  }

  private static MethodInfo[] GetMethodSet(string name, int expected)
  {
    MethodInfo[] methods = typeof (InstanceOps).GetMethods();
    MethodInfo[] methodSet = new MethodInfo[expected];
    int num = 0;
    for (int index = 0; index < methods.Length; ++index)
    {
      if (methods[index].Name == name)
      {
        methodSet[num++] = methods[index];
        if (num == expected)
          break;
      }
    }
    return methodSet;
  }

  public static Dictionary<string, PythonOperationKind> InitializeOperatorTable()
  {
    return new Dictionary<string, PythonOperationKind>()
    {
      ["__add__"] = PythonOperationKind.Add,
      ["__radd__"] = PythonOperationKind.ReverseAdd,
      ["__iadd__"] = PythonOperationKind.InPlaceAdd,
      ["__sub__"] = PythonOperationKind.Subtract,
      ["__rsub__"] = PythonOperationKind.ReverseSubtract,
      ["__isub__"] = PythonOperationKind.InPlaceSubtract,
      ["__pow__"] = PythonOperationKind.Power,
      ["__rpow__"] = PythonOperationKind.ReversePower,
      ["__ipow__"] = PythonOperationKind.InPlacePower,
      ["__mul__"] = PythonOperationKind.Multiply,
      ["__rmul__"] = PythonOperationKind.ReverseMultiply,
      ["__imul__"] = PythonOperationKind.InPlaceMultiply,
      ["__floordiv__"] = PythonOperationKind.FloorDivide,
      ["__rfloordiv__"] = PythonOperationKind.ReverseFloorDivide,
      ["__ifloordiv__"] = PythonOperationKind.InPlaceFloorDivide,
      ["__div__"] = PythonOperationKind.Divide,
      ["__rdiv__"] = PythonOperationKind.ReverseDivide,
      ["__idiv__"] = PythonOperationKind.InPlaceDivide,
      ["__truediv__"] = PythonOperationKind.TrueDivide,
      ["__rtruediv__"] = PythonOperationKind.ReverseTrueDivide,
      ["__itruediv__"] = PythonOperationKind.InPlaceTrueDivide,
      ["__mod__"] = PythonOperationKind.Mod,
      ["__rmod__"] = PythonOperationKind.ReverseMod,
      ["__imod__"] = PythonOperationKind.InPlaceMod,
      ["__lshift__"] = PythonOperationKind.LeftShift,
      ["__rlshift__"] = PythonOperationKind.ReverseLeftShift,
      ["__ilshift__"] = PythonOperationKind.InPlaceLeftShift,
      ["__rshift__"] = PythonOperationKind.RightShift,
      ["__rrshift__"] = PythonOperationKind.ReverseRightShift,
      ["__irshift__"] = PythonOperationKind.InPlaceRightShift,
      ["__and__"] = PythonOperationKind.BitwiseAnd,
      ["__rand__"] = PythonOperationKind.ReverseBitwiseAnd,
      ["__iand__"] = PythonOperationKind.InPlaceBitwiseAnd,
      ["__or__"] = PythonOperationKind.BitwiseOr,
      ["__ror__"] = PythonOperationKind.ReverseBitwiseOr,
      ["__ior__"] = PythonOperationKind.InPlaceBitwiseOr,
      ["__xor__"] = PythonOperationKind.ExclusiveOr,
      ["__rxor__"] = PythonOperationKind.ReverseExclusiveOr,
      ["__ixor__"] = PythonOperationKind.InPlaceExclusiveOr,
      ["__lt__"] = PythonOperationKind.LessThan,
      ["__gt__"] = PythonOperationKind.GreaterThan,
      ["__le__"] = PythonOperationKind.LessThanOrEqual,
      ["__ge__"] = PythonOperationKind.GreaterThanOrEqual,
      ["__eq__"] = PythonOperationKind.Equal,
      ["__ne__"] = PythonOperationKind.NotEqual,
      ["__lg__"] = PythonOperationKind.LessThanGreaterThan,
      ["__getitem__"] = PythonOperationKind.GetItem,
      ["__setitem__"] = PythonOperationKind.SetItem,
      ["__delitem__"] = PythonOperationKind.DeleteItem,
      ["__cmp__"] = PythonOperationKind.Compare,
      ["__pos__"] = PythonOperationKind.Positive,
      ["__neg__"] = PythonOperationKind.Negate,
      ["__invert__"] = PythonOperationKind.OnesComplement,
      ["__len__"] = PythonOperationKind.Length,
      ["__divmod__"] = PythonOperationKind.DivMod,
      ["__rdivmod__"] = PythonOperationKind.ReverseDivMod,
      ["__pow__"] = PythonOperationKind.Power,
      ["__contains__"] = PythonOperationKind.Contains,
      ["__abs__"] = PythonOperationKind.AbsoluteValue
    };
  }

  public static class _Object
  {
    public static readonly MethodInfo GetType = typeof (object).GetMethod(nameof (GetType));
  }

  public static class _IPythonObject
  {
    public static readonly PropertyInfo PythonType = typeof (IPythonObject).GetProperty(nameof (PythonType));
    public static readonly PropertyInfo Dict = typeof (IPythonObject).GetProperty(nameof (Dict));
  }

  public static class _PythonOps
  {
    public static readonly MethodInfo SlotTryGetBoundValue = typeof (PythonOps).GetMethod(nameof (SlotTryGetBoundValue));
    public static readonly MethodInfo GetTypeVersion = typeof (PythonOps).GetMethod(nameof (GetTypeVersion));
    public static readonly MethodInfo CheckTypeVersion = typeof (PythonOps).GetMethod(nameof (CheckTypeVersion));
  }

  public static class _OperationFailed
  {
    public static readonly FieldInfo Value = typeof (OperationFailed).GetField(nameof (Value));
  }

  public static class _PythonDictionary
  {
    public static readonly MethodInfo TryGetvalue = typeof (PythonDictionary).GetMethod("TryGetValue");
  }

  public static class _PythonGenerator
  {
    public static readonly ConstructorInfo Ctor = typeof (PythonGenerator).GetConstructor(new Type[1]
    {
      typeof (PythonFunction)
    });
  }

  private abstract class MemberResolver
  {
    public abstract MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name);

    public IList<ResolvedMember> ResolveMembers(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      Dictionary<string, ResolvedMember> dictionary = new Dictionary<string, ResolvedMember>();
      foreach (string candidateName in this.GetCandidateNames(binder, action, type))
      {
        if (!dictionary.ContainsKey(candidateName))
        {
          MemberGroup member = this.ResolveMember(binder, action, type, candidateName);
          if (member.Count > 0)
            dictionary[candidateName] = new ResolvedMember(candidateName, member);
        }
      }
      ResolvedMember[] array = new ResolvedMember[dictionary.Count];
      dictionary.Values.CopyTo(array, 0);
      return (IList<ResolvedMember>) array;
    }

    protected abstract IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type);
  }

  private class OneOffResolver : PythonTypeInfo.MemberResolver
  {
    private string _name;
    private Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> _resolver;

    public OneOffResolver(
      string name,
      Func<PythonTypeInfo.MemberBinder, Type, MemberGroup> resolver)
    {
      this._name = name;
      this._resolver = resolver;
    }

    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      return name == this._name ? this._resolver(binder, type) : MemberGroup.EmptyGroup;
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      yield return this._name;
    }
  }

  private class StandardResolver : PythonTypeInfo.MemberResolver
  {
    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      if (name == ".ctor" || name == ".cctor")
        return MemberGroup.EmptyGroup;
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        MemberGroup memberGroup = PythonTypeInfo.FilterSpecialNames(binder.GetMember(contributingType, name), name, action);
        if (memberGroup.Count > 0)
          return memberGroup;
      }
      if (type.IsInterface)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          MemberGroup memberGroup = PythonTypeInfo.FilterSpecialNames(binder.GetMember(type1, name), name, action);
          if (memberGroup.Count > 0)
            return memberGroup;
        }
      }
      return MemberGroup.EmptyGroup;
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        MemberInfo[] memberInfoArray = contributingType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        for (int index = 0; index < memberInfoArray.Length; ++index)
        {
          MemberInfo memberInfo = memberInfoArray[index];
          if (memberInfo.MemberType == MemberTypes.Method)
          {
            MethodInfo methodInfo = (MethodInfo) memberInfo;
            if (methodInfo.IsSpecialName)
            {
              if (methodInfo.IsDefined(typeof (PropertyMethodAttribute), true))
              {
                if (methodInfo.Name.StartsWith("Get") || methodInfo.Name.StartsWith("Set"))
                {
                  yield return methodInfo.Name.Substring(3);
                  continue;
                }
                yield return methodInfo.Name.Substring(6);
                continue;
              }
              continue;
            }
          }
          yield return memberInfo.Name;
        }
        memberInfoArray = (MemberInfo[]) null;
      }
    }
  }

  private class EqualityResolver : PythonTypeInfo.MemberResolver
  {
    public static readonly PythonTypeInfo.EqualityResolver Instance = new PythonTypeInfo.EqualityResolver();

    private EqualityResolver()
    {
    }

    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      bool flag;
      switch (name)
      {
        case "__eq__":
          flag = true;
          break;
        case "__ne__":
          flag = false;
          break;
        default:
          return MemberGroup.EmptyGroup;
      }
      return typeof (IStructuralEquatable).IsAssignableFrom(type) ? new MemberGroup((MemberTracker[]) PythonTypeInfo.GetEqualityMethods(type, flag ? "StructuralEqualityMethod" : "StructuralInequalityMethod")) : MemberGroup.EmptyGroup;
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      yield return "__eq__";
      yield return "__ne__";
    }
  }

  private class ComparisonResolver : PythonTypeInfo.MemberResolver
  {
    private readonly bool _excludePrimitiveTypes;
    private readonly Type _comparable;
    private readonly Dictionary<string, string> _helperMap;

    public ComparisonResolver(Type comparable, string helperPrefix)
    {
      this._excludePrimitiveTypes = comparable == typeof (IComparable);
      this._comparable = comparable;
      this._helperMap = new Dictionary<string, string>();
      this._helperMap["__eq__"] = helperPrefix + "Equality";
      this._helperMap["__ne__"] = helperPrefix + "Inequality";
      this._helperMap["__gt__"] = helperPrefix + "GreaterThan";
      this._helperMap["__lt__"] = helperPrefix + "LessThan";
      this._helperMap["__ge__"] = helperPrefix + "GreaterEqual";
      this._helperMap["__le__"] = helperPrefix + "LessEqual";
    }

    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      string name1;
      return this._excludePrimitiveTypes && (type.IsPrimitive() || type == typeof (BigInteger) || type == typeof (string) || type == typeof (Decimal)) || !this._helperMap.TryGetValue(name, out name1) || !this._comparable.IsAssignableFrom(type) ? MemberGroup.EmptyGroup : new MemberGroup((MemberTracker[]) PythonTypeInfo.GetEqualityMethods(type, name1));
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      return (IEnumerable<string>) this._helperMap.Keys;
    }
  }

  private class OperatorResolver : PythonTypeInfo.MemberResolver
  {
    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      if (type.IsSealed() && type.IsAbstract())
        return MemberGroup.EmptyGroup;
      PythonTypeInfo.EnsureOperatorTable();
      PythonOperationKind pythonOperationKind;
      if (PythonTypeInfo._pythonOperatorTable.TryGetValue(name, out pythonOperationKind) && PythonTypeInfo.IncludeOperatorMethod(type, pythonOperationKind))
      {
        OperatorMapping opInfo = !PythonTypeInfo.IsReverseOperator(pythonOperationKind) ? OperatorMapping.GetOperatorMapping(pythonOperationKind) : OperatorMapping.GetOperatorMapping(pythonOperationKind & ~PythonOperationKind.Reversed);
        if (opInfo != null)
        {
          foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
          {
            if (contributingType == typeof (double))
            {
              if ((opInfo.Operator & PythonOperationKind.Comparison) != PythonOperationKind.None)
                continue;
            }
            else if (contributingType == typeof (BigInteger))
            {
              if (opInfo.Operator == PythonOperationKind.Mod || opInfo.Operator == PythonOperationKind.RightShift || opInfo.Operator == PythonOperationKind.LeftShift || opInfo.Operator == PythonOperationKind.Compare || opInfo.Operator == PythonOperationKind.Divide)
                continue;
            }
            else if (contributingType == typeof (Complex) && opInfo.Operator == PythonOperationKind.Divide)
              continue;
            MemberGroup group = binder.GetMember(contributingType, opInfo.Name);
            if (group.Count == 0 && opInfo.AlternateName != null)
            {
              MemberGroup memberGroup = binder.GetMember(contributingType, opInfo.AlternateName);
              if (opInfo.AlternateName == "Equals")
                memberGroup = PythonTypeInfo.OperatorResolver.FilterObjectEquality(memberGroup);
              group = PythonTypeInfo.OperatorResolver.FilterAlternateMethods(opInfo, memberGroup);
            }
            if (group.Count > 0)
              return PythonTypeInfo.FilterForwardReverseMethods(name, group, type, pythonOperationKind);
          }
        }
      }
      if (name == "__call__")
      {
        MemberGroup member = binder.GetMember(type, "Call");
        if (member.Count > 0)
          return member;
      }
      return MemberGroup.EmptyGroup;
    }

    private static MemberGroup FilterAlternateMethods(OperatorMapping opInfo, MemberGroup res)
    {
      if (res.Count > 0 && opInfo.AlternateExpectedType != (Type) null)
      {
        List<MemberTracker> memberTrackerList = new List<MemberTracker>();
        for (int index = 0; index < res.Count; ++index)
        {
          MemberTracker re = res[index];
          if (re.MemberType == TrackerTypes.Method && ((MethodTracker) re).Method.ReturnType == opInfo.AlternateExpectedType)
            memberTrackerList.Add(re);
        }
        res = memberTrackerList.Count != 0 ? new MemberGroup(memberTrackerList.ToArray()) : MemberGroup.EmptyGroup;
      }
      return res;
    }

    private static MemberGroup FilterObjectEquality(MemberGroup group)
    {
      List<MemberTracker> memberTrackerList = (List<MemberTracker>) null;
      for (int index1 = 0; index1 < group.Count; ++index1)
      {
        MemberTracker memberTracker = group[index1];
        if (memberTracker.MemberType == TrackerTypes.Method && (memberTracker.DeclaringType == typeof (object) || memberTracker.DeclaringType == typeof (double) || memberTracker.DeclaringType == typeof (float)) && memberTracker.Name == "Equals")
        {
          if (memberTrackerList == null)
          {
            memberTrackerList = new List<MemberTracker>();
            for (int index2 = 0; index2 < index1; ++index2)
              memberTrackerList.Add(group[index2]);
          }
        }
        else if (memberTracker.MemberType == TrackerTypes.Method && memberTracker.DeclaringType == typeof (ValueType) && memberTracker.Name == "Equals")
        {
          if (memberTrackerList == null)
          {
            memberTrackerList = new List<MemberTracker>();
            for (int index3 = 0; index3 < index1; ++index3)
              memberTrackerList.Add(group[index3]);
          }
          memberTrackerList.Add(MemberTracker.FromMemberInfo((MemberInfo) typeof (object).GetMethod("Equals", new Type[1]
          {
            typeof (object)
          })));
        }
        else
          memberTrackerList?.Add(group[index1]);
      }
      if (memberTrackerList == null)
        return group;
      return memberTrackerList.Count == 0 ? MemberGroup.EmptyGroup : new MemberGroup(memberTrackerList.ToArray());
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      PythonTypeInfo.EnsureOperatorTable();
      foreach (string key in PythonTypeInfo._pythonOperatorTable.Keys)
        yield return key;
      yield return "__call__";
    }
  }

  private class PrivateBindingResolver : PythonTypeInfo.MemberResolver
  {
    private const BindingFlags _privateFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      if (binder.DomainManager.Configuration.PrivateBinding)
      {
        string str = $"_{type.Name}__";
        if (name.StartsWith(str))
        {
          string name1 = name.Substring(str.Length);
          MemberGroup members1 = new MemberGroup(type.GetMember(name1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));
          if (members1.Count > 0)
            return PythonTypeInfo.FilterFieldAndEvent(members1);
          MemberGroup members2 = new MemberGroup(type.GetMember(name1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
          if (members2.Count > 0)
            return PythonTypeInfo.FilterFieldAndEvent(members2);
        }
      }
      return MemberGroup.EmptyGroup;
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      if (binder.DomainManager.Configuration.PrivateBinding)
      {
        MemberInfo[] memberInfoArray = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        for (int index = 0; index < memberInfoArray.Length; ++index)
        {
          MemberInfo memberInfo = memberInfoArray[index];
          yield return $"_{memberInfo.DeclaringType.Name}__{memberInfo.Name}";
        }
        memberInfoArray = (MemberInfo[]) null;
      }
    }
  }

  private class ProtectedMemberResolver : PythonTypeInfo.MemberResolver
  {
    public override MemberGroup ResolveMember(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type,
      string name)
    {
      using (IEnumerator<Type> enumerator = binder.GetContributingTypes(type).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          MemberGroup group = new MemberGroup(((IEnumerable<MemberInfo>) enumerator.Current.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)).Where<MemberInfo>(new Func<MemberInfo, bool>(PythonTypeInfo.ProtectedOnly)).ToArray<MemberInfo>());
          for (int index1 = 0; index1 < group.Count; ++index1)
          {
            if (group[index1] is MethodTracker methodTracker && methodTracker.Name == "Finalize" && methodTracker.Method.GetBaseDefinition() == typeof (object).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic))
            {
              MemberTracker[] memberTrackerArray = new MemberTracker[group.Count - 1];
              if (group.Count == 1)
              {
                group = MemberGroup.EmptyGroup;
                break;
              }
              for (int index2 = 0; index2 < index1; ++index2)
                memberTrackerArray[index2] = group[index2];
              for (int index3 = index1 + 1; index3 < group.Count; ++index3)
                memberTrackerArray[index3 - 1] = group[index3];
              group = new MemberGroup(memberTrackerArray);
              break;
            }
          }
          return PythonTypeInfo.FilterSpecialNames(group, name, action);
        }
      }
      return MemberGroup.EmptyGroup;
    }

    protected override IEnumerable<string> GetCandidateNames(
      PythonTypeInfo.MemberBinder binder,
      MemberRequestKind action,
      Type type)
    {
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        MemberInfo[] memberInfoArray = contributingType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        for (int index = 0; index < memberInfoArray.Length; ++index)
        {
          MemberInfo input = memberInfoArray[index];
          if (input.MemberType == MemberTypes.Method)
          {
            MethodInfo methodInfo = (MethodInfo) input;
            if (methodInfo.IsSpecialName)
            {
              if (methodInfo.IsDefined(typeof (PropertyMethodAttribute), true) && PythonTypeInfo.ProtectedOnly(input))
              {
                if (methodInfo.Name.StartsWith("Get") || methodInfo.Name.StartsWith("Set"))
                {
                  yield return methodInfo.Name.Substring(3);
                  continue;
                }
                yield return methodInfo.Name.Substring(6);
                continue;
              }
              continue;
            }
          }
          if (PythonTypeInfo.ProtectedOnly(input))
            yield return input.Name;
        }
        memberInfoArray = (MemberInfo[]) null;
      }
    }
  }

  private class DocumentationDescriptor : PythonTypeSlot
  {
    internal override bool TryGetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      out object value)
    {
      if (owner.IsSystemType)
      {
        value = (object) PythonTypeOps.GetDocumentation(owner.UnderlyingSystemType);
        return true;
      }
      value = (object) null;
      return false;
    }

    internal override bool GetAlwaysSucceeds => true;

    internal override bool TrySetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      object value)
    {
      if (!(instance is IPythonObject self) || !self.PythonType.HasDictionary)
        return false;
      UserTypeOps.GetDictionary(self)[(object) "__doc__"] = value;
      return true;
    }
  }

  private class OneOffOperatorBinder
  {
    private string _methodName;
    private string _pythonName;
    private PythonOperationKind _op;

    public OneOffOperatorBinder(string methodName, string pythonName, PythonOperationKind opMap)
    {
      this._methodName = methodName;
      this._pythonName = pythonName;
      this._op = opMap;
    }

    public MemberGroup Resolver(PythonTypeInfo.MemberBinder binder, Type type)
    {
      if (type.IsSealed() && type.IsAbstract())
        return MemberGroup.EmptyGroup;
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        MemberGroup member = binder.GetMember(contributingType, this._methodName);
        if (member.Count > 0)
          return PythonTypeInfo.FilterForwardReverseMethods(this._pythonName, member, type, this._op);
      }
      return MemberGroup.EmptyGroup;
    }
  }

  private class OneOffPowerBinder
  {
    private string _pythonName;
    private PythonOperationKind _op;

    public OneOffPowerBinder(string pythonName, PythonOperationKind op)
    {
      this._pythonName = pythonName;
      this._op = op;
    }

    public MemberGroup Resolver(PythonTypeInfo.MemberBinder binder, Type type)
    {
      if (type.IsSealed() && type.IsAbstract())
        return MemberGroup.EmptyGroup;
      foreach (Type contributingType in (IEnumerable<Type>) binder.GetContributingTypes(type))
      {
        if (!(contributingType == typeof (BigInteger)))
        {
          MemberGroup member1 = binder.GetMember(contributingType, "op_Power");
          if (member1.Count > 0)
            return PythonTypeInfo.FilterForwardReverseMethods(this._pythonName, member1, type, this._op);
          MemberGroup member2 = binder.GetMember(contributingType, "Power");
          if (member2.Count > 0)
            return PythonTypeInfo.FilterForwardReverseMethods(this._pythonName, member2, type, this._op);
        }
      }
      return MemberGroup.EmptyGroup;
    }
  }

  private abstract class MemberBinder
  {
    private PythonBinder _binder;

    public MemberBinder(PythonBinder binder) => this._binder = binder;

    public abstract IList<Type> GetContributingTypes(Type t);

    public abstract IList<Type> GetInterfaces(Type t);

    public abstract MemberGroup GetBaseInstanceMethod(Type type, params string[] name);

    public abstract MemberGroup GetMember(Type type, string name);

    public PythonBinder Binder => this._binder;

    public ScriptDomainManager DomainManager => this._binder.DomainManager;

    protected MemberGroup GetMember(Type type, string name, BindingFlags flags)
    {
      IEnumerable<MemberInfo> memberInfos = (IEnumerable<MemberInfo>) type.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | flags);
      if (!this.Binder.DomainManager.Configuration.PrivateBinding)
        memberInfos = CompilerHelpers.FilterNonVisibleMembers(type, memberInfos);
      MemberInfo[] array = memberInfos.ToArray<MemberInfo>();
      List<MemberInfo> memberInfoList = (List<MemberInfo>) null;
      for (int index1 = 0; index1 < array.Length; ++index1)
      {
        if (array[index1].DeclaringType.IsDefined(typeof (PythonHiddenBaseClassAttribute), false))
        {
          if (memberInfoList == null)
          {
            memberInfoList = new List<MemberInfo>();
            for (int index2 = 0; index2 < index1; ++index2)
              memberInfoList.Add(array[index2]);
          }
        }
        else
          memberInfoList?.Add(array[index1]);
      }
      if (memberInfoList != null)
        array = memberInfoList.ToArray();
      MemberGroup collection = new MemberGroup(array);
      Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public | flags);
      string str = name + "`";
      List<Type> typeList = (List<Type>) null;
      foreach (Type type1 in nestedTypes)
      {
        if (type1.Name.StartsWith(str))
        {
          if (typeList == null)
            typeList = new List<Type>();
          typeList.Add(type1);
        }
      }
      if (typeList != null)
      {
        List<MemberTracker> memberTrackerList = new List<MemberTracker>((IEnumerable<MemberTracker>) collection);
        foreach (Type member in typeList)
          memberTrackerList.Add(MemberTracker.FromMemberInfo((MemberInfo) member));
        return new MemberGroup(memberTrackerList.ToArray());
      }
      if (collection.Count == 0)
        collection = (flags & BindingFlags.DeclaredOnly) != BindingFlags.Default ? this.Binder.GetExtensionMembers(type, name) : this.Binder.GetAllExtensionMembers(type, name);
      return collection;
    }
  }

  private class ResolveBinder(PythonBinder binder) : PythonTypeInfo.MemberBinder(binder)
  {
    public override IList<Type> GetInterfaces(Type t) => (IList<Type>) t.GetInterfaces();

    public override MemberGroup GetBaseInstanceMethod(Type type, params string[] name)
    {
      return PythonTypeInfo.GetInstanceOpsMethod(type, name);
    }

    public override IList<Type> GetContributingTypes(Type t)
    {
      List<Type> contributingTypes = new List<Type>();
      IList<PythonType> resolutionOrder = DynamicHelpers.GetPythonTypeFromType(t).ResolutionOrder;
      foreach (PythonType pythonType in (IEnumerable<PythonType>) resolutionOrder)
        contributingTypes.Add(pythonType.UnderlyingSystemType);
      foreach (PythonType pythonType in (IEnumerable<PythonType>) resolutionOrder)
        contributingTypes.AddRange((IEnumerable<Type>) this.Binder.GetExtensionTypesInternal(pythonType.UnderlyingSystemType));
      if (t.IsInterface())
      {
        foreach (Type type in t.GetInterfaces())
          contributingTypes.Add(type);
      }
      return (IList<Type>) contributingTypes;
    }

    public override MemberGroup GetMember(Type type, string name)
    {
      return this.GetMember(type, name, BindingFlags.Default);
    }
  }

  private class LookupBinder(PythonBinder binder) : PythonTypeInfo.MemberBinder(binder)
  {
    public override IList<Type> GetInterfaces(Type t)
    {
      if (t.IsInterface())
        return (IList<Type>) t.GetInterfaces();
      Type[] interfaces1 = t.GetInterfaces();
      List<Type> interfaces2 = new List<Type>();
      foreach (Type interfaceType in interfaces1)
      {
        try
        {
          foreach (MethodInfo targetMethod in t.GetInterfaceMap(interfaceType).TargetMethods)
          {
            if (targetMethod != (MethodInfo) null && targetMethod.DeclaringType == t)
            {
              interfaces2.Add(interfaceType);
              break;
            }
          }
        }
        catch (ArgumentException ex)
        {
        }
      }
      return (IList<Type>) interfaces2;
    }

    public override MemberGroup GetBaseInstanceMethod(Type type, params string[] name)
    {
      return type.BaseType == typeof (object) || type.BaseType == typeof (ValueType) ? PythonTypeInfo.GetInstanceOpsMethod(type, name) : MemberGroup.EmptyGroup;
    }

    public override IList<Type> GetContributingTypes(Type t)
    {
      List<Type> contributingTypes = new List<Type>();
      contributingTypes.Add(t);
      contributingTypes.AddRange((IEnumerable<Type>) this.Binder.GetExtensionTypesInternal(t));
      return (IList<Type>) contributingTypes;
    }

    public override MemberGroup GetMember(Type type, string name)
    {
      return this.GetMember(type, name, BindingFlags.DeclaredOnly);
    }
  }
}
