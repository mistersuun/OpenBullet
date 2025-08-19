// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonObjectAttribute
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
internal sealed class JsonObjectAttribute : JsonContainerAttribute
{
  private MemberSerialization _memberSerialization;
  internal Required? _itemRequired;

  public MemberSerialization MemberSerialization
  {
    get => this._memberSerialization;
    set => this._memberSerialization = value;
  }

  public Required ItemRequired
  {
    get => this._itemRequired ?? Required.Default;
    set => this._itemRequired = new Required?(value);
  }

  public JsonObjectAttribute()
  {
  }

  public JsonObjectAttribute(MemberSerialization memberSerialization)
  {
    this.MemberSerialization = memberSerialization;
  }

  public JsonObjectAttribute(string id)
    : base(id)
  {
  }
}
