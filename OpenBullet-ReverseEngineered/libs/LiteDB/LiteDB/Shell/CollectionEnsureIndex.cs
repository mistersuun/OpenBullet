// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionEnsureIndex
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "ensureIndex", Syntax = "db.<collection>.ensureIndex <field|name> [unique] [using <expression|path>]", Description = "Create a new index to collection based on field or an expressions. Index could be with unique values only. Expressions can returns list of values (values from an array).", Examples = new string[] {"db.customers.ensureIndex name", "db.customers.ensureIndex email unique", "db.customers.ensureIndex tags using $.tags[*]", "db.customers.ensureIndex mobile_phones using $.phones[@.type = 'Mobile'].Number"})]
internal class CollectionEnsureIndex : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "ensure[iI]ndex");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    CollectionEnsureIndex collectionEnsureIndex = this;
    string collection = collectionEnsureIndex.ReadCollection(engine, s);
    string field = s.Scan(collectionEnsureIndex.FieldPattern).Trim().ThrowIfEmpty("Invalid field/index name", s);
    bool unique = false;
    string expression = (string) null;
    s.Scan("\\s*");
    if (!s.HasTerminated)
    {
      unique = s.Scan("unique\\s*").Length > 0;
      if (s.Scan("\\s*using\\s+").Length > 0)
        expression = BsonExpression.ReadExpression(s, true, false)?.Source;
    }
    s.ThrowIfNotFinish();
    yield return (BsonValue) engine.EnsureIndex(collection, field, expression, unique);
  }
}
