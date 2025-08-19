// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonConverter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

internal abstract class JsonConverter
{
  public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);

  public abstract object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer);

  public abstract bool CanConvert(Type objectType);

  public virtual bool CanRead => true;

  public virtual bool CanWrite => true;
}
