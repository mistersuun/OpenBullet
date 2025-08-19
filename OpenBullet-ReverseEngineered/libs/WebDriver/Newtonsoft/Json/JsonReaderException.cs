// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonReaderException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
internal class JsonReaderException : JsonException
{
  public int LineNumber { get; }

  public int LinePosition { get; }

  public string Path { get; }

  public JsonReaderException()
  {
  }

  public JsonReaderException(string message)
    : base(message)
  {
  }

  public JsonReaderException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public JsonReaderException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public JsonReaderException(
    string message,
    string path,
    int lineNumber,
    int linePosition,
    Exception innerException)
    : base(message, innerException)
  {
    this.Path = path;
    this.LineNumber = lineNumber;
    this.LinePosition = linePosition;
  }

  internal static JsonReaderException Create(JsonReader reader, string message)
  {
    return JsonReaderException.Create(reader, message, (Exception) null);
  }

  internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
  {
    return JsonReaderException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
  }

  internal static JsonReaderException Create(
    IJsonLineInfo lineInfo,
    string path,
    string message,
    Exception ex)
  {
    message = JsonPosition.FormatMessage(lineInfo, path, message);
    int lineNumber;
    int linePosition;
    if (lineInfo != null && lineInfo.HasLineInfo())
    {
      lineNumber = lineInfo.LineNumber;
      linePosition = lineInfo.LinePosition;
    }
    else
    {
      lineNumber = 0;
      linePosition = 0;
    }
    return new JsonReaderException(message, path, lineNumber, linePosition, ex);
  }
}
