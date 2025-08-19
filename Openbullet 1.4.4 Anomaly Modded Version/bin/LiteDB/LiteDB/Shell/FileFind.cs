// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.FileFind
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "FileStorage", Name = "find", Syntax = "fs.find [<fileId>]", Description = "List all files or filter them by fileId string key (use startsWith clause).", Examples = new string[] {"fs.find", "fs.find my_pho"})]
internal class FileFind : BaseStorage, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsFileCommand(s, "find");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    FileFind fileFind = this;
    LiteStorage liteStorage = new LiteStorage(engine);
    IEnumerable<LiteFileInfo> all;
    if (s.HasTerminated)
    {
      all = liteStorage.FindAll();
    }
    else
    {
      string startsWith = fileFind.ReadId(s);
      s.ThrowIfNotFinish();
      all = liteStorage.Find(startsWith);
    }
    foreach (BsonValue bsonValue in all.Select<LiteFileInfo, BsonDocument>((Func<LiteFileInfo, BsonDocument>) (x => x.AsDocument)))
      yield return bsonValue;
  }
}
