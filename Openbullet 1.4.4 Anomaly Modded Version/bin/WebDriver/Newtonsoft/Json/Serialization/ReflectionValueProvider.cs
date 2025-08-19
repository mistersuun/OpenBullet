// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ReflectionValueProvider
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class ReflectionValueProvider : IValueProvider
{
  private readonly MemberInfo _memberInfo;

  public ReflectionValueProvider(MemberInfo memberInfo)
  {
    ValidationUtils.ArgumentNotNull((object) memberInfo, nameof (memberInfo));
    this._memberInfo = memberInfo;
  }

  public void SetValue(object target, object value)
  {
    try
    {
      ReflectionUtils.SetMemberValue(this._memberInfo, target, value);
    }
    catch (Exception ex)
    {
      throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._memberInfo.Name, (object) target.GetType()), ex);
    }
  }

  public object GetValue(object target)
  {
    try
    {
      return ReflectionUtils.GetMemberValue(this._memberInfo, target);
    }
    catch (Exception ex)
    {
      throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._memberInfo.Name, (object) target.GetType()), ex);
    }
  }
}
