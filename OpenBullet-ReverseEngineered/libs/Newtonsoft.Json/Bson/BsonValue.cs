// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonValue
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
