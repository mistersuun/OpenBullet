// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonArrayAttribute
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
internal sealed class JsonArrayAttribute : JsonContainerAttribute
{
  private bool _allowNullItems;

  public bool AllowNullItems
  {
    get => this._allowNullItems;
    set => this._allowNullItems = value;
  }

  public JsonArrayAttribute()
  {
  }

  public JsonArrayAttribute(bool allowNullItems) => this._allowNullItems = allowNullItems;

  public JsonArrayAttribute(string id)
    : base(id)
  {
  }
}
