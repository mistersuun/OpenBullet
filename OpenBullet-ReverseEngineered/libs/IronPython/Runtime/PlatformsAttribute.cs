// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PlatformsAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

public class PlatformsAttribute : Attribute
{
  public static readonly PlatformID[] WindowsFamily = new PlatformID[5]
  {
    PlatformID.Win32NT,
    PlatformID.Win32S,
    PlatformID.Win32Windows,
    PlatformID.WinCE,
    PlatformID.Xbox
  };
  public static readonly PlatformID[] UnixFamily = new PlatformID[2]
  {
    PlatformID.MacOSX,
    PlatformID.Unix
  };

  public PlatformID[] ValidPlatforms { get; protected set; }

  public bool IsPlatformValid
  {
    get
    {
      return this.ValidPlatforms == null || this.ValidPlatforms.Length == 0 || Array.IndexOf<PlatformID>(this.ValidPlatforms, Environment.OSVersion.Platform) >= 0;
    }
  }

  protected void SetValidPlatforms(
    PlatformsAttribute.PlatformFamily validPlatformFamily)
  {
    if (validPlatformFamily == PlatformsAttribute.PlatformFamily.Unix)
      this.ValidPlatforms = PlatformsAttribute.UnixFamily;
    else
      this.ValidPlatforms = PlatformsAttribute.WindowsFamily;
  }

  public enum PlatformFamily
  {
    Windows,
    Unix,
  }
}
