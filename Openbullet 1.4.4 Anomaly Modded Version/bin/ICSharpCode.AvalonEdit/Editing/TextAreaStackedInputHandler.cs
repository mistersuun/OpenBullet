// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextAreaStackedInputHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public abstract class TextAreaStackedInputHandler : ITextAreaInputHandler
{
  private readonly TextArea textArea;

  public TextArea TextArea => this.textArea;

  protected TextAreaStackedInputHandler(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
  }

  public virtual void Attach()
  {
  }

  public virtual void Detach()
  {
  }

  public virtual void OnPreviewKeyDown(KeyEventArgs e)
  {
  }

  public virtual void OnPreviewKeyUp(KeyEventArgs e)
  {
  }
}
