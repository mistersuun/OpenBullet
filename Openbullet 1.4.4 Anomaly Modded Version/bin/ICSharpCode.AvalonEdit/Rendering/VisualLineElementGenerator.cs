// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineElementGenerator
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class VisualLineElementGenerator
{
  internal int cachedInterest;

  protected ITextRunConstructionContext CurrentContext { get; private set; }

  public virtual void StartGeneration(ITextRunConstructionContext context)
  {
    this.CurrentContext = context != null ? context : throw new ArgumentNullException(nameof (context));
  }

  public virtual void FinishGeneration()
  {
    this.CurrentContext = (ITextRunConstructionContext) null;
  }

  public abstract int GetFirstInterestedOffset(int startOffset);

  public abstract VisualLineElement ConstructElement(int offset);
}
