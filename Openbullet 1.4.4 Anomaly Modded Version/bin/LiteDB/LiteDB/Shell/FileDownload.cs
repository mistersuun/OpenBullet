// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.FileDownload
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "FileStorage", Name = "find", Syntax = "fs.download <fileId> <filename>", Description = "Download file inside storage to local computer. Returns true if file has been saved on local disk.", Examples = new string[] {"fs.download my_photo001 C:/Temp/my_photo001.jpg"})]
internal class FileDownload : BaseStorage, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsFileCommand(s, "download");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    FileDownload fileDownload = this;
    LiteStorage liteStorage = new LiteStorage(engine);
    string str = fileDownload.ReadId(s);
    string filename = s.Scan("\\s*.*").Trim();
    string id = str;
    LiteFileInfo byId = liteStorage.FindById(id);
    if (byId != null)
    {
      byId.SaveAs(filename);
      yield return (BsonValue) byId.AsDocument;
    }
  }
}
