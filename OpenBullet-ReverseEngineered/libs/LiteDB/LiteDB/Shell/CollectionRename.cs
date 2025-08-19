// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionRename
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "rename", Syntax = "db.<collection>.rename <new_name>", Description = "Rename a collection. New name can't exists", Examples = new string[] {"db.customers.rename new_cust"})]
internal class CollectionRename : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "rename");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CollectionRename collectionRename = this;
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
    string collection = collectionRename.ReadCollection(engine, s);
    string newName = s.Scan("[\\w-]+").ThrowIfEmpty("Invalid new collection name", s);
    s.ThrowIfNotFinish();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (BsonValue) engine.RenameCollection(collection, newName);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
