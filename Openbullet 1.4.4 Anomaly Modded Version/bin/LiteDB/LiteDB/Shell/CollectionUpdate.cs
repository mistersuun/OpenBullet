// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionUpdate
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "update", Syntax = "db.<collection>.update <jsonDoc> | <field|path> [+]= <value|expression> [where <filter>]", Description = "Update a single document by _id value or a range of document using field=value pairs and where filter clause. If your field is an array, can be use += to add new value. Returns number of updated documents.", Examples = new string[] {"db.orders.update { _id: 1, customerName: \"John\" }", "db.orders.update name = \"John\" where _id = 22", "db.orders.update name = UPPER($.name), $.age = $.age + 1", "db.orders.update $.tags += \"blue\" where _id in [1, 2, 44]"})]
internal class CollectionUpdate : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "update");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    CollectionUpdate collectionUpdate = this;
    string col = collectionUpdate.ReadCollection(engine, s);
    if (s.Match("\\s*\\{"))
    {
      BsonDocument asDocument = JsonSerializer.Deserialize(s.ToString()).AsDocument;
      s.ThrowIfNotFinish();
      yield return (BsonValue) engine.Update(col, asDocument);
    }
    else
    {
      List<CollectionUpdate.UpdateData> updates = new List<CollectionUpdate.UpdateData>();
      Query query = Query.All();
      while (!s.HasTerminated)
      {
        string source = BsonExpression.ReadExpression(s, true, true).Source;
        string str = s.Scan("\\s*\\+?=\\s*").Trim().ThrowIfEmpty("Invalid operator (support = or +=)", s);
        BsonValue bsonValue = collectionUpdate.ReadBsonValue(s);
        BsonExpression bsonExpression = bsonValue == (BsonValue) null ? BsonExpression.ReadExpression(s, true, false) : (BsonExpression) null;
        if (str != "+=" && str != "=")
          throw LiteException.SyntaxError(s);
        if (bsonValue == (BsonValue) null && bsonExpression == null)
          throw LiteException.SyntaxError(s);
        updates.Add(new CollectionUpdate.UpdateData()
        {
          Path = source,
          Value = bsonValue,
          Expr = bsonExpression,
          Add = str == "+="
        });
        s.Scan("\\s*");
        if (s.Scan(",\\s*").Length <= 0)
        {
          if (s.Scan("where\\s*").Length <= 0 && !s.HasTerminated)
            throw LiteException.SyntaxError(s);
          break;
        }
      }
      if (!s.HasTerminated)
        query = collectionUpdate.ReadQuery(s, false);
      s.ThrowIfNotFinish();
      yield return (BsonValue) engine.Update(col, collectionUpdate.FetchDocuments(engine, col, query, updates));
    }
  }

  private IEnumerable<BsonDocument> FetchDocuments(
    LiteEngine engine,
    string col,
    Query query,
    List<CollectionUpdate.UpdateData> updates)
  {
    foreach (BsonDocument bsonDocument in engine.Find(col, query))
    {
      bool flag = false;
      foreach (CollectionUpdate.UpdateData update in updates)
      {
        if (!(update.Value == (BsonValue) null) ? bsonDocument.Set(update.Path, update.Value, update.Add) : bsonDocument.Set(update.Path, update.Expr, update.Add))
          flag = true;
      }
      if (flag)
        yield return bsonDocument;
    }
  }

  public class UpdateData
  {
    public string Path { get; set; }

    public BsonValue Value { get; set; }

    public BsonExpression Expr { get; set; }

    public bool Add { get; set; }
  }
}
