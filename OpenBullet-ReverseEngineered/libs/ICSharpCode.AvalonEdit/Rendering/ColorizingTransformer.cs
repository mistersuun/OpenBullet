// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.ColorizingTransformer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class ColorizingTransformer : IVisualLineTransformer, ITextViewConnect
{
  protected IList<VisualLineElement> CurrentElements { get; private set; }

  public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
  {
    if (elements == null)
      throw new ArgumentNullException(nameof (elements));
    this.CurrentElements = this.CurrentElements == null ? elements : throw new InvalidOperationException("Recursive Transform() call");
    try
    {
      this.Colorize(context);
    }
    finally
    {
      this.CurrentElements = (IList<VisualLineElement>) null;
    }
  }

  protected abstract void Colorize(ITextRunConstructionContext context);

  protected void ChangeVisualElements(
    int visualStartColumn,
    int visualEndColumn,
    Action<VisualLineElement> action)
  {
    if (action == null)
      throw new ArgumentNullException(nameof (action));
    for (int index = 0; index < this.CurrentElements.Count; ++index)
    {
      VisualLineElement currentElement = this.CurrentElements[index];
      if (currentElement.VisualColumn > visualEndColumn)
        break;
      if (currentElement.VisualColumn < visualStartColumn && currentElement.VisualColumn + currentElement.VisualLength > visualStartColumn && currentElement.CanSplit)
        currentElement.Split(visualStartColumn, this.CurrentElements, index--);
      else if (currentElement.VisualColumn >= visualStartColumn && currentElement.VisualColumn < visualEndColumn)
      {
        if (currentElement.VisualColumn + currentElement.VisualLength > visualEndColumn)
        {
          if (currentElement.CanSplit)
            currentElement.Split(visualEndColumn, this.CurrentElements, index--);
        }
        else
          action(currentElement);
      }
    }
  }

  protected virtual void OnAddToTextView(TextView textView)
  {
  }

  protected virtual void OnRemoveFromTextView(TextView textView)
  {
  }

  void ITextViewConnect.AddToTextView(TextView textView) => this.OnAddToTextView(textView);

  void ITextViewConnect.RemoveFromTextView(TextView textView)
  {
    this.OnRemoveFromTextView(textView);
  }
}
