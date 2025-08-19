// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.ILineTracker
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public interface ILineTracker
{
  void BeforeRemoveLine(DocumentLine line);

  void SetLineLength(DocumentLine line, int newTotalLength);

  void LineInserted(DocumentLine insertionPos, DocumentLine newLine);

  void RebuildDocument();

  void ChangeComplete(DocumentChangeEventArgs e);
}
