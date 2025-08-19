// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonSerializer
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class BsonSerializer
{
  public static byte[] Serialize(BsonDocument doc)
  {
    return !((BsonValue) doc == (BsonValue) null) ? new BsonWriter().Serialize(doc) : throw new ArgumentNullException(nameof (doc));
  }

  public static BsonDocument Deserialize(byte[] bson, bool utcDate = false)
  {
    return bson != null && bson.Length != 0 ? new BsonReader(utcDate).Deserialize(bson) : throw new ArgumentNullException(nameof (bson));
  }
}
