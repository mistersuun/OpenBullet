// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.DelegateHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal static class DelegateHelpers
{
  private static Dictionary<ICollection<Type>, Type> _DelegateTypes;
  private const int MaximumArity = 17;

  private static Type MakeCustomDelegate(Type[] types)
  {
    if (DelegateHelpers._DelegateTypes == null)
      Interlocked.CompareExchange<Dictionary<ICollection<Type>, Type>>(ref DelegateHelpers._DelegateTypes, new Dictionary<ICollection<Type>, Type>((IEqualityComparer<ICollection<Type>>) ListEqualityComparer<Type>.Instance), (Dictionary<ICollection<Type>, Type>) null);
    Type type1;
    bool flag;
    lock (DelegateHelpers._DelegateTypes)
      flag = DelegateHelpers._DelegateTypes.TryGetValue((ICollection<Type>) types, out type1);
    if (!flag && type1 != (Type) null)
      return type1;
    Type type2 = DelegateHelpers.MakeNewCustomDelegate(types);
    lock (DelegateHelpers._DelegateTypes)
    {
      Type type3;
      if (DelegateHelpers._DelegateTypes.TryGetValue((ICollection<Type>) types, out type3) && type3 != (Type) null)
        type2 = type3;
      else
        DelegateHelpers._DelegateTypes[(ICollection<Type>) types] = type2;
    }
    return type2;
  }

  private static Type MakeNewCustomDelegate(Type[] types)
  {
    Type type = types[types.Length - 1];
    Type[] typeArray = types.RemoveLast<Type>();
    return Snippets.Shared.DefineDelegate("Delegate" + (object) types.Length, type, typeArray);
  }

  internal static Type MakeDelegate(Type[] types)
  {
    if (types.Length > 17 || ((IEnumerable<Type>) types).Any<Type>((Func<Type, bool>) (t => t.IsByRef)))
      return DelegateHelpers.MakeCustomDelegate(types);
    if (types[types.Length - 1] == typeof (void))
    {
      types = types.RemoveLast<Type>();
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
      }
    }
    else
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
      }
    }
    throw Assert.Unreachable;
  }
}
