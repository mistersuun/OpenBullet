// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonWriterException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
internal class JsonWriterException : JsonException
{
  public string Path { get; }

  public JsonWriterException()
  {
  }

  public JsonWriterException(string message)
    : base(message)
  {
  }

  public JsonWriterException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public JsonWriterException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public JsonWriterException(string message, string path, Exception innerException)
    : base(message, innerException)
  {
    this.Path = path;
  }

  internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex)
  {
    return JsonWriterException.Create(writer.ContainerPath, message, ex);
  }

  internal static JsonWriterException Create(string path, string message, Exception ex)
  {
    message = JsonPosition.FormatMessage((IJsonLineInfo) null, path, message);
    return new JsonWriterException(message, path, ex);
  }
}
