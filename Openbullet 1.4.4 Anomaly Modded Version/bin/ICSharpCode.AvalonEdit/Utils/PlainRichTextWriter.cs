// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.PlainRichTextWriter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal class PlainRichTextWriter : RichTextWriter
{
  protected readonly TextWriter textWriter;
  private string indentationString = "\t";
  private int indentationLevel;
  private char prevChar;

  public PlainRichTextWriter(TextWriter textWriter)
  {
    this.textWriter = textWriter != null ? textWriter : throw new ArgumentNullException(nameof (textWriter));
  }

  public string IndentationString
  {
    get => this.indentationString;
    set => this.indentationString = value;
  }

  protected override void BeginUnhandledSpan()
  {
  }

  public override void EndSpan()
  {
  }

  private void WriteIndentation()
  {
    for (int index = 0; index < this.indentationLevel; ++index)
      this.textWriter.Write(this.indentationString);
  }

  protected void WriteIndentationIfNecessary()
  {
    if (this.prevChar != '\n')
      return;
    this.WriteIndentation();
    this.prevChar = char.MinValue;
  }

  protected virtual void AfterWrite()
  {
  }

  public override void Write(char value)
  {
    if (this.prevChar == '\n')
      this.WriteIndentation();
    this.textWriter.Write(value);
    this.prevChar = value;
    this.AfterWrite();
  }

  public override void Indent() => ++this.indentationLevel;

  public override void Unindent()
  {
    if (this.indentationLevel == 0)
      throw new NotSupportedException();
    --this.indentationLevel;
  }

  public override Encoding Encoding => this.textWriter.Encoding;

  public override IFormatProvider FormatProvider => this.textWriter.FormatProvider;

  public override string NewLine
  {
    get => this.textWriter.NewLine;
    set => this.textWriter.NewLine = value;
  }
}
