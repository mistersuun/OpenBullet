// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonSerializationException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
internal class JsonSerializationException : JsonException
{
  public JsonSerializationException()
  {
  }

  public JsonSerializationException(string message)
    : base(message)
  {
  }

  public JsonSerializationException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public JsonSerializationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  internal static JsonSerializationException Create(JsonReader reader, string message)
  {
    return JsonSerializationException.Create(reader, message, (Exception) null);
  }

  internal static JsonSerializationException Create(
    JsonReader reader,
    string message,
    Exception ex)
  {
    return JsonSerializationException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
  }

  internal static JsonSerializationException Create(
    IJsonLineInfo lineInfo,
    string path,
    string message,
    Exception ex)
  {
    message = JsonPosition.FormatMessage(lineInfo, path, message);
    return new JsonSerializationException(message, ex);
  }
}
