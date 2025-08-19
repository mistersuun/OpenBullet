// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.FileUpload
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace LiteDB.Shell;

[Help(Category = "FileStorage", Name = "upload", Syntax = "fs.upload <fileId> <filename>", Description = "Upload a local file to data file.", Examples = new string[] {"fs.upload my_photo_001 c:/Temp/my_photo_001.jpg"})]
internal class FileUpload : BaseStorage, ICommand
{
  public bool IsCommand(StringScanner s) => this.IsFileCommand(s, "upload");

  public IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine)
  {
    FileUpload fileUpload = this;
    LiteStorage liteStorage = new LiteStorage(engine);
    string str = fileUpload.ReadId(s);
    string path = s.Scan("\\s*.*").Trim();
    if (!File.Exists(path))
      throw new IOException($"File {path} not found");
    string id = str;
    string filename = path;
    yield return (BsonValue) liteStorage.Upload(id, filename).AsDocument;
  }
}
