// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
internal class JsonException : Exception
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
