// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteFileInfo
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

public class LiteFileInfo
{
  public const string ID_PATTERN = "^[\\w-$@!+%;\\.]+(\\/[\\w-$@!+%;\\.]+)*$";
  private static Regex IdPattern = new Regex("^[\\w-$@!+%;\\.]+(\\/[\\w-$@!+%;\\.]+)*$", RegexOptions.Compiled);
  private LiteEngine _engine;

  public string Id { get; private set; }

  public string Filename { get; private set; }

  public string MimeType { get; private set; }

  public long Length { get; internal set; }

  public int Chunks { get; internal set; }

  public DateTime UploadDate { get; internal set; }

  public BsonDocument Metadata { get; set; }

  internal LiteFileInfo(LiteEngine engine, string id, string filename)
  {
    if (!LiteFileInfo.IdPattern.IsMatch(id))
      throw LiteException.InvalidFormat(id);
    this._engine = engine;
    this.Id = id;
    this.Filename = Path.GetFileName(filename);
    this.MimeType = MimeTypeConverter.GetMimeType(this.Filename);
    this.Length = 0L;
    this.Chunks = 0;
    this.UploadDate = DateTime.Now;
    this.Metadata = new BsonDocument();
  }

  internal LiteFileInfo(LiteEngine engine, BsonDocument doc)
  {
    this._engine = engine;
    this.Id = doc["_id"].AsString;
    this.Filename = doc["filename"].AsString;
    this.MimeType = doc["mimeType"].AsString;
    this.Length = doc["length"].AsInt64;
    this.Chunks = doc["chunks"].AsInt32;
    this.UploadDate = doc["uploadDate"].AsDateTime;
    this.Metadata = doc["metadata"].AsDocument;
  }

  public BsonDocument AsDocument
  {
    get
    {
      return new BsonDocument()
      {
        {
          "_id",
          (BsonValue) this.Id
        },
        {
          "filename",
          (BsonValue) this.Filename
        },
        {
          "mimeType",
          (BsonValue) this.MimeType
        },
        {
          "length",
          (BsonValue) this.Length
        },
        {
          "chunks",
          (BsonValue) this.Chunks
        },
        {
          "uploadDate",
          (BsonValue) this.UploadDate
        },
        {
          "metadata",
          (BsonValue) (this.Metadata ?? new BsonDocument())
        }
      };
    }
  }

  public LiteFileStream OpenRead() => new LiteFileStream(this._engine, this, FileAccess.Read);

  public LiteFileStream OpenWrite() => new LiteFileStream(this._engine, this, FileAccess.Write);

  public void CopyTo(Stream stream)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    using (LiteFileStream liteFileStream = this.OpenRead())
      liteFileStream.CopyTo(stream);
  }

  public void SaveAs(string filename, bool overwritten = true)
  {
    if (filename.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (filename));
    using (FileStream destination = File.Open(filename, overwritten ? System.IO.FileMode.Create : System.IO.FileMode.CreateNew))
      this.OpenRead().CopyTo((Stream) destination);
  }
}
