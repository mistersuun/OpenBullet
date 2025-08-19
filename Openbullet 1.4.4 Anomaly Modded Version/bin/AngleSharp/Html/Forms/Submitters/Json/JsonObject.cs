// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.Json.JsonObject
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters.Json;

internal sealed class JsonObject : JsonElement
{
  private readonly Dictionary<string, JsonElement> _properties = new Dictionary<string, JsonElement>();

  public override JsonElement this[string key]
  {
    get
    {
      JsonElement jsonElement;
      this._properties.TryGetValue(key.ToString(), out jsonElement);
      return jsonElement;
    }
    set => this._properties[key] = value;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = StringBuilderPool.Obtain().Append('{');
    bool flag = false;
    foreach (KeyValuePair<string, JsonElement> property in this._properties)
    {
      if (flag)
        stringBuilder.Append(',');
      stringBuilder.Append('"').Append(property.Key).Append('"');
      stringBuilder.Append(':').Append(property.Value.ToString());
      flag = true;
    }
    return stringBuilder.Append('}').ToPool();
  }
}
