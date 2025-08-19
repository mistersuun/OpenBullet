// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.JsonTokenUtils
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal static class JsonTokenUtils
{
  internal static bool IsEndToken(JsonToken token)
  {
    switch (token)
    {
      case JsonToken.EndObject:
      case JsonToken.EndArray:
      case JsonToken.EndConstructor:
        return true;
      default:
        return false;
    }
  }

  internal static bool IsStartToken(JsonToken token)
  {
    switch (token)
    {
      case JsonToken.StartObject:
      case JsonToken.StartArray:
      case JsonToken.StartConstructor:
        return true;
      default:
        return false;
    }
  }

  internal static bool IsPrimitiveToken(JsonToken token)
  {
    switch (token)
    {
      case JsonToken.Integer:
      case JsonToken.Float:
      case JsonToken.String:
      case JsonToken.Boolean:
      case JsonToken.Null:
      case JsonToken.Undefined:
      case JsonToken.Date:
      case JsonToken.Bytes:
        return true;
      default:
        return false;
    }
  }
}
