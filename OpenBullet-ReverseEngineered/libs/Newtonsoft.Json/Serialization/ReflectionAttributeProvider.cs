// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ReflectionAttributeProvider
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization;

public class ReflectionAttributeProvider : IAttributeProvider
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
