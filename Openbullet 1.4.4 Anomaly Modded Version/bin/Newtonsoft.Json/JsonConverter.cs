// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonConverter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

public abstract class JsonConverter
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
