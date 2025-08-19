// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonEmpty
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonEmpty : BsonToken
{
  public static readonly BsonToken Null = (BsonToken) new BsonEmpty(BsonType.Null);
  public static readonly BsonToken Undefined = (BsonToken) new BsonEmpty(BsonType.Undefined);

  private BsonEmpty(BsonType type) => this.Type = type;

  public override BsonType Type { get; }
}
