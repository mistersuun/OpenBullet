// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.IDocument
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public interface IDocument : ITextSource, IServiceProvider
{
  new string Text { get; set; }

  event EventHandler<TextChangeEventArgs> TextChanging;

  event EventHandler<TextChangeEventArgs> TextChanged;

  event EventHandler ChangeCompleted;

  int LineCount { get; }

  IDocumentLine GetLineByNumber(int lineNumber);

  IDocumentLine GetLineByOffset(int offset);

  int GetOffset(int line, int column);

  int GetOffset(TextLocation location);

  TextLocation GetLocation(int offset);

  void Insert(int offset, string text);

  void Insert(int offset, ITextSource text);

  void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType);

  void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType);

  void Remove(int offset, int length);

  void Replace(int offset, int length, string newText);

  void Replace(int offset, int length, ITextSource newText);

  void StartUndoableAction();

  void EndUndoableAction();

  IDisposable OpenUndoGroup();

  ITextAnchor CreateAnchor(int offset);

  string FileName { get; }

  event EventHandler FileNameChanged;
}
