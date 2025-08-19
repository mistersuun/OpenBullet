// Decompiled with JetBrains decompiler
// Type: IronPython.OSPlatform
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython;

internal readonly struct OSPlatform : IEquatable<OSPlatform>
{
  private readonly string _osPlatform;

  public static OSPlatform Linux { get; } = new OSPlatform("LINUX");

  public static OSPlatform OSX { get; } = new OSPlatform(nameof (OSX));

  public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");

  private OSPlatform(string osPlatform) => this._osPlatform = osPlatform;

  public bool Equals(OSPlatform other) => this._osPlatform.Equals(other._osPlatform);

  public override bool Equals(object obj) => obj is OSPlatform other && this.Equals(other);

  public override int GetHashCode()
  {
    return this._osPlatform != null ? this._osPlatform.GetHashCode() : 0;
  }

  public override string ToString() => this._osPlatform ?? string.Empty;

  public static bool operator ==(OSPlatform left, OSPlatform right) => left.Equals(right);

  public static bool operator !=(OSPlatform left, OSPlatform right) => !(left == right);
}
