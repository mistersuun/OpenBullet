// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionDelete
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "delete", Syntax = "db.<collection>.delete [filter]", Description = "Delete documents according filter clause (required). Retruns deleted document count.", Examples = new string[] {"db.orders.delete _id = 2", "db.orders.delete customer = \"John Doe\"", "db.orders.delete customer startsWith \"John\" and YEAR($.orderDate) >= 2015"})]
internal class CollectionDelete : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "delete");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CollectionDelete collectionDelete = this;
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
    string collection = collectionDelete.ReadCollection(engine, s);
    Query query = collectionDelete.ReadQuery(s, true);
    s.ThrowIfNotFinish();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (BsonValue) engine.Delete(collection, query);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
