// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.Json.JsonArray
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters.Json;

internal sealed class JsonArray : JsonElement, IEnumerable<JsonElement>, IEnumerable
{
  private readonly List<JsonElement> _elements = new List<JsonElement>();

  public int Length => this._elements.Count;

  public void Push(JsonElement element) => this._elements.Add(element);

  public JsonElement this[int key]
  {
    get => this._elements.ElementAtOrDefault<JsonElement>(key);
    set
    {
      for (int count = this._elements.Count; count <= key; ++count)
        this._elements.Add((JsonElement) null);
      this._elements[key] = value;
    }
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = StringBuilderPool.Obtain().Append('[');
    bool flag = false;
    foreach (JsonElement element in this._elements)
    {
      if (flag)
        stringBuilder.Append(',');
      stringBuilder.Append(element?.ToString() ?? "null");
      flag = true;
    }
    return stringBuilder.Append(']').ToPool();
  }

  public IEnumerator<JsonElement> GetEnumerator()
  {
    return (IEnumerator<JsonElement>) this._elements.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
