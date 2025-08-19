// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonContainerContract
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class JsonContainerContract : JsonContract
{
  private JsonContract _itemContract;
  private JsonContract _finalItemContract;

  internal JsonContract ItemContract
  {
    get => this._itemContract;
    set
    {
      this._itemContract = value;
      if (this._itemContract != null)
        this._finalItemContract = this._itemContract.UnderlyingType.IsSealed() ? this._itemContract : (JsonContract) null;
      else
        this._finalItemContract = (JsonContract) null;
    }
  }

  internal JsonContract FinalItemContract => this._finalItemContract;

  public JsonConverter ItemConverter { get; set; }

  public bool? ItemIsReference { get; set; }

  public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

  public TypeNameHandling? ItemTypeNameHandling { get; set; }

  internal JsonContainerContract(Type underlyingType)
    : base(underlyingType)
  {
    JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) underlyingType);
    if (cachedAttribute == null)
      return;
    if (cachedAttribute.ItemConverterType != (Type) null)
      this.ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(cachedAttribute.ItemConverterType, cachedAttribute.ItemConverterParameters);
    this.ItemIsReference = cachedAttribute._itemIsReference;
    this.ItemReferenceLoopHandling = cachedAttribute._itemReferenceLoopHandling;
    this.ItemTypeNameHandling = cachedAttribute._itemTypeNameHandling;
  }
}
