// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.CollectionBulk
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "Collection", Name = "bulk", Syntax = "db.<collection>.bulk <filename>", Description = "Bulk insert a json file with documents. Json file must be an array with documents. Returns number of document inserted.", Examples = new string[] {"db.orders.bulk C:/Temp/orders.json"})]
internal class CollectionBulk : BaseCollection, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsCollectionCommand(s, "bulk");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    string collection = this.ReadCollection(engine, s);
    using (StreamReader sr = new StreamReader((Stream) new FileStream(s.Scan(".*"), System.IO.FileMode.Open)))
    {
      IEnumerable<BsonValue> source = JsonSerializer.DeserializeArray((TextReader) sr);
      yield return (BsonValue) engine.InsertBulk(collection, source.Select<BsonValue, BsonDocument>((Func<BsonValue, BsonDocument>) (x => x.AsDocument)));
    }
  }
}
