// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaType
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Schema;

[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
[Flags]
internal enum JsonSchemaType
{
  None = 0,
  String = 1,
  Float = 2,
  Integer = 4,
  Boolean = 8,
  Object = 16, // 0x00000010
  Array = 32, // 0x00000020
  Null = 64, // 0x00000040
  Any = Null | Array | Object | Boolean | Integer | Float | String, // 0x0000007F
}
