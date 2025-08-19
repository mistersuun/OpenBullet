// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ReflectionDelegateFactory
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal abstract class ReflectionDelegateFactory
{
  public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
  {
    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
    if (propertyInfo != (PropertyInfo) null)
      return this.CreateGet<T>(propertyInfo);
    FieldInfo fieldInfo = memberInfo as FieldInfo;
    return fieldInfo != (FieldInfo) null ? this.CreateGet<T>(fieldInfo) : throw new Exception("Could not create getter for {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberInfo));
  }

  public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
  {
    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
    if (propertyInfo != (PropertyInfo) null)
      return this.CreateSet<T>(propertyInfo);
    FieldInfo fieldInfo = memberInfo as FieldInfo;
    return fieldInfo != (FieldInfo) null ? this.CreateSet<T>(fieldInfo) : throw new Exception("Could not create setter for {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberInfo));
  }

  public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);

  public abstract ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method);

  public abstract Func<T> CreateDefaultConstructor<T>(Type type);

  public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);

  public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);

  public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);

  public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);
}
