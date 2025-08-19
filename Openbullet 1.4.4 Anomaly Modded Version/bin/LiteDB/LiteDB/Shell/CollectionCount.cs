// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionCount
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "count", Syntax = "db.<collection>.count [filter]", Description = "Show count rows according query filter", Examples = new string[] {"db.orders.count", "db.orders.count customer = \"John Doe\"", "db.orders.count customer startsWith \"John\" and YEAR($.orderDate) >= 2015"})]
internal class CollectionCount : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "count");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CollectionCount collectionCount = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    string collection = collectionCount.ReadCollection(engine, s);
    Query query = collectionCount.ReadQuery(s, false);
    s.ThrowIfNotFinish();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (BsonValue) engine.Count(collection, query);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
