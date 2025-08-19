// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.CharArrayJsonConverter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class CharArrayJsonConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return objectType != (Type) null && objectType.IsAssignableFrom(typeof (char[]));
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    if (writer == null)
      return;
    writer.WriteStartArray();
    if (value is char[] chArray)
    {
      for (int index = 0; index < chArray.Length; ++index)
      {
        char ch = chArray[index];
        int int32 = Convert.ToInt32(ch);
        if (int32 >= 32 /*0x20*/ && int32 <= 126)
        {
          writer.WriteValue(ch);
        }
        else
        {
          string str = "\\u" + Convert.ToString(int32, 16 /*0x10*/).PadLeft(4, '0');
          writer.WriteRawValue($"\"{str}\"");
        }
      }
    }
    writer.WriteEndArray();
  }

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
    throw new NotImplementedException();
  }
}
