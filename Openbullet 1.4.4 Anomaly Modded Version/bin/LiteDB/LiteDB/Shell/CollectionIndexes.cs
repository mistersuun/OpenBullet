// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionIndexes
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "indexes", Syntax = "db.<collection>.indexes", Description = "Return all created indexes inside this collection")]
internal class CollectionIndexes : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "indexes");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    string collection = this.ReadCollection(engine, s);
    s.ThrowIfNotFinish();
    foreach (IndexInfo index in engine.GetIndexes(collection))
      yield return (BsonValue) new BsonDocument()
      {
        {
          "slot",
          (BsonValue) index.Slot
        },
        {
          "field",
          (BsonValue) index.Field
        },
        {
          "expression",
          (BsonValue) index.Expression
        },
        {
          "unique",
          (BsonValue) index.Unique
        },
        {
          "maxLevel",
          (BsonValue) Convert.ToInt32(index.MaxLevel)
        }
      };
  }
}
