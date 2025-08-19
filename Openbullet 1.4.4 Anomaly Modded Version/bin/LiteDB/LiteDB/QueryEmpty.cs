// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryEmpty
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class QueryEmpty : Query
{
  public QueryEmpty()
    : base((string) null)
  {
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    yield break;
  }

  internal override bool FilterDocument(BsonDocument doc) => false;

  public override string ToString() => string.Format("(false)");
}
