// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonEmpty
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonEmpty : BsonToken
{
  public static readonly BsonToken Null = (BsonToken) new BsonEmpty(BsonType.Null);
  public static readonly BsonToken Undefined = (BsonToken) new BsonEmpty(BsonType.Undefined);

  private BsonEmpty(BsonType type) => this.Type = type;

  public override BsonType Type { get; }
}
