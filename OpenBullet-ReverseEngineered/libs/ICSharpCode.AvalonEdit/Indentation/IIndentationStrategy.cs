// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Indentation.IIndentationStrategy
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Indentation;

public interface IIndentationStrategy
{
  void IndentLine(TextDocument document, DocumentLine line);

  void IndentLines(TextDocument document, int beginLine, int endLine);
}
