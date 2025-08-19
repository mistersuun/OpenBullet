// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.DocumentChangeOperation
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class DocumentChangeOperation : IUndoableOperationWithContext, IUndoableOperation
{
  private TextDocument document;
  private DocumentChangeEventArgs change;

  public DocumentChangeOperation(TextDocument document, DocumentChangeEventArgs change)
  {
    this.document = document;
    this.change = change;
  }

  public void Undo(UndoStack stack)
  {
    stack.RegisterAffectedDocument(this.document);
    stack.state = 2;
    this.Undo();
    stack.state = 1;
  }

  public void Redo(UndoStack stack)
  {
    stack.RegisterAffectedDocument(this.document);
    stack.state = 2;
    this.Redo();
    stack.state = 1;
  }

  public void Undo()
  {
    this.document.Replace(this.change.Offset, this.change.InsertionLength, this.change.RemovedText, this.change.OffsetChangeMapOrNull?.Invert());
  }

  public void Redo()
  {
    this.document.Replace(this.change.Offset, this.change.RemovalLength, this.change.InsertedText, this.change.OffsetChangeMapOrNull);
  }
}
