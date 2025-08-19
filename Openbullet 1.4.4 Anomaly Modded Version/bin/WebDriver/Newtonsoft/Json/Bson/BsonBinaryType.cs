// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBinaryType
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Bson;

internal enum BsonBinaryType : byte
{
  Binary = 0,
  Function = 1,
  [Obsolete("This type has been deprecated in the BSON specification. Use Binary instead.")] BinaryOld = 2,
  [Obsolete("This type has been deprecated in the BSON specification. Use Uuid instead.")] UuidOld = 3,
  Uuid = 4,
  Md5 = 5,
  UserDefined = 128, // 0x80
}
