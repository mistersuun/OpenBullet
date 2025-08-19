// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonRegex
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonRegex : BsonToken
{
  public BsonString Pattern { get; set; }

  public BsonString Options { get; set; }

  public BsonRegex(string pattern, string options)
  {
    this.Pattern = new BsonString((object) pattern, false);
    this.Options = new BsonString((object) options, false);
  }

  public override BsonType Type => BsonType.Regex;
}
