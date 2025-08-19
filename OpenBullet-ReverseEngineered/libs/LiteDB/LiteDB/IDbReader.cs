// Decompiled with JetBrains decompiler
// Type: LiteDB.IDbReader
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace LiteDB;

internal interface IDbReader : IDisposable
{
  bool Initialize(Stream stream, string password);

  IEnumerable<string> GetCollections();

  IEnumerable<string> GetUniqueIndexes(string collection);

  IEnumerable<BsonDocument> GetDocuments(string collection);
}
