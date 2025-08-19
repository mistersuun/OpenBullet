// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.QueryMoveFocusEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class QueryMoveFocusEventArgs : RoutedEventArgs
{
  private FocusNavigationDirection m_navigationDirection;
  private bool m_reachedMaxLength;
  private bool m_canMove = true;

  private QueryMoveFocusEventArgs()
  {
  }

  internal QueryMoveFocusEventArgs(FocusNavigationDirection direction, bool reachedMaxLength)
    : base(AutoSelectTextBox.QueryMoveFocusEvent)
  {
    this.m_navigationDirection = direction;
    this.m_reachedMaxLength = reachedMaxLength;
  }

  public FocusNavigationDirection FocusNavigationDirection => this.m_navigationDirection;

  public bool ReachedMaxLength => this.m_reachedMaxLength;

  public bool CanMoveFocus
  {
    get => this.m_canMove;
    set => this.m_canMove = value;
  }
}
