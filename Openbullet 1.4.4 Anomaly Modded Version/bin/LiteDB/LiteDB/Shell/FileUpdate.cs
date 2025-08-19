// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.FileUpdate
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "FileStorage", Name = "update", Syntax = "fs.update <fileId> <jsonDoc>", Description = "Update metada from a file inside file storage.", Examples = new string[] {"fs.update my_photo_001 { author: \"John Doe\" }"})]
internal class FileUpdate : BaseStorage, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsFileCommand(s, "update");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    FileUpdate fileUpdate = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    LiteStorage liteStorage = new LiteStorage(engine);
    string str = fileUpdate.ReadId(s);
    BsonDocument asDocument = JsonSerializer.Deserialize(s.ToString()).AsDocument;
    s.ThrowIfNotFinish();
    string id = str;
    BsonDocument metadata = asDocument;
    liteStorage.SetMetadata(id, metadata);
    return false;
  }
}
