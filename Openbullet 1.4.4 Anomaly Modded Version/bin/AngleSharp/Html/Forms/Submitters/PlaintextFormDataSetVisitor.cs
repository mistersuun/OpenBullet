// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.PlaintextFormDataSetVisitor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io.Dom;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters;

internal sealed class PlaintextFormDataSetVisitor : IFormSubmitter, IFormDataSetVisitor
{
  private readonly List<string> _lines;

  public PlaintextFormDataSetVisitor() => this._lines = new List<string>();

  public void Text(FormDataSetEntry entry, string value)
  {
    if (!entry.HasName || value == null)
      return;
    this.Add(entry.Name, value);
  }

  public void File(FormDataSetEntry entry, string fileName, string contentType, IFile content)
  {
    if (!entry.HasName || content == null || content.Name == null)
      return;
    this.Add(entry.Name, content.Name);
  }

  public void Serialize(StreamWriter stream)
  {
    string str = string.Join("\r\n", (IEnumerable<string>) this._lines);
    stream.Write(str);
  }

  private void Add(string name, string value) => this._lines.Add($"{name}={value}");
}
