// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Tuple`2
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

#nullable disable
namespace System.Windows.Controls;

internal class Tuple<T0, T1>
{
  public T0 First { get; private set; }

  public T1 Second { get; private set; }

  public Tuple(T0 first, T1 second)
  {
    this.First = first;
    this.Second = second;
  }
}
