// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.ValidationEventArgs
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;
using System;

#nullable disable
namespace Newtonsoft.Json.Schema;

[Obsolete("JSON Schema validation has been moved to its own package. See https://www.newtonsoft.com/jsonschema for more details.")]
public class ValidationEventArgs : EventArgs
{
  private readonly JsonSchemaException _ex;

  internal ValidationEventArgs(JsonSchemaException ex)
  {
    ValidationUtils.ArgumentNotNull((object) ex, nameof (ex));
    this._ex = ex;
  }

  public JsonSchemaException Exception => this._ex;

  public string Path => this._ex.Path;

  public string Message => this._ex.Message;
}
