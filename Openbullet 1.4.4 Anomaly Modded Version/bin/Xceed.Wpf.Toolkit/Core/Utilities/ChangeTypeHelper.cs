// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ChangeTypeHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class ChangeTypeHelper
{
  internal static object ChangeType(object value, Type conversionType, IFormatProvider provider)
  {
    if (conversionType == (Type) null)
      throw new ArgumentNullException(nameof (conversionType));
    if (conversionType == typeof (Guid))
      return (object) new Guid(value.ToString());
    if (conversionType == typeof (Guid?))
      return value == null ? (object) null : (object) new Guid(value.ToString());
    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
    {
      if (value == null)
        return (object) null;
      conversionType = new NullableConverter(conversionType).UnderlyingType;
    }
    return Convert.ChangeType(value, conversionType, provider);
  }
}
