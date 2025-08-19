// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonWriterException
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
public class JsonWriterException : JsonException
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
