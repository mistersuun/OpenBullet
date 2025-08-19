// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextAreaInputHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class TextAreaInputHandler : ITextAreaInputHandler
{
  private readonly ObserveAddRemoveCollection<CommandBinding> commandBindings;
  private readonly ObserveAddRemoveCollection<InputBinding> inputBindings;
  private readonly ObserveAddRemoveCollection<ITextAreaInputHandler> nestedInputHandlers;
  private readonly TextArea textArea;
  private bool isAttached;

  public TextAreaInputHandler(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
    this.commandBindings = new ObserveAddRemoveCollection<CommandBinding>(new Action<CommandBinding>(this.CommandBinding_Added), new Action<CommandBinding>(this.CommandBinding_Removed));
    this.inputBindings = new ObserveAddRemoveCollection<InputBinding>(new Action<InputBinding>(this.InputBinding_Added), new Action<InputBinding>(this.InputBinding_Removed));
    this.nestedInputHandlers = new ObserveAddRemoveCollection<ITextAreaInputHandler>(new Action<ITextAreaInputHandler>(this.NestedInputHandler_Added), new Action<ITextAreaInputHandler>(this.NestedInputHandler_Removed));
  }

  public TextArea TextArea => this.textArea;

  public bool IsAttached => this.isAttached;

  public ICollection<CommandBinding> CommandBindings
  {
    get => (ICollection<CommandBinding>) this.commandBindings;
  }

  private void CommandBinding_Added(CommandBinding commandBinding)
  {
    if (!this.isAttached)
      return;
    this.textArea.CommandBindings.Add(commandBinding);
  }

  private void CommandBinding_Removed(CommandBinding commandBinding)
  {
    if (!this.isAttached)
      return;
    this.textArea.CommandBindings.Remove(commandBinding);
  }

  public ICollection<InputBinding> InputBindings => (ICollection<InputBinding>) this.inputBindings;

  private void InputBinding_Added(InputBinding inputBinding)
  {
    if (!this.isAttached)
      return;
    this.textArea.InputBindings.Add(inputBinding);
  }

  private void InputBinding_Removed(InputBinding inputBinding)
  {
    if (!this.isAttached)
      return;
    this.textArea.InputBindings.Remove(inputBinding);
  }

  public void AddBinding(
    ICommand command,
    ModifierKeys modifiers,
    Key key,
    ExecutedRoutedEventHandler handler)
  {
    this.CommandBindings.Add(new CommandBinding(command, handler));
    this.InputBindings.Add((InputBinding) new KeyBinding(command, key, modifiers));
  }

  public ICollection<ITextAreaInputHandler> NestedInputHandlers
  {
    get => (ICollection<ITextAreaInputHandler>) this.nestedInputHandlers;
  }

  private void NestedInputHandler_Added(ITextAreaInputHandler handler)
  {
    if (handler == null)
      throw new ArgumentNullException(nameof (handler));
    if (handler.TextArea != this.textArea)
      throw new ArgumentException("The nested handler must be working for the same text area!");
    if (!this.isAttached)
      return;
    handler.Attach();
  }

  private void NestedInputHandler_Removed(ITextAreaInputHandler handler)
  {
    if (!this.isAttached)
      return;
    handler.Detach();
  }

  public virtual void Attach()
  {
    this.isAttached = !this.isAttached ? true : throw new InvalidOperationException("Input handler is already attached");
    this.textArea.CommandBindings.AddRange((ICollection) this.commandBindings);
    this.textArea.InputBindings.AddRange((ICollection) this.inputBindings);
    foreach (ITextAreaInputHandler nestedInputHandler in (Collection<ITextAreaInputHandler>) this.nestedInputHandlers)
      nestedInputHandler.Attach();
  }

  public virtual void Detach()
  {
    this.isAttached = this.isAttached ? false : throw new InvalidOperationException("Input handler is not attached");
    foreach (CommandBinding commandBinding in (Collection<CommandBinding>) this.commandBindings)
      this.textArea.CommandBindings.Remove(commandBinding);
    foreach (InputBinding inputBinding in (Collection<InputBinding>) this.inputBindings)
      this.textArea.InputBindings.Remove(inputBinding);
    foreach (ITextAreaInputHandler nestedInputHandler in (Collection<ITextAreaInputHandler>) this.nestedInputHandlers)
      nestedInputHandler.Detach();
  }
}
