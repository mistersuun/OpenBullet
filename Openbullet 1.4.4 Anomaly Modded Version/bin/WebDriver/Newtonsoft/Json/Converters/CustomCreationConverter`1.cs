// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.CustomCreationConverter`1
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal abstract class CustomCreationConverter<T> : JsonConverter
{
  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
  }

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
    if (reader.TokenType == JsonToken.Null)
      return (object) null;
    T target = this.Create(objectType);
    if ((object) target == null)
      throw new JsonSerializationException("No object created.");
    serializer.Populate(reader, (object) target);
    return (object) target;
  }

  public abstract T Create(Type objectType);

  public override bool CanConvert(Type objectType) => typeof (T).IsAssignableFrom(objectType);

  public override bool CanWrite => false;
}
