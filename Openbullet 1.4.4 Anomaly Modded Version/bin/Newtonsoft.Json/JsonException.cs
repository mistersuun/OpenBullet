// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonException
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
public class JsonException : Exception
{
  public JsonException()
  {
  }

  public JsonException(string message)
    : base(message)
  {
  }

  public JsonException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public JsonException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  internal static JsonException Create(IJsonLineInfo lineInfo, string path, string message)
  {
    message = JsonPosition.FormatMessage(lineInfo, path, message);
    return new JsonException(message);
  }
}
