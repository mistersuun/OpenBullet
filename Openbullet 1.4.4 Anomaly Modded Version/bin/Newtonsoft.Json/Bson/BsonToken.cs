// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonToken
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal abstract class BsonToken
{
  public abstract BsonType Type { get; }

  public BsonToken Parent { get; set; }

  public int CalculatedSize { get; set; }
}
