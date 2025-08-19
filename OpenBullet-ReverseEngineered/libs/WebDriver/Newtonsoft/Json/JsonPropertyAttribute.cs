// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonPropertyAttribute
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
internal sealed class JsonPropertyAttribute : Attribute
{
  internal NullValueHandling? _nullValueHandling;
  internal DefaultValueHandling? _defaultValueHandling;
  internal ReferenceLoopHandling? _referenceLoopHandling;
  internal ObjectCreationHandling? _objectCreationHandling;
  internal TypeNameHandling? _typeNameHandling;
  internal bool? _isReference;
  internal int? _order;
  internal Required? _required;
  internal bool? _itemIsReference;
  internal ReferenceLoopHandling? _itemReferenceLoopHandling;
  internal TypeNameHandling? _itemTypeNameHandling;

  public Type ItemConverterType { get; set; }

  public object[] ItemConverterParameters { get; set; }

  public Type NamingStrategyType { get; set; }

  public object[] NamingStrategyParameters { get; set; }

  public NullValueHandling NullValueHandling
  {
    get => this._nullValueHandling ?? NullValueHandling.Include;
    set => this._nullValueHandling = new NullValueHandling?(value);
  }

  public DefaultValueHandling DefaultValueHandling
  {
    get => this._defaultValueHandling ?? DefaultValueHandling.Include;
    set => this._defaultValueHandling = new DefaultValueHandling?(value);
  }

  public ReferenceLoopHandling ReferenceLoopHandling
  {
    get => this._referenceLoopHandling ?? ReferenceLoopHandling.Error;
    set => this._referenceLoopHandling = new ReferenceLoopHandling?(value);
  }

  public ObjectCreationHandling ObjectCreationHandling
  {
    get => this._objectCreationHandling ?? ObjectCreationHandling.Auto;
    set => this._objectCreationHandling = new ObjectCreationHandling?(value);
  }

  public TypeNameHandling TypeNameHandling
  {
    get => this._typeNameHandling ?? TypeNameHandling.None;
    set => this._typeNameHandling = new TypeNameHandling?(value);
  }

  public bool IsReference
  {
    get => this._isReference ?? false;
    set => this._isReference = new bool?(value);
  }

  public int Order
  {
    get => this._order ?? 0;
    set => this._order = new int?(value);
  }

  public Required Required
  {
    get => this._required ?? Required.Default;
    set => this._required = new Required?(value);
  }

  public string PropertyName { get; set; }

  public ReferenceLoopHandling ItemReferenceLoopHandling
  {
    get => this._itemReferenceLoopHandling ?? ReferenceLoopHandling.Error;
    set => this._itemReferenceLoopHandling = new ReferenceLoopHandling?(value);
  }

  public TypeNameHandling ItemTypeNameHandling
  {
    get => this._itemTypeNameHandling ?? TypeNameHandling.None;
    set => this._itemTypeNameHandling = new TypeNameHandling?(value);
  }

  public bool ItemIsReference
  {
    get => this._itemIsReference ?? false;
    set => this._itemIsReference = new bool?(value);
  }

  public JsonPropertyAttribute()
  {
  }

  public JsonPropertyAttribute(string propertyName) => this.PropertyName = propertyName;
}
