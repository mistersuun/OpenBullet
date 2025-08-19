// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionMin
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "min", Syntax = "db.<collection>.min [<field>]", Description = "Returns min/first value from collection using index field. Use default _id index if not defined", Examples = new string[] {"db.orders.min", "db.orders.min order_date"})]
internal class CollectionMin : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "min");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    CollectionMin collectionMin = this;
    string collection = collectionMin.ReadCollection(engine, s);
    string str = s.Scan(collectionMin.FieldPattern).Trim();
    if (!s.HasTerminated)
      throw LiteException.SyntaxError(s, "Invalid field/index name");
    yield return engine.Min(collection, str.Length == 0 ? "_id" : str);
  }
}
