// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.FormDataSet
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Forms;
using AngleSharp.Html.Forms.Submitters;
using AngleSharp.Io.Dom;
using AngleSharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Html;

public sealed class FormDataSet : IEnumerable<string>, IEnumerable
{
  private readonly List<FormDataSetEntry> _entries;
  private string _boundary;

  public FormDataSet()
  {
    this._boundary = Guid.NewGuid().ToString();
    this._entries = new List<FormDataSetEntry>();
  }

  public string Boundary => this._boundary;

  public Stream AsMultipart(Encoding encoding = null)
  {
    return this.BuildRequestContent(encoding, (Action<StreamWriter>) (stream => this.Connect((IFormSubmitter) new MultipartFormDataSetVisitor(stream.Encoding, this._boundary), stream)));
  }

  public Stream AsUrlEncoded(Encoding encoding = null)
  {
    return this.BuildRequestContent(encoding, (Action<StreamWriter>) (stream => this.Connect((IFormSubmitter) new UrlEncodedFormDataSetVisitor(stream.Encoding), stream)));
  }

  public Stream AsPlaintext(Encoding encoding = null)
  {
    return this.BuildRequestContent(encoding, (Action<StreamWriter>) (stream => this.Connect((IFormSubmitter) new PlaintextFormDataSetVisitor(), stream)));
  }

  public Stream AsJson()
  {
    return this.BuildRequestContent(TextEncoding.Utf8, (Action<StreamWriter>) (stream => this.Connect((IFormSubmitter) new JsonFormDataSetVisitor(), stream)));
  }

  public Stream As(IFormSubmitter submitter, Encoding encoding = null)
  {
    return this.BuildRequestContent(encoding, (Action<StreamWriter>) (stream => this.Connect(submitter, stream)));
  }

  public void Append(string name, string value, string type)
  {
    if (type.Isi(TagNames.Textarea))
    {
      name = name.NormalizeLineEndings();
      value = value.NormalizeLineEndings();
    }
    this._entries.Add((FormDataSetEntry) new TextDataSetEntry(name, value, type));
  }

  public void Append(string name, IFile value, string type)
  {
    if (type.Isi(InputTypeNames.File))
      name = name.NormalizeLineEndings();
    this._entries.Add((FormDataSetEntry) new FileDataSetEntry(name, value, type));
  }

  private Stream BuildRequestContent(Encoding encoding, Action<StreamWriter> process)
  {
    encoding = encoding ?? TextEncoding.Utf8;
    MemoryStream memoryStream = new MemoryStream();
    this.FixPotentialBoundaryCollisions(encoding);
    this.ReplaceCharset(encoding);
    StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, encoding);
    process(streamWriter);
    streamWriter.Flush();
    memoryStream.Position = 0L;
    return (Stream) memoryStream;
  }

  private void Connect(IFormSubmitter submitter, StreamWriter stream)
  {
    foreach (FormDataSetEntry entry in this._entries)
      entry.Accept((IFormDataSetVisitor) submitter);
    submitter.Serialize(stream);
  }

  private void ReplaceCharset(Encoding encoding)
  {
    for (int index = 0; index < this._entries.Count; ++index)
    {
      FormDataSetEntry entry = this._entries[index];
      if (!string.IsNullOrEmpty(entry.Name) && entry.Name.Is("_charset_") && entry.Type.Isi(InputTypeNames.Hidden))
        this._entries[index] = (FormDataSetEntry) new TextDataSetEntry(entry.Name, encoding.WebName, entry.Type);
    }
  }

  private void FixPotentialBoundaryCollisions(Encoding encoding)
  {
    bool flag = false;
    do
    {
      for (int index = 0; index < this._entries.Count; ++index)
      {
        if (flag = this._entries[index].Contains(this._boundary, encoding))
        {
          this._boundary = Guid.NewGuid().ToString();
          break;
        }
      }
    }
    while (flag);
  }

  public IEnumerator<string> GetEnumerator()
  {
    return this._entries.Select<FormDataSetEntry, string>((Func<FormDataSetEntry, string>) (m => m.Name)).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
