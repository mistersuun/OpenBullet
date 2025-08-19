// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionDropIndex
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "dropIndex", Syntax = "db.<collection>.dropIndex [field|index]", Description = "Drop an index and make index area free to use with another. Returns true if index has been deleted.", Examples = new string[] {"db.orders.dropIndex customerName", "db.orders.dropIndex index_name"})]
internal class CollectionDropIndex : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "drop[iI]ndex");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CollectionDropIndex collectionDropIndex = this;
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
    string collection = collectionDropIndex.ReadCollection(engine, s);
    string field = s.Scan(collectionDropIndex.FieldPattern).Trim().ThrowIfEmpty("Missing field index name", s);
    s.ThrowIfNotFinish();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (BsonValue) engine.DropIndex(collection, field);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
