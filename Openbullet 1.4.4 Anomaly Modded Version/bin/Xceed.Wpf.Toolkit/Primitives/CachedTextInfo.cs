// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.CachedTextInfo
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

internal class CachedTextInfo : ICloneable
{
  private CachedTextInfo(string text, int caretIndex, int selectionStart, int selectionLength)
  {
    this.Text = text;
    this.CaretIndex = caretIndex;
    this.SelectionStart = selectionStart;
    this.SelectionLength = selectionLength;
  }

  public CachedTextInfo(TextBox textBox)
    : this(textBox.Text, textBox.CaretIndex, textBox.SelectionStart, textBox.SelectionLength)
  {
  }

  public string Text { get; private set; }

  public int CaretIndex { get; private set; }

  public int SelectionStart { get; private set; }

  public int SelectionLength { get; private set; }

  public object Clone()
  {
    return (object) new CachedTextInfo(this.Text, this.CaretIndex, this.SelectionStart, this.SelectionLength);
  }
}
