// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.AutoCompletingMaskEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class AutoCompletingMaskEventArgs : CancelEventArgs
{
  private MaskedTextProvider m_maskedTextProvider;
  private int m_startPosition;
  private int m_selectionLength;
  private string m_input;
  private int m_autoCompleteStartPosition;
  private string m_autoCompleteText;

  public AutoCompletingMaskEventArgs(
    MaskedTextProvider maskedTextProvider,
    int startPosition,
    int selectionLength,
    string input)
  {
    this.m_autoCompleteStartPosition = -1;
    this.m_maskedTextProvider = maskedTextProvider;
    this.m_startPosition = startPosition;
    this.m_selectionLength = selectionLength;
    this.m_input = input;
  }

  public MaskedTextProvider MaskedTextProvider => this.m_maskedTextProvider;

  public int StartPosition => this.m_startPosition;

  public int SelectionLength => this.m_selectionLength;

  public string Input => this.m_input;

  public int AutoCompleteStartPosition
  {
    get => this.m_autoCompleteStartPosition;
    set => this.m_autoCompleteStartPosition = value;
  }

  public string AutoCompleteText
  {
    get => this.m_autoCompleteText;
    set => this.m_autoCompleteText = value;
  }
}
