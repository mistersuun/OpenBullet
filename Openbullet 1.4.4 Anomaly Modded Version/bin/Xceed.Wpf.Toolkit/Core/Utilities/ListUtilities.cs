// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ListUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class ListUtilities
{
  internal static Type GetListItemType(Type listType)
  {
    Type type = ((IEnumerable<Type>) listType.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IList<>)));
    return !(type != (Type) null) ? (Type) null : type.GetGenericArguments()[0];
  }

  internal static Type GetCollectionItemType(Type colType)
  {
    Type type = (!colType.IsGenericType ? 0 : (colType.GetGenericTypeDefinition() == typeof (ICollection<>) ? 1 : 0)) == 0 ? ((IEnumerable<Type>) colType.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICollection<>))) : colType;
    return !(type != (Type) null) ? (Type) null : type.GetGenericArguments()[0];
  }

  internal static Type[] GetDictionaryItemsType(Type dictType)
  {
    if ((!dictType.IsGenericType ? 0 : (dictType.GetGenericTypeDefinition() == typeof (Dictionary<,>) ? 1 : (dictType.GetGenericTypeDefinition() == typeof (IDictionary<,>) ? 1 : 0))) == 0)
      return (Type[]) null;
    return new Type[2]
    {
      dictType.GetGenericArguments()[0],
      dictType.GetGenericArguments()[1]
    };
  }

  internal static object CreateEditableKeyValuePair(
    object key,
    Type keyType,
    object value,
    Type valueType)
  {
    return Activator.CreateInstance(ListUtilities.CreateEditableKeyValuePairType(keyType, valueType), key, value);
  }

  internal static Type CreateEditableKeyValuePairType(Type keyType, Type valueType)
  {
    return typeof (EditableKeyValuePair<,>).MakeGenericType(keyType, valueType);
  }
}
