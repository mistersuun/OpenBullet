// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Schema;

[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
[Serializable]
internal class JsonSchemaException : JsonException
{
  public int LineNumber { get; }

  public int LinePosition { get; }

  public string Path { get; }

  public JsonSchemaException()
  {
  }

  public JsonSchemaException(string message)
    : base(message)
  {
  }

  public JsonSchemaException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public JsonSchemaException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  internal JsonSchemaException(
    string message,
    Exception innerException,
    string path,
    int lineNumber,
    int linePosition)
    : base(message, innerException)
  {
    this.Path = path;
    this.LineNumber = lineNumber;
    this.LinePosition = linePosition;
  }
}
