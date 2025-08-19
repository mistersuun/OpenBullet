// Decompiled with JetBrains decompiler
// Type: IronPython.RuntimeInformation
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.IO;

#nullable disable
namespace IronPython;

internal static class RuntimeInformation
{
  private static readonly OSPlatform _osPlatform;

  static RuntimeInformation()
  {
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Unix:
        if (Directory.Exists("/Applications") & Directory.Exists("/System") & Directory.Exists("/Users") & Directory.Exists("/Volumes"))
        {
          RuntimeInformation._osPlatform = OSPlatform.OSX;
          break;
        }
        RuntimeInformation._osPlatform = OSPlatform.Linux;
        break;
      case PlatformID.MacOSX:
        RuntimeInformation._osPlatform = OSPlatform.OSX;
        break;
      default:
        RuntimeInformation._osPlatform = OSPlatform.Windows;
        break;
    }
  }

  public static bool IsOSPlatform(OSPlatform osPlatform)
  {
    return RuntimeInformation._osPlatform == osPlatform;
  }
}
