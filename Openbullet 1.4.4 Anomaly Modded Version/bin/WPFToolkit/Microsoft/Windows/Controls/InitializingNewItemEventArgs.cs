// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.InitializingNewItemEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;

#nullable disable
namespace Microsoft.Windows.Controls;

public class InitializingNewItemEventArgs : EventArgs
{
  private object _newItem;

  public InitializingNewItemEventArgs(object newItem) => this._newItem = newItem;

  public object NewItem => this._newItem;
}
