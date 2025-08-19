// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonString
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonString : BsonValue
{
  public int ByteCount { get; set; }

  public bool IncludeLength { get; }

  public BsonString(object value, bool includeLength)
    : base(value, BsonType.String)
  {
    this.IncludeLength = includeLength;
  }
}
