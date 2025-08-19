// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.UndoOperationGroup
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class UndoOperationGroup : IUndoableOperationWithContext, IUndoableOperation
{
  private IUndoableOperation[] undolist;

  public UndoOperationGroup(Deque<IUndoableOperation> stack, int numops)
  {
    if (stack == null)
      throw new ArgumentNullException(nameof (stack));
    this.undolist = new IUndoableOperation[numops];
    for (int index = 0; index < numops; ++index)
      this.undolist[index] = stack.PopBack();
  }

  public void Undo()
  {
    for (int index = 0; index < this.undolist.Length; ++index)
      this.undolist[index].Undo();
  }

  public void Undo(UndoStack stack)
  {
    for (int index = 0; index < this.undolist.Length; ++index)
      stack.RunUndo(this.undolist[index]);
  }

  public void Redo()
  {
    for (int index = this.undolist.Length - 1; index >= 0; --index)
      this.undolist[index].Redo();
  }

  public void Redo(UndoStack stack)
  {
    for (int index = this.undolist.Length - 1; index >= 0; --index)
      stack.RunRedo(this.undolist[index]);
  }
}
