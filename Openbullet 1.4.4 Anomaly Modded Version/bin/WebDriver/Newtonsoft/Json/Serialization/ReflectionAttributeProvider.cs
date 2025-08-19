// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ReflectionAttributeProvider
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class ReflectionAttributeProvider : IAttributeProvider
{
  private readonly object _attributeProvider;

  public ReflectionAttributeProvider(object attributeProvider)
  {
    ValidationUtils.ArgumentNotNull(attributeProvider, nameof (attributeProvider));
    this._attributeProvider = attributeProvider;
  }

  public IList<Attribute> GetAttributes(bool inherit)
  {
    return (IList<Attribute>) ReflectionUtils.GetAttributes(this._attributeProvider, (Type) null, inherit);
  }

  public IList<Attribute> GetAttributes(Type attributeType, bool inherit)
  {
    return (IList<Attribute>) ReflectionUtils.GetAttributes(this._attributeProvider, attributeType, inherit);
  }
}
