// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBinary
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonBinary : BsonValue
{
  public BsonBinaryType BinaryType { get; set; }

  public BsonBinary(byte[] value, BsonBinaryType binaryType)
    : base((object) value, BsonType.Binary)
  {
    this.BinaryType = binaryType;
  }
}
