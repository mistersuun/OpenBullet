// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionMax
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "max", Syntax = "db.<collection>.max [<field>]", Description = "Returns max/last value from collection using index field. Use default _id index if not defined", Examples = new string[] {"db.orders.max age"})]
internal class CollectionMax : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "max");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    CollectionMax collectionMax = this;
    string collection = collectionMax.ReadCollection(engine, s);
    string str = s.Scan(collectionMax.FieldPattern).Trim();
    if (!s.HasTerminated)
      throw LiteException.SyntaxError(s, "Invalid field/index name");
    yield return engine.Max(collection, str.Length == 0 ? "_id" : str);
  }
}
