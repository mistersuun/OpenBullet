// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.FileDelete
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "FileStorage", Name = "delete", Syntax = "fs.delete <fileId>", Description = "Delete a file using fileId key. Return true if file has been deleted.", Examples = new string[] {"fs.delete my_photo_001"})]
internal class FileDelete : BaseStorage, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsFileCommand(s, "delete");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    FileDelete fileDelete = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    LiteStorage liteStorage = new LiteStorage(engine);
    string id = fileDelete.ReadId(s);
    s.ThrowIfNotFinish();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (BsonValue) liteStorage.Delete(id);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
