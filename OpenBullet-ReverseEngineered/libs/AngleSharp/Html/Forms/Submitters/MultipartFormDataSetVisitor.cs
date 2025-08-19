// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.MultipartFormDataSetVisitor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters;

internal sealed class MultipartFormDataSetVisitor : IFormSubmitter, IFormDataSetVisitor
{
  private static readonly string DashDash = "--";
  private readonly Encoding _encoding;
  private readonly List<Action<StreamWriter>> _writers;
  private readonly string _boundary;

  public MultipartFormDataSetVisitor(Encoding encoding, string boundary)
  {
    this._encoding = encoding;
    this._writers = new List<Action<StreamWriter>>();
    this._boundary = boundary;
  }

  public void Text(FormDataSetEntry entry, string value)
  {
    if (!entry.HasName || value == null)
      return;
    this._writers.Add((Action<StreamWriter>) (stream =>
    {
      stream.WriteLine("Content-Disposition: form-data; name=\"{0}\"", (object) entry.Name.HtmlEncode(this._encoding));
      stream.WriteLine();
      stream.WriteLine(value.HtmlEncode(this._encoding));
    }));
  }

  public void File(FormDataSetEntry entry, string fileName, string contentType, IFile content)
  {
    if (!entry.HasName)
      return;
    this._writers.Add((Action<StreamWriter>) (stream =>
    {
      int num = content == null || content?.Name == null || content.Type == null ? 0 : (content.Body != null ? 1 : 0);
      stream.WriteLine("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", (object) entry.Name.HtmlEncode(this._encoding), (object) fileName.HtmlEncode(this._encoding));
      stream.WriteLine("Content-Type: {0}", (object) contentType);
      stream.WriteLine();
      if (num != 0)
      {
        stream.Flush();
        content.Body.CopyTo(stream.BaseStream);
      }
      stream.WriteLine();
    }));
  }

  public void Serialize(StreamWriter stream)
  {
    foreach (Action<StreamWriter> writer in this._writers)
    {
      stream.Write(MultipartFormDataSetVisitor.DashDash);
      stream.WriteLine(this._boundary);
      StreamWriter streamWriter = stream;
      writer(streamWriter);
    }
    stream.Write(MultipartFormDataSetVisitor.DashDash);
    stream.Write(this._boundary);
    stream.Write(MultipartFormDataSetVisitor.DashDash);
  }
}
