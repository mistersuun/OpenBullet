// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.WeakLineTracker
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class WeakLineTracker : ILineTracker
{
  private TextDocument textDocument;
  private WeakReference targetObject;

  private WeakLineTracker(TextDocument textDocument, ILineTracker targetTracker)
  {
    this.textDocument = textDocument;
    this.targetObject = new WeakReference((object) targetTracker);
  }

  public static WeakLineTracker Register(TextDocument textDocument, ILineTracker targetTracker)
  {
    if (textDocument == null)
      throw new ArgumentNullException(nameof (textDocument));
    WeakLineTracker weakLineTracker = targetTracker != null ? new WeakLineTracker(textDocument, targetTracker) : throw new ArgumentNullException(nameof (targetTracker));
    textDocument.LineTrackers.Add((ILineTracker) weakLineTracker);
    return weakLineTracker;
  }

  public void Deregister()
  {
    if (this.textDocument == null)
      return;
    this.textDocument.LineTrackers.Remove((ILineTracker) this);
    this.textDocument = (TextDocument) null;
  }

  void ILineTracker.BeforeRemoveLine(DocumentLine line)
  {
    if (this.targetObject.Target is ILineTracker target)
      target.BeforeRemoveLine(line);
    else
      this.Deregister();
  }

  void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
  {
    if (this.targetObject.Target is ILineTracker target)
      target.SetLineLength(line, newTotalLength);
    else
      this.Deregister();
  }

  void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
  {
    if (this.targetObject.Target is ILineTracker target)
      target.LineInserted(insertionPos, newLine);
    else
      this.Deregister();
  }

  void ILineTracker.RebuildDocument()
  {
    if (this.targetObject.Target is ILineTracker target)
      target.RebuildDocument();
    else
      this.Deregister();
  }

  void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
  {
    if (this.targetObject.Target is ILineTracker target)
      target.ChangeComplete(e);
    else
      this.Deregister();
  }
}
