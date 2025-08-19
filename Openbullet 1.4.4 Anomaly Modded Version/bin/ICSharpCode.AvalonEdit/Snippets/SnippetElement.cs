// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.SnippetElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public abstract class SnippetElement
{
  public abstract void Insert(InsertionContext context);

  public virtual Inline ToTextRun() => (Inline) null;
}
