// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonValue
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonValue : BsonToken
{
  private readonly object _value;
  private readonly BsonType _type;

  public BsonValue(object value, BsonType type)
  {
    this._value = value;
    this._type = type;
  }

  public object Value => this._value;

  public override BsonType Type => this._type;
}
