// Decompiled with JetBrains decompiler
// Type: InteropDotNet.SystemManager
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace InteropDotNet;

internal static class SystemManager
{
  public static string GetPlatformName() => IntPtr.Size != 4 ? "x64" : "x86";

  public static OperatingSystem GetOperatingSystem()
  {
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.Win32NT:
      case PlatformID.WinCE:
        return OperatingSystem.Windows;
      case PlatformID.Unix:
      case (PlatformID) 128 /*0x80*/:
        return OperatingSystem.Unix;
      case PlatformID.MacOSX:
        return OperatingSystem.MacOSX;
      default:
        return OperatingSystem.Unknown;
    }
  }
}
