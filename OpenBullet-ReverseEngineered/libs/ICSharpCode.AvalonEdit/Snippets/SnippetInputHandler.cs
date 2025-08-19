// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetInputHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

internal sealed class SnippetInputHandler : TextAreaStackedInputHandler
{
  private readonly InsertionContext context;

  public SnippetInputHandler(InsertionContext context)
    : base(context.TextArea)
  {
    this.context = context;
  }

  public override void Attach()
  {
    base.Attach();
    this.SelectElement(this.FindNextEditableElement(-1, false));
  }

  public override void Detach()
  {
    base.Detach();
    this.context.Deactivate(new SnippetEventArgs(DeactivateReason.InputHandlerDetached));
  }

  public override void OnPreviewKeyDown(KeyEventArgs e)
  {
    base.OnPreviewKeyDown(e);
    if (e.Key == Key.Escape)
    {
      this.context.Deactivate(new SnippetEventArgs(DeactivateReason.EscapePressed));
      e.Handled = true;
    }
    else if (e.Key == Key.Return)
    {
      this.context.Deactivate(new SnippetEventArgs(DeactivateReason.ReturnPressed));
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Tab)
        return;
      this.SelectElement(this.FindNextEditableElement(this.TextArea.Caret.Offset, e.KeyboardDevice.Modifiers == ModifierKeys.Shift));
      e.Handled = true;
    }
  }

  private void SelectElement(IActiveElement element)
  {
    if (element == null)
      return;
    this.TextArea.Selection = Selection.Create(this.TextArea, element.Segment);
    this.TextArea.Caret.Offset = element.Segment.EndOffset;
  }

  private IActiveElement FindNextEditableElement(int offset, bool backwards)
  {
    IEnumerable<IActiveElement> source = this.context.ActiveElements.Where<IActiveElement>((Func<IActiveElement, bool>) (e => e.IsEditable && e.Segment != null));
    if (backwards)
    {
      source = source.Reverse<IActiveElement>();
      foreach (IActiveElement nextEditableElement in source)
      {
        if (offset > nextEditableElement.Segment.EndOffset)
          return nextEditableElement;
      }
    }
    else
    {
      foreach (IActiveElement nextEditableElement in source)
      {
        if (offset < nextEditableElement.Segment.Offset)
          return nextEditableElement;
      }
    }
    return source.FirstOrDefault<IActiveElement>();
  }
}
