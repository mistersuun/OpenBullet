// Decompiled with JetBrains decompiler
// Type: Standard.TITLEBARINFOEX
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

#nullable disable
namespace Standard;

internal struct TITLEBARINFOEX
{
  public int cbSize;
  public RECT rcTitleBar;
  public STATE_SYSTEM rgstate_TitleBar;
  public STATE_SYSTEM rgstate_Reserved;
  public STATE_SYSTEM rgstate_MinimizeButton;
  public STATE_SYSTEM rgstate_MaximizeButton;
  public STATE_SYSTEM rgstate_HelpButton;
  public STATE_SYSTEM rgstate_CloseButton;
  public RECT rgrect_TitleBar;
  public RECT rgrect_Reserved;
  public RECT rgrect_MinimizeButton;
  public RECT rgrect_MaximizeButton;
  public RECT rgrect_HelpButton;
  public RECT rgrect_CloseButton;
}
