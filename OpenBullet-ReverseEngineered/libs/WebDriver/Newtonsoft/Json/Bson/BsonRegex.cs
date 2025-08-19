// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonRegex
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
