// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.PopulatedEventArgs
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections;

#nullable disable
namespace System.Windows.Controls;

public class PopulatedEventArgs : RoutedEventArgs
{
  public IEnumerable Data { get; private set; }

  public PopulatedEventArgs(IEnumerable data) => this.Data = data;

  public PopulatedEventArgs(IEnumerable data, RoutedEvent routedEvent)
    : base(routedEvent)
  {
    this.Data = data;
  }
}
