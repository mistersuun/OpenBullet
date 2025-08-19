// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionFind
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "find", Syntax = "db.<collection>.find [<filter>] [skip <N>] [limit <M>]", Description = "Search documents inside collection using filter clause. If filter omited, return all documents. Can be used with skip/limit to restrict results. Can be use an index or full scan query.", Examples = new string[] {"db.customers.find _id > 10", "db.customers.find _id != 1 and YEAR($.birthday) <= 1977", "db.customers.find name startsWith \"John\" skip 50 limit 25", "db.customers.find UPPER($.tags[*]) startsWith \"NEW\""})]
internal class CollectionFind : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "find");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    CollectionFind collectionFind = this;
    string collection = collectionFind.ReadCollection(engine, s);
    Query query = collectionFind.ReadQuery(s, false);
    KeyValuePair<int, int> keyValuePair = collectionFind.ReadSkipLimit(s);
    string[] includes = collectionFind.ReadIncludes(s);
    s.ThrowIfNotFinish();
    foreach (BsonValue bsonValue in engine.Find(collection, query, includes, keyValuePair.Key, keyValuePair.Value))
      yield return bsonValue;
  }
}
