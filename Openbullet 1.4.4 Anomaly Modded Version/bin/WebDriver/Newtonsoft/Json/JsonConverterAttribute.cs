// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonConverterAttribute
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
internal sealed class JsonConverterAttribute : Attribute
{
  private readonly Type _converterType;

  public Type ConverterType => this._converterType;

  public object[] ConverterParameters { get; }

  public JsonConverterAttribute(Type converterType)
  {
    this._converterType = !(converterType == (Type) null) ? converterType : throw new ArgumentNullException(nameof (converterType));
  }

  public JsonConverterAttribute(Type converterType, params object[] converterParameters)
    : this(converterType)
  {
    this.ConverterParameters = converterParameters;
  }
}
