// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextAreaDefaultInputHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class TextAreaDefaultInputHandler : TextAreaInputHandler
{
  public TextAreaInputHandler CaretNavigation { get; private set; }

  public TextAreaInputHandler Editing { get; private set; }

  public ITextAreaInputHandler MouseSelection { get; private set; }

  public TextAreaDefaultInputHandler(TextArea textArea)
    : base(textArea)
  {
    this.NestedInputHandlers.Add((ITextAreaInputHandler) (this.CaretNavigation = CaretNavigationCommandHandler.Create(textArea)));
    this.NestedInputHandlers.Add((ITextAreaInputHandler) (this.Editing = EditingCommandHandler.Create(textArea)));
    this.NestedInputHandlers.Add(this.MouseSelection = (ITextAreaInputHandler) new SelectionMouseHandler(textArea));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Undo, new ExecutedRoutedEventHandler(this.ExecuteUndo), new CanExecuteRoutedEventHandler(this.CanExecuteUndo)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Redo, new ExecutedRoutedEventHandler(this.ExecuteRedo), new CanExecuteRoutedEventHandler(this.CanExecuteRedo)));
  }

  internal static KeyBinding CreateFrozenKeyBinding(
    ICommand command,
    ModifierKeys modifiers,
    Key key)
  {
    KeyBinding frozenKeyBinding = new KeyBinding(command, key, modifiers);
    if (frozenKeyBinding is Freezable freezable)
      freezable.Freeze();
    return frozenKeyBinding;
  }

  internal static void WorkaroundWPFMemoryLeak(List<InputBinding> inputBindings)
  {
    new UIElement().InputBindings.AddRange((ICollection) inputBindings);
  }

  private UndoStack GetUndoStack() => this.TextArea.Document?.UndoStack;

  private void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
  {
    UndoStack undoStack = this.GetUndoStack();
    if (undoStack == null)
      return;
    if (undoStack.CanUndo)
    {
      undoStack.Undo();
      this.TextArea.Caret.BringCaretToView();
    }
    e.Handled = true;
  }

  private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
  {
    UndoStack undoStack = this.GetUndoStack();
    if (undoStack == null)
      return;
    e.Handled = true;
    e.CanExecute = undoStack.CanUndo;
  }

  private void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
  {
    UndoStack undoStack = this.GetUndoStack();
    if (undoStack == null)
      return;
    if (undoStack.CanRedo)
    {
      undoStack.Redo();
      this.TextArea.Caret.BringCaretToView();
    }
    e.Handled = true;
  }

  private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
  {
    UndoStack undoStack = this.GetUndoStack();
    if (undoStack == null)
      return;
    e.Handled = true;
    e.CanExecute = undoStack.CanRedo;
  }
}
