// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.TimeItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class TimeItem
{
  public string Display { get; set; }

  public TimeSpan Time { get; set; }

  public TimeItem(string display, TimeSpan time)
  {
    this.Display = display;
    this.Time = time;
  }

  public override bool Equals(object obj) => obj is TimeItem timeItem && this.Time == timeItem.Time;

  public override int GetHashCode() => this.Time.GetHashCode();
}
