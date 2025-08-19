// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBinaryType
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
