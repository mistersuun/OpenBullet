// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionInsert
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "insert", Syntax = "db.<collection>.insert <jsonDoc> [id:[int|long|date|guid|objectId]]", Description = "Insert a new document inside collection. Can define auto id data type that will be used when _id missing in document.", Examples = new string[] {"db.customers.insert { _id: 1, name: \"John\" }", "db.customers.insert { name: \"Carlos\" } id:int", "db.customers.insert { name: \"July\", birthDate: { $date: \"2011-08-10\" } } id:guid"})]
internal class CollectionInsert : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "insert");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    string col = this.ReadCollection(engine, s);
    BsonValue value = JsonSerializer.Deserialize(s);
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
    BsonType autoId = (BsonType) num;
    s.ThrowIfNotFinish();
    if (value.IsArray)
    {
      yield return (BsonValue) engine.InsertBulk(col, value.AsArray.RawValue.Select<BsonValue, BsonDocument>((Func<BsonValue, BsonDocument>) (x => x.AsDocument)), autoId: autoId);
    }
    else
    {
      if (!value.IsDocument)
        throw LiteException.SyntaxError(s, "Invalid JSON value (must be a document or an array)");
      engine.Insert(col, (IEnumerable<BsonDocument>) new BsonDocument[1]
      {
        value.AsDocument
      }, autoId);
      yield return value.AsDocument["_id"];
    }
  }
}
