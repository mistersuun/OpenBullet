// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBoolean
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonBoolean : BsonValue
{
  public static readonly BsonBoolean False = new BsonBoolean(false);
  public static readonly BsonBoolean True = new BsonBoolean(true);

  private BsonBoolean(bool value)
    : base((object) value, BsonType.Boolean)
  {
  }
}
