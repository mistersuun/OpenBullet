// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBoolean
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
