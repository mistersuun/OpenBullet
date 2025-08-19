// Decompiled with JetBrains decompiler
// Type: System.Windows.VisualStateChangedEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows.Controls;

#nullable disable
namespace System.Windows;

public sealed class VisualStateChangedEventArgs : EventArgs
{
  private VisualState _oldState;
  private VisualState _newState;
  private Control _control;

  internal VisualStateChangedEventArgs(VisualState oldState, VisualState newState, Control control)
  {
    this._oldState = oldState;
    this._newState = newState;
    this._control = control;
  }

  public VisualState OldState => this._oldState;

  public VisualState NewState => this._newState;

  public Control Control => this._control;
}
