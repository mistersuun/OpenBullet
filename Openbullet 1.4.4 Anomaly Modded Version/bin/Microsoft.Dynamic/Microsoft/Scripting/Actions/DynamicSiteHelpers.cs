// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.DynamicSiteHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Actions;

public static class DynamicSiteHelpers
{
  private const int MaximumArity = 17;
  private static Dictionary<ICollection<Type>, Type> _DelegateTypes;

  public static Type MakeCallSiteDelegate(params Type[] types)
  {
    return DynamicSiteHelpers.MakeDelegate(((IList<Type>) types).AddFirst<Type>(typeof (CallSite)));
  }

  private static Type MakeDelegate(Type[] types)
  {
    Type standardDelegateType = DynamicSiteHelpers.GetStandardDelegateType(types);
    return (object) standardDelegateType != null ? standardDelegateType : DynamicSiteHelpers.MakeCustomDelegate(types);
  }

  public static Type GetStandardDelegateType(Type[] types)
  {
    ContractUtils.RequiresNotEmpty<Type>((ICollection<Type>) types, nameof (types));
    return types.Length > 17 || ((IEnumerable<Type>) types).Any<Type>((Func<Type, bool>) (t => t.IsByRef)) ? (Type) null : (!(types[types.Length - 1] == typeof (void)) ? DynamicSiteHelpers.GetFuncType(types) : DynamicSiteHelpers.GetActionType(types.RemoveLast<Type>()));
  }

  private static Type GetFuncType(Type[] types)
  {
    switch (types.Length)
    {
      case 1:
        return typeof (Func<>).MakeGenericType(types);
      case 2:
        return typeof (Func<,>).MakeGenericType(types);
      case 3:
        return typeof (Func<,,>).MakeGenericType(types);
      case 4:
        return typeof (Func<,,,>).MakeGenericType(types);
      case 5:
        return typeof (Func<,,,,>).MakeGenericType(types);
      case 6:
        return typeof (Func<,,,,,>).MakeGenericType(types);
      case 7:
        return typeof (Func<,,,,,,>).MakeGenericType(types);
      case 8:
        return typeof (Func<,,,,,,,>).MakeGenericType(types);
      case 9:
        return typeof (Func<,,,,,,,,>).MakeGenericType(types);
      case 10:
        return typeof (Func<,,,,,,,,,>).MakeGenericType(types);
      case 11:
        return typeof (Func<,,,,,,,,,,>).MakeGenericType(types);
      case 12:
        return typeof (Func<,,,,,,,,,,,>).MakeGenericType(types);
      case 13:
        return typeof (Func<,,,,,,,,,,,,>).MakeGenericType(types);
      case 14:
        return typeof (Func<,,,,,,,,,,,,,>).MakeGenericType(types);
      case 15:
        return typeof (Func<,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 16 /*0x10*/:
        return typeof (Func<,,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 17:
        return typeof (Func<,,,,,,,,,,,,,,,,>).MakeGenericType(types);
      default:
        return (Type) null;
    }
  }

  private static Type GetActionType(Type[] types)
  {
    switch (types.Length)
    {
      case 0:
        return typeof (Action);
      case 1:
        return typeof (Action<>).MakeGenericType(types);
      case 2:
        return typeof (Action<,>).MakeGenericType(types);
      case 3:
        return typeof (Action<,,>).MakeGenericType(types);
      case 4:
        return typeof (Action<,,,>).MakeGenericType(types);
      case 5:
        return typeof (Action<,,,,>).MakeGenericType(types);
      case 6:
        return typeof (Action<,,,,,>).MakeGenericType(types);
      case 7:
        return typeof (Action<,,,,,,>).MakeGenericType(types);
      case 8:
        return typeof (Action<,,,,,,,>).MakeGenericType(types);
      case 9:
        return typeof (Action<,,,,,,,,>).MakeGenericType(types);
      case 10:
        return typeof (Action<,,,,,,,,,>).MakeGenericType(types);
      case 11:
        return typeof (Action<,,,,,,,,,,>).MakeGenericType(types);
      case 12:
        return typeof (Action<,,,,,,,,,,,>).MakeGenericType(types);
      case 13:
        return typeof (Action<,,,,,,,,,,,,>).MakeGenericType(types);
      case 14:
        return typeof (Action<,,,,,,,,,,,,,>).MakeGenericType(types);
      case 15:
        return typeof (Action<,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 16 /*0x10*/:
        return typeof (Action<,,,,,,,,,,,,,,,>).MakeGenericType(types);
      default:
        return (Type) null;
    }
  }

  private static T[] AddFirst<T>(this IList<T> list, T item)
  {
    T[] array = new T[list.Count + 1];
    array[0] = item;
    list.CopyTo(array, 1);
    return array;
  }

  private static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
  {
    foreach (T obj in source)
    {
      if (predicate(obj))
        return true;
    }
    return false;
  }

  private static T[] RemoveLast<T>(this T[] array)
  {
    T[] destinationArray = new T[array.Length - 1];
    Array.Copy((Array) array, 0, (Array) destinationArray, 0, destinationArray.Length);
    return destinationArray;
  }

  private static Type MakeCustomDelegate(Type[] types)
  {
    if (DynamicSiteHelpers._DelegateTypes == null)
      Interlocked.CompareExchange<Dictionary<ICollection<Type>, Type>>(ref DynamicSiteHelpers._DelegateTypes, new Dictionary<ICollection<Type>, Type>((IEqualityComparer<ICollection<Type>>) ListEqualityComparer<Type>.Instance), (Dictionary<ICollection<Type>, Type>) null);
    Type type1;
    bool flag;
    lock (DynamicSiteHelpers._DelegateTypes)
      flag = DynamicSiteHelpers._DelegateTypes.TryGetValue((ICollection<Type>) types, out type1);
    if (!flag && type1 != (Type) null)
      return type1;
    Type type2 = DynamicSiteHelpers.MakeNewCustomDelegate(types);
    lock (DynamicSiteHelpers._DelegateTypes)
    {
      Type type3;
      if (DynamicSiteHelpers._DelegateTypes.TryGetValue((ICollection<Type>) types, out type3) && type3 != (Type) null)
        type2 = type3;
      else
        DynamicSiteHelpers._DelegateTypes[(ICollection<Type>) types] = type2;
    }
    return type2;
  }

  private static Type MakeNewCustomDelegate(Type[] types)
  {
    Type type = types[types.Length - 1];
    Type[] typeArray = types.RemoveLast<Type>();
    return Snippets.Shared.DefineDelegate("Delegate" + (object) types.Length, type, typeArray);
  }

  public static bool IsInvisibleDlrStackFrame(MethodBase mb)
  {
    return mb.Name == "_Scripting_" || mb.DeclaringType != (Type) null && mb.DeclaringType.Namespace != null && mb.DeclaringType.Namespace.StartsWith("Microsoft.Scripting", StringComparison.Ordinal) || CallSiteHelpers.IsInternalFrame(mb);
  }

  private delegate object CreateSite(CallSiteBinder binder);
}
