// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonReaderException
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json;

[Serializable]
public class JsonReaderException : JsonException
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
