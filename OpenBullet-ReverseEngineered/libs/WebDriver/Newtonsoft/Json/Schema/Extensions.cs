// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.Extensions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Schema;

[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
internal static class Extensions
{
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public static bool IsValid(this JToken source, JsonSchema schema)
  {
    bool valid = true;
    source.Validate(schema, (ValidationEventHandler) ((sender, args) => valid = false));
    return valid;
  }

  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public static bool IsValid(
    this JToken source,
    JsonSchema schema,
    out IList<string> errorMessages)
  {
    IList<string> errors = (IList<string>) new List<string>();
    source.Validate(schema, (ValidationEventHandler) ((sender, args) => errors.Add(args.Message)));
    errorMessages = errors;
    return errorMessages.Count == 0;
  }

  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public static void Validate(this JToken source, JsonSchema schema)
  {
    source.Validate(schema, (ValidationEventHandler) null);
  }

  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public static void Validate(
    this JToken source,
    JsonSchema schema,
    ValidationEventHandler validationEventHandler)
  {
    ValidationUtils.ArgumentNotNull((object) source, nameof (source));
    ValidationUtils.ArgumentNotNull((object) schema, nameof (schema));
    using (JsonValidatingReader validatingReader = new JsonValidatingReader(source.CreateReader()))
    {
      validatingReader.Schema = schema;
      if (validationEventHandler != null)
        validatingReader.ValidationEventHandler += validationEventHandler;
      do
        ;
      while (validatingReader.Read());
    }
  }
}
