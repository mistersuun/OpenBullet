// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.Select
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "select", Syntax = "db.<collection>.select <expression|path> [into <new_collection> [id:<data-type>]] [where <filter>] [includes <path1>,<path2>,...<pathN>] [skip N] [limit <M>]", Description = "Search for document using filter. Support document transforms using expression (see `help expression`). Can include DbRef documents in results. Can skip/limit results.", Examples = new string[] {"db.orders.select", "db.orders.select $ where _id > 100", "db.orders.select { name: $.name, age: $.age - 2017 } where age < 30 limit 100", "db.orders.select { name: UPPER($.name), mobile: FIRST($.phones[@.type = 'Mobile'].Number) }", "db.orders.select $ into new_orders where DATEDIFF('day', $.orderDate, DATE()) = 0", "db.orders.select $ include $.customer, $.produts[*] where _id = 22"})]
internal class Select : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "select");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    string collection1 = this.ReadCollection(engine, s);
    BsonExpression expression = BsonExpression.ReadExpression(s, false, false) ?? BsonExpression.ReadExpression(s, true, true);
    Query query = Query.All();
    string collection2 = s.Scan("\\s*into\\s+([\\w-]+)", 1);
    BsonType autoId = BsonType.ObjectId;
    if (collection2.Length > 0)
    {
      int num;
      switch (s.Scan("\\s+_?id:(int32|int64|int|long|objectid|datetime|date|guid)", 1).Trim().ToLower())
      {
        case "int32":
        case "int":
          num = 2;
          break;
        case "int64":
        case "long":
          num = 3;
          break;
        case "date":
        case "datetime":
          num = 13;
          break;
        case "guid":
          num = 11;
          break;
        default:
          num = 10;
          break;
      }
      autoId = (BsonType) num;
    }
    if (s.Scan("\\s*where\\s*").Length > 0)
      query = this.ReadQuery(s, true);
    KeyValuePair<int, int> keyValuePair = this.ReadSkipLimit(s);
    string[] includes = this.ReadIncludes(s);
    s.ThrowIfNotFinish();
    IEnumerable<BsonDocument> docs = engine.Find(collection1, query, includes, keyValuePair.Key, keyValuePair.Value);
    if (collection2.Length <= 0)
      return this.Execute(docs, expression).Select<BsonDocument, BsonValue>((Func<BsonDocument, BsonValue>) (x => (BsonValue) x));
    return (IEnumerable<BsonValue>) new BsonValue[1]
    {
      (BsonValue) engine.InsertBulk(collection2, this.Execute(docs, expression), autoId: autoId)
    };
  }

  private IEnumerable<BsonDocument> Execute(
    IEnumerable<BsonDocument> docs,
    BsonExpression expression)
  {
    foreach (BsonDocument doc in docs)
    {
      foreach (BsonValue value in expression.Execute(doc, false))
      {
        if (value.IsDocument)
          yield return value.AsDocument;
        else
          yield return new BsonDocument()
          {
            ["expr"] = value
          };
      }
    }
  }
}
