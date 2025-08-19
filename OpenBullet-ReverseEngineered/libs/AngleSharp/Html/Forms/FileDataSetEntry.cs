// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.FileDataSetEntry
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io;
using AngleSharp.Io.Dom;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms;

internal sealed class FileDataSetEntry : FormDataSetEntry
{
  private readonly IFile _value;

  public FileDataSetEntry(string name, IFile value, string type)
    : base(name, type)
  {
    this._value = value;
  }

  public string FileName => this._value?.Name ?? string.Empty;

  public string ContentType => this._value?.Type ?? MimeTypeNames.Binary;

  public override bool Contains(string boundary, Encoding encoding)
  {
    bool flag = false;
    Stream body = this._value?.Body;
    if (body != null && body.CanSeek)
    {
      using (StreamReader streamReader = new StreamReader(body, encoding, false, 4096 /*0x1000*/, true))
      {
        while (streamReader.Peek() != -1)
        {
          if (streamReader.ReadLine().Contains(boundary))
          {
            flag = true;
            break;
          }
        }
      }
      body.Seek(0L, SeekOrigin.Begin);
    }
    return flag;
  }

  public override void Accept(IFormDataSetVisitor visitor)
  {
    visitor.File((FormDataSetEntry) this, this.FileName, this.ContentType, this._value);
  }
}
