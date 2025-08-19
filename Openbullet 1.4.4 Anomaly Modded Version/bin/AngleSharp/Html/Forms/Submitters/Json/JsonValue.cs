// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.Json.JsonValue
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters.Json;

internal sealed class JsonValue : JsonElement
{
  private readonly string _value;

  public JsonValue(string value) => this._value = value.CssString();

  public JsonValue(double value)
  {
    this._value = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public JsonValue(bool value) => this._value = value ? "true" : "false";

  public override string ToString() => this._value;
}
