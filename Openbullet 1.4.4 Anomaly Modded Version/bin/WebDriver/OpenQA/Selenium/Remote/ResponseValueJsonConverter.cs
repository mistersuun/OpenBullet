// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.ResponseValueJsonConverter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class ResponseValueJsonConverter : JsonConverter
{
  public override bool CanConvert(Type objectType) => true;

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
    return this.ProcessToken(reader);
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    serializer?.Serialize(writer, value);
  }

  private object ProcessToken(JsonReader reader)
  {
    object obj = (object) null;
    if (reader != null)
    {
      reader.DateParseHandling = DateParseHandling.None;
      if (reader.TokenType == JsonToken.StartObject)
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        while (reader.Read() && reader.TokenType != JsonToken.EndObject)
        {
          string key = reader.Value.ToString();
          reader.Read();
          dictionary.Add(key, this.ProcessToken(reader));
        }
        obj = (object) dictionary;
      }
      else if (reader.TokenType == JsonToken.StartArray)
      {
        List<object> objectList = new List<object>();
        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
          objectList.Add(this.ProcessToken(reader));
        obj = (object) objectList.ToArray();
      }
      else
        obj = reader.Value;
    }
    return obj;
  }
}
