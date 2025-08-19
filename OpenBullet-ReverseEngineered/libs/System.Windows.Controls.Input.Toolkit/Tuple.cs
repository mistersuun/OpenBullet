// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Tuple
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

#nullable disable
namespace System.Windows.Controls;

internal static class Tuple
{
  public static Tuple<T0, T1> Create<T0, T1>(T0 arg0, T1 arg1) => new Tuple<T0, T1>(arg0, arg1);
}
