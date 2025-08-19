// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonObjectAttribute
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class JsonObjectAttribute : JsonContainerAttribute
{
  private MemberSerialization _memberSerialization;
  internal MissingMemberHandling? _missingMemberHandling;
  internal Required? _itemRequired;
  internal NullValueHandling? _itemNullValueHandling;

  public MemberSerialization MemberSerialization
  {
    get => this._memberSerialization;
    set => this._memberSerialization = value;
  }

  public MissingMemberHandling MissingMemberHandling
  {
    get => this._missingMemberHandling ?? MissingMemberHandling.Ignore;
    set => this._missingMemberHandling = new MissingMemberHandling?(value);
  }

  public NullValueHandling ItemNullValueHandling
  {
    get => this._itemNullValueHandling ?? NullValueHandling.Include;
    set => this._itemNullValueHandling = new NullValueHandling?(value);
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
