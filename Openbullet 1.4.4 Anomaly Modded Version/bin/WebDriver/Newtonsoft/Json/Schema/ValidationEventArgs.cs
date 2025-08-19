// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.ValidationEventArgs
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;

#nullable disable
namespace Newtonsoft.Json.Schema;

[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
internal class ValidationEventArgs : EventArgs
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
